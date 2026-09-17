using Cassandra;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ICassandraContext _context;
    private readonly ILogger<HotelRepository> _logger;

    private PreparedStatement? _getAllHotelsStmt;
    private PreparedStatement? _getHotelsByCityStmt;
    private PreparedStatement? _getHotelByIdStmt;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public HotelRepository(ICassandraContext context, ILogger<HotelRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task EnsurePreparedStatementsAsync()
    {
        if (_getAllHotelsStmt != null && _getHotelsByCityStmt != null && _getHotelByIdStmt != null)
        {
            return;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_getAllHotelsStmt == null)
            {
                _getAllHotelsStmt = await _context.Session.PrepareAsync(
                    "SELECT hotel_id, hotel_name, city, address, star_rating, phone FROM hotels;");
            }

            if (_getHotelsByCityStmt == null)
            {
                _getHotelsByCityStmt = await _context.Session.PrepareAsync(
                    "SELECT hotel_id, hotel_name, city, address, star_rating, phone FROM hotels_by_city WHERE city = ?;");
            }

            if (_getHotelByIdStmt == null)
            {
                _getHotelByIdStmt = await _context.Session.PrepareAsync(
                    "SELECT hotel_id, hotel_name, city, address, star_rating, phone FROM hotels WHERE hotel_id = ?;");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi chuẩn bị PreparedStatement cho HotelRepository");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
    {
        await EnsurePreparedStatementsAsync();
        try
        {
            var bound = _getAllHotelsStmt!.Bind();
            var rowSet = await _context.Session.ExecuteAsync(bound);
            return rowSet.Select(MapRowToHotel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách tất cả khách sạn từ bảng hotels");
            throw;
        }
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByCityAsync(string city)
    {
        await EnsurePreparedStatementsAsync();
        try
        {
            // Truy vấn tối ưu Query-First theo Partition Key (city), dữ liệu tự động sắp xếp theo star_rating DESC
            var bound = _getHotelsByCityStmt!.Bind(city);
            var rowSet = await _context.Session.ExecuteAsync(bound);
            return rowSet.Select(MapRowToHotel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách khách sạn theo thành phố: {City} từ bảng hotels_by_city", city);
            throw;
        }
    }

    public async Task<Hotel?> GetHotelByIdAsync(string hotelId)
    {
        await EnsurePreparedStatementsAsync();
        try
        {
            var bound = _getHotelByIdStmt!.Bind(hotelId);
            var rowSet = await _context.Session.ExecuteAsync(bound);
            var row = rowSet.FirstOrDefault();
            return row != null ? MapRowToHotel(row) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy chi tiết khách sạn ID: {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetAvailableCitiesAsync()
    {
        // Danh sách các thành phố chuẩn trong hệ thống
        var defaultCities = new List<string>
        {
            "Ho Chi Minh City",
            "Ha Noi",
            "Da Nang",
            "Nha Trang",
            "Da Lat",
            "Vung Tau",
            "Can Tho"
        };

        try
        {
            var all = await GetAllHotelsAsync();
            var distinctCities = all.Select(h => h.City)
                                    .Where(c => !string.IsNullOrWhiteSpace(c))
                                    .Distinct()
                                    .OrderBy(c => c)
                                    .ToList();

            return distinctCities.Count > 0 ? distinctCities : defaultCities;
        }
        catch
        {
            return defaultCities;
        }
    }

    private static Hotel MapRowToHotel(Row row)
    {
        return new Hotel
        {
            HotelId = row.IsNull("hotel_id") ? string.Empty : row.GetValue<string>("hotel_id"),
            HotelName = row.IsNull("hotel_name") ? string.Empty : row.GetValue<string>("hotel_name"),
            City = row.IsNull("city") ? string.Empty : row.GetValue<string>("city"),
            Address = row.IsNull("address") ? string.Empty : row.GetValue<string>("address"),
            StarRating = row.IsNull("star_rating") ? 3 : row.GetValue<int>("star_rating"),
            Phone = row.IsNull("phone") ? string.Empty : row.GetValue<string>("phone")
        };
    }
}
