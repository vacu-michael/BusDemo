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
                        cfg.UseServiceBusMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                });
            });
}
