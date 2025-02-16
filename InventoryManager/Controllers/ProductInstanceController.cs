using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInstanceController : ControllerBase
    {




        //Create
        [HttpPost]
        public Task<IActionResult> CreateProductInstance([FromBody] ProductInstanceCreateRequest productInstanceCreateRequest)
        {
            
        }
        //Read
        //Update
        //Delete
        
    }
}
