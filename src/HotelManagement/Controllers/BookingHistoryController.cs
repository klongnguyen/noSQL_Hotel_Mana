using Microsoft.AspNetCore.Mvc;
using HotelManagement.Repositories;
using HotelManagement.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Controllers
{
    public class BookingHistoryController : Controller
    {
        private readonly BookingHistoryRepository _repository;
        private readonly ILogger<BookingHistoryController> _logger;

        public BookingHistoryController(
            BookingHistoryRepository repository,
            ILogger<BookingHistoryController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Action nhận yêu cầu tra cứu. Tham số customerId được chuẩn hóa trim()
        public async Task<IActionResult> Index(string? customerId)
        {
            var trimmedId = customerId?.Trim();
            if (string.IsNullOrEmpty(trimmedId))
            {
                return View(new List<BookingHistory>());
            }

            try
            {
                // Gọi Repository để lấy dữ liệu từ Cassandra DB
                var histories = await _repository.GetHistoryByCustomerIdAsync(trimmedId);
                ViewBag.CustomerId = trimmedId;
                return View(histories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tra cứu lịch sử lưu trú của khách hàng {CustomerId}", trimmedId);
                ViewBag.ErrorMessage = "Không thể tải lịch sử lưu trú. Vui lòng kiểm tra kết nối cơ sở dữ liệu.";
                return View(new List<BookingHistory>());
            }
        }
    }
}