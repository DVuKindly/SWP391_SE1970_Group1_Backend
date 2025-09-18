using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class RegisterEmployeeRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(254, ErrorMessage = "Email không vượt quá 254 ký tự.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [MaxLength(128, ErrorMessage = "Mật khẩu không vượt quá 128 ký tự.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [MaxLength(150, ErrorMessage = "Tên không vượt quá 150 ký tự.")]
        public string FullName { get; set; } = default!;

        [MaxLength(30, ErrorMessage = "Số điện thoại không vượt quá 30 ký tự.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role là bắt buộc.")]
        public string RoleName { get; set; } = default!;
      
    }
}
