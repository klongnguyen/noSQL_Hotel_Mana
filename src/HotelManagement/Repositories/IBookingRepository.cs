using HotelManagement.Models;

namespace HotelManagement.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetByGuestAsync(string guestId);
    Task CreateAsync(Booking booking);
    Task CancelAsync(Booking booking, Room room);
}
