using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using InventoryManager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;


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
            if (true)
            {
                await _productService.CreateProduct(productRequest);
            }
            

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

            

            return Ok(productRequest);
        }


        //Read
        [HttpGet]
        public IActionResult Get([FromQuery] ProductGetRequest productGetRequest) 
        {

            return Ok(productGetRequest);
        }

        //Update


        //Delete
    }
}
