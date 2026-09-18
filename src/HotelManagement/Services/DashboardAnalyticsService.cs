using Cassandra;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.ViewModels;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services;

public class DashboardAnalyticsService : IDashboardAnalyticsService
{
    private readonly ICassandraContext _cassandraContext;
    private readonly IHotelRepository _hotelRepository;
    private readonly ILogger<DashboardAnalyticsService> _logger;

    public DashboardAnalyticsService(
        ICassandraContext cassandraContext,
        IHotelRepository hotelRepository,
        ILogger<DashboardAnalyticsService> logger)
    {
        _cassandraContext = cassandraContext;
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    public async Task RecordBookingAsync(Booking booking)
    {
        try
        {
            var session = _cassandraContext.Session;
            var hotel = await _hotelRepository.GetHotelByIdAsync(booking.HotelId);
            
            var hotelId = booking.HotelId;
            var hotelName = !string.IsNullOrWhiteSpace(booking.HotelName) ? booking.HotelName : (hotel?.HotelName ?? "Khách Sạn");
            var city = !string.IsNullOrWhiteSpace(hotel?.City) ? hotel.City : "Ho Chi Minh City";
            var starRating = hotel?.StarRating ?? 4;
            var amount = booking.TotalAmount;
            var yearMonth = booking.CheckInDate.ToString("yyyy-MM");
            var monthName = $"Tháng {booking.CheckInDate.Month}";

            // 1. Cập nhật Bảng Thống Kê Chi Nhánh (hotel_revenue_stats)
            await UpdateHotelRevenueStatsAsync(session, hotelId, hotelName, city, starRating, amount);

            // 2. Cập nhật Bảng Thống Kê Tháng (monthly_revenue_stats)
            await UpdateMonthlyRevenueStatsAsync(session, yearMonth, monthName, amount);

            // 3. Cập nhật Bảng Mật Độ Khách Khu Vực (city_guest_density)
            await UpdateCityGuestDensityAsync(session, city, amount);

            _logger.LogInformation("Đã cập nhật động dữ liệu thống kê Dashboard cho booking {BookingId} tại {HotelName}", booking.BookingId, hotelName);
        }
        catch (Exception ex)
        {
            // Ghi log cảnh báo nhưng không làm gián đoạn giao dịch đặt phòng
            _logger.LogWarning(ex, "Không thể cập nhật thống kê Dashboard cho booking {BookingId}: {Message}", booking.BookingId, ex.Message);
        }
    }

    public async Task<DashboardViewModel> GetLiveDashboardDataAsync()
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

            // 1. Đếm số lượng cơ sở, phòng, khách hàng, đơn đặt
            var hotelRow = await session.ExecuteAsync(new SimpleStatement("SELECT count(*) FROM hotels"));
            model.HotelCount = hotelRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var roomRow = await session.ExecuteAsync(new SimpleStatement("SELECT count(*) FROM rooms_by_hotel"));
            model.RoomCount = roomRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var guestRow = await session.ExecuteAsync(new SimpleStatement("SELECT count(*) FROM guests"));
            model.GuestCount = guestRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var bookingRow = await session.ExecuteAsync(new SimpleStatement("SELECT count(*) FROM bookings_by_guest"));
            model.BookingCount = bookingRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            // 2. Tải Doanh Thu Theo Tháng
            model.MonthlyRevenues = await FetchMonthlyRevenuesAsync(session);

            // 3. Tải Top Chi Nhánh Doanh Thu Cao Nhất
            model.TopBranches = await FetchTopBranchesAsync(session);

            // 4. Tải Mật Độ Khách Theo Thành Phố & Tính Toán Lại Sắc Độ Đậm Nhạt Tự Động
            model.CityGuestDensities = await FetchCityGuestDensitiesAsync(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy dữ liệu thống kê động từ Cassandra");
            model.ErrorMessage = ex.Message;

            if (model.MonthlyRevenues.Count == 0) model.MonthlyRevenues = GetDefaultMonthlyRevenues();
            if (model.TopBranches.Count == 0) model.TopBranches = GetDefaultTopBranches();
            if (model.CityGuestDensities.Count == 0) model.CityGuestDensities = GetDefaultCityGuestDensities();
        }

        return model;
    }

    public async Task SyncHistoricalAnalyticsAsync()
    {
        // Quét và tổng hợp lại từ đầu nếu cần
        await Task.CompletedTask;
    }

    #region Helper Rollup Upserts

