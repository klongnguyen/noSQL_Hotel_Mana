using Cassandra;
using HotelManagement.Models;
using HotelManagement.Data; 

namespace HotelManagement.Repositories
{
    public class BookingHistoryRepository
    {
        private readonly Cassandra.ISession _session;

        // BÍ QUYẾT Ở ĐÂY: Chuyển sang dùng Interface ICassandraContext thay vì class thông thường
        public BookingHistoryRepository(ICassandraContext context)
        {
            _session = context.Session; 
            // (Nếu chữ Session bị gạch đỏ, có thể TV1 viết là context.GetSession(), bạn cứ gõ context. rồi xem VS Code gợi ý nhé)
        }

        public async Task<List<BookingHistory>> GetHistoryByCustomerIdAsync(string customerId)
        {
            var histories = new List<BookingHistory>();
            string cql = "SELECT * FROM booking_history WHERE customer_id = ?";
            var statement = new SimpleStatement(cql, customerId);
            RowSet rowSet = await _session.ExecuteAsync(statement);

            foreach (Row row in rowSet)
            {
                histories.Add(new BookingHistory
                {
                    CustomerId = row.GetValue<string>("customer_id"),
                    BookingId = row.GetValue<Guid>("booking_id"),
                    HotelId = row.GetValue<string>("hotel_id"),
                    HotelName = row.GetValue<string>("hotel_name"),
                    CheckInDate = row.GetValue<DateTime>("check_in_date"),
                    CheckOutDate = row.GetValue<DateTime>("check_out_date"),
                    TotalAmount = row.GetValue<decimal>("total_amount"),
                    Status = row.GetValue<string>("status")
                });
            }
            return histories;
        }
    }
}