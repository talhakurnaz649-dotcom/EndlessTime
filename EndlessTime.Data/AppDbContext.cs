using Microsoft.EntityFrameworkCore;
using EndlessTime.Model.Entities;
using System;

namespace EndlessTime.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1. Kategoriler
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Automatic" },
            new Category { Id = 2, Name = "Köstekli" },
            new Category { Id = 3, Name = "Spor" },
            new Category { Id = 4, Name = "Akıllı" },
            new Category { Id = 5, Name = "Kadın" },
            new Category { Id = 6, Name = "Erkek" }
        );

        // AppDbContext.cs içindeki Hazır Kullanıcılar alanını bu ID'lerle değiştirin:
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 100, 
                Email = "admin@endlesstime.com",
                PasswordHash = "admin123",
                FullName = "EndlessTime Admin",
                Role = "Admin",
                Address = "EndlessTime Yönetim Merkezi, İstanbul",
                PhoneNumber = "0212 555 5555",
                CardNumber = "0000 0000 0000 0000",
                CardHolderName = "ADMIN USER",
                CardExpiry = "12/29",
                CardCvv = "000"
            },
            new User
            {
                Id = 101, 
                Email = "user@endlesstime.com",
                PasswordHash = "asd123",
                FullName = "asddas",
                Role = "User",
                Address = "Kadıköy, İstanbul",
                PhoneNumber = "0555 555 5555",
                CardNumber = "1111 2222 3333 4444",
                CardHolderName = "asddas",
                CardExpiry = "05/30",
                CardCvv = "123"
            }
        );

        // 3. 12 Adet Lüks Saat
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Rolex Submariner Gold", Description = "18 karat altın kaplama otomatik lüks erkek saati.", Price = 640000, Stock = 3, ImageUrl = "https://images.unsplash.com/photo-1547996160-81dfa63595aa?q=80&w=600", CategoryId = 6 },
            new Product { Id = 2, Name = "Omega Speedmaster", Description = "Efsanevi otomatik kronograf İsviçre şaheseri.", Price = 380000, Stock = 5, ImageUrl = "https://images.unsplash.com/photo-1524592094714-0f0654e20314?q=80&w=600", CategoryId = 1 },
            new Product { Id = 3, Name = "Seiko Presage Cocktail", Description = "Şık deri kordonlu otomatik klasik erkek saati.", Price = 28000, Stock = 12, ImageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?q=80&w=600", CategoryId = 6 },
            new Product { Id = 4, Name = "Casio Retro Dijital", Description = "Klasik spor retro dijital çelik saat.", Price = 3200, Stock = 20, ImageUrl = "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?q=80&w=600", CategoryId = 3 },
            new Product { Id = 5, Name = "Minimalist Beyaz Saat", Description = "Sade tasarıma sahip deri kordonlu şık kadın saati.", Price = 4500, Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?q=80&w=600", CategoryId = 5 },
            new Product { Id = 6, Name = "Fossil Siyah Klasik", Description = "Mat siyah çelik spor erkek kol saati.", Price = 8500, Stock = 8, ImageUrl = "https://images.unsplash.com/photo-1612817288484-6f916006741a?q=80&w=600", CategoryId = 6 },
            new Product { Id = 7, Name = "Altın Köstekli Saat", Description = "Antika işlemeli, zincirli altın kaplama klasik köstekli saat.", Price = 75000, Stock = 2, ImageUrl = "https://images.unsplash.com/photo-1509048191080-d2984bad6ae5?q=80&w=600", CategoryId = 2 },
            new Product { Id = 8, Name = "Apple Watch Sport", Description = "Akıllı aktivite takibi yapan yeni nesil spor saat.", Price = 12000, Stock = 10, ImageUrl = "https://images.unsplash.com/photo-1434494878577-86c23bcb06b9?q=80&w=600", CategoryId = 4 },
            new Product { Id = 9, Name = "Tommy Hilfiger Sport", Description = "Koyu lacivert silikon kordonlu dayanıklı spor saat.", Price = 14500, Stock = 7, ImageUrl = "https://images.unsplash.com/photo-1542496658-e33a6d0d50f6?q=80&w=600", CategoryId = 3 },
            new Product { Id = 10, Name = "Daniel Wellington Rose", Description = "Rose gold kaplama ince kadranlı kadın kol saati.", Price = 7800, Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?q=80&w=600", CategoryId = 5 },
            new Product { Id = 11, Name = "Seiko Gold Chronograph", Description = "Altın sarısı çelik kasa ve kordonlu erkek kronograf saati.", Price = 34000, Stock = 6, ImageUrl = "https://images.unsplash.com/photo-1509048191080-d2984bad6ae5?q=80&w=600", CategoryId = 6 },
            new Product { Id = 12, Name = "Huawei Smart Fit Black", Description = "AMOLED ekranlı, nabız ölçerli siyah akıllı saat.", Price = 9500, Stock = 11, ImageUrl = "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?q=80&w=600", CategoryId = 4 }
        );

        // 4. Sistem Günlükleri
        modelBuilder.Entity<Log>().HasData(
            new Log { Id = 1, LogLevel = "Information", Message = "Veritabanı bağlantısı başarıyla kuruldu. Tablo şema doğrulaması tamamlandı.", Timestamp = DateTime.Now.AddDays(-2), Exception = "" },
            new Log { Id = 2, LogLevel = "Information", Message = "Test yöneticisi hesabı (admin@endlesstime.com) sisteme tanımlandı.", Timestamp = DateTime.Now.AddDays(-2), Exception = "" },
            new Log { Id = 3, LogLevel = "Warning", Message = "Yüksek yoğunluklu şüpheli giriş denemesi engellendi (IP: 192.168.1.107).", Timestamp = DateTime.Now.AddDays(-1), Exception = "" },
            new Log { Id = 4, LogLevel = "Information", Message = "Müşteri 'Ahmet Yılmaz' (user@endlesstime.com) yeni bir hesap oluşturdu.", Timestamp = DateTime.Now.AddHours(-12), Exception = "" },
            new Log { Id = 5, LogLevel = "Information", Message = "Rolex Submariner Gold ürünü için stok seviyesi 3 adede güncellendi.", Timestamp = DateTime.Now.AddHours(-8), Exception = "" },
            new Log { Id = 6, LogLevel = "Information", Message = "QuestPDF kütüphanesi başarıyla yüklendi. PDF çıktı servisi aktif.", Timestamp = DateTime.Now.AddHours(-5), Exception = "" },
            new Log { Id = 7, LogLevel = "Warning", Message = "Kullanıcı 'Ahmet Yılmaz' admin yetkilendirme paneline sızmaya çalıştı; yetkisiz erişim engellendi.", Timestamp = DateTime.Now.AddHours(-2), Exception = "" },
            new Log { Id = 8, LogLevel = "Error", Message = "SQL Server LocalDB kilitli dosya erişimi algılandı. Bağlantı otomatik yenilendi.", Timestamp = DateTime.Now.AddMinutes(-30), Exception = "System.IO.IOException: File is locked by another IIS process." }
        );

        // 5. İletişim Mesajları (Hazır Cevap Bilgileriyle Seed Yapıldı 🚨)
        modelBuilder.Entity<ContactMessage>().HasData(
            new ContactMessage { Id = 1, Name = "Ahmet Yılmaz", Email = "user@endlesstime.com", Subject = "Kargo Süresi", MessageText = "İzmir'e siparişler kaç günde geliyor? Teşekkürler.", SentDate = DateTime.Now.AddDays(-1), ReplyText = "Merhabalar Ahmet Bey, İzmir siparişlerimiz 24 saat içinde teslim edilmektedir.", IsReplied = true },
            new ContactMessage { Id = 2, Name = "Ahmet Yılmaz", Email = "user@endlesstime.com", Subject = "Özel Ahşap Kutu", MessageText = "Omega saat orijinal kutusunda mı gönderiliyor?", SentDate = DateTime.Now.AddHours(-5), ReplyText = null, IsReplied = false }
        );
    }
}