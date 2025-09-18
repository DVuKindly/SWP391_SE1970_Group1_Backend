using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class RegistrationRequestDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [MaxLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [MaxLength(20)]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Nội dung khám là bắt buộc.")]
        [MaxLength(1000, ErrorMessage = "Nội dung không vượt quá 1000 ký tự.")]
        public string Content { get; set; } = default!;
    }
}
