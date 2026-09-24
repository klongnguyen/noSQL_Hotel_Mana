using HotelManagement.Models;
using HotelManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers;

public class GuestsController : Controller
{
    private readonly IGuestRepository _repository;
    private readonly ILogger<GuestsController> _logger;

    public GuestsController(IGuestRepository repository, ILogger<GuestsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm)
    {
        try
        {
            IEnumerable<Guest> guests;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                guests = await _repository.SearchAsync(term);
                ViewBag.SearchTerm = term;
            }
            else
            {
                guests = await _repository.GetAllAsync();
            }

            return View(guests.OrderBy(g => g.GuestId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tải danh sách khách hàng");
            TempData["ErrorMessage"] = "Không thể tải danh sách khách hàng.";
            return View(new List<Guest>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(Index));
        var guest = await _repository.GetByIdAsync(id);
        if (guest == null) return NotFound();
        return View(guest);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Guest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guest guest)
    {
        ValidateGuest(guest);

        if (!ModelState.IsValid) return View(guest);

        try
        {
            var exists = await _repository.GetByIdAsync(guest.GuestId.Trim());
            if (exists != null)
            {
                ModelState.AddModelError(nameof(guest.GuestId), "Mã khách hàng đã tồn tại.");
                return View(guest);
            }

            guest.GuestId = guest.GuestId.Trim();
            await _repository.CreateAsync(guest);

            TempData["SuccessMessage"] = $"Đã thêm khách hàng {guest.FullName}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm khách hàng {GuestId}", guest.GuestId);
            ModelState.AddModelError("", "Không thể thêm khách hàng. Kiểm tra kết nối Cassandra.");
            return View(guest);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(Index));

        var guest = await _repository.GetByIdAsync(id);
        if (guest == null) return NotFound();

        return View(guest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Guest guest)
    {
        if (!string.Equals(id, guest.GuestId, StringComparison.OrdinalIgnoreCase))
            return BadRequest();

        ValidateGuest(guest);

        if (!ModelState.IsValid)
            return View(guest);

        try
        {
            var exists = await _repository.GetByIdAsync(id);
            if (exists == null)
                return NotFound();

            await _repository.UpdateAsync(guest);
            TempData["SuccessMessage"] = $"Đã cập nhật khách hàng {guest.FullName}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật khách hàng {GuestId}", id);
            ModelState.AddModelError("", "Không thể cập nhật khách hàng.");
            return View(guest);
        }
    }

    private void ValidateGuest(Guest guest)
    {
        guest.GuestId = guest.GuestId?.Trim() ?? string.Empty;
        guest.FullName = guest.FullName?.Trim() ?? string.Empty;
        guest.Phone = guest.Phone?.Trim() ?? string.Empty;
        guest.Email = guest.Email?.Trim() ?? string.Empty;
        guest.NationalId = guest.NationalId?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(guest.GuestId))
        {
            ModelState.AddModelError(nameof(guest.GuestId), "Mã khách hàng không được để trống.");
        }
        else if (guest.GuestId.Length < 3 || guest.GuestId.Length > 20)
        {
            ModelState.AddModelError(nameof(guest.GuestId), "Mã khách hàng phải từ 3 đến 20 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(guest.FullName))
        {
            ModelState.AddModelError(nameof(guest.FullName), "Họ và tên khách hàng không được để trống.");
        }
        else if (guest.FullName.Length < 2 || guest.FullName.Length > 100)
        {
            ModelState.AddModelError(nameof(guest.FullName), "Họ và tên phải từ 2 đến 100 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(guest.Phone))
        {
            ModelState.AddModelError(nameof(guest.Phone), "Số điện thoại không được để trống.");
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(guest.Phone, @"^0\d{9}$"))
        {
            ModelState.AddModelError(nameof(guest.Phone), "Số điện thoại không hợp lệ (phải gồm đúng 10 chữ số và bắt đầu bằng số 0).");
        }

        if (string.IsNullOrWhiteSpace(guest.Email))
        {
            ModelState.AddModelError(nameof(guest.Email), "Địa chỉ email không được để trống.");
        }
        else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(guest.Email))
        {
            ModelState.AddModelError(nameof(guest.Email), "Địa chỉ email không đúng định dạng.");
        }

        if (string.IsNullOrWhiteSpace(guest.NationalId))
        {
            ModelState.AddModelError(nameof(guest.NationalId), "Số Căn cước công dân (CCCD) không được để trống.");
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(guest.NationalId, @"^\d{12}$"))
        {
            ModelState.AddModelError(nameof(guest.NationalId), "Số Căn cước công dân (CCCD) phải bao gồm đúng 12 chữ số.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(Index));

        var guest = await _repository.GetByIdAsync(id);
        if (guest == null) return NotFound();

        return View(guest);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var guest = await _repository.GetByIdAsync(id);
            if (guest == null) return NotFound();

            await _repository.DeleteAsync(id);
            TempData["SuccessMessage"] = $"Đã xóa khách hàng {guest.FullName}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa khách hàng {GuestId}", id);
            TempData["ErrorMessage"] = "Không thể xóa khách hàng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
