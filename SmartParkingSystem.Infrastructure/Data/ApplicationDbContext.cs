using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Core.Entities;
using SmartParkingSystem.Infrastructure.Identity;

namespace SmartParkingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ParkingZone> ParkingZones { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ParkingZone>()
                .Property(pz => pz.HourlyRate)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Reservation>()
                .Property(r => r.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure relation between Zone and Slot
            builder.Entity<ParkingSlot>()
                .HasOne(s => s.ParkingZone)
                .WithMany(z => z.Slots)
                .HasForeignKey(s => s.ParkingZoneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
