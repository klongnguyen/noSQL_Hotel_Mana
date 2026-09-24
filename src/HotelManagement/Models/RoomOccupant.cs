namespace HotelManagement.Models;

/// <summary>
/// Đại diện cho thông tin người lưu trú cùng phòng (không cần tạo tài khoản trong hệ thống)
/// </summary>
public class RoomOccupant
{
    /// <summary>
    /// Họ và tên người lưu trú
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Số CCCD / CMND hoặc Hộ chiếu
    /// </summary>
    public string CitizenId { get; set; } = string.Empty;

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Đánh dấu là người đại diện đặt phòng (Primary Booker) hay người đi cùng
    /// </summary>
    public bool IsPrimary { get; set; } = false;
}
