namespace HotelManagement.Models
{
    public class Invoice
    {
        // BookingId là khóa chính (Partition Key) để tìm đúng hóa đơn của một lượt đặt phòng
        public Guid BookingId { get; set; }
        
        public string? InvoiceId { get; set; }
        
        public string? CustomerName { get; set; }
        
        // Các khoản phí theo đúng yêu cầu từ Jira (STORY-302)
        public decimal RoomCharge { get; set; } // Tiền phòng
        
        public decimal Tax { get; set; } // Thuế
        
        public decimal AdditionalFees { get; set; } // Phụ phí
        
        // Tổng tiền = Tiền phòng + Thuế + Phụ phí
        public decimal TotalAmount { get; set; } 
        
        public DateTime IssueDate { get; set; } // Ngày xuất hóa đơn
        
        public string? Status { get; set; } // Ví dụ: "Paid" (Đã thanh toán), "Unpaid" (Chưa thanh toán)
    }
}