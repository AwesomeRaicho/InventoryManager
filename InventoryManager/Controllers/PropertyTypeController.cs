using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.DTO;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyTypeController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PropertyTypeCreateRequest propertyTypeCreateRequest)
        {
            if(propertyTypeCreateRequest == null)
            {
                return BadRequest(new { Error = "Property Type Cannot be null."});
            }

            var response = await _propertyTypeService.Create(propertyTypeCreateRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new {PropertyType = response.Value});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "ProductType Id is required." });
            }

            var response = await _propertyTypeService.GetById(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new {propertyType = response.Value});

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PropertyTypeGetRequest propertyTypeGetRequest)
         {
            if(propertyTypeGetRequest == null)
            {
                return BadRequest(new { Error = "Get request Cannot be null." });
            }

            var response = await _propertyTypeService.GetAll(propertyTypeGetRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new {PropertyTypes = response.Value});

        }

        [HttpPut]
        public async Task<IActionResult> Update(PropertyTypePutRequest propertyTypePutRequest)
        {
            if(propertyTypePutRequest == null)
            {
                return BadRequest(new { Error = "PropertyTypePutRequest cannot be null." });
            }

            var response = await _propertyTypeService.Update(propertyTypePutRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Updated_PropertyType = response.Value});

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "PropertyType ID is required." });
            }

            var response = await _propertyTypeService.Delete(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new { Deleted_Successfully = response.Value});
        }

    }
}
