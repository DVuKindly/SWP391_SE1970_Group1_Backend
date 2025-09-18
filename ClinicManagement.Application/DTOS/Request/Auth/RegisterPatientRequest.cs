using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class RegisterPatientRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(254, ErrorMessage = "Email không được vượt quá 254 ký tự.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [MaxLength(128, ErrorMessage = "Mật khẩu không được vượt quá 128 ký tự.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [MaxLength(150, ErrorMessage = "Họ và tên không được vượt quá 150 ký tự.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string Phone { get; set; } = default!;

 
     
    }
}
