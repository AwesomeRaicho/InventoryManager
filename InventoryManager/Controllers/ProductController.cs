using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using InventoryManager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection.Metadata.Ecma335;


namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //admin create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest productRequest)
        {
            

            if (!ModelState.IsValid)
            {
                var modelstate = ModelState.Where(e => e.Value != null).SelectMany(v => v.Value != null ? v.Value.Errors : new ModelErrorCollection(), (v, e) => new
                    {
                        Field = v.Key,
                        Value = v.Value?.Errors
                    }
                );

                return BadRequest(modelstate);
            }

            var created = await _productService.CreateProduct(productRequest);



            return Ok(productRequest);
        }


        //Read
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(string id) 
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

        //Update


        //Delete
    }
}
