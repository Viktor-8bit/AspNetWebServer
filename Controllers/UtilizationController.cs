using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AspNetWebServer.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class UtilizationController : ControllerBase
    {
        private readonly ApplicationContext _dbContext;

        public UtilizationController(ApplicationContext dbContext )
        {
            _dbContext = dbContext;
        }

        [HttpPost("SendFromPc/{hostname}")]
        public async Task<IActionResult>  SendData([FromRoute] string hostname)
        {
            Pc PC = _dbContext.Pcs.FirstOrDefault<Pc>(pc => pc.hostname == hostname);
            
            Console.WriteLine(Request.Body);
            JObject requestData = null;

            var reader = new StreamReader(Request.Body);

            var jsonReader = new JsonTextReader(reader);
                
            
            requestData = await JObject.LoadAsync(jsonReader);
            
            float? CPU = (float)requestData["CPU"];
            float? RAM = (float)requestData["RAM"];
            DateTime Date = DateTime.Now;
            if (CPU != null && RAM != null ) {
                _dbContext.Utilizations.Add(new Utilization() { RAM = (float)RAM, CPU_load = (float)CPU, Pc = PC, Date = Date });
                _dbContext.SaveChangesAsync();
            }

            
            return Ok();
        }
        
        
        [HttpGet("GetFromPc/{hostname}/{at}/{to}")]
        public async Task<IEnumerable<Utilization>> GetUsers([FromRoute] string hostname, [FromRoute] string at, [FromRoute] string to)
        {
            List<Utilization> my_util = _dbContext.Utilizations.Where(util => util.Pc.hostname == hostname).OrderByDescending(u => u.Date).Take(5).ToList<Utilization>();
            return my_util;
        }
        
        [HttpGet("deleteall")]
        public void DeleteAll()
        {
             _dbContext.Utilizations.RemoveRange(_dbContext.Utilizations.ToList<Utilization>());
             _dbContext.SaveChangesAsync();
             //return _dbContext.Utilizations.ToList<Utilization>();
        }
    }
}
