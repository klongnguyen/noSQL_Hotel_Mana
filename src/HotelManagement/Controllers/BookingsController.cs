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
    public async Task<IActionResult> GetRooms(string hotelId, DateTime? checkIn = null, DateTime? checkOut = null)
    {
        if (string.IsNullOrWhiteSpace(hotelId)) return Json(new List<object>());
        try
        {
            var rooms = (await _roomRepository.GetRoomsByHotelAsync(hotelId.Trim()))
                .OrderBy(r => r.RoomNumber)
                .ToList();

            List<Booking> activeBookings = new();
            bool checkDates = checkIn.HasValue && checkOut.HasValue && checkOut.Value.Date > checkIn.Value.Date;
            DateTime inDate = DateTime.MinValue;
            DateTime outDate = DateTime.MinValue;
            if (checkDates)
            {
                inDate = checkIn!.Value.Date;
                outDate = checkOut!.Value.Date;
                var allBookings = await _bookingRepository.GetByHotelAsync(hotelId.Trim());
                activeBookings = allBookings
                    .Where(b => !b.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var result = rooms.Select(r =>
            {
                bool isAvailable = true;
                string conflictReason = string.Empty;

                if (r.Status.Equals("MAINTENANCE", StringComparison.OrdinalIgnoreCase))
                {
                    isAvailable = false;
                    conflictReason = "Phòng đang bảo trì";
                }
                else if (checkDates)
                {
                    var conflict = activeBookings.FirstOrDefault(b =>
                        b.RoomNumber == r.RoomNumber &&
                        inDate < b.CheckOutDate.Date &&
                        outDate > b.CheckInDate.Date);

                    if (conflict != null)
                    {
                        isAvailable = false;
                        conflictReason = $"Đã có khách đặt ({conflict.CheckInDate:dd/MM} - {conflict.CheckOutDate:dd/MM})";
                    }
                }

                return new
                {
                    roomNumber = r.RoomNumber,
                    roomType = r.RoomType,
                    pricePerNight = r.PricePerNight,
                    status = r.Status,
                    capacity = r.Capacity,
                    isAvailable = isAvailable,
                    conflictReason = conflictReason
                };
            });

            return Json(result);
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

        // 2. Validate Sức chứa phòng (Capacity Validation)
        if (room != null)
        {
            if (booking.NumberOfOccupants <= 0)
            {
                booking.NumberOfOccupants = 1;
            }

            if (booking.NumberOfOccupants > room.Capacity)
            {
                ModelState.AddModelError(nameof(booking.NumberOfOccupants),
                    $"Phòng {room.RoomNumber} ({room.RoomType}) chỉ chứa tối đa {room.Capacity} người. Bạn đang đăng ký {booking.NumberOfOccupants} người.");
            }
        }

        // 3. Xử lý và Validate thông tin người ở cùng phòng (Occupants)
        var validOccupants = new List<RoomOccupant>();
        if (guest != null)
        {
            // Người đại diện (Người 1)
            var primaryCid = !string.IsNullOrWhiteSpace(guest.NationalId) ? guest.NationalId.Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(primaryCid))
            {
                ModelState.AddModelError(nameof(booking.GuestId), $"Khách hàng '{guest.FullName}' chưa có số Căn cước công dân trong hồ sơ. Vui lòng cập nhật hồ sơ khách trước.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(primaryCid, @"^\d{12}$"))
            {
                ModelState.AddModelError(nameof(booking.GuestId), $"Số Căn cước công dân của khách hàng '{guest.FullName}' ({primaryCid}) không hợp lệ (phải gồm đúng 12 chữ số). Vui lòng cập nhật hồ sơ khách trước.");
            }

            validOccupants.Add(new RoomOccupant
            {
                FullName = guest.FullName,
                CitizenId = !string.IsNullOrWhiteSpace(primaryCid) ? primaryCid : guest.GuestId,
                DateOfBirth = null,
                IsPrimary = true
            });

            // Người ở cùng (Người 2..N)
            if (booking.NumberOfOccupants > 1)
            {
                for (int i = 1; i < booking.NumberOfOccupants; i++)
                {
                    var occInput = (booking.Occupants != null && i < booking.Occupants.Count)
                        ? booking.Occupants[i]
                        : null;

                    var occName = occInput?.FullName?.Trim() ?? string.Empty;
                    var occCid = occInput?.CitizenId?.Trim() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(occName))
                    {
                        ModelState.AddModelError($"Occupants[{i}].FullName", $"Vui lòng nhập Họ và tên cho người ở cùng thứ {i + 1}.");
                    }
                    if (string.IsNullOrWhiteSpace(occCid))
                    {
                        ModelState.AddModelError($"Occupants[{i}].CitizenId", $"Vui lòng nhập số Căn cước công dân (CCCD) cho người ở cùng thứ {i + 1}.");
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(occCid, @"^\d{12}$"))
                    {
                        ModelState.AddModelError($"Occupants[{i}].CitizenId", $"Số Căn cước công dân của người ở cùng thứ {i + 1} ({occCid}) không hợp lệ (phải gồm đúng 12 chữ số).");
                    }

                    if (occInput?.DateOfBirth.HasValue == true)
                    {
                        if (occInput.DateOfBirth.Value.Date > DateTime.Today)
                        {
                            ModelState.AddModelError($"Occupants[{i}].DateOfBirth", $"Ngày sinh của người ở cùng thứ {i + 1} không thể là ngày trong tương lai.");
                        }
                        else if (occInput.DateOfBirth.Value.Year < 1900)
                        {
                            ModelState.AddModelError($"Occupants[{i}].DateOfBirth", $"Năm sinh của người ở cùng thứ {i + 1} không hợp lệ (phải từ năm 1900 trở lại đây).");
                        }
                    }

                    validOccupants.Add(new RoomOccupant
                    {
                        FullName = occName,
                        CitizenId = occCid,
                        DateOfBirth = occInput?.DateOfBirth,
                        IsPrimary = false
                    });
                }
            }
        }

        // 4. Kiểm tra xung đột lịch đặt phòng (Overlap Detection) bằng Cassandra table bookings_by_hotel_date
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
            booking.Occupants = validOccupants;
            booking.OccupantsJson = System.Text.Json.JsonSerializer.Serialize(validOccupants);

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
