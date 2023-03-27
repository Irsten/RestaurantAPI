using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System.Security.Claims;

namespace RestaurantAPI.Authorization
{
    public class RestaurantOperationRequirementHandler : AuthorizationHandler<RestaurantOperationRequirement, Restaurant>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            RestaurantOperationRequirement requirement, 
            Restaurant restaurant)
        {
            if (requirement.RestaurantOperation == RestaurantOperation.Read ||
                requirement.RestaurantOperation == RestaurantOperation.Create)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (restaurant.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
