



using System.Collections.Immutable;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using AspNetWebServer.Model.DTO;
using AspNetWebServer.Model.Data;
using AspNetWebServer.Model.Data.ProcessMonitoring;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AspNetWebServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcessController : ControllerBase
{
    private readonly ApplicationContext _dbContext;

    public ProcessController(ApplicationContext dbContext )
    {
        _dbContext = dbContext;
    }


    // отправить действие над процессом 
    [HttpPost("SendProcessAction/{hostname}")]
    public async Task<IActionResult> SendProcessAction([FromRoute] string hostname, [FromBody] List<JsonProcessAction> ActionProcesses)
    {
        Pc pcsender = _dbContext.Pcs.FirstOrDefault<Pc>(pc => pc.hostname == hostname);

        if (pcsender == null)
            return NotFound();
        
        
        MountedProcess last = _dbContext.MountedProcesses
            .Where(p => p.PcSender.hostname == hostname)
            .OrderByDescending(x => x.MonutedIndex)
            .FirstOrDefault<MountedProcess>();
        int index = last.MonutedIndex;
        
        DateTime SendDate = DateTime.Now;
        
        foreach (var aprocess in ActionProcesses)
        {
            await _dbContext.ProcessActions.AddAsync(new ProcessAction()
            {
                Name = aprocess.Name,
                ProcessId = aprocess.Id,
                Action = (aprocess.Action == '-') ? ProcessActions.Closed : ProcessActions.Opened,
                Date = SendDate,
                PcSender = pcsender,
                MountedProcess = last
            });
        }
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    
    [HttpPost("SendProcessMounted/{hostname}")]
    public async Task<IActionResult> SendProcessMounted([FromRoute] string hostname, [FromBody] List<JsonProcess> MountProcesses)
    {
        
        Pc pcsender = _dbContext.Pcs.FirstOrDefault<Pc>(pc => pc.hostname == hostname);

        if (pcsender == null)
            return NotFound();
        
        int index = 0;
        
        MountedProcess last = _dbContext.MountedProcesses
            .Where(p => p.PcSender.hostname == hostname)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault<MountedProcess>();

        if (last != null)
        {
            index = last.MonutedIndex + 1;
        }

        DateTime SendDate = DateTime.Now;
        
        foreach (JsonProcess jp in MountProcesses)
        {
            _dbContext.MountedProcesses.Add(new MountedProcess()
            {
                Name = jp.Name,
                ProcessId = jp.Id,
                Date = SendDate,
                MonutedIndex = index,
                PcSender = pcsender,
            });
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    
    // получаем процессы по дате
    [HttpGet("GetProcessesByDate/{hostname}/{date}")] public async Task<List<JsonProcessClient>> GetAllProcessByDate([FromRoute] string hostname, [FromRoute] DateTime date)
    {
        
        int mountedindex = 
            _dbContext.ProcessActions
                .Where(pa => pa.Date.Equals(date))
                .Select(p => p.MountedProcess)
                .FirstOrDefault<MountedProcess>().MonutedIndex;
        
        // вытаскиваем начальное положение
        List<JsonProcessClient> start_processes = 
            _dbContext.MountedProcesses
                .Where(mp => mp.MonutedIndex == mountedindex)
                .Select(mounted => new JsonProcessClient
                {
                    Name = mounted.Name,
                    Date = mounted.Date,
                    ProcessId = mounted.ProcessId,
                }).ToList<JsonProcessClient>();

        
        // вытаскиваем действия над начальным положением 
        List<ProcessAction> process_actions = _dbContext.ProcessActions
                .Where(pa => pa.MountedProcess.MonutedIndex == mountedindex && pa.Date <= date)
                .OrderBy(pa => pa.Date)
                .ToList<ProcessAction>();
        
        foreach (ProcessAction pa in  process_actions)
        {
            if (pa.Action.Equals(ProcessActions.Closed))
            {
                
                JsonProcessClient delete_process = start_processes
                        .Where(pac => pac.ProcessId == pa.ProcessId && pac.Name == pa.Name)
                        .FirstOrDefault<JsonProcessClient>();
                
                start_processes.Remove(delete_process);
            }

            if (pa.Action.Equals(ProcessActions.Opened))
            {
                start_processes.Add(new JsonProcessClient()
                {
                    Name = pa.Name, 
                    Date = pa.Date, 
                    ProcessId = pa.ProcessId
                } );
            }
        }
        
        return start_processes;
    }

    
    // можем взять статичные процессы, Mid - MonutedIndex
    [HttpGet("GetProcessMounted/{hostname}/{Mid}")]
    public async Task<List<MountedProcess>> GetMounted([FromRoute] string hostname, [FromRoute] int Mid)
    {
        return _dbContext.MountedProcesses
            .Where(p => p.PcSender.hostname == hostname && p.MonutedIndex == Mid)
            .OrderByDescending(x => x.MonutedIndex)
            .ToList<MountedProcess>();
    }

    
    // берем даты монументальнх процессов
    [HttpGet("GetAllProcessMountedDate/{hostname}")]
    public async Task<List<DateTime>> GetAllMountedDate([FromRoute] string hostname)
    {
        return _dbContext.MountedProcesses
            .Where(x => x.PcSender.hostname == hostname)
            .GroupBy(x => x.Date)
            .Select(x => x.Key)
            .ToList();;
    }
    
    
    // берем даты действий с процессами
    [HttpGet("GetAllProcessActionDate/{hostname}")]
    public async Task<List<DateTime>> GetAllActionDate([FromRoute] string hostname)
    {
        return _dbContext.ProcessActions
            .Where(x => x.PcSender.hostname == hostname)
            .GroupBy(x => x.Date)
            .Select(x => x.Key)
            .ToList();
    }
    
    
    [HttpGet("DeleteAllProcesses")]
    public async Task<IActionResult> DeleteAll()
    {
        _dbContext.ProcessActions.RemoveRange(_dbContext.ProcessActions.ToList<ProcessAction>());
        _dbContext.MountedProcesses.RemoveRange(_dbContext.MountedProcesses.ToList<MountedProcess>());
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
   
}