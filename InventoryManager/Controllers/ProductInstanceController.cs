using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInstanceController : ControllerBase
    {
        private readonly IProductInstanceService _productInstanceService;
       public ProductInstanceController(IProductInstanceService productInstanceService) 
        { 
            _productInstanceService = productInstanceService;
        }


        //Create
        [HttpPost]
        public async Task<IActionResult> CreateProductInstance([FromBody] ProductInstanceCreateRequest productInstanceCreateRequest)
        {
            if (productInstanceCreateRequest == null)
            {
                return BadRequest();
            }

            var result = await _productInstanceService.CreateProductInstance(productInstanceCreateRequest);

            if(result.IsSuccess) 
            { 
                return Ok(result); 
            }else
            {
                return BadRequest(new {Success = "False", Error = result.Error});
            }

        }
        //Read
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductInstance(string? id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest(new { error = "Id must be provided" });
            }

            //get instance:
            var response = await _productInstanceService.GetProductInstanceById(id);

            if(!response.IsSuccess)
            {
                return NotFound(new { Error = response.Error});
            }

            return Ok(new { ProductInstance = response.Value });

        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductInstance([FromQuery] ProductInstanceGetRequest productInstanceGetRequest)
        {
            if(productInstanceGetRequest == null)
            {  return BadRequest(new {Error = "Get request cannot be null."}); }

            var result = await _productInstanceService.GetAllProductInstances(productInstanceGetRequest);

            if(!result.IsSuccess)
            {
                return BadRequest(new {Error = result.Error});
            }

            return Ok(new {ProductInstances = result.Value});

        }

        //Update
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductInstancePutRequest productInstancePutRequest)
        {

            if(productInstancePutRequest == null)
            {
                return BadRequest(new { Error = "Product instance being updated cannot be null" });
            }

            var response = await _productInstanceService.UpdateProductInstance(productInstancePutRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }

            return Ok(new {UpdatedInstance = response.Value });
        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "Id cannot be blank." });
            }

            var response = await _productInstanceService.DeleteProductInstance(id);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = "Something when wrong, could not remove product instance."});
            }

            return Ok(new {Result = "Product instance has been deleted."});
        }

        [HttpGet("by-product")]
        public async Task<IActionResult> ByProduct([FromQuery] ProductInstanceGetRequest productInstanceGetRequest)
        {
            if(productInstanceGetRequest == null)
            {
                return BadRequest(new { Error = "Get request cannot be null."});
            }

            var response = await _productInstanceService.GetByProductId(productInstanceGetRequest);

            if(!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }


            return Ok(new {product_instances_by_product = response.Value});

        }

        [HttpGet("by-product-sold")]
        public async Task<IActionResult> ByProductSold([FromQuery] ProductInstanceGetRequest productInstanceGetRequest)
        {
            if (productInstanceGetRequest == null)
            {
                return BadRequest(new { Error = "Get request cannot be null." });
            }

            var response = await _productInstanceService.GetSoldByProductId(productInstanceGetRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }


            return Ok(new { product_instances_by_product = response.Value });

        }



    }
}

