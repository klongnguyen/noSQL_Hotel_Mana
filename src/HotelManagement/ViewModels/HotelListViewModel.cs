using HotelManagement.Models;

namespace HotelManagement.ViewModels;

public class HotelListViewModel
{
    public IEnumerable<Hotel> Hotels { get; set; } = new List<Hotel>();
    public string? SelectedCity { get; set; }
    public int? SelectedStarRating { get; set; }
    public string? SearchKeyword { get; set; }
    public IEnumerable<string> AvailableCities { get; set; } = new List<string>();
    public int TotalCount => Hotels.Count();
}
