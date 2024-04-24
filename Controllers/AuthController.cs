
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using AspNetWebServer.Model;
using Microsoft.AspNetCore.Http.HttpResults;

using AspNetWebServer.Model.DTO; 
using AspNetWebServer.Model.Data;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

using Scrypt; 
    
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
    
    [HttpPost("Registration")]
    public async Task<IActionResult>  Registration([FromBody] JsonInfoSecSpec newInfoSecSpec)
    {
        
        if (_dbContext.infoSecuritySpecialists.Where(iSS => iSS.Login.Equals(newInfoSecSpec.Login)).Count() > 0)
        {
            return BadRequest();
        }

        if (newInfoSecSpec.Password == null || newInfoSecSpec == null || newInfoSecSpec.Login == null)
            return BadRequest();

        _dbContext.infoSecuritySpecialists.Add(
            new infoSecuritySpecialist()
            {
                Login = newInfoSecSpec.Login,
                HashPassword = newInfoSecSpec.Password
            });
        
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    
   

    
}