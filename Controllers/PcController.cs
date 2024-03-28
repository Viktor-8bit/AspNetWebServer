


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using AspNetWebServer.Model;
using AspNetWebServer.Model.Data;

using System.Web.Http.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace AspNetWebServer.Controllers;

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
    public async Task<List<Pc>> GetAllPc() {
        return await _dbContext.Pcs.ToListAsync<Pc>();
    }

    [HttpPost("AddPC/{hostname}")]
    public async Task AddPc([FromRoute] string hostname)
    {
        if (await _dbContext.Pcs.CountAsync<Pc>(pc => pc.hostname.Equals(hostname)) == 0)
        {
            await _dbContext.Pcs.AddAsync(new Pc() { hostname = hostname, Online = true });
            await _dbContext.SaveChangesAsync();
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
            await _dbContext.SaveChangesAsync();
        }
    }
    
}
