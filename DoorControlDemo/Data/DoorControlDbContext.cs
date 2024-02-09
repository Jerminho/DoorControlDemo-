using DoorControlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace DoorControlDemo.Data
{
    public class DoorControlDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Device> Devices { get; set; }

        // Constructor with DbContextOptions parameter
        public DoorControlDbContext(DbContextOptions<DoorControlDbContext> options) 
            : base(options)
        {
        }

        // Configure the relationships using Fluent API
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            // Configure the relationship between Badge and User

            modelbuilder.Entity<Badge>()
                //Each badge has one user
                .HasOne(b => b.User)
                // And each user can have many/multiple badges
                .WithMany(u => u.AssignedBadges)
                // The foreign key for this relationship is defined
                // in the 'Badge' class as 'UserId'
                .HasForeignKey(b => b.UserId);


            // Configure the relationship between User and Device

            modelbuilder.Entity<User>()
                // Each user has one device
                .HasOne(u => u.Device)
                // And each device has many/multiple users
                .WithMany(d => d.AssignedUsers)
                // The foreign key for this relationship is defined
                // in the 'User' class as 'DeviceId'
                .HasForeignKey(u => u.DeviceId);

            // Configure Device entity with a primary key
            modelbuilder.Entity<Device>()
                .HasKey(d => d.Id);
        }
    }
}
