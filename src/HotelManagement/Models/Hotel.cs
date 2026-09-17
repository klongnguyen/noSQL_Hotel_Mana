namespace HotelManagement.Models;

public class Hotel
{
    public string HotelId { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int StarRating { get; set; } = 3;
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Hiển thị số sao dạng chuỗi ký tự ngôi sao ★★★☆☆
    /// </summary>
    public string StarDisplay => new string('★', Math.Clamp(StarRating, 1, 5)) + new string('☆', Math.Max(0, 5 - StarRating));
}
