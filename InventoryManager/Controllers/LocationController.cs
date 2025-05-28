using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManager.Core.Interfaces;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateRequest locationCreateRequest)
        {
            if(locationCreateRequest == null || string.IsNullOrEmpty(locationCreateRequest.Name))
            {
                return BadRequest(new { Error = "Location cannot be null." });
            }

            var response = await _locationService.CreateLocation(locationCreateRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {New_Location = response.Value});

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "id Cannot be null." });
            }

            var response = await _locationService.GetLocationById(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Location = response.Value});

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] LocationGetRequest locationGetRequest)
        {
            if(locationGetRequest == null)
            {
                return BadRequest(new { Error = "Location reqwuest cannot be null" });
            }

            var response = await _locationService.GetAllLocations(locationGetRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new
            {
                Locations = response.Value,
            });

        }

        [HttpPut]
        public async Task<IActionResult> Put(LocationPutRequest locationPutRequest)
        {
            if(locationPutRequest == null)
            {
                return BadRequest("Location request cannot be null.");
            }

            var response = await _locationService.UpdateLocation(locationPutRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Updated_Location = response.Value});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "Id must be provided when deleting Location"});
            }

            var response = await _locationService.DeleteLocation(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new { Deleted_Successfully = response.Value});
        }


    }
}
