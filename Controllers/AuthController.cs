
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
    
    private readonly ApplicationContext _dbContext;

    
    public AuthController(ApplicationContext dbContext )
    {
        _dbContext = dbContext;
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
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(400)), // 400 минут
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
    }


    [HttpGet("LoginCheck/")]
    [Authorize]
    public async Task<IActionResult> LoginCheck()
    {
        
        return Ok();
    }


}