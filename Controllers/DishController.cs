using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost("create-dish")]
        public async Task<ActionResult> CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var dishId = await _dishService.Create(restaurantId, dto);
            return Created($"/api/{restaurantId}/dish/{dishId}", null);
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetAll([FromRoute] int restaurantId)
        {
            //var dishes = await _dishService.GetAll(restaurantId);
            var result = await _dishService.GetAll(restaurantId);

            return Ok(result);
        }

        [HttpGet("get/{dishId}")]
        public async Task<ActionResult<DishDto>> GetById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = await _dishService.GetById(restaurantId, dishId);
            return Ok(dish);
        }

        [HttpDelete("delete-all")]
        public async Task<ActionResult> DeleteAll([FromRoute] int restaurantId)
        {
            await _dishService.DeleteAll(restaurantId);
            return NoContent();
        }

        [HttpDelete("delete/{dishId}")]
        public async Task<ActionResult> DeleteById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            await _dishService.DeleteById(restaurantId, dishId);
            return NoContent();
        }
    }
}
