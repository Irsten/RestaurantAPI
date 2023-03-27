using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public enum DishOperation
    {
        Create,
        Read,
        Update,
        Delete,
    }

    public class DishOperationRequirement : IAuthorizationRequirement
    {
        public DishOperation DishOperation { get; set; }

        public DishOperationRequirement(DishOperation dishOperation)
        {
            DishOperation = dishOperation;
        }
    }
}
