using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Service.Interfaces;

namespace Presentaion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IUserAccountService _userAccountService;

    public UserAccountController(IConfiguration configuration, IUserAccountService userAccountService)
    {
        _configuration = configuration;
        _userAccountService = userAccountService;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = _userAccountService.Authenticate(request.UserName, request.Password);

        if (user == null || user.Result == null)
            return Unauthorized();

        var token = GenerateJSONWebToken(user.Result);

        return Ok(token);
    }

    private string GenerateJSONWebToken(UserAccount systemUserAccount)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"]
            , _configuration["Jwt:Audience"]
            , new Claim[]
            {
                new(ClaimTypes.Name, systemUserAccount.UserEmail),
                new(ClaimTypes.Role, systemUserAccount.Role.ToString())
            },
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }


    public sealed record LoginRequest(string UserName, string Password);
}