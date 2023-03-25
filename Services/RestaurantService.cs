using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<int> CreateRestaurant(CreateRestaurantDto dto);
        Task<IEnumerable<RestaurantDto>> GetAll();
        Task<RestaurantDto> GetById(int id);
        Task<bool> Delete(int id);
        Task<bool> Update(int id, UpdateRestaurationDto dto);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> CreateRestaurant(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            await _dbContext.Restaurants.AddAsync(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public async Task<bool> Update(int id, UpdateRestaurationDto dto)
        {
            var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null) { return false; }
            restaurant.Name = dto.Name;
            restaurant.Description= dto.Description;
            restaurant.HasDelivery= dto.HasDelivery;
            _dbContext.SaveChanges();

            return true;
        }
        public async Task<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurants = await _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToListAsync();
            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantsDtos;
        }

        public async Task<RestaurantDto> GetById(int id)
        {
            var restaurant = await _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null) { return null; }
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantDto;
        }
        public async Task<bool> Delete(int id)
        {
            var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null) { return false; }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
