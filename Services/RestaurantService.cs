using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<int> Create(CreateRestaurantDto dto);
        Task<IEnumerable<RestaurantDto>> GetAll();
        Task<RestaurantDto> GetById(int id);
        Task Update(int id, UpdateRestaurationDto dto);
        Task Delete(int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, 
            IMapper mapper, 
            IAuthorizationService authorizationService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public async Task<int> Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            await _dbContext.Restaurants.AddAsync(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public async Task Update(int id, UpdateRestaurationDto dto)
        {
            var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null) { throw new NotFoundExcepiton("Restaurant not found."); }

            await CheckAuthorization(restaurant, RestaurantOperation.Update);

            restaurant.Name = dto.Name;
            restaurant.Description= dto.Description;
            restaurant.HasDelivery= dto.HasDelivery;
            _dbContext.SaveChanges();
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
            if (restaurant == null) { throw new NotFoundExcepiton("Restaurant not found."); }
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantDto;
        }
        public async Task Delete(int id)
        {
            var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null) { throw new NotFoundExcepiton("Restaurant not found."); }

            await CheckAuthorization(restaurant, RestaurantOperation.Delete);

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        // private methods

        private async Task<AuthorizationResult> CheckAuthorization(Restaurant restaurant, RestaurantOperation operation)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new RestaurantOperationRequirement(operation));
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException($"You have no permission to {operation} a dish for this restaurant.");
            }

            return authorizationResult;
        }
    }
}
