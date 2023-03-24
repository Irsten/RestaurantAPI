using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;

        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast food",
                    Description = "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery= true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Price = 25.50,
                        },
                        new Dish()
                        {
                            Name = "Chicken Nuggets",
                            Price = 12.50,
                        },
                        new Dish()
                        {
                            Name = "Qurrito",
                            Price = 20.00,
                        },
                        new Dish()
                        {
                            Name = "iTwist",
                            Price = 8.90,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001",
                    }
                },
                new Restaurant()
                {
                    Name = "McDonald's",
                    Category = "Fast food",
                    Description = "McDonald's is an American fast food restaurant chain.",
                    ContactEmail = "contact@mcd.com",
                    HasDelivery= true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Supreme Chicken Sweet & Spicy Burger",
                            Price = 25.50,
                        },
                        new Dish()
                        {
                            Name = "Big Mac",
                            Price = 18.90,
                        },
                        new Dish()
                        {
                            Name = "Double McRoyal",
                            Price = 24.40,
                        },
                        new Dish()
                        {
                            Name = "Chikker",
                            Price = 8.90,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Szewska 2",
                        PostalCode = "31-009",
                    }
                },
            };
            return restaurants;
        }
    }
}
