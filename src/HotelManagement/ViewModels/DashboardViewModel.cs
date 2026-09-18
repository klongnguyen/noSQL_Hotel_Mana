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

    // Dữ liệu cho các biểu đồ Dashboard
    public List<MonthlyRevenueItem> MonthlyRevenues { get; set; } = new();
    public List<BranchRevenueItem> TopBranches { get; set; } = new();
    public List<CityGuestDensityItem> CityGuestDensities { get; set; } = new();

    // Các chỉ số tài chính tổng hợp
    public decimal TotalAnnualRevenue => MonthlyRevenues.Sum(x => x.TotalRevenue);
    public long TotalAnnualGuests => CityGuestDensities.Sum(x => x.GuestCount);
}

public class MonthlyRevenueItem
{
    public string YearMonth { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int BookingCount { get; set; }
    public int GuestCount { get; set; }
}

public class BranchRevenueItem
{
    public string HotelId { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public decimal TotalRevenue { get; set; }
    public int BookingCount { get; set; }
    public double OccupancyRate { get; set; }
}

public class CityGuestDensityItem
{
    public string City { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public int BookingCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public double PercentageShare { get; set; }
    public int DensityRank { get; set; }
    public string DensityLevel { get; set; } = "MEDIUM"; // VERY_HIGH, HIGH, MEDIUM, MODERATE
    public string ColorHex { get; set; } = "#4B5563";     // Mã màu độ đậm nhạt monochrome
}

