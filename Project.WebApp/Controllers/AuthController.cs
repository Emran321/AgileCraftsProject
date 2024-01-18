using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (model.Username == "imran" && model.Password == "imran")
        {
            var token = GenerateJwtToken(model.Username);
            var tenantId = GetTenantId();
            Response.Headers.Add("Abp.TenantId", tenantId);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
    private string GenerateJwtToken(string username)
    {
        var key = new byte[256 / 8]; // 256 bits key size
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(key);
        }

        var signingKey = new SymmetricSecurityKey(key);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Name, username),
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private string GetTenantId()
    {
        var tenantIdClaim = HttpContext.User?.FindFirst("Abp.TenantId");
        return tenantIdClaim?.Value ?? "DefaultTenantId";
    }
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
