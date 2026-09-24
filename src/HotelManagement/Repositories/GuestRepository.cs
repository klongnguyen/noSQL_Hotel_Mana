using Cassandra;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Repositories;

public class GuestRepository : IGuestRepository
{
    private readonly ICassandraContext _context;
    private readonly ILogger<GuestRepository> _logger;

    public GuestRepository(ICassandraContext context, ILogger<GuestRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        const string cql = """
            SELECT guest_id, full_name, phone, email, citizen_id
            FROM guests;
            """;

        var rows = await _context.Session.ExecuteAsync(new SimpleStatement(cql));
        return rows.Select(MapRow).ToList();
    }

    public async Task<Guest?> GetByIdAsync(string guestId)
    {
        const string cql = """
            SELECT guest_id, full_name, phone, email, citizen_id
            FROM guests
            WHERE guest_id = ?;
            """;

        var rows = await _context.Session.ExecuteAsync(new SimpleStatement(cql, guestId));

        var row = rows.FirstOrDefault();
        return row == null ? null : MapRow(row);
    }

    public async Task<IEnumerable<Guest>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllAsync();
        }

        var term = keyword.Trim();

        // Nếu người dùng nhập đúng mã GuestId chính xác, thử tìm trực tiếp bằng Partition Key
        var exactGuest = await GetByIdAsync(term);
        if (exactGuest != null)
        {
            return new List<Guest> { exactGuest };
        }

        // Tìm kiếm đa năng theo mã khách hàng, số điện thoại, họ tên hoặc CCCD/CMND
        var allGuests = await GetAllAsync();
        return allGuests.Where(g =>
            (!string.IsNullOrEmpty(g.GuestId) && g.GuestId.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(g.Phone) && g.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(g.FullName) && g.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(g.NationalId) && g.NationalId.Contains(term, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public async Task CreateAsync(Guest guest)
    {
        const string cql = """
            INSERT INTO guests (guest_id, full_name, phone, email, citizen_id)
            VALUES (?, ?, ?, ?, ?);
            """;

        await _context.Session.ExecuteAsync(new SimpleStatement(
            cql,
            guest.GuestId,
            guest.FullName,
            guest.Phone,
            guest.Email,
            guest.NationalId));
    }

    public async Task UpdateAsync(Guest guest)
    {
        const string cql = """
            UPDATE guests
            SET full_name = ?, phone = ?, email = ?, citizen_id = ?
            WHERE guest_id = ?;
            """;

        await _context.Session.ExecuteAsync(new SimpleStatement(
            cql,
            guest.FullName,
            guest.Phone,
            guest.Email,
            guest.NationalId,
            guest.GuestId));
    }

    public async Task DeleteAsync(string guestId)
    {
        const string cql = "DELETE FROM guests WHERE guest_id = ?;";
        await _context.Session.ExecuteAsync(new SimpleStatement(cql, guestId));
    }

    private static Guest MapRow(Row row)
    {
        return new Guest
        {
            GuestId = row.GetValue<string>("guest_id") ?? string.Empty,
            FullName = row.GetValue<string>("full_name") ?? string.Empty,
            Phone = row.GetValue<string>("phone") ?? string.Empty,
            Email = row.GetValue<string>("email") ?? string.Empty,
            NationalId = row.GetValue<string>("citizen_id") ?? string.Empty
        };
    }
}
