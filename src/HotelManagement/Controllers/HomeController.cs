using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Models;
using HotelManagement.ViewModels;
using HotelManagement.Services;

namespace HotelManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardAnalyticsService _analyticsService;

    public HomeController(
        ILogger<HomeController> logger,
        IDashboardAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    public async Task<IActionResult> Index()
    {
        // Tải dữ liệu Dashboard cập nhật động theo thời gian thực từ Analytics Service
        var model = await _analyticsService.GetLiveDashboardDataAsync();
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
