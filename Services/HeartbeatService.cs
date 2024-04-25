


using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;


using AspNetWebServer.Model.DTO;
using AspNetWebServer.Model.Data;

namespace AspNetWebServer.Services;

public class HeartbeatService : BackgroundService 
{
    private readonly IServiceProvider _serviceProvider;
    
    public HeartbeatService(IServiceProvider serviceProvider )
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var Hosts = _dbContext.Pcs.ToList();
            bool ihavechanges = false;
            
            foreach (var host in Hosts)
            {
                var Now = DateTime.UtcNow;
                var LastUtilDate = _dbContext.Utilizations
                    .Where(U => U.Pc.Id == host.Id)
                    .OrderBy(U => U.Date)
                    .LastOrDefault().Date;

                if ((Now - LastUtilDate).TotalSeconds > 20)
                {
                    if (host.Online != false)
                    {
                        host.Online = false;
                        ihavechanges = true;
                    }
                }
                else
                {
                    if (host.Online != true)
                    {
                        Console.WriteLine($"сейчас {Now} последнее время отправки {LastUtilDate}");
                        host.Online = true;
                        ihavechanges = true;
                    }
                }
            }
            if (ihavechanges)
                _dbContext.SaveChanges();
            
            await Task.Delay(20000);
        }

    }
    
}