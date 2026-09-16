using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Models;
using HotelManagement.Data;
using HotelManagement.ViewModels;

namespace HotelManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICassandraContext _cassandraContext;

    public HomeController(ILogger<HomeController> logger, ICassandraContext cassandraContext)
    {
        _logger = logger;
        _cassandraContext = cassandraContext;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            IsConnected = _cassandraContext.IsConnected,
            Keyspace = _cassandraContext.Keyspace,
            ReleaseVersion = _cassandraContext.ClusterReleaseVersion
        };

        try
        {
            var session = _cassandraContext.Session;

            // Thực thi truy vấn kiểm tra dữ liệu từ Cassandra
            var hotelRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM hotels"));
            model.HotelCount = hotelRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var roomRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM rooms_by_hotel"));
            model.RoomCount = roomRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var guestRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM guests"));
            model.GuestCount = guestRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;

            var bookingRow = await session.ExecuteAsync(new Cassandra.SimpleStatement("SELECT count(*) FROM bookings_by_guest"));
            model.BookingCount = bookingRow.FirstOrDefault()?.GetValue<long>(0) ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy dữ liệu thống kê từ Cassandra");
            model.ErrorMessage = ex.Message;
        }

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
