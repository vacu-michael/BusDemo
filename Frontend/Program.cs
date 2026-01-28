using BLL;
using DAL;
using SAL;
using Microsoft.EntityFrameworkCore;
using Frontend.Components;
using Frontend.Consumers;
using MassTransit;
using Models;
using Models.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure MassTransit with Azure Service Bus
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<ProcessCompletedConsumer>();
    x.AddConsumer<ProcessRescheduledConsumer>();
    x.AddConsumer<ProcessErrorConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

        cfg.Host(connectionString, h =>
        {
            h.TransportType = Azure.Messaging.ServiceBus.ServiceBusTransportType.AmqpWebSockets;
        });

        // Configure subscription endpoints for one-to-many messaging
        cfg.SubscriptionEndpoint<ProcessCompleted>("process-completed-subscription", e =>
        {
            e.ConfigureConsumer<ProcessCompletedConsumer>(context);
        });

        cfg.SubscriptionEndpoint<CommandDeferred>("process-rescheduled-subscription", e =>
        {
            e.ConfigureConsumer<ProcessRescheduledConsumer>(context);
        });

        cfg.SubscriptionEndpoint<ProcessErrored>("process-error-subscription", e =>
        {
            e.ConfigureConsumer<ProcessErrorConsumer>(context);
        });
    });
});

// Register DemoDbContext with connection string from user-secrets (Db:ConnectionString)
builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseSqlServer(builder.Configuration["Db:ConnectionString"]));

// Register BusService and DemoBusinessLogic for DI
builder.Services.AddSingleton<BusService>();
builder.Services.AddScoped<DemoBusinessLogic>();
builder.Services.AddScoped<AdminBusinessLogic>();

// Register EventDbContext and Helper service for logging application events
builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseSqlServer(builder.Configuration["Db:ConnectionString"]));
builder.Services.AddScoped<EventService>();

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
