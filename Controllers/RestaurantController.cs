using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;


        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> Create([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateRestaurationDto dto)
        {
            await _restaurantService.Update(id, dto);
            return Ok();
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
        {
            var restaurants = await _restaurantService.GetAll();
            return Ok(restaurants);
        }

        [HttpGet("get/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RestaurantDto>> GetById([FromRoute] int id)
        {
            var restaurant = await _restaurantService.GetById(id);
            return Ok(restaurant);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _restaurantService.Delete(id);
            return NoContent();
        }
    }
}
