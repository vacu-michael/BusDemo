using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Worker.Consumers;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Worker;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<DAL.DemoDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration["Db:ConnectionString"]));
                services.AddScoped<BLL.DemoBusinessLogic>();
                services.AddScoped<SAL.BusService>();

                services.AddMassTransit(x =>
                {
                    x.AddServiceBusMessageScheduler();

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<StartWorkflowConsumer>();

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        var connectionString = hostContext.Configuration["AzureServiceBus:ConnectionString"];
                        if (string.IsNullOrWhiteSpace(connectionString))
                            throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

                        cfg.Host(connectionString, h =>
                        {
                            h.TransportType = Azure.Messaging.ServiceBus.ServiceBusTransportType.AmqpWebSockets;
                        });
                        cfg.ConfigureEndpoints(context);
                        cfg.UseServiceBusMessageScheduler();
                    });
                });
            });
}
