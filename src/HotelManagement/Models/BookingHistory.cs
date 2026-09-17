namespace HotelManagement.Models
{
    public class BookingHistory
    {
        public string? CustomerId { get; set; } 
        public Guid BookingId { get; set; }
        public string? HotelId { get; set; }
        public string? HotelName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
}