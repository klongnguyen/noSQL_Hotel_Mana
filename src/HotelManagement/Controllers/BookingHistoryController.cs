using Microsoft.AspNetCore.Mvc;
using HotelManagement.Repositories;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    public class BookingHistoryController : Controller
    {
        private readonly BookingHistoryRepository _repository;

        // Tiêm (Inject) BookingHistoryRepository vào Controller
        public BookingHistoryController(BookingHistoryRepository repository)
        {
            _repository = repository;
        }

        // Action nhận yêu cầu tra cứu. Tham số customerId có thể rỗng khi mới load trang.
        public async Task<IActionResult> Index(string customerId)
        {
            // Nếu người dùng chưa nhập mã khách hàng, truyền sang View một danh sách rỗng
            if (string.IsNullOrEmpty(customerId))
            {
                return View(new List<BookingHistory>());
            }

            // Gọi Repository để lấy dữ liệu từ Cassandra DB
            var histories = await _repository.GetHistoryByCustomerIdAsync(customerId);
            
            // Trả danh sách kết quả về cho View hiển thị
            return View(histories);
        }
    }
}