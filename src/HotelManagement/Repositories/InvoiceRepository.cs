using Cassandra;
using HotelManagement.Models;
using HotelManagement.Data; 

namespace HotelManagement.Repositories
{
    public class InvoiceRepository
    {
        private readonly Cassandra.ISession _session;

        public InvoiceRepository(ICassandraContext context)
        {
            _session = context.Session;
        }

        public async Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId)
        {
            const string cql = "SELECT booking_id, invoice_id, guest_id, hotel_id, issued_at, payment_status, room_charge, service_charge, tax, total_amount FROM invoices_by_booking WHERE booking_id = ?;";
            var statement = new SimpleStatement(cql, bookingId);
            
            RowSet rowSet = await _session.ExecuteAsync(statement);
            Row? row = rowSet.FirstOrDefault();

            if (row == null)
            {
                return null;
            }

            var issuedAt = row.GetValue<DateTimeOffset>("issued_at");

            return new Invoice
            {
                BookingId = row.GetValue<Guid>("booking_id"),
                InvoiceId = row.GetValue<Guid>("invoice_id").ToString(),
                CustomerName = row.GetValue<string>("guest_id"),
                RoomCharge = row.GetValue<decimal>("room_charge"),
                Tax = row.GetValue<decimal>("tax"),
                AdditionalFees = row.GetValue<decimal>("service_charge"),
                TotalAmount = row.GetValue<decimal>("total_amount"),
                IssueDate = issuedAt.LocalDateTime,
                Status = row.GetValue<string>("payment_status")
            };
        }
    }
}