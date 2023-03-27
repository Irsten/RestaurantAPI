using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public enum RestaurantOperation
    {
        Create,
        Read,
        Update,
        Delete,
    }

    public class RestaurantOperationRequirement : IAuthorizationRequirement
    {
        public RestaurantOperation RestaurantOperation { get; set;}
        
        public RestaurantOperationRequirement(RestaurantOperation restaurantOperation)
        {
            RestaurantOperation = restaurantOperation;
        }
    }
}
