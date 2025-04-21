using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            if(request == null)
            {
                return BadRequest(new {error = "UserCreateRequest cannot be null."});
            }

            var result = await _usersService.CreateUser(request);

            if(result.IsSuccess) 
            { 
                return Ok(new {New_User = result.Value});
            }
            else
            {
                return BadRequest(new {error = result.Error});
            }


        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =  await _usersService.GetAllUsers();

            return Ok(new {Users = result.Value});
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            if(string.IsNullOrEmpty(userId)) 
            {
                return BadRequest(new { error = "userId cannot be null" });
            }

            var result = await _usersService.DeleteUser(userId);

            if(result.IsSuccess) 
            { 
                return Ok();
            }else
            {
                return BadRequest(new {error = result.Error});
            }
        }
    }
}
