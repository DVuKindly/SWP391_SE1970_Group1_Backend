using System;
using System.Collections.Generic;

namespace ClinicManagement.Application.DTOS.Request.Dashboard
{
    public class AccountDto
    {
        public int Id { get; set; }

    public List<string> Roles { get; set; } = new();

        public string Role => Roles.Count > 0 ? Roles[0] : string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
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
