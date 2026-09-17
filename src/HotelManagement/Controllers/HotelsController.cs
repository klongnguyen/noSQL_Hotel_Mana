using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers;

public class HotelsController : Controller
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        ILogger<HotelsController> logger)
    {
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _logger = logger;
    }

    /// <summary>
    /// Trang danh sách khách sạn kèm bộ lọc Query-First theo Thành phố và Số sao
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? city, int? starRating, string? search)
    {
        try
        {
            IEnumerable<Hotel> hotels;

            // 1. Nếu người dùng chọn Thành phố: Query trực tiếp vào bảng hotels_by_city (Chuẩn Query-First Cassandra)
            if (!string.IsNullOrWhiteSpace(city))
            {
                hotels = await _hotelRepository.GetHotelsByCityAsync(city);
            }
            else
            {
                // Nếu xem tất cả: Query vào bảng hotels
                hotels = await _hotelRepository.GetAllHotelsAsync();
            }

            // 2. Lọc thêm theo số sao (nếu có)
            if (starRating.HasValue && starRating.Value > 0)
            {
                hotels = hotels.Where(h => h.StarRating == starRating.Value);
            }

            // 3. Tìm kiếm theo từ khóa tên hoặc địa chỉ (nếu có)
            if (!string.IsNullOrWhiteSpace(search))
            {
                hotels = hotels.Where(h =>
                    h.HotelName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    h.Address.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var availableCities = await _hotelRepository.GetAvailableCitiesAsync();

            var viewModel = new HotelListViewModel
            {
                Hotels = hotels.ToList(),
                SelectedCity = city,
                SelectedStarRating = starRating,
                SearchKeyword = search,
                AvailableCities = availableCities
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi nạp danh sách khách sạn");
            TempData["ErrorMessage"] = "Không thể kết nối đến CSDL Cassandra hoặc xảy ra lỗi tải dữ liệu.";
            return View(new HotelListViewModel());
        }
    }

    /// <summary>
    /// Xem thông tin chi tiết một khách sạn theo ID
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                ViewBag.HotelId = id;
                return View("HotelNotFound");
            }

            // Lấy danh sách phòng thuộc khách sạn này từ bảng rooms_by_hotel
            var rooms = (await _roomRepository.GetRoomsByHotelAsync(id)).ToList();
            ViewBag.FeaturedRooms = rooms.Take(2).ToList();
            ViewBag.TotalRooms = rooms.Count;

            return View(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy chi tiết khách sạn ID: {HotelId}", id);
            TempData["ErrorMessage"] = $"Lỗi khi tải thông tin khách sạn {id}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
