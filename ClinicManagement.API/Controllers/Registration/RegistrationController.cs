using ClinicManagement.Application.DTOS.Request.Auth;

using ClinicManagement.Application.Interfaces.Services.Registration;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers
{
    [ApiController]
    [Route("api/public/registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _svc;

        public RegistrationController(IRegistrationService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto req, CancellationToken ct)
        {
            var result = await _svc.CreateRegistrationAsync(req, ct);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
    }
}
