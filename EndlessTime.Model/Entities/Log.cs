using System;

namespace EndlessTime.Model.Entities;

public class Log
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string LogLevel { get; set; } 
    public string Message { get; set; }
    public string? Exception { get; set; }
}