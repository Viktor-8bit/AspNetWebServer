using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;

using System.Web.Http.Cors;

using Microsoft.AspNetCore.Mvc;


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
            else
            {
                
            }
        }
        
        [HttpGet("InfoPc/{hostname}")]
        public Pc InfoPc([FromRoute] string hostname)
        {
            return _dbContext.Pcs.FirstOrDefault<Pc>(pc => pc.hostname == hostname);
        }
    }
}