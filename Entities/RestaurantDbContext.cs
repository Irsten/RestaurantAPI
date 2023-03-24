using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Restaurant> Restaurants { get; set;}
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        public RestaurantDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Category)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.HasDelivery)
                .IsRequired();

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.AddressId)
                .IsRequired();

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.ContactEmail)
                .IsRequired();

            modelBuilder.Entity<Dish>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<Dish>()
                .Property(r => r.Price)
                .IsRequired();

            modelBuilder.Entity<Dish>()
                .Property(r => r.RestaurantId)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(r => r.City)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(r => r.Street)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(r => r.PostalCode)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
