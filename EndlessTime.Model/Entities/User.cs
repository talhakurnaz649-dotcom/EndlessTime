namespace EndlessTime.Model.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; } 


    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? CardNumber { get; set; }
    public string? CardHolderName { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvv { get; set; }
}