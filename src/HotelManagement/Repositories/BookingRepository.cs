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
                   room_number, check_out_date, total_amount, status, num_occupants, occupants_json
            FROM bookings_by_guest
            WHERE guest_id = ?;
            """;

        var rows = await _context.Session.ExecuteAsync(
            new SimpleStatement(cql, guestId));

        return rows.Select(MapGuestBooking).OrderByDescending(b => b.CheckInDate).ToList();
    }

    public async Task<IEnumerable<Booking>> GetByHotelAsync(string hotelId)
    {
        const string cql = """
            SELECT hotel_id, check_in_date, booking_id, guest_id, guest_name,
                   room_number, check_out_date, total_amount, status, num_occupants, occupants_json
            FROM bookings_by_hotel_date
            WHERE hotel_id = ?;
            """;

        var rows = await _context.Session.ExecuteAsync(
            new SimpleStatement(cql, hotelId));

        return rows.Select(MapHotelBooking).OrderBy(b => b.CheckInDate).ToList();
    }

    public async Task CreateAsync(Booking booking)
    {
        // CQL date -> Cassandra.LocalDate.
        var checkIn = ToLocalDate(booking.CheckInDate);
        var checkOut = ToLocalDate(booking.CheckOutDate);
        var occupantsJson = string.IsNullOrWhiteSpace(booking.OccupantsJson) ? "[]" : booking.OccupantsJson;
        var numOccupants = booking.NumberOfOccupants > 0 ? booking.NumberOfOccupants : 1;

        var insertByGuest = await _context.Session.PrepareAsync("""
            INSERT INTO bookings_by_guest
            (guest_id, check_in_date, booking_id, hotel_id, hotel_name,
             room_number, check_out_date, total_amount, status, num_occupants, occupants_json)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            """);

        var insertByHotel = await _context.Session.PrepareAsync("""
            INSERT INTO bookings_by_hotel_date
            (hotel_id, check_in_date, booking_id, guest_id, guest_name,
             room_number, check_out_date, total_amount, status, num_occupants, occupants_json)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            """);

        // Dual-write: hai bản sao booking được lưu qua Logged Batch.
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
                "CONFIRMED",
                numOccupants,
                occupantsJson))
            .Add(insertByHotel.Bind(
                booking.HotelId,
                checkIn,
                booking.BookingId,
                booking.GuestId,
                booking.GuestName,
                booking.RoomNumber,
                checkOut,
                booking.TotalAmount,
                "CONFIRMED",
                numOccupants,
                occupantsJson));

        // Chỉ chuyển trạng thái phòng sang OCCUPIED nếu đơn đặt phòng nhận phòng ngay hôm nay
        bool isOccupiedToday = booking.CheckInDate.Date <= DateTime.Today && booking.CheckOutDate.Date > DateTime.Today;
        if (isOccupiedToday)
        {
            var updateRoom = await _context.Session.PrepareAsync("""
                UPDATE rooms_by_hotel
                SET status = ?
                WHERE hotel_id = ? AND room_number = ?;
                """);

            var deleteAvailableRoomStatus = await _context.Session.PrepareAsync("""
                DELETE FROM rooms_by_hotel_status
                WHERE hotel_id = ? AND status = ? AND room_number = ?;
                """);

            var insertOccupiedRoomStatus = await _context.Session.PrepareAsync("""
                INSERT INTO rooms_by_hotel_status
                (hotel_id, status, room_number, room_type, price_per_night, capacity)
                VALUES (?, ?, ?, ?, ?, ?);
                """);

            batch.Add(updateRoom.Bind(
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
                    booking.RoomPricePerNight,
                    booking.NumberOfOccupants > 1 ? booking.NumberOfOccupants : 2));
        }

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
            (hotel_id, status, room_number, room_type, price_per_night, capacity)
            VALUES (?, ?, ?, ?, ?, ?);
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
                room.PricePerNight,
                room.Capacity));

        await _context.Session.ExecuteAsync(batch);
    }

    private static Booking MapGuestBooking(Row row)
    {
        var (numOccupants, occupantsJson, occupants) = ParseOccupants(row);

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
            Status = row.GetValue<string>("status"),
            NumberOfOccupants = numOccupants,
            OccupantsJson = occupantsJson,
            Occupants = occupants
        };
    }

    private static Booking MapHotelBooking(Row row)
    {
        var (numOccupants, occupantsJson, occupants) = ParseOccupants(row);

        return new Booking
        {
            BookingId = row.GetValue<Guid>("booking_id"),
            HotelId = row.GetValue<string>("hotel_id"),
            CheckInDate = FromLocalDate(row.GetValue<LocalDate>("check_in_date")),
            GuestId = row.GetValue<string>("guest_id"),
            GuestName = row.GetValue<string>("guest_name"),
            RoomNumber = row.GetValue<int>("room_number"),
            CheckOutDate = FromLocalDate(row.GetValue<LocalDate>("check_out_date")),
            TotalAmount = row.GetValue<decimal>("total_amount"),
            Status = row.GetValue<string>("status"),
            NumberOfOccupants = numOccupants,
            OccupantsJson = occupantsJson,
            Occupants = occupants
        };
    }

    private static (int numOccupants, string occupantsJson, List<RoomOccupant> occupants) ParseOccupants(Row row)
    {
        int numOccupants = 1;
        try
        {
            if (row.GetColumn("num_occupants") != null && !row.IsNull("num_occupants"))
            {
                numOccupants = row.GetValue<int>("num_occupants");
            }
        }
        catch { }

        string occupantsJson = "[]";
        try
        {
            if (row.GetColumn("occupants_json") != null && !row.IsNull("occupants_json"))
            {
                occupantsJson = row.GetValue<string>("occupants_json") ?? "[]";
            }
        }
        catch { }

        var occupants = new List<RoomOccupant>();
        if (!string.IsNullOrWhiteSpace(occupantsJson) && occupantsJson != "[]")
        {
            try
            {
                occupants = System.Text.Json.JsonSerializer.Deserialize<List<RoomOccupant>>(occupantsJson) ?? new();
            }
            catch { }
        }

        return (numOccupants, occupantsJson, occupants);
    }

    private static LocalDate ToLocalDate(DateTime date)
        => new(date.Year, date.Month, date.Day);

    private static DateTime FromLocalDate(LocalDate date)
        => new(date.Year, date.Month, date.Day);
}
