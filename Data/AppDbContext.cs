using AllulExpressApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AllulExpressApi.Data
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Represent the "Employees" table
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<ValidToken> ValidTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Clients>()
                  .HasIndex(c => c.Phonenum1)
                  .IsUnique();

            modelBuilder.Entity<Employees>()
           .HasIndex(c => c.Phonenum1)
           .IsUnique();
            modelBuilder.Entity<Drivers>()
           .HasIndex(c => c.Phonenum1)
           .IsUnique();
            modelBuilder.Entity<Drivers>()
            .HasMany(d => d.Cities)
            .WithMany(c => c.Drivers)
            .UsingEntity(j => j.ToTable("DriverCities"));

            modelBuilder.Entity<Posts>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.ClientId);
            // modelBuilder.Entity<Posts>()
            // .HasOne(p => p.Driver)
            // .WithMany(d => d.Posts)
            // .HasForeignKey(p => p.DriverId);


        }

    }
}
