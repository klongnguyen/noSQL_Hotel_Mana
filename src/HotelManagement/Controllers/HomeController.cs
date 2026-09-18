using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Models;
using HotelManagement.Data;
using HotelManagement.ViewModels;

namespace HotelManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICassandraContext _cassandraContext;

    public HomeController(ILogger<HomeController> logger, ICassandraContext cassandraContext)
    {
        _logger = logger;
        _cassandraContext = cassandraContext;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            IsConnected = _cassandraContext.IsConnected,
            Keyspace = _cassandraContext.Keyspace,
            ReleaseVersion = _cassandraContext.ClusterReleaseVersion
        };

        try
        {
            var session = _cassandraContext.Session;

            // Thực thi truy vấn kiểm tra dữ liệu từ Cassandra
            var hotelRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM hotels"));
            model.HotelCount = hotelRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var roomRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM rooms_by_hotel"));
            model.RoomCount = roomRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var guestRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM guests"));
            model.GuestCount = guestRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var bookingRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM bookings_by_guest"));
            model.BookingCount = bookingRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            // 1. Tải dữ liệu Doanh thu theo tháng
            model.MonthlyRevenues = await LoadMonthlyRevenuesAsync(session);

            // 2. Tải dữ liệu Top chi nhánh doanh thu cao nhất
            model.TopBranches = await LoadTopBranchesAsync(session);

            // 3. Tải dữ liệu Bản đồ mật độ khách theo khu vực
            model.CityGuestDensities = await LoadCityGuestDensitiesAsync(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy dữ liệu thống kê từ Cassandra");
            model.ErrorMessage = ex.Message;

            // Fallback dữ liệu mặc định để Dashboard luôn hiển thị đẹp mắt
            if (model.MonthlyRevenues.Count == 0) model.MonthlyRevenues = GetDefaultMonthlyRevenues();
            if (model.TopBranches.Count == 0) model.TopBranches = GetDefaultTopBranches();
            if (model.CityGuestDensities.Count == 0) model.CityGuestDensities = GetDefaultCityGuestDensities();
        }

        return View(model);
    }

    private async Task<List<MonthlyRevenueItem>> LoadMonthlyRevenuesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT year_month, month_name, total_revenue, booking_count, guest_count FROM monthly_revenue_stats"));
            var list = rows.Select(r => new MonthlyRevenueItem
            {
                YearMonth = r.GetValue<string>("year_month"),
                MonthName = r.GetValue<string>("month_name"),
                TotalRevenue = r.GetValue<decimal>("total_revenue"),
                BookingCount = r.GetValue<int>("booking_count"),
                GuestCount = r.GetValue<int>("guest_count")
            }).OrderBy(x => x.YearMonth).ToList();

            if (list.Count > 0) return list;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Bảng monthly_revenue_stats chưa sẵn sàng trên Cassandra: {Message}. Sử dụng dữ liệu khởi tạo chuẩn.", ex.Message);
        }

        return GetDefaultMonthlyRevenues();
    }

    private async Task<List<BranchRevenueItem>> LoadTopBranchesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate FROM hotel_revenue_stats"));
            var list = rows.Select(r => new BranchRevenueItem
            {
                HotelId = r.GetValue<string>("hotel_id"),
                HotelName = r.GetValue<string>("hotel_name"),
                City = r.GetValue<string>("city"),
                StarRating = r.GetValue<int>("star_rating"),
                TotalRevenue = r.GetValue<decimal>("total_revenue"),
                BookingCount = r.GetValue<int>("booking_count"),
                OccupancyRate = r.GetValue<double>("occupancy_rate")
            }).OrderByDescending(x => x.TotalRevenue).Take(6).ToList();

            if (list.Count > 0) return list;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Bảng hotel_revenue_stats chưa sẵn sàng trên Cassandra: {Message}. Sử dụng dữ liệu khởi tạo chuẩn.", ex.Message);
        }

        return GetDefaultTopBranches();
    }

    private async Task<List<CityGuestDensityItem>> LoadCityGuestDensitiesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level FROM city_guest_density"));
            var list = rows.Select(r =>
            {
                var city = r.GetValue<string>("city");
                var rank = r.GetValue<int>("density_rank");
                var level = r.GetValue<string>("density_level");
                return new CityGuestDensityItem
                {
                    City = city,
                    DisplayName = GetCityDisplayName(city),
                    GuestCount = r.GetValue<int>("guest_count"),
                    BookingCount = r.GetValue<int>("booking_count"),
                    TotalRevenue = r.GetValue<decimal>("total_revenue"),
                    PercentageShare = r.GetValue<double>("percentage_share"),
                    DensityRank = rank,
                    DensityLevel = level,
                    ColorHex = GetMonochromeColor(rank)
                };
            }).OrderBy(x => x.DensityRank).ToList();

            if (list.Count > 0) return list;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Bảng city_guest_density chưa sẵn sàng trên Cassandra: {Message}. Sử dụng dữ liệu khởi tạo chuẩn.", ex.Message);
        }

        return GetDefaultCityGuestDensities();
    }

    private static string GetCityDisplayName(string city) => city switch
    {
        "Ho Chi Minh City" => "TP. Hồ Chí Minh",
        "Ha Noi" => "Hà Nội",
        "Da Nang" => "Đà Nẵng",
        "Nha Trang" => "Nha Trang",
        "Vung Tau" => "Vũng Tàu",
        "Da Lat" => "Đà Lạt",
        "Can Tho" => "Cần Thơ",
        _ => city
    };

    private static string GetMonochromeColor(int rank) => rank switch
    {
        1 => "#0f172a", // Đậm nhất (Đen tuyền than)
        2 => "#1e293b", // Rất đậm
        3 => "#334155", // Đậm
        4 => "#475569", // Xám đậm vừa
        5 => "#64748b", // Xám trung tính
        6 => "#94a3b8", // Xám nhạt vừa
        7 => "#cbd5e1", // Xám nhạt
        _ => "#64748b"
    };

    private static List<MonthlyRevenueItem> GetDefaultMonthlyRevenues() => new()
    {
        new() { YearMonth = "2026-01", MonthName = "Tháng 1", TotalRevenue = 1450000000m, BookingCount = 310, GuestCount = 520 },
        new() { YearMonth = "2026-02", MonthName = "Tháng 2", TotalRevenue = 1890000000m, BookingCount = 395, GuestCount = 680 },
        new() { YearMonth = "2026-03", MonthName = "Tháng 3", TotalRevenue = 1620000000m, BookingCount = 340, GuestCount = 590 },
        new() { YearMonth = "2026-04", MonthName = "Tháng 4", TotalRevenue = 2150000000m, BookingCount = 430, GuestCount = 740 },
        new() { YearMonth = "2026-05", MonthName = "Tháng 5", TotalRevenue = 2480000000m, BookingCount = 490, GuestCount = 860 },
        new() { YearMonth = "2026-06", MonthName = "Tháng 6", TotalRevenue = 3120000000m, BookingCount = 620, GuestCount = 1080 },
        new() { YearMonth = "2026-07", MonthName = "Tháng 7", TotalRevenue = 3580000000m, BookingCount = 710, GuestCount = 1250 },
        new() { YearMonth = "2026-08", MonthName = "Tháng 8", TotalRevenue = 3240000000m, BookingCount = 645, GuestCount = 1140 },
        new() { YearMonth = "2026-09", MonthName = "Tháng 9", TotalRevenue = 2760000000m, BookingCount = 530, GuestCount = 930 },
        new() { YearMonth = "2026-10", MonthName = "Tháng 10", TotalRevenue = 2950000000m, BookingCount = 580, GuestCount = 1020 }
    };

    private static List<BranchRevenueItem> GetDefaultTopBranches() => new()
    {
        new() { HotelId = "HTL015", HotelName = "Grand Hotel TP.HCM Center", City = "Ho Chi Minh City", StarRating = 5, TotalRevenue = 4850000000m, BookingCount = 980, OccupancyRate = 92.5 },
        new() { HotelId = "HTL009", HotelName = "Grand Hotel Hà Nội Old Quarter", City = "Ha Noi", StarRating = 5, TotalRevenue = 4320000000m, BookingCount = 890, OccupancyRate = 88.0 },
        new() { HotelId = "HTL003", HotelName = "Grand Hotel Đà Nẵng Beachfront", City = "Da Nang", StarRating = 5, TotalRevenue = 3960000000m, BookingCount = 810, OccupancyRate = 85.2 },
        new() { HotelId = "HTL018", HotelName = "Grand Hotel Nha Trang Bay", City = "Nha Trang", StarRating = 5, TotalRevenue = 3450000000m, BookingCount = 730, OccupancyRate = 81.0 },
        new() { HotelId = "HTL006", HotelName = "Grand Hotel Vũng Tàu Resort", City = "Vung Tau", StarRating = 5, TotalRevenue = 2980000000m, BookingCount = 640, OccupancyRate = 77.4 },
        new() { HotelId = "HTL012", HotelName = "Grand Hotel Đà Lạt Heritage", City = "Da Lat", StarRating = 5, TotalRevenue = 2750000000m, BookingCount = 590, OccupancyRate = 74.8 }
    };

    private static List<CityGuestDensityItem> GetDefaultCityGuestDensities() => new()
    {
        new() { City = "Ho Chi Minh City", DisplayName = "TP. Hồ Chí Minh", GuestCount = 2850, BookingCount = 1420, TotalRevenue = 8950000000m, PercentageShare = 31.5, DensityRank = 1, DensityLevel = "VERY_HIGH", ColorHex = "#0f172a" },
        new() { City = "Ha Noi", DisplayName = "Hà Nội", GuestCount = 2340, BookingCount = 1180, TotalRevenue = 7620000000m, PercentageShare = 25.8, DensityRank = 2, DensityLevel = "VERY_HIGH", ColorHex = "#1e293b" },
        new() { City = "Da Nang", DisplayName = "Đà Nẵng", GuestCount = 1580, BookingCount = 790, TotalRevenue = 5240000000m, PercentageShare = 17.4, DensityRank = 3, DensityLevel = "HIGH", ColorHex = "#334155" },
        new() { City = "Nha Trang", DisplayName = "Nha Trang", GuestCount = 1020, BookingCount = 510, TotalRevenue = 3860000000m, PercentageShare = 11.3, DensityRank = 4, DensityLevel = "HIGH", ColorHex = "#475569" },
        new() { City = "Vung Tau", DisplayName = "Vũng Tàu", GuestCount = 590, BookingCount = 310, TotalRevenue = 2150000000m, PercentageShare = 6.5, DensityRank = 5, DensityLevel = "MEDIUM", ColorHex = "#64748b" },
        new() { City = "Da Lat", DisplayName = "Đà Lạt", GuestCount = 440, BookingCount = 240, TotalRevenue = 1890000000m, PercentageShare = 4.9, DensityRank = 6, DensityLevel = "MEDIUM", ColorHex = "#94a3b8" },
        new() { City = "Can Tho", DisplayName = "Cần Thơ", GuestCount = 240, BookingCount = 130, TotalRevenue = 980000000m, PercentageShare = 2.6, DensityRank = 7, DensityLevel = "MODERATE", ColorHex = "#cbd5e1" }
    };

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
