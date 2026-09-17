using HotelManagement.Models;

namespace HotelManagement.Repositories;

public interface IRoomRepository
{
    /// <summary>
    /// Lấy toàn bộ danh sách phòng thuộc một khách sạn từ bảng rooms_by_hotel
    /// (Truy vấn theo Partition Key: hotel_id, tự động sắp xếp theo Clustering Key: room_number ASC)
    /// </summary>
    Task<IEnumerable<Room>> GetRoomsByHotelAsync(string hotelId);

    /// <summary>
    /// Lấy chi tiết một phòng cụ thể theo hotel_id và room_number
    /// </summary>
    Task<Room?> GetRoomByNumberAsync(string hotelId, int roomNumber);

    /// <summary>
    /// Lọc danh sách phòng theo khách sạn và trạng thái từ bảng rooms_by_hotel_status (Story-105)
    /// (Truy vấn theo Composite Partition Key: (hotel_id, status), Clustering Key: room_number ASC)
    /// Tuyệt đối không sử dụng ALLOW FILTERING theo chuẩn Query-First.
    /// </summary>
    Task<IEnumerable<Room>> GetRoomsByStatusAsync(string hotelId, string status);
}

