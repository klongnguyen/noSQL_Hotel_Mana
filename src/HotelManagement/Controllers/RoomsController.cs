using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers;

public class RoomsController : Controller
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        ILogger<RoomsController> logger)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    /// <summary>
    /// Trang quản lý và tra cứu danh sách phòng theo Khách sạn (Module M3 - STORY-104)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? hotelId)
    {
        try
        {
            var hotels = (await _hotelRepository.GetAllHotelsAsync())
                .OrderBy(h => h.HotelId)
                .ToList();

            // Nếu chưa chọn khách sạn cụ thể: Mặc định chọn khách sạn đầu tiên theo Lựa chọn 1 của Admin
            if (string.IsNullOrWhiteSpace(hotelId))
            {
                hotelId = hotels.FirstOrDefault()?.HotelId ?? "HTL001";
            }

            var currentHotel = hotels.FirstOrDefault(h => h.HotelId == hotelId)
                               ?? await _hotelRepository.GetHotelByIdAsync(hotelId);

            var rooms = await _roomRepository.GetRoomsByHotelAsync(hotelId);

            var viewModel = new RoomListViewModel
            {
                SelectedHotelId = hotelId,
                CurrentHotel = currentHotel,
                AvailableHotels = hotels,
                Rooms = rooms
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi nạp danh sách phòng cho khách sạn ID: {HotelId}", hotelId);
            TempData["ErrorMessage"] = $"Lỗi khi tải danh sách phòng của khách sạn {hotelId}.";
            return View(new RoomListViewModel());
        }
    }
}
