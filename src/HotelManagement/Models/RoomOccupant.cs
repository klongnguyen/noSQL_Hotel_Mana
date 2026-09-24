using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models;

/// <summary>
/// Đại diện cho thông tin người lưu trú cùng phòng (không cần tạo tài khoản trong hệ thống)
/// </summary>
public class RoomOccupant
{
    /// <summary>
    /// Họ và tên người lưu trú
    /// </summary>
    [Required(ErrorMessage = "Họ và tên người lưu trú không được để trống.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 100 ký tự.")]
    [Display(Name = "Họ Và Tên")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Số Căn cước công dân (CCCD phải gồm đúng 12 chữ số)
    /// </summary>
    [Required(ErrorMessage = "Số Căn cước công dân không được để trống.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Số Căn cước công dân (CCCD) phải bao gồm đúng 12 chữ số.")]
    [Display(Name = "Số Căn Cước Công Dân (CCCD)")]
    public string CitizenId { get; set; } = string.Empty;

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    [Display(Name = "Ngày Sinh")]
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Đánh dấu là người đại diện đặt phòng (Primary Booker) hay người đi cùng
    /// </summary>
    public bool IsPrimary { get; set; } = false;
}
