namespace HotelManagement.Models;

public class Booking
{
    public Guid BookingId { get; set; }

    public string GuestId { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;

    public string HotelId { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;

    public int RoomNumber { get; set; }
    public string RoomType { get; set; } = "Standard";
    public decimal RoomPricePerNight { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "CONFIRMED";

    /// <summary>
    /// Số lượng người lưu trú trong phòng
    /// </summary>
    public int NumberOfOccupants { get; set; } = 1;

    /// <summary>
    /// Danh sách người lưu trú (gồm người đại diện và người ở cùng)
    /// </summary>
    public List<RoomOccupant> Occupants { get; set; } = new();

    /// <summary>
    /// Dữ liệu JSON lưu vào Cassandra table (occupants_json)
    /// </summary>
    public string OccupantsJson { get; set; } = "[]";
}
