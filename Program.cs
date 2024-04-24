



using Microsoft.AspNetCore.Hosting;

namespace AspNetWebServer;

class Program {



    static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
        
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", false, true)
                    //.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                    //    true, true)
                    .AddEnvironmentVariables();
                
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}