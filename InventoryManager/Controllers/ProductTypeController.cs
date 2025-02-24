using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using System.Net.WebSockets;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {

        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeCreateRequest productTypeCreateRequest)
        {
            if (productTypeCreateRequest == null)
            {
                return BadRequest(new { Error = "ProductType cannot be null" });
            }

            var response = await _productTypeService.CreateProductType(productTypeCreateRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }


            return Ok(new { ProductType = response.Value });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { Error = "Product type ID cannot be null" });
            }

            var response = await _productTypeService.GetById(id);

            if (!response.IsSuccess)
            {
                return NotFound(new { Error = response.Error });
            }

            return Ok(new { ProductType = response.Value });

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductTypeGetRequest productTypeGetRequest)
        {
            if (productTypeGetRequest == null)
            {
                return BadRequest(new { Error = "Requestcannot be null." });
            }


            var res = await _productTypeService.GetAllProductTypes(productTypeGetRequest);

            if (!res.IsSuccess)
            {
                return BadRequest(new { Error = res.Error });
            }

            return Ok(new { ProductTypes = res.Value });

        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductTypePutRequest productTypePutRequest)
        {
            if(productTypePutRequest == null)
            {
                return BadRequest(new { Error = "Product being edited cannot be null" });
            }

            var result = await _productTypeService.UpdateProductType(productTypePutRequest);
            if (!result.IsSuccess)
            {
                return BadRequest(new {Error = result.Error});
            }

            return Ok(new {Updated_ProductType = result.Value});
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            if(string.IsNullOrWhiteSpace(Id))
            {
                return BadRequest(new { Error = "Id need to be provided to delete product type." });
            }

            var response = await _productTypeService.DeleteProductType(Id);

            if (!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Succesfully_Deleted = response.Value});


        }


    }
}
