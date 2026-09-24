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
    private readonly HotelManagement.Services.IDashboardAnalyticsService _analyticsService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository bookingRepository,
        IGuestRepository guestRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        HotelManagement.Services.IDashboardAnalyticsService analyticsService,
        ILogger<BookingsController> logger)
    {
        _bookingRepository = bookingRepository;
        _guestRepository = guestRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _analyticsService = analyticsService;
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
    public async Task<IActionResult> Create(string? hotelId, int? roomNumber, string? guestId)
    {
        try
        {
            var hotels = (await _hotelRepository.GetAllHotelsAsync())
                .OrderBy(h => h.HotelId)
                .ToList();

            var guests = (await _guestRepository.GetAllAsync())
                .OrderBy(g => g.GuestId)
                .ToList();

            ViewBag.Hotels = hotels;
            ViewBag.Guests = guests;

            var selectedHotelId = !string.IsNullOrWhiteSpace(hotelId)
                ? hotelId.Trim()
                : hotels.FirstOrDefault()?.HotelId ?? "HTL001";

            var rooms = (await _roomRepository.GetRoomsByHotelAsync(selectedHotelId))
                .OrderBy(r => r.RoomNumber)
                .ToList();

            ViewBag.Rooms = rooms;

            var selectedRoomNumber = roomNumber.HasValue && roomNumber.Value > 0
                ? roomNumber.Value
                : (rooms.FirstOrDefault()?.RoomNumber ?? 101);

            var selectedGuestId = !string.IsNullOrWhiteSpace(guestId)
                ? guestId.Trim()
                : (guests.FirstOrDefault()?.GuestId ?? "GUEST001");

            var model = new Booking
            {
                HotelId = selectedHotelId,
                RoomNumber = selectedRoomNumber,
                GuestId = selectedGuestId,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi khởi tạo form tạo đặt phòng mới");
            TempData["ErrorMessage"] = "Không thể tải danh sách dữ liệu gợi ý.";
            return View(new Booking
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms(string hotelId)
    {
        if (string.IsNullOrWhiteSpace(hotelId)) return Json(new List<object>());
        try
        {
            var rooms = (await _roomRepository.GetRoomsByHotelAsync(hotelId.Trim()))
                .OrderBy(r => r.RoomNumber)
                .Select(r => new
                {
                    roomNumber = r.RoomNumber,
                    roomType = r.RoomType,
                    pricePerNight = r.PricePerNight,
                    status = r.Status
                });
            return Json(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách phòng cho khách sạn {HotelId}", hotelId);
            return StatusCode(500, new { error = "Không thể tải danh sách phòng." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBookedDates(string hotelId, int roomNumber)
    {
        if (string.IsNullOrWhiteSpace(hotelId) || roomNumber <= 0)
            return Json(new List<object>());

        try
        {
            var bookings = await _bookingRepository.GetByHotelAsync(hotelId.Trim());
            var activeRoomBookings = bookings
                .Where(b => b.RoomNumber == roomNumber && !b.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.CheckInDate)
                .Select(b => new
                {
                    bookingId = b.BookingId,
                    guestId = b.GuestId,
                    guestName = b.GuestName,
                    checkInDate = b.CheckInDate.ToString("yyyy-MM-dd"),
                    checkOutDate = b.CheckOutDate.ToString("yyyy-MM-dd"),
                    checkInDisplay = b.CheckInDate.ToString("dd/MM/yyyy"),
                    checkOutDisplay = b.CheckOutDate.ToString("dd/MM/yyyy"),
                    status = b.Status,
                    totalAmount = b.TotalAmount
                })
                .ToList();

            return Json(activeRoomBookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy lịch đặt phòng {RoomNumber} tại {HotelId}", roomNumber, hotelId);
            return StatusCode(500, new { error = "Không thể tải lịch đặt phòng." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        // BookingId do server tạo để tránh trùng khóa.
        booking.BookingId = Guid.NewGuid();

        // 1. Kiểm tra tính hợp lệ của ngày tháng
        if (booking.CheckInDate.Date < DateTime.Today)
        {
            ModelState.AddModelError(nameof(booking.CheckInDate), "Ngày nhận phòng không được là ngày trong quá khứ.");
        }

        if (booking.CheckOutDate.Date <= booking.CheckInDate.Date)
        {
            ModelState.AddModelError(nameof(booking.CheckOutDate), "Ngày trả phòng (check-out) phải sau ngày nhận phòng ít nhất 1 đêm.");
        }

        Guest? guest = null;
        if (!string.IsNullOrWhiteSpace(booking.GuestId))
        {
            guest = await _guestRepository.GetByIdAsync(booking.GuestId.Trim());
            if (guest == null)
            {
                ModelState.AddModelError(nameof(booking.GuestId), $"Mã khách hàng '{booking.GuestId}' không tồn tại trong hệ thống.");
            }
        }
        else
        {
            ModelState.AddModelError(nameof(booking.GuestId), "Vui lòng chọn hoặc nhập mã khách hàng.");
        }

        Hotel? hotel = null;
        if (!string.IsNullOrWhiteSpace(booking.HotelId))
        {
            hotel = await _hotelRepository.GetHotelByIdAsync(booking.HotelId.Trim());
            if (hotel == null)
            {
                ModelState.AddModelError(nameof(booking.HotelId), $"Mã khách sạn '{booking.HotelId}' không tồn tại trong hệ thống.");
            }
        }
        else
        {
            ModelState.AddModelError(nameof(booking.HotelId), "Vui lòng chọn hoặc nhập mã khách sạn.");
        }

        Room? room = null;
        if (hotel != null && booking.RoomNumber > 0)
        {
            room = await _roomRepository.GetRoomByNumberAsync(booking.HotelId.Trim(), booking.RoomNumber);
            if (room == null)
            {
                ModelState.AddModelError(nameof(booking.RoomNumber), $"Không tìm thấy phòng số {booking.RoomNumber} tại chi nhánh {hotel.HotelName}.");
            }
            else if (room.Status.Equals("MAINTENANCE", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(booking.RoomNumber), $"Phòng {room.RoomNumber} hiện đang BẢO TRÌ (MAINTENANCE), không thể tiếp nhận đặt phòng.");
            }
        }
        else if (booking.RoomNumber <= 0)
        {
            ModelState.AddModelError(nameof(booking.RoomNumber), "Vui lòng chọn số phòng hợp lệ.");
        }

        // 2. Kiểm tra xung đột lịch đặt phòng (Overlap Detection) bằng Cassandra table bookings_by_hotel_date
        if (hotel != null && room != null && ModelState.IsValid)
        {
            var existingBookings = await _bookingRepository.GetByHotelAsync(booking.HotelId.Trim());
            var conflicts = existingBookings
                .Where(b => b.RoomNumber == booking.RoomNumber &&
                            !b.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase) &&
                            booking.CheckInDate.Date < b.CheckOutDate.Date &&
                            booking.CheckOutDate.Date > b.CheckInDate.Date)
                .OrderBy(b => b.CheckInDate)
                .ToList();

            if (conflicts.Any())
            {
                var firstConflict = conflicts.First();
                ModelState.AddModelError(string.Empty,
                    $"Phòng {booking.RoomNumber} đã có khách đặt từ {firstConflict.CheckInDate:dd/MM/yyyy} đến {firstConflict.CheckOutDate:dd/MM/yyyy} (Mã đơn: {firstConflict.BookingId.ToString()[..8]}...). Khoảng ngày bạn chọn không liên tục vì đã có người đặt ở giữa. Vui lòng chọn khoảng ngày trống khác!");
            }
        }

        if (!ModelState.IsValid)
        {
            await ReloadViewBagsAsync(booking.HotelId);
            return View(booking);
        }

        try
        {
            var nights = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;

            booking.GuestId = guest!.GuestId;
            booking.GuestName = guest.FullName;
            booking.HotelId = hotel!.HotelId;
            booking.HotelName = hotel.HotelName;
            booking.RoomType = room!.RoomType;
            booking.RoomPricePerNight = room.PricePerNight;
            booking.TotalAmount = room.PricePerNight * nights;
            booking.Status = "CONFIRMED";

            await _bookingRepository.CreateAsync(booking);

            // Tự động cập nhật thống kê động lên Dashboard (Doanh thu, Top chi nhánh, Bản đồ mật độ)
            await _analyticsService.RecordBookingAsync(booking);

            TempData["SuccessMessage"] =
                $"Đặt phòng thành công cho khách hàng {booking.GuestName} ({booking.GuestId}) tại phòng {booking.RoomNumber} - {booking.HotelName}. Mã booking: {booking.BookingId}";

            return RedirectToAction(nameof(Index), new { guestId = booking.GuestId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo booking");
            ModelState.AddModelError("", "Không thể tạo đặt phòng. Kiểm tra kết nối Cassandra và cấu trúc bảng.");
            await ReloadViewBagsAsync(booking.HotelId);
            return View(booking);
        }
    }

    private async Task ReloadViewBagsAsync(string? selectedHotelId)
    {
        try
        {
            var hotels = (await _hotelRepository.GetAllHotelsAsync()).OrderBy(h => h.HotelId).ToList();
            var guests = (await _guestRepository.GetAllAsync()).OrderBy(g => g.GuestId).ToList();
            ViewBag.Hotels = hotels;
            ViewBag.Guests = guests;

            var hotelId = !string.IsNullOrWhiteSpace(selectedHotelId)
                ? selectedHotelId.Trim()
                : hotels.FirstOrDefault()?.HotelId ?? "HTL001";

            var rooms = (await _roomRepository.GetRoomsByHotelAsync(hotelId)).OrderBy(r => r.RoomNumber).ToList();
            ViewBag.Rooms = rooms;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi nạp lại ViewBag cho View Bookings/Create");
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
