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
}