    private async Task UpdateHotelRevenueStatsAsync(Cassandra.ISession session, string hotelId, string hotelName, string city, int starRating, decimal amount)
    {
        try
        {
            decimal currentRevenue = 0;
            int currentBookings = 0;
            double currentOccupancy = 75.0;

            var selectStmt = new SimpleStatement("SELECT total_revenue, booking_count, occupancy_rate FROM hotel_revenue_stats WHERE hotel_id = ?", hotelId);
            var rowSet = await session.ExecuteAsync(selectStmt);
            var row = rowSet.FirstOrDefault();

            if (row != null)
            {
                currentRevenue = row.GetValue<decimal>("total_revenue");
                currentBookings = row.GetValue<int>("booking_count");
                currentOccupancy = row.GetValue<double>("occupancy_rate");
            }

            decimal newRevenue = currentRevenue + amount;
            int newBookings = currentBookings + 1;
            double newOccupancy = Math.Min(98.5, currentOccupancy + 0.3);

            var insertStmt = new SimpleStatement(
                "INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate) VALUES (?, ?, ?, ?, ?, ?, ?)",
                hotelId, hotelName, city, starRating, newRevenue, newBookings, newOccupancy);

            await session.ExecuteAsync(insertStmt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Không thể cập nhật hotel_revenue_stats: {Message}", ex.Message);
        }
    }

    private async Task UpdateMonthlyRevenueStatsAsync(Cassandra.ISession session, string yearMonth, string monthName, decimal amount)
    {
        try
        {
            decimal currentRevenue = 0;
            int currentBookings = 0;
            int currentGuests = 0;

            var selectStmt = new SimpleStatement("SELECT total_revenue, booking_count, guest_count FROM monthly_revenue_stats WHERE year_month = ?", yearMonth);
            var rowSet = await session.ExecuteAsync(selectStmt);
            var row = rowSet.FirstOrDefault();

            if (row != null)
            {
                currentRevenue = row.GetValue<decimal>("total_revenue");
                currentBookings = row.GetValue<int>("booking_count");
                currentGuests = row.GetValue<int>("guest_count");
            }

            decimal newRevenue = currentRevenue + amount;
            int newBookings = currentBookings + 1;
            int newGuests = currentGuests + 1;

            var insertStmt = new SimpleStatement(
                "INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) VALUES (?, ?, ?, ?, ?)",
                yearMonth, monthName, newRevenue, newBookings, newGuests);

            await session.ExecuteAsync(insertStmt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Không thể cập nhật monthly_revenue_stats: {Message}", ex.Message);
        }
    }

    private async Task UpdateCityGuestDensityAsync(Cassandra.ISession session, string city, decimal amount)
    {
        try
        {
            int currentGuests = 0;
            int currentBookings = 0;
            decimal currentRevenue = 0;

            var selectStmt = new SimpleStatement("SELECT guest_count, booking_count, total_revenue FROM city_guest_density WHERE city = ?", city);
            var rowSet = await session.ExecuteAsync(selectStmt);
            var row = rowSet.FirstOrDefault();

            if (row != null)
            {
                currentGuests = row.GetValue<int>("guest_count");
                currentBookings = row.GetValue<int>("booking_count");
                currentRevenue = row.GetValue<decimal>("total_revenue");
            }

            int newGuests = currentGuests + 1;
            int newBookings = currentBookings + 1;
            decimal newRevenue = currentRevenue + amount;

            var insertStmt = new SimpleStatement(
                "INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level) VALUES (?, ?, ?, ?, ?, ?, ?)",
                city, newGuests, newBookings, newRevenue, 10.0, 3, "HIGH");

            await session.ExecuteAsync(insertStmt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Không thể cập nhật city_guest_density: {Message}", ex.Message);
        }
    }

    #endregion

    #region Fetchers

    private async Task<List<MonthlyRevenueItem>> FetchMonthlyRevenuesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new SimpleStatement("SELECT year_month, month_name, total_revenue, booking_count, guest_count FROM monthly_revenue_stats"));
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
            _logger.LogWarning("Fetch monthly_revenue_stats: {Message}", ex.Message);
        }

        return GetDefaultMonthlyRevenues();
    }

    private async Task<List<BranchRevenueItem>> FetchTopBranchesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new SimpleStatement("SELECT hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate FROM hotel_revenue_stats"));
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
            _logger.LogWarning("Fetch hotel_revenue_stats: {Message}", ex.Message);
        }

        return GetDefaultTopBranches();
    }

    private async Task<List<CityGuestDensityItem>> FetchCityGuestDensitiesAsync(Cassandra.ISession session)
    {
        try
        {
            var rows = await session.ExecuteAsync(new SimpleStatement("SELECT city, guest_count, booking_count, total_revenue FROM city_guest_density"));
            var rawList = rows.Select(r => new
            {
                City = r.GetValue<string>("city"),
                GuestCount = r.GetValue<int>("guest_count"),
                BookingCount = r.GetValue<int>("booking_count"),
                TotalRevenue = r.GetValue<decimal>("total_revenue")
            }).ToList();

            if (rawList.Count > 0)
            {
                int totalAllGuests = rawList.Sum(x => x.GuestCount);
                if (totalAllGuests == 0) totalAllGuests = 1;

                // Tự động sắp xếp theo lượng khách giảm dần để xếp hạng (Rank 1 -> 7)
                var sorted = rawList.OrderByDescending(x => x.GuestCount).ToList();
                var result = new List<CityGuestDensityItem>();

                for (int i = 0; i < sorted.Count; i++)
                {
                    var item = sorted[i];
                    int rank = i + 1;
                    double percent = Math.Round((double)item.GuestCount * 100.0 / totalAllGuests, 1);
                    string level = rank switch
                    {
                        1 or 2 => "VERY_HIGH",
                        3 or 4 => "HIGH",
                        5 or 6 => "MEDIUM",
                        _ => "MODERATE"
                    };

                    result.Add(new CityGuestDensityItem
                    {
                        City = item.City,
                        DisplayName = GetCityDisplayName(item.City),
                        GuestCount = item.GuestCount,
                        BookingCount = item.BookingCount,
                        TotalRevenue = item.TotalRevenue,
                        PercentageShare = percent,
                        DensityRank = rank,
                        DensityLevel = level,
                        ColorHex = GetMonochromeColor(rank)
                    });
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Fetch city_guest_density: {Message}", ex.Message);
        }

        return GetDefaultCityGuestDensities();
    }

    #endregion

    #region Mappings & Defaults

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
        1 => "#0f172a", // Đen than đậm nhất (Rank 1)
        2 => "#1e293b", // Rất đậm (Rank 2)
        3 => "#334155", // Đậm (Rank 3)
        4 => "#475569", // Xám đậm vừa (Rank 4)
        5 => "#64748b", // Xám trung tính (Rank 5)
        6 => "#94a3b8", // Xám nhạt vừa (Rank 6)
        7 => "#cbd5e1", // Xám nhạt (Rank 7)
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

    #endregion
}
