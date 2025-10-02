using ClinicManagement.Application.DTOS.Common;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Auth
{
    public class RoleService
    {
        private readonly ClinicDbContext _ctx;

        public RoleService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ServiceResult<Role>> CreateRoleAsync(string name, string? description, CancellationToken ct = default)
        {
            var exists = await _ctx.Roles.AnyAsync(r => r.Name == name, ct);
            if (exists)
                return ServiceResult<Role>.Fail("Role đã tồn tại.");

            var role = new Role
            {
                Name = name.Trim(),
                Description = description,
                CreatedAtUtc = DateTime.UtcNow
            };

            _ctx.Roles.Add(role);
            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<Role>.Ok(role, "Tạo role thành công.");
        }

        public async Task<ServiceResult<Role>> UpdateRoleAsync(int roleId, UpdateRoleRequest req, CancellationToken ct = default)
        {
            var role = await _ctx.Roles.FindAsync(new object[] { roleId }, ct);
            if (role == null)
                return ServiceResult<Role>.Fail("Role không tồn tại.");

            role.Name = req.Name.Trim();
            role.Description = req.Description?.Trim();
            role.UpdatedAtUtc = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(ct);
            return ServiceResult<Role>.Ok(role, "Cập nhật role thành công.");
        }

    }
}
