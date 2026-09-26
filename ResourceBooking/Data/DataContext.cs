using Microsoft.EntityFrameworkCore;
using ResourceBooking.Models;

namespace ResourceBooking.Data
{
    // DataContext inheriting from DbContext
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure email is unique
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // One-to-many relationship between User and Booking
            modelBuilder
                .Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            // One-to-many relationship between ResourceType and Resource
            modelBuilder
                .Entity<ResourceType>()
                .HasMany(rt => rt.Resources)
                .WithOne(r => r.ResourceType)
                .HasForeignKey(r => r.ResourceTypeId);

            // One-to-many relationship between Resource and Booking
            modelBuilder
                .Entity<Resource>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Resource)
                .HasForeignKey(b => b.ResourceId);

            // Ensure ResourceType is unique
            modelBuilder.Entity<ResourceType>().HasIndex(rt => rt.TypeName).IsUnique();

            modelBuilder
                .Entity<Booking>()
                .HasIndex(b => new
                {
                    b.ResourceId,
                    b.StartDate,
                    b.EndDate,
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
