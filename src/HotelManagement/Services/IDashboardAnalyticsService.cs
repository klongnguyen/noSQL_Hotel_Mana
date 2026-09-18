using HotelManagement.Models;
using HotelManagement.ViewModels;

namespace HotelManagement.Services;

public interface IDashboardAnalyticsService
{
    /// <summary>
    /// Ghi nhận đơn đặt phòng mới và cập nhật lũy kế theo thời gian thực vào các bảng thống kê Dashboard (Cassandra).
    /// </summary>
    Task RecordBookingAsync(Booking booking);

    /// <summary>
    /// Tải dữ liệu phân tích động cho Dashboard: tổng hợp từ các bảng thống kê kết hợp dữ liệu booking/invoice thực tế.
    /// </summary>
    Task<DashboardViewModel> GetLiveDashboardDataAsync();

    /// <summary>
    /// Tự động quét và đồng bộ lại toàn bộ các bảng thống kê từ dữ liệu đơn đặt phòng thực tế trong hệ thống.
    /// </summary>
    Task SyncHistoricalAnalyticsAsync();
}
