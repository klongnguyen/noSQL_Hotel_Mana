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

    [HttpGet]
    public async Task<IActionResult> Index(string? hotelId, string? status)
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

            // 1. Nạp danh sách toàn bộ phòng thuộc khách sạn để tổng hợp số liệu KPI tổng thể
            var allHotelRooms = (await _roomRepository.GetRoomsByHotelAsync(hotelId)).ToList();
            var totalRooms = allHotelRooms.Count;
            var availableCount = allHotelRooms.Count(r => r.Status.Equals("AVAILABLE", StringComparison.OrdinalIgnoreCase));
            var occupiedCount = allHotelRooms.Count(r => r.Status.Equals("OCCUPIED", StringComparison.OrdinalIgnoreCase));
            var maintenanceCount = allHotelRooms.Count(r => r.Status.Equals("MAINTENANCE", StringComparison.OrdinalIgnoreCase));

            // 2. Kiểm tra điều kiện lọc theo Trạng thái (Story-105)
            IEnumerable<Room> displayRooms;
            string querySourceTable;
            string executedCql;
            var isFilteringStatus = !string.IsNullOrWhiteSpace(status) && !status.Equals("ALL", StringComparison.OrdinalIgnoreCase);

            if (isFilteringStatus)
            {
                var normStatus = status!.Trim().ToUpperInvariant();
                // Query trực tiếp vào bảng chuyên dụng rooms_by_hotel_statu
                displayRooms = await _roomRepository.GetRoomsByStatusAsync(hotelId, normStatus);
                querySourceTable = "rooms_by_hotel_status";
                executedCql = $"SELECT hotel_id, status, room_number, room_type, price_per_night FROM rooms_by_hotel_status WHERE hotel_id = '{hotelId}' AND status = '{normStatus}';";
            }
            else
            {
                // Mặc định lấy từ bảng rooms_by_hotel
                displayRooms = allHotelRooms;
                querySourceTable = "rooms_by_hotel";
                executedCql = $"SELECT hotel_id, room_number, room_type, price_per_night, status FROM rooms_by_hotel WHERE hotel_id = '{hotelId}';";
            }

            var viewModel = new RoomListViewModel
            {
                SelectedHotelId = hotelId,
                SelectedStatus = isFilteringStatus ? status!.Trim().ToUpperInvariant() : "ALL",
                CurrentHotel = currentHotel,
                AvailableHotels = hotels,
                Rooms = displayRooms,
                QuerySourceTable = querySourceTable,
                ExecutedCqlQuery = executedCql,
                TotalRooms = totalRooms,
                AvailableCount = availableCount,
                OccupiedCount = occupiedCount,
                MaintenanceCount = maintenanceCount
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi nạp danh sách phòng cho khách sạn ID: {HotelId}, status: {Status}", hotelId, status);
            TempData["ErrorMessage"] = $"Lỗi khi tải danh sách phòng của khách sạn {hotelId}.";
            return View(new RoomListViewModel());
        }
    }
}
