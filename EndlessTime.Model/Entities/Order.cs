using System;

namespace EndlessTime.Model.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Status { get; set; } = "Sipariş Verildi";
}