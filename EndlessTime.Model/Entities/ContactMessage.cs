using System;

namespace EndlessTime.Model.Entities;

public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public DateTime SentDate { get; set; } = DateTime.Now;

    
    public string? ReplyText { get; set; }
    public bool IsReplied { get; set; } = false;
}