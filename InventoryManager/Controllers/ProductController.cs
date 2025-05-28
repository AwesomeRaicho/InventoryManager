using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using InventoryManager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection.Metadata.Ecma335;
using System.Net.WebSockets;


namespace InventoryManager.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProduct_PropertyService _productPropertyService;

        public ProductController(IProductService productService, IProduct_PropertyService productPropertyService)
        {
            _productService = productService;
            _productPropertyService = productPropertyService;
        }
        //Read
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID( string id) 
        {

            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty", nameof(id));
            }

            var result = await _productService.GetById(id);

            if(result == null)
            {
                return NotFound(new {Error = "Id provided does not match any existing products."});
            }

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] ProductGetRequest productGetRequest)
        {

            var productResponses = await _productService.GetAllProducts(productGetRequest);

            return Ok(productResponses);
        }

        //admin create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest productRequest)
        {
            
            if(productRequest == null)
            {
                return BadRequest(new {Error = "Create request cannot be null"}); 
            }

            var response = await _productService.CreateProduct(productRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new {Error = response.Error});
            }

            return Ok(new {Product_Created = response.Value});
        }



        //Update
        [HttpPut]
        public async Task<IActionResult> Put(ProductPutRequest productPutRequest)
        {

            var updatedProduct = await _productService.UpdateProduct(productPutRequest);

            if(updatedProduct.IsSuccess)
            {
                return Ok(new {updatedProduct.Value });
            }
            else
            {
                return BadRequest(new {updatedProduct.Error });
            }

        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null when deleteing.", nameof(id));
            }

            var deleteProduct = await _productService.DeleteProduct(id);

            if(deleteProduct.IsSuccess)
            {
                return Ok(new {deleteProduct.Value});
            }
            else
            {
                return BadRequest(new {deleteProduct.Error});
            }
        }

        [HttpGet("by-product-type")]
        public async Task<IActionResult> GetByProductId([FromQuery] ProductGetRequest productGetRequest)
        {

            if(productGetRequest == null)
            {
                return BadRequest(new {Error = "Get request cannot be null."});
            }

            var response = await _productService.GetAllByProductTypeId(productGetRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(new { Error = response.Error });
            }


            return Ok(new {Product_List = response.Value});
        }
    }
}
