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
            SELECT guest_id, full_name, phone, email, national_id
            FROM guests;
            """;

        var rows = await _context.Session.ExecuteAsync(new SimpleStatement(cql));
        return rows.Select(MapRow).ToList();
    }

    public async Task<Guest?> GetByIdAsync(string guestId)
    {
        const string cql = """
            SELECT guest_id, full_name, phone, email, national_id
            FROM guests
            WHERE guest_id = ?;
            """;

        var rows = await _context.Session.ExecuteAsync(new SimpleStatement(cql, guestId));

        var row = rows.FirstOrDefault();
        return row == null ? null : MapRow(row);
    }

    public async Task CreateAsync(Guest guest)
    {
        const string cql = """
            INSERT INTO guests (guest_id, full_name, phone, email, national_id)
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
            SET full_name = ?, phone = ?, email = ?, national_id = ?
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
            GuestId = row.GetValue<string>("guest_id"),
            FullName = row.GetValue<string>("full_name"),
            Phone = row.GetValue<string>("phone"),
            Email = row.GetValue<string>("email"),
            NationalId = row.GetValue<string>("national_id")
        };
    }
}
