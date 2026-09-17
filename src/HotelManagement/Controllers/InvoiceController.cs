using Microsoft.AspNetCore.Mvc;
using HotelManagement.Repositories;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository _repository;

        public InvoiceController(InvoiceRepository repository)
        {
            _repository = repository;
        }

        // Action nhận mã bookingId kiểu chuỗi (string) để tránh lỗi nếu người dùng nhập sai định dạng
        public async Task<IActionResult> Index(string bookingId)
        {
            // Nếu chưa nhập mã, truyền null sang View để hiển thị ô tìm kiếm
            if (string.IsNullOrEmpty(bookingId))
            {
                return View(null);
            }

            // Kiểm tra xem mã nhập vào có đúng chuẩn Guid không (tránh lỗi sập web)
            if (!Guid.TryParse(bookingId, out Guid parsedGuid))
            {
                ViewBag.ErrorMessage = "Mã đặt phòng không đúng định dạng. Vui lòng kiểm tra lại.";
                return View(null);
            }

            // Gọi Repository để tìm hóa đơn
            var invoice = await _repository.GetInvoiceByBookingIdAsync(parsedGuid);
            
            if (invoice == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy hóa đơn cho mã đặt phòng này.";
            }

            return View(invoice);
        }
    }
}