using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DunakanyarHouseIngatlan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseWebRoot("wwwroot")
                .UseStartup<Startup>();
    }
}
