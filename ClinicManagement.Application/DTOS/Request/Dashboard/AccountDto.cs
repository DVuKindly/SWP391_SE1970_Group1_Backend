using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard
{
    public class AccountDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        public string Role { get; set; } = default!;

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = default!;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string Phone { get; set; } = default!;

        public bool IsActive { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}