namespace HotelManagement.Models;

public class Room
{
    public string HotelId { get; set; } = string.Empty;
    public int RoomNumber { get; set; }
    public string RoomType { get; set; } = string.Empty; // Standard, Deluxe, Suite
    public decimal PricePerNight { get; set; }
    public string Status { get; set; } = "AVAILABLE"; // AVAILABLE, OCCUPIED, MAINTENANCE

    /// <summary>
    /// Sức chứa tối đa của phòng (Standard: 2, Deluxe: 3, Suite: 4)
    /// </summary>
    public int Capacity { get; set; } = 2;

    /// <summary>
    /// Hiển thị giá tiền dạng chuẩn Việt Nam: ví dụ "800.000 VND"
    /// </summary>
    public string PriceFormatted => $"{PricePerNight:N0} VND";

    /// <summary>
    /// Tên trạng thái tiếng Việt thân thiện cho Admin
    /// </summary>
    public string StatusDisplay => Status.ToUpperInvariant() switch
    {
        "AVAILABLE" => "Trống / Sẵn sàng",
        "OCCUPIED" => "Đang có khách",
        "MAINTENANCE" => "Đang bảo trì",
        _ => Status
    };

    /// <summary>
    /// Class màu sắc Bootstrap Badge theo trạng thái
    /// </summary>
    public string StatusBadgeClass => Status.ToUpperInvariant() switch
    {
        "AVAILABLE" => "bg-success-subtle text-success border border-success-subtle",
        "OCCUPIED" => "bg-danger-subtle text-danger border border-danger-subtle",
        "MAINTENANCE" => "bg-warning-subtle text-warning-emphasis border border-warning-subtle",
        _ => "bg-secondary-subtle text-secondary border border-secondary-subtle"
    };

    /// <summary>
    /// Icon đại diện cho trạng thái
    /// </summary>
    public string StatusIcon => Status.ToUpperInvariant() switch
    {
        "AVAILABLE" => "bi-check-circle-fill text-success",
        "OCCUPIED" => "bi-person-fill text-danger",
        "MAINTENANCE" => "bi-tools text-warning",
        _ => "bi-question-circle"
    };
}
