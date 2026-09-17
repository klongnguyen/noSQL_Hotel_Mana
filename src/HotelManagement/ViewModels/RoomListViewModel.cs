using HotelManagement.Models;

namespace HotelManagement.ViewModels;

public class RoomListViewModel
{
    public string SelectedHotelId { get; set; } = string.Empty;
    public string? SelectedStatus { get; set; }
    public Hotel? CurrentHotel { get; set; }
    public IEnumerable<Hotel> AvailableHotels { get; set; } = new List<Hotel>();
    public IEnumerable<Room> Rooms { get; set; } = new List<Room>();

    // Thông tin kỹ thuật NoSQL phục vụ minh họa và kiểm chứng
    public string QuerySourceTable { get; set; } = "rooms_by_hotel";
    public string ExecutedCqlQuery { get; set; } = string.Empty;
    public bool IsFilteredByStatus => !string.IsNullOrWhiteSpace(SelectedStatus) && !SelectedStatus.Equals("ALL", StringComparison.OrdinalIgnoreCase);
    public int FilteredCount => Rooms.Count();

    // Các thông số thống kê tổng thể của khách sạn (tính trên toàn bộ phòng của hotel_id)
    public int TotalRooms { get; set; }
    public int AvailableCount { get; set; }
    public int OccupiedCount { get; set; }
    public int MaintenanceCount { get; set; }
}

