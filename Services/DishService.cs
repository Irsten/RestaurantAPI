using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        Task<int> Create(int restaurantId, CreateDishDto dto);
        Task<List<DishDto>> GetAll(int restaurantId);
        Task<DishDto> GetById(int restaurantId, int dishId);
        Task DeleteAll(int restaurantId);
        Task DeleteById(int restaurantId, int dishId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public DishService(
            RestaurantDbContext dbContext,
            IMapper mapper,
            IAuthorizationService authorizationService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public async Task<int> Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = await GetRestaurantById(restaurantId);

            await CheckAuthorization(restaurant, DishOperation.Create);

            var dish = _mapper.Map<Dish>(dto);
            dish.RestaurantId = restaurant.Id;
            _dbContext.Dishes.Add(dish);
            _dbContext.SaveChanges();

            return dish.Id;
        }

        public async Task<List<DishDto>> GetAll(int restaurantId)
        {
            var restaurant = await GetRestaurantById(restaurantId);

            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }

        public async Task<DishDto> GetById(int restaurantId, int dishId)
        {
            var restaurant = await GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurant, dishId);

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public async Task DeleteAll(int restaurantId)
        {
            var restaurant = await GetRestaurantById(restaurantId);

            await CheckAuthorization(restaurant, DishOperation.Delete);

            _dbContext.Dishes.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }

        public async Task DeleteById(int restaurantId, int dishId)
        {
            var restaurant = await GetRestaurantById(restaurantId);

            await CheckAuthorization(restaurant, DishOperation.Delete);

            var dish = GetDishById(restaurant, dishId);

            _dbContext.Dishes.Remove(dish);
            _dbContext.SaveChanges();
        }

        // private methods
        private async Task<Restaurant> GetRestaurantById(int restaurantId)
        {
            var restaurant = await _dbContext
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null) { throw new NotFoundExcepiton("Restaurant not found."); }

            return restaurant;
        }

        private Dish GetDishById(Restaurant restaurant, int dishId)
        {
            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish == null || dish.RestaurantId != restaurant.Id) 
                { throw new NotFoundExcepiton("Dish not found."); }

            return dish;
        }

        private async Task CheckAuthorization(Restaurant restaurant, DishOperation operation)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new DishOperationRequirement(operation));
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException($"You have no permission to {operation} a dish for this restaurant.");
            }
        }
    }
}
