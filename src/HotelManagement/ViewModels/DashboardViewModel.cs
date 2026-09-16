namespace HotelManagement.ViewModels;

public class DashboardViewModel
{
    public bool IsConnected { get; set; }
    public string Keyspace { get; set; } = string.Empty;
    public string? ReleaseVersion { get; set; }
    public long HotelCount { get; set; }
    public long RoomCount { get; set; }
    public long GuestCount { get; set; }
    public long BookingCount { get; set; }
    public string? ErrorMessage { get; set; }
}
