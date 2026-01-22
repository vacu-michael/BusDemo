namespace Models;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents an application event for auditing and tracking.
/// </summary>
public class ApplicationEvent
{
    /// <summary>
    /// Gets or sets the unique event identifier (primary key).
    /// </summary>
    [Key]
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracking the event.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the application where the event originated.
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets serialized event data.
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationEvent"/> for EF Core.
    /// </summary>
    public ApplicationEvent() { }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationEvent"/> with required properties.
    /// </summary>
    /// <param name="correlationId">Correlation identifier.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <param name="description">Event description.</param>
    /// <param name="applicationName">Name of the application.</param>
    /// <param name="data">Serialized event data.</param>
    public ApplicationEvent(
        Guid correlationId,
        string eventType,
        string description = "",
        string applicationName = "",
        string data = "")
    {
        CorrelationId = correlationId;
        EventType = eventType;
        Description = description;
        ApplicationName = applicationName;
        Data = data;
        Timestamp = DateTime.UtcNow;
    }
}