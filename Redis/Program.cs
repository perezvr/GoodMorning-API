using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Redis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("Redis:InstanceName", Environment.GetEnvironmentVariable("var_redis_instance_name"));
            Environment.SetEnvironmentVariable("Redis:GetForecastTimeout", Environment.GetEnvironmentVariable("var_redis_get_forecast_timeout"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
