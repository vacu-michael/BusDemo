using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Worker.Consumers;

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
                services.AddMassTransit(x =>
                {
                    x.AddServiceBusMessageScheduler();

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<StartWorkflowCommandConsumer>();

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
                        if (string.IsNullOrWhiteSpace(connectionString))
                            throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

                        cfg.Host(connectionString);
                        cfg.ConfigureEndpoints(context);
                        cfg.UseServiceBusMessageScheduler();
                    });
                });
            });
}
