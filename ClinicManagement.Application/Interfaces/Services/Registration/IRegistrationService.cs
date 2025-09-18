using ClinicManagement.Application.DTOS.Request.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Registration
{
    public interface IRegistrationService
    {
        Task<ServiceResult<int>> CreateRegistrationAsync(RegistrationRequestDto req, CancellationToken ct = default);
    }
}
