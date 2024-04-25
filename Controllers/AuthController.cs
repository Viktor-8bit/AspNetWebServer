
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using AspNetWebServer.Model;
using Microsoft.AspNetCore.Http.HttpResults;

using AspNetWebServer.Model.DTO; 
using AspNetWebServer.Model.Data;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

using BCrypt.Net;    

namespace AspNetWebServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public TimeZoneInfo KraskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Krasnoyarsk");

    private readonly ApplicationContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuthController(ApplicationContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    [HttpPost("Registration/")]
    public async Task<IActionResult>  Registration([FromBody] JsonInfoSecSpec newInfoSecSpec)
    {
        
        if (_dbContext.infoSecuritySpecialists.Where(iSS => iSS.Login.Equals(newInfoSecSpec.Login)).Count() > 0)
        {
            return BadRequest();
        }

        if (newInfoSecSpec.Password == null || newInfoSecSpec.Login == null)
            return BadRequest();

        
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newInfoSecSpec.Password, salt);;
        
        _dbContext.infoSecuritySpecialists.Add(
            new infoSecuritySpecialist()
            {
                Login = newInfoSecSpec.Login,
                HashPassword = hashedPassword,
                Salt = salt
            });
        
        await _dbContext.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("Login/")]
    public async Task<IActionResult> Login([FromBody] JsonInfoSecSpec authInfoSecSpec)
    {
        
        if (_dbContext.infoSecuritySpecialists.Where(iSS => iSS.Login.Equals(authInfoSecSpec.Login)).Count() == 0)
        {
            return BadRequest();
        }

        infoSecuritySpecialist? bdInfoSecSpec = _dbContext.infoSecuritySpecialists
            .Where(iSS => iSS.Login.Equals(authInfoSecSpec.Login))
            .FirstOrDefault<infoSecuritySpecialist>();
        
        string userHashedPassword = BCrypt.Net.BCrypt.HashPassword(authInfoSecSpec.Password, bdInfoSecSpec!.Salt);
        
        if (!userHashedPassword.Equals(bdInfoSecSpec.HashPassword))
        {
            return ValidationProblem();
        } 

        var claims = new List<Claim> {new Claim(ClaimTypes.Name, bdInfoSecSpec.Login) };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1000)), // 400 минут
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        _dbContext.LoginHistoryLog.Add(new LoginHistory()
            { 
                User = bdInfoSecSpec,
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(), 
                LoginTime = DateTime.UtcNow, 
                UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()
            });
        await _dbContext.SaveChangesAsync();
        
        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
    }


    [HttpGet("LoginHistory/{username}")]
    [Authorize]
    public async Task<IActionResult> LoginHistoryLogs([FromRoute] String username)
    {
        var currentUserName = User.Identity.Name; 
        
        if (currentUserName != username)
        {
            return Unauthorized();
        }
        
        return Ok(
            _dbContext.LoginHistoryLog.Where(Lh => Lh.User.Login.Equals(username))
                .ToList()
                .OrderByDescending(Lh => Lh.LoginTime)
                .Select(Lh =>
                {
                    Lh.LoginTime = TimeZoneInfo.ConvertTimeFromUtc(Lh.LoginTime, KraskTimeZone);
                    return Lh;
                }));
    }


    [HttpGet("LoginCheck/")]
    [Authorize]
    public async Task<IActionResult> LoginCheck()
    {
        return Ok();
    }


}