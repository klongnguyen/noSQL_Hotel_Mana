using HotelManagement.Models;

namespace HotelManagement.ViewModels;

public class RoomListViewModel
{
    public string SelectedHotelId { get; set; } = string.Empty;
    public Hotel? CurrentHotel { get; set; }
    public IEnumerable<Hotel> AvailableHotels { get; set; } = new List<Hotel>();
    public IEnumerable<Room> Rooms { get; set; } = new List<Room>();

    // Các thông số thống kê phục vụ Admin Dashboard
    public int TotalRooms => Rooms.Count();
    public int AvailableCount => Rooms.Count(r => r.Status.Equals("AVAILABLE", StringComparison.OrdinalIgnoreCase));
    public int OccupiedCount => Rooms.Count(r => r.Status.Equals("OCCUPIED", StringComparison.OrdinalIgnoreCase));
    public int MaintenanceCount => Rooms.Count(r => r.Status.Equals("MAINTENANCE", StringComparison.OrdinalIgnoreCase));
}
