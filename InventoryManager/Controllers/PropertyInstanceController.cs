using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyInstanceController : ControllerBase
    {
        private readonly IPropertyInstanceService _propertyInstanceService;

        public PropertyInstanceController(IPropertyInstanceService propertyInstanceService)
        {
            _propertyInstanceService = propertyInstanceService;
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PropertyInstanceCreateRequest propertyInstanceCreateRequest)
        {
            if (propertyInstanceCreateRequest == null)
            {
                return BadRequest(new { Error = "Propoerty Instance create request cannot be null." });
            }

            if (string.IsNullOrEmpty(propertyInstanceCreateRequest.Name) || string.IsNullOrEmpty(propertyInstanceCreateRequest.PropertyTypeId))
            {
                return BadRequest(new { Error = "Name and PropertyTypeId cannot be null." });
            }

            var response = await _propertyInstanceService.CreatePropertyInstance(propertyInstanceCreateRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new { Property_Instance_Created = response.Value });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "Property Instance Id cannot be null." });
            }

            var response = await _propertyInstanceService.GetPropertyInstance(id);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new { Property_Instance = response.Value });
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PropertyInstanceGetRequest propertyInstanceGetRequest)
        {
            if (propertyInstanceGetRequest == null)
            {
                return BadRequest(new { Error = "Get Request cannot be null." });
            }

            var response = await _propertyInstanceService.GetAllPropertyInstance(propertyInstanceGetRequest);


            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new { Property_Instances = response.Value });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new {Error = "Delete ID cannot be null."});
            }

            var response = await _propertyInstanceService.DeletePropertyInstance(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Deleted_isSuccessful = response.Value});
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PropertyInstancePutRequest propertyInstancePutRequest )
        {
            if(propertyInstancePutRequest == null)
            {
                return BadRequest(new { Error = "Put Request cannot be null." });
            }

            var response = await _propertyInstanceService.UpdatePropertyInstance(propertyInstancePutRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Updated_PropeertyInstance = response.Value});

        }

        [HttpGet("by-property-type")]
        public async Task<IActionResult> GetByPropertyTypeId([FromQuery] PropertyInstanceGetRequest propertyInstanceGetRequest)
        {
            if (propertyInstanceGetRequest == null)
            {
                return BadRequest(new { Error = "Property request cannot be null." });
            }

            var response = await _propertyInstanceService.GetPropertyInstancesByPropertyTypeId(propertyInstanceGetRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });

            }

            return Ok(new { Property_Instances = response.Value });
        }




        [HttpGet("all-property-types-and-instances")]
        public async Task<IActionResult> AllPropertyTypesAndInstances()
        {
            var resDictionary = await _propertyInstanceService.GetAllPropertyTypesWithInstances();

            if(resDictionary.IsSuccess)
            {
                return Ok(new {all_properties = resDictionary });

            }else
            {
                return BadRequest(new {error = resDictionary.Error});
            }



        }

    }
}
