using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;

using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Auth
{
    [Route("api/auth/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AdminController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            if (req is null) return BadRequest();

            var res = await _auth.LoginAdminAsync(req, ct);
            if (res == null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(res);
        }
    }
}
