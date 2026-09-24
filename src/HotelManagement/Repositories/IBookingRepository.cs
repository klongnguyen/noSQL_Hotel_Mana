using HotelManagement.Models;

namespace HotelManagement.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetByGuestAsync(string guestId);
    Task<IEnumerable<Booking>> GetByHotelAsync(string hotelId);
    Task CreateAsync(Booking booking);
    Task CancelAsync(Booking booking, Room room);
}
