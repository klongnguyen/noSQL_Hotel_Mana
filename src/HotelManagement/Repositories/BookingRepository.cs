using Cassandra;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ICassandraContext _context;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(
        ICassandraContext context,
        ILogger<BookingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Booking>> GetByGuestAsync(string guestId)
    {
        const string cql = """
            SELECT guest_id, check_in_date, booking_id, hotel_id, hotel_name,
                   room_number, check_out_date, total_amount, status
            FROM bookings_by_guest
            WHERE guest_id = ?;
            """;

        var rows = await _context.Session.ExecuteAsync(
            new SimpleStatement(cql, guestId));

        return rows.Select(MapGuestBooking).OrderByDescending(b => b.CheckInDate).ToList();
    }

    public async Task CreateAsync(Booking booking)
    {
        // CQL date -> Cassandra.LocalDate.
        var checkIn = ToLocalDate(booking.CheckInDate);
        var checkOut = ToLocalDate(booking.CheckOutDate);

        var insertByGuest = await _context.Session.PrepareAsync("""
            INSERT INTO bookings_by_guest
            (guest_id, check_in_date, booking_id, hotel_id, hotel_name,
             room_number, check_out_date, total_amount, status)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);
            """);

        var insertByHotel = await _context.Session.PrepareAsync("""
            INSERT INTO bookings_by_hotel_date
            (hotel_id, check_in_date, booking_id, guest_id, guest_name,
             room_number, check_out_date, total_amount, status)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);
            """);

        var updateRoom = await _context.Session.PrepareAsync("""
            UPDATE rooms_by_hotel
            SET status = ?
            WHERE hotel_id = ? AND room_number = ?;
            """);

        // rooms_by_hotel_status là bảng denormalized dùng cho truy vấn theo status.
        var deleteAvailableRoomStatus = await _context.Session.PrepareAsync("""
            DELETE FROM rooms_by_hotel_status
            WHERE hotel_id = ? AND status = ? AND room_number = ?;
            """);

        var insertOccupiedRoomStatus = await _context.Session.PrepareAsync("""
            INSERT INTO rooms_by_hotel_status
            (hotel_id, status, room_number, room_type, price_per_night)
            VALUES (?, ?, ?, ?, ?);
            """);

        // Dual-write: hai bản sao booking + trạng thái phòng được gửi trong cùng BatchStatement.
        var batch = new BatchStatement()
            .SetBatchType(BatchType.Logged)
            .Add(insertByGuest.Bind(
                booking.GuestId,
                checkIn,
                booking.BookingId,
                booking.HotelId,
                booking.HotelName,
                booking.RoomNumber,
                checkOut,
                booking.TotalAmount,
                "CONFIRMED"))
            .Add(insertByHotel.Bind(
                booking.HotelId,
                checkIn,
                booking.BookingId,
                booking.GuestId,
                booking.GuestName,
                booking.RoomNumber,
                checkOut,
                booking.TotalAmount,
                "CONFIRMED"))
            .Add(updateRoom.Bind(
                "OCCUPIED",
                booking.HotelId,
                booking.RoomNumber))
            .Add(deleteAvailableRoomStatus.Bind(
                booking.HotelId,
                "AVAILABLE",
                booking.RoomNumber))
            .Add(insertOccupiedRoomStatus.Bind(
                booking.HotelId,
                "OCCUPIED",
                booking.RoomNumber,
                booking.RoomType,
                booking.RoomPricePerNight));

        await _context.Session.ExecuteAsync(batch);
    }

    public async Task CancelAsync(Booking booking, Room room)
    {
        var checkIn = ToLocalDate(booking.CheckInDate);

        var updateByGuest = await _context.Session.PrepareAsync("""
            UPDATE bookings_by_guest
            SET status = ?
            WHERE guest_id = ? AND check_in_date = ? AND booking_id = ?;
            """);

        var updateByHotel = await _context.Session.PrepareAsync("""
            UPDATE bookings_by_hotel_date
            SET status = ?
            WHERE hotel_id = ? AND check_in_date = ? AND booking_id = ?;
            """);

        var updateRoom = await _context.Session.PrepareAsync("""
            UPDATE rooms_by_hotel
            SET status = ?
            WHERE hotel_id = ? AND room_number = ?;
            """);

        var deleteOccupiedRoomStatus = await _context.Session.PrepareAsync("""
            DELETE FROM rooms_by_hotel_status
            WHERE hotel_id = ? AND status = ? AND room_number = ?;
            """);

        var insertAvailableRoomStatus = await _context.Session.PrepareAsync("""
            INSERT INTO rooms_by_hotel_status
            (hotel_id, status, room_number, room_type, price_per_night)
            VALUES (?, ?, ?, ?, ?);
            """);

        // Hủy booking: cập nhật CANCELLED ở cả hai bảng denormalized
        // và hoàn trả phòng về AVAILABLE.
        var batch = new BatchStatement()
            .SetBatchType(BatchType.Logged)
            .Add(updateByGuest.Bind(
                "CANCELLED",
                booking.GuestId,
                checkIn,
                booking.BookingId))
            .Add(updateByHotel.Bind(
                "CANCELLED",
                booking.HotelId,
                checkIn,
                booking.BookingId))
            .Add(updateRoom.Bind(
                "AVAILABLE",
                booking.HotelId,
                booking.RoomNumber))
            .Add(deleteOccupiedRoomStatus.Bind(
                booking.HotelId,
                "OCCUPIED",
                booking.RoomNumber))
            .Add(insertAvailableRoomStatus.Bind(
                booking.HotelId,
                "AVAILABLE",
                booking.RoomNumber,
                room.RoomType,
                room.PricePerNight));

        await _context.Session.ExecuteAsync(batch);
    }

    private static Booking MapGuestBooking(Row row)
    {
        return new Booking
        {
            BookingId = row.GetValue<Guid>("booking_id"),
            GuestId = row.GetValue<string>("guest_id"),
            HotelId = row.GetValue<string>("hotel_id"),
            HotelName = row.GetValue<string>("hotel_name"),
            RoomNumber = row.GetValue<int>("room_number"),
            CheckInDate = FromLocalDate(row.GetValue<LocalDate>("check_in_date")),
            CheckOutDate = FromLocalDate(row.GetValue<LocalDate>("check_out_date")),
            TotalAmount = row.GetValue<decimal>("total_amount"),
            Status = row.GetValue<string>("status")
        };
    }

    private static LocalDate ToLocalDate(DateTime date)
        => new(date.Year, date.Month, date.Day);

    private static DateTime FromLocalDate(LocalDate date)
        => new(date.Year, date.Month, date.Day);
}
