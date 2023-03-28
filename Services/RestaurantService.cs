using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<int> Create(CreateRestaurantDto dto);
        Task<PagedResult<RestaurantDto>> GetAll(RestaurantQuery query);
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
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
        }
        public async Task<PagedResult<RestaurantDto>> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase != null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                                                    || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>()
                {
                    {nameof(Restaurant.Name), r => r.Name },
                    {nameof(Restaurant.Description), r => r.Description },
                    {nameof(Restaurant.Category), r => r.Category }
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC ?
                    baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = await baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToListAsync();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
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
