using HotelManagement.Models;
using HotelManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers;

public class BookingsController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository bookingRepository,
        IGuestRepository guestRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        ILogger<BookingsController> logger)
    {
        _bookingRepository = bookingRepository;
        _guestRepository = guestRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? guestId)
    {
        if (string.IsNullOrWhiteSpace(guestId)) return View(new List<Booking>());
        try
        {
            var bookings = await _bookingRepository.GetByGuestAsync(guestId.Trim());
            ViewBag.GuestId = guestId.Trim();
            return View(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tra cứu booking của khách {GuestId}", guestId);
            TempData["ErrorMessage"] = "Không thể tải danh sách đặt phòng.";
            return View(new List<Booking>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new Booking
        {
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(1)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        // BookingId do server tạo để tránh trùng khóa.
        booking.BookingId = Guid.NewGuid();

        if (booking.CheckOutDate.Date <= booking.CheckInDate.Date)
            ModelState.AddModelError(nameof(booking.CheckOutDate), "Ngày check-out phải sau ngày check-in.");

        if (!ModelState.IsValid) return View(booking);

        try
        {
            var guest = await _guestRepository.GetByIdAsync(booking.GuestId.Trim());
            if (guest == null)
            {
                ModelState.AddModelError(nameof(booking.GuestId), "Không tìm thấy khách hàng.");
                return View(booking);
            }

            var hotel = await _hotelRepository.GetHotelByIdAsync(booking.HotelId.Trim());
            if (hotel == null)
            {
                ModelState.AddModelError(nameof(booking.HotelId), "Không tìm thấy khách sạn.");
                return View(booking);
            }

            var room = await _roomRepository.GetRoomByNumberAsync(
                booking.HotelId.Trim(),
                booking.RoomNumber);

            if (room == null)
            {
                ModelState.AddModelError(nameof(booking.RoomNumber), "Không tìm thấy phòng.");
                return View(booking);
            }

            if (!room.Status.Equals("AVAILABLE", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(booking.RoomNumber),
                    $"Phòng {room.RoomNumber} hiện có trạng thái {room.Status}, không thể đặt.");
                return View(booking);
            }

            var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

            booking.GuestId = guest.GuestId;
            booking.GuestName = guest.FullName;
            booking.HotelId = hotel.HotelId;
            booking.HotelName = hotel.HotelName;
            booking.RoomType = room.RoomType;
            booking.RoomPricePerNight = room.PricePerNight;
            booking.TotalAmount = room.PricePerNight * nights;
            booking.Status = "CONFIRMED";

            await _bookingRepository.CreateAsync(booking);

            TempData["SuccessMessage"] =
                $"Đặt phòng thành công. Mã booking: {booking.BookingId}";

            return RedirectToAction(nameof(Index), new { guestId = booking.GuestId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo booking");
            ModelState.AddModelError("", "Không thể tạo đặt phòng. Kiểm tra Cassandra và cấu trúc bảng.");
            return View(booking);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(
        Guid bookingId,
        string guestId,
        string hotelId,
        int roomNumber,
        DateTime checkInDate)
    {
        try
        {
            var bookings = await _bookingRepository.GetByGuestAsync(guestId);
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null || !booking.HotelId.Equals(hotelId, StringComparison.OrdinalIgnoreCase) ||
                booking.RoomNumber != roomNumber ||
                booking.CheckInDate.Date != checkInDate.Date)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin booking cần hủy.";
                return RedirectToAction(nameof(Index), new { guestId });
            }

            if (booking.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Booking này đã được hủy trước đó.";
                return RedirectToAction(nameof(Index), new { guestId });
            }

            var room = await _roomRepository.GetRoomByNumberAsync(hotelId, roomNumber);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phòng cần hoàn trả trạng thái.";
                return RedirectToAction(nameof(Index), new { guestId });
            }

            await _bookingRepository.CancelAsync(booking, room);

            TempData["SuccessMessage"] =
                $"Đã hủy booking {bookingId} và chuyển phòng {roomNumber} về AVAILABLE.";

            return RedirectToAction(nameof(Index), new { guestId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hủy booking {BookingId}", bookingId);
            TempData["ErrorMessage"] = "Không thể hủy booking.";
            return RedirectToAction(nameof(Index), new { guestId });
        }
    }
}
