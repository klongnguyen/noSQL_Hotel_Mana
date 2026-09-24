using HotelManagement.Models;

namespace HotelManagement.Repositories;

public interface IGuestRepository
{
    Task<IEnumerable<Guest>> GetAllAsync();
    Task<Guest?> GetByIdAsync(string guestId);
    Task<IEnumerable<Guest>> SearchAsync(string keyword);
    Task CreateAsync(Guest guest);
    Task UpdateAsync(Guest guest);
    Task DeleteAsync(string guestId);
}
