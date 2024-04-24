using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;
using AspNetWebServer.Model.DTO;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AspNetWebServer.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class UtilizationController : ControllerBase
    {
        public TimeZoneInfo KraskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Krasnoyarsk");

        private readonly ApplicationContext _dbContext;

        public UtilizationController(ApplicationContext dbContext )
        {
            _dbContext = dbContext;
        }

        [HttpPost("SendFromPc/{hostname}")]
        public async Task<IActionResult>  SendData([FromRoute] string hostname, [FromBody] List<JsonUtilization> StashedUtilizations)
        {
            try
            {
                Pc PC = _dbContext.Pcs.FirstOrDefault<Pc>(pc => pc.hostname == hostname);
                foreach (JsonUtilization stashed_util in StashedUtilizations)
                {
                    if (stashed_util.CPU != null && stashed_util.RAM != null)
                    {
                        await _dbContext.Utilizations.AddAsync(new Utilization()
                        {
                            RAM = stashed_util.RAM, 
                            CPU_load = stashed_util.CPU, 
                            Pc = PC, 
                            Date = stashed_util.Date
                        });
                    }
                }
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
        
        
        [HttpGet("GetFromPc/{hostname}")]
        public async Task<IEnumerable<Utilization>> GetFromPC([FromRoute] string hostname) 
        {
            List<Utilization> my_util = _dbContext.Utilizations
                .Where(util => util.Pc.hostname == hostname)
                .OrderByDescending(u => u.Date)
                .Take(5)
                .ToList<Utilization>()
                .Select(U =>
                {
                    U.Date = TimeZoneInfo.ConvertTimeFromUtc(U.Date, KraskTimeZone);
                    return U;
                } ).ToList<Utilization>();
            return my_util;
        }
        
        [HttpGet("deleteall")]
        public void DeleteAll()
        {
             _dbContext.Utilizations.RemoveRange(_dbContext.Utilizations.ToList<Utilization>());
             _dbContext.SaveChangesAsync();
        }
    }
}
