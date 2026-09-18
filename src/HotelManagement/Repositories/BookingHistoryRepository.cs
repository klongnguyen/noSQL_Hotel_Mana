using Cassandra;
using HotelManagement.Models;
using HotelManagement.Data; 

namespace HotelManagement.Repositories
{
    public class BookingHistoryRepository
    {
        private readonly Cassandra.ISession _session;

        public BookingHistoryRepository(ICassandraContext context)
        {
            _session = context.Session; 
        }

        public async Task<List<BookingHistory>> GetHistoryByCustomerIdAsync(string customerId)
        {
            var histories = new List<BookingHistory>();
            string cql = "SELECT guest_id, booking_id, hotel_id, hotel_name, check_in_date, check_out_date, total_amount, status FROM bookings_by_guest WHERE guest_id = ?;";
            var statement = new SimpleStatement(cql, customerId);
            RowSet rowSet = await _session.ExecuteAsync(statement);

            foreach (Row row in rowSet)
            {
                var checkIn = row.GetValue<LocalDate>("check_in_date");
                var checkOut = row.GetValue<LocalDate>("check_out_date");

                histories.Add(new BookingHistory
                {
                    CustomerId = row.GetValue<string>("guest_id"),
                    BookingId = row.GetValue<Guid>("booking_id"),
                    HotelId = row.GetValue<string>("hotel_id"),
                    HotelName = row.GetValue<string>("hotel_name"),
                    CheckInDate = new DateTime(checkIn.Year, checkIn.Month, checkIn.Day),
                    CheckOutDate = new DateTime(checkOut.Year, checkOut.Month, checkOut.Day),
                    TotalAmount = row.GetValue<decimal>("total_amount"),
                    Status = row.GetValue<string>("status")
                });
            }
            return histories;
        }
    }
}