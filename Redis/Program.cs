using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Redis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("Redis:InstanceName", Environment.GetEnvironmentVariable("var_redis_instance_name"));
            Environment.SetEnvironmentVariable("Redis:GetForecastTimeout", Environment.GetEnvironmentVariable("var_redis_get_forecast_timeout"));
            Environment.SetEnvironmentVariable("Redis:ConnectionString", Environment.GetEnvironmentVariable("var_redis_conn_string"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureLogging(loggerBuilder =>
          {
              loggerBuilder
                  .ClearProviders()
                  // Example output: [20/11/20 14:31:30:409] info: ...
                  .AddConsole(configure => configure.TimestampFormat = "[dd/MM/yy HH:mm:ss:fff] ");

          })
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.UseStartup<Startup>();
          });
    }
}
