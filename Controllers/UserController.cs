using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebServer.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _dbContext;

        public UserController(ApplicationContext dbContext )
        {
            _dbContext = dbContext;
        } 


        [HttpGet("GetAllUsers")]
        public async Task<List<User>> GetUsers() {
            return _dbContext.Users.ToList<User>();
        }
    }
}
