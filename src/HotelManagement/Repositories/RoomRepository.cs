using Cassandra;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ICassandraContext _context;
    private readonly ILogger<RoomRepository> _logger;

    private PreparedStatement? _getRoomsByHotelStmt;
    private PreparedStatement? _getRoomByNumberStmt;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public RoomRepository(ICassandraContext context, ILogger<RoomRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task EnsurePreparedStatementsAsync()
    {
        if (_getRoomsByHotelStmt != null && _getRoomByNumberStmt != null)
        {
            return;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_getRoomsByHotelStmt == null)
            {
                _getRoomsByHotelStmt = await _context.Session.PrepareAsync(
                    "SELECT hotel_id, room_number, room_type, price_per_night, status FROM rooms_by_hotel WHERE hotel_id = ?;");
            }

            if (_getRoomByNumberStmt == null)
            {
                _getRoomByNumberStmt = await _context.Session.PrepareAsync(
                    "SELECT hotel_id, room_number, room_type, price_per_night, status FROM rooms_by_hotel WHERE hotel_id = ? AND room_number = ?;");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi chuẩn bị PreparedStatement cho RoomRepository");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(string hotelId)
    {
        await EnsurePreparedStatementsAsync();
        try
        {
            // Truy vấn đơn phân vùng (Single Partition Read) theo Partition Key: hotel_id
            var bound = _getRoomsByHotelStmt!.Bind(hotelId);
            var rowSet = await _context.Session.ExecuteAsync(bound);
            return rowSet.Select(MapRowToRoom).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách phòng cho khách sạn ID: {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<Room?> GetRoomByNumberAsync(string hotelId, int roomNumber)
    {
        await EnsurePreparedStatementsAsync();
        try
        {
            var bound = _getRoomByNumberStmt!.Bind(hotelId, roomNumber);
            var rowSet = await _context.Session.ExecuteAsync(bound);
            var row = rowSet.FirstOrDefault();
            return row != null ? MapRowToRoom(row) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin phòng {RoomNumber} tại khách sạn {HotelId}", roomNumber, hotelId);
            throw;
        }
    }

    private static Room MapRowToRoom(Row row)
    {
        return new Room
        {
            HotelId = row.IsNull("hotel_id") ? string.Empty : row.GetValue<string>("hotel_id"),
            RoomNumber = row.IsNull("room_number") ? 0 : row.GetValue<int>("room_number"),
            RoomType = row.IsNull("room_type") ? "Standard" : row.GetValue<string>("room_type"),
            PricePerNight = ParsePrice(row, "price_per_night"),
            Status = row.IsNull("status") ? "AVAILABLE" : row.GetValue<string>("status")
        };
    }

    private static decimal ParsePrice(Row row, string column)
    {
        if (row.IsNull(column)) return 0m;
        var val = row.GetValue<object>(column);
        return val switch
        {
            decimal d => d,
            long l => l,
            int i => i,
            double dbl => (decimal)dbl,
            _ => Convert.ToDecimal(val)
        };
    }
}
