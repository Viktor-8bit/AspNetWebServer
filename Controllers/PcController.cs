using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;

using System.Web.Http.Cors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AspNetWebServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PcController
    {
        
        private readonly ApplicationContext _dbContext;
        public PcController(ApplicationContext dbContext )
        {
            _dbContext = dbContext;
        } 
        
        [HttpGet("GetAllPC")]
        public List<Pc> GetAllPc() {
            return _dbContext.Pcs.ToList<Pc>();
        }

        [HttpPost("AddPC/{hostname}")]
        public void PostPc([FromRoute] string hostname)
        {
            if (_dbContext.Pcs.Count<Pc>(pc => pc.hostname.Equals(hostname)) == 0)
            {
                _dbContext.Pcs.Add(new Pc() { hostname = hostname, Online = true });
                _dbContext.SaveChanges();
            }
        }
        
        [HttpGet("InfoPc/{hostname}")]
        public async Task<Pc> InfoPc([FromRoute] string hostname)
        {
            return await _dbContext.Pcs.FirstOrDefaultAsync<Pc>(pc => pc.hostname == hostname);
        }
        
        
        
        [HttpPost("DeletePC/{hostname}")]
        public async Task DeletePc([FromRoute] string hostname)
        {
            Pc? pc = await _dbContext.Pcs.FirstOrDefaultAsync<Pc>(pc => pc.hostname == hostname);
            if (pc != null)
            {
                _dbContext.Pcs.Remove(pc);
                _dbContext.SaveChanges();
            }
        }
        
    }
}