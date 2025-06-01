using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpPost("submit-form")]
        public IActionResult SubmitForm(TestFormRequest formRequest)
        {
            if (formRequest == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
