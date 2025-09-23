using BLL;
using DAL;
using SAL;
using Microsoft.EntityFrameworkCore;
using Frontend.Components;
using Frontend.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure MassTransit with Azure Service Bus
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<WorkflowCompletedConsumer>();
    x.AddConsumer<WorkflowRescheduledConsumer>();
    x.AddConsumer<WorkflowErrorConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {

        var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
        cfg.Host(connectionString, h =>
        {
            h.TransportType = Azure.Messaging.ServiceBus.ServiceBusTransportType.AmqpWebSockets;
        });
        cfg.ConfigureEndpoints(context);
    });
});

// Register DemoDbContext with connection string from user-secrets (Db:ConnectionString)
builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseSqlServer(builder.Configuration["Db:ConnectionString"]));

// Register BusService and DemoBusinessLogic for DI
builder.Services.AddSingleton<BusService>();
builder.Services.AddScoped<DemoBusinessLogic>();
builder.Services.AddScoped<AdminBusinessLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
