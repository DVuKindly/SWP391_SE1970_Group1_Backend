using System.Collections.Generic;
using System.Security.Claims;

namespace ClinicManagement.Application.Interfaces.JWT
{
    public interface IJwtService
    {
     
        (string accessToken, string refreshToken) GenerateTokens(IEnumerable<Claim> claims);
    }
}
