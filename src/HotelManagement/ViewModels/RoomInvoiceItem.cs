namespace HotelManagement.ViewModels;

public class RoomInvoiceItem
{
    public Guid BookingId { get; set; }
    public string? InvoiceId { get; set; }
    public string GuestId { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "UNPAID";
    public bool IsSelected { get; set; }
}
