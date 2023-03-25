using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;


        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost("create-restaurant")]
        public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var id = await _restaurantService.CreateRestaurant(dto);

            return Created($"/api/restaurant/{id}", null);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateRestaurationDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var isUpdated = await _restaurantService.Update(id, dto);
            if(!isUpdated) { return NotFound(); }

            return Ok();
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
        {
            var restaurants = await _restaurantService.GetAll();

            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetById([FromRoute] int id)
        {
            var restaurant = await _restaurantService.GetById(id);
            if (restaurant == null) { return NotFound("This restaurant does not exist."); }

            return Ok(restaurant);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var isDeleted = await _restaurantService.Delete(id);
            if(!isDeleted) { return NotFound("This restaurant does not exist."); }
            return NoContent();
        }
    }
}
