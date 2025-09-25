
using ClinicManagement.Application.DTOS.Common;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Auth
{
    public interface IAuthService
    {
        //patient 
        Task<ServiceResult<AuthResponse>> LoginPatientAsync(LoginRequest req, CancellationToken ct = default);
        Task<ServiceResult<AuthResponse>> RegisterPatientAsync(RegisterPatientRequest req, CancellationToken ct = default);

        // employee
        Task<ServiceResult<AuthResponse>> LoginEmployeeAsync(LoginRequest req, CancellationToken ct = default);



        Task<ServiceResult<AuthResponse>> RegisterStaffAsync(RegisterEmployeeRequest req, int createdById, CancellationToken ct = default);
        Task<ServiceResult<AuthResponse>> RegisterDoctorAsync(
     CreateDoctorRequest req,
     int createdById,
     CancellationToken ct = default
 );

        //tạo account 2-3 role
        Task<ServiceResult<AuthResponse>> CreateAccountWithRolesAsync(
    CreateAccountRequest req,
    int createdById,
    CancellationToken ct = default);


    }
}
