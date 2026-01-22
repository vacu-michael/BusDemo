using DAL;
using Microsoft.Extensions.Logging;

namespace SAL;

public class EventService(EventDbContext dbContext, ILogger<EventService> logger)
{
    static readonly string applicationName = "BusDemo";
    public async Task LogEventAsync(string eventType, Guid correlationId, string description = "", object? data = null)
    {
        logger.LogDebug("Logging event of type {EventType} with CorrelationId {CorrelationId}.", eventType, correlationId);

        dbContext.ApplicationEvents.Add(new(
            eventType: eventType,
            correlationId: correlationId,
            applicationName: applicationName,
            description: description,
            data: data is null ? string.Empty : System.Text.Json.JsonSerializer.Serialize(data)
        ));
        await dbContext.SaveChangesAsync();
    }
}