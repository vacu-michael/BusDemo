namespace Models;

public class Application
{
    public int Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? AccountNumber { get; set; }
}