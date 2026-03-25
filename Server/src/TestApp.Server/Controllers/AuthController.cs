using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApp.Server.Settings;

namespace TestApp.Server.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private const string API_KEY = "Nobody!Knows#This9995";
    private readonly JwtSettings _jwtSettings;

    public AuthController(IOptions<JwtSettings> options)
    {
      _jwtSettings = options.Value;
    }

    /// <summary>
    /// Returns a JWT token that is usable for 24 hours when the given api key is correct 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GetToken([FromHeader] string apiKey)
    {
      if (apiKey != API_KEY)
        return Unauthorized("Invalid API Key");

      var claims = new[]
      {
            new Claim(ClaimTypes.Name, "DefaultUser")
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          expires: DateTime.UtcNow.AddHours(24),// relog once a day
          signingCredentials: creds,
          claims: claims
      );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return Ok(new { token = tokenString });
    }
  }
}
