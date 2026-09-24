using Microsoft.AspNetCore.Mvc;
using HotelManagement.Repositories;
using HotelManagement.Models;
using HotelManagement.ViewModels;

namespace HotelManagement.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            InvoiceRepository invoiceRepository,
            IHotelRepository hotelRepository,
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            ILogger<InvoiceController> logger)
        {
            _invoiceRepository = invoiceRepository;
            _hotelRepository = hotelRepository;
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? bookingId, string? hotelId, int? roomNumber)
        {
            try
            {
                // Nạp danh sách khách sạn cho dropdown tra cứu
                var hotels = (await _hotelRepository.GetAllHotelsAsync())
                    .OrderBy(h => h.HotelId)
                    .ToList();
                ViewBag.Hotels = hotels;

                var selectedHotelId = !string.IsNullOrWhiteSpace(hotelId)
                    ? hotelId.Trim()
                    : hotels.FirstOrDefault()?.HotelId ?? "HTL001";

                ViewBag.SelectedHotelId = selectedHotelId;

                // Nạp danh sách phòng của khách sạn được chọn cho Combobox phòng
                var availableRooms = (await _roomRepository.GetRoomsByHotelAsync(selectedHotelId))
                    .OrderBy(r => r.RoomNumber)
                    .ToList();
                ViewBag.AvailableRooms = availableRooms;

                ViewBag.SelectedRoomNumber = roomNumber;
                ViewBag.BookingId = bookingId;

                // TRƯỜNG HỢP 1: Tra cứu theo Khách sạn + Số phòng
                if (roomNumber.HasValue && roomNumber.Value > 0)
                {
                    var allHotelBookings = await _bookingRepository.GetByHotelAsync(selectedHotelId);
                    var roomBookings = allHotelBookings
                        .Where(b => b.RoomNumber == roomNumber.Value && !b.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(b => b.CheckInDate)
                        .ToList();

                    if (!roomBookings.Any())
                    {
                        ViewBag.ErrorMessage = $"Không tìm thấy lịch sử đặt phòng nào của phòng {roomNumber.Value} tại chi nhánh được chọn.";
                        return View(null);
                    }

                    // Lấy thông tin hóa đơn cho các lượt đặt phòng của phòng này
                    var roomInvoices = new List<RoomInvoiceItem>();
                    foreach (var b in roomBookings)
                    {
                        var inv = await _invoiceRepository.GetInvoiceByBookingIdAsync(b.BookingId);
                        roomInvoices.Add(new RoomInvoiceItem
                        {
                            BookingId = b.BookingId,
                            InvoiceId = inv?.InvoiceId,
                            GuestId = b.GuestId,
                            GuestName = b.GuestName,
                            CheckInDate = b.CheckInDate,
                            CheckOutDate = b.CheckOutDate,
                            TotalAmount = inv?.TotalAmount ?? b.TotalAmount,
                            PaymentStatus = inv?.Status ?? "UNPAID",
                            IsSelected = (bookingId != null && Guid.TryParse(bookingId, out var g) && g == b.BookingId)
                        });
                    }

                    ViewBag.RoomInvoices = roomInvoices;

                    // Nếu người dùng đã chọn 1 booking cụ thể từ danh sách (hoặc phòng này chỉ có 1 booking)
                    Guid targetBookingId;
                    if (!string.IsNullOrWhiteSpace(bookingId) && Guid.TryParse(bookingId, out var parsedGuid))
                    {
                        targetBookingId = parsedGuid;
                    }
                    else if (roomInvoices.Count == 1)
                    {
                        targetBookingId = roomInvoices[0].BookingId;
                        roomInvoices[0].IsSelected = true;
                    }
                    else
                    {
                        // Hiển thị danh sách các hóa đơn của phòng để lễ tân chọn xem
                        return View(null);
                    }

                    var targetBooking = roomBookings.FirstOrDefault(b => b.BookingId == targetBookingId);
                    ViewBag.CurrentBooking = targetBooking;

                    var selectedInvoice = await _invoiceRepository.GetInvoiceByBookingIdAsync(targetBookingId);
                    if (selectedInvoice == null)
                    {
                        // Nếu chưa có dòng hóa đơn trong DB, tự động tạo hóa đơn hiển thị tạm thời từ booking
                        if (targetBooking != null)
                        {
                            selectedInvoice = new Invoice
                            {
                                BookingId = targetBooking.BookingId,
                                InvoiceId = "INV-" + targetBooking.BookingId.ToString()[..8].ToUpper(),
                                CustomerName = targetBooking.GuestName,
                                RoomCharge = targetBooking.TotalAmount * 0.85m,
                                Tax = targetBooking.TotalAmount * 0.08m,
                                AdditionalFees = targetBooking.TotalAmount * 0.07m,
                                TotalAmount = targetBooking.TotalAmount,
                                IssueDate = DateTime.Now,
                                Status = "UNPAID"
                            };
                        }
                    }

                    return View(selectedInvoice);
                }

                // TRƯỜNG HỢP 2: Tra cứu trực tiếp theo mã bookingId GUID
                if (!string.IsNullOrWhiteSpace(bookingId))
                {
                    if (!Guid.TryParse(bookingId.Trim(), out Guid parsedGuid))
                    {
                        ViewBag.ErrorMessage = "Mã đặt phòng không đúng định dạng GUID. Vui lòng kiểm tra lại.";
                        return View(null);
                    }

                    var invoice = await _invoiceRepository.GetInvoiceByBookingIdAsync(parsedGuid);
                    if (invoice == null)
                    {
                        ViewBag.ErrorMessage = "Không tìm thấy hóa đơn cho mã đặt phòng này.";
                    }
                    else
                    {
                        // Tìm thông tin booking để lấy danh sách người lưu trú
                        try
                        {
                            foreach (var h in hotels)
                            {
                                var hBookings = await _bookingRepository.GetByHotelAsync(h.HotelId);
                                var matched = hBookings.FirstOrDefault(b => b.BookingId == parsedGuid);
                                if (matched != null)
                                {
                                    ViewBag.CurrentBooking = matched;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }

                    return View(invoice);
                }

                // Mặc định: Chưa tìm kiếm
                return View(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý tra cứu hóa đơn");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình tra cứu hóa đơn.";
                return View(null);
            }
        }
    }
}