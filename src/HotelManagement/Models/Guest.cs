using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models;

public class Guest
{
    [Required(ErrorMessage = "Mã khách hàng không được để trống.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Mã khách hàng phải từ 3 đến 20 ký tự.")]
    [Display(Name = "Mã Khách Hàng")]
    public string GuestId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 100 ký tự.")]
    [Display(Name = "Họ Và Tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]
    [Display(Name = "Số Điện Thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ email không được để trống.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
    [Display(Name = "Địa Chỉ Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số Căn cước công dân (CCCD) không được để trống.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Số Căn cước công dân (CCCD) phải bao gồm đúng 12 chữ số.")]
    [Display(Name = "Số Căn Cước Công Dân (CCCD)")]
    public string NationalId { get; set; } = string.Empty;
}