using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Persistence
{
    public class WebApiDbContext : DbContext
    {
        public WebApiDbContext(DbContextOptions<WebApiDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<PaymentWebhook> PaymentWebhooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de índices
            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.IsActive);

            modelBuilder.Entity<AppSetting>()
                .HasKey(s => s.Key);

            modelBuilder.Entity<PaymentWebhook>()
                .HasIndex(p => p.Processed);

            // Seed de configuraciones iniciales
            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting
                {
                    Key = "CompanyName",
                    Value = "AzzDashboard",
                    CreatedAt = DateTime.UtcNow
                },
                new AppSetting
                {
                    Key = "SupportEmail",
                    Value = "support@azzdashboard.com",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
