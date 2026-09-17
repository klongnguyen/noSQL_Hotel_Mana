using Cassandra;
using HotelManagement.Models;
using HotelManagement.Data; 

namespace HotelManagement.Repositories
{
    public class InvoiceRepository
    {
        private readonly Cassandra.ISession _session;

        // Tiêm ICassandraContext 
        public InvoiceRepository(ICassandraContext context)
        {
            _session = context.Session;
        }

        public async Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId)
        {
            // Truy vấn hóa đơn từ bảng invoice_by_booking theo đúng yêu cầu Jira KAN-17
            string cql = "SELECT * FROM invoice_by_booking WHERE booking_id = ?";
            var statement = new SimpleStatement(cql, bookingId);
            
            RowSet rowSet = await _session.ExecuteAsync(statement);
            
            // Lấy dòng dữ liệu đầu tiên tìm được (nếu có)
            Row? row = rowSet.FirstOrDefault();

            if (row == null)
            {
                return null; // Không tìm thấy hóa đơn
            }

            // Ánh xạ dữ liệu từ CSDL vào Model Invoice
            return new Invoice
            {
                BookingId = row.GetValue<Guid>("booking_id"),
                InvoiceId = row.GetValue<string>("invoice_id"),
                CustomerName = row.GetValue<string>("customer_name"),
                RoomCharge = row.GetValue<decimal>("room_charge"),
                Tax = row.GetValue<decimal>("tax"),
                AdditionalFees = row.GetValue<decimal>("additional_fees"),
                TotalAmount = row.GetValue<decimal>("total_amount"),
                IssueDate = row.GetValue<DateTime>("issue_date"),
                Status = row.GetValue<string>("status")
            };
        }
    }
}