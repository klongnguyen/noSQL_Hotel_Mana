using HotelManagement.Models;

namespace HotelManagement.Repositories;

public interface IHotelRepository
{
    /// <summary>
    /// Lấy toàn bộ danh sách khách sạn từ bảng hotels
    /// </summary>
    Task<IEnumerable<Hotel>> GetAllHotelsAsync();

    /// <summary>
    /// Lấy danh sách khách sạn theo thành phố từ bảng hotels_by_city (Query-First, tối ưu NoSQL)
    /// </summary>
    Task<IEnumerable<Hotel>> GetHotelsByCityAsync(string city);

    /// <summary>
    /// Lấy chi tiết một khách sạn theo ID từ bảng hotels
    /// </summary>
    Task<Hotel?> GetHotelByIdAsync(string hotelId);

    /// <summary>
    /// Lấy danh sách các thành phố hiện có
    /// </summary>
    Task<IEnumerable<string>> GetAvailableCitiesAsync();
}
