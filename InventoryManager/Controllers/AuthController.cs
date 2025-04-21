using InventoryManager.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryManager.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUsersService _usersService;

        public AuthController(IJwtService jwtService, IUsersService usersService)
        {
            _jwtService = jwtService;
            _usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            if(request == null)
            {
                return BadRequest(new {error = "TokenRequest cannot be null."});
            }

            var correctCreds = await _usersService.CheckUserAndPassword(request);

            if (correctCreds.IsSuccess)
            {
                string? usernName = correctCreds.Value?.Name;
                string? userId = correctCreds.Value?.Id;
                var roles = correctCreds.Value?.Roles;

                if (!string.IsNullOrEmpty(usernName) && !string.IsNullOrEmpty(userId) && roles != null)
                {
                    var token = _jwtService.GenerateTokenAsync(userId, usernName, roles);

                    return Ok(token);

                }
                else
                {
                    return BadRequest(new {error = "Something went wrong creating Token, please try again later."});
                }
            }
            else
            {
                return BadRequest(new { error = correctCreds.Error });
            }






            //Mock login logic(replace with real user lookup)
            //if (request.UserName == "admin" && request.Password == "admin123")
            //{
            //    var roles = new List<string> { "Admin" };
            //    var token = _jwtService.GenerateTokenAsync("1", request.UserName, roles);
            //    return Ok(token);
            //}

            //if (request.UserName == "employee" && request.Password == "employee123")
            //{
            //    var roles = new List<string> { "Employee" };
            //    var token = _jwtService.GenerateTokenAsync("2", request.UserName, roles);
            //    return Ok(token);
            //}

            //return Unauthorized("Invalid credentials");

        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenResponse expiredToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(expiredToken.AccessToken);
            if (principal == null)
                return BadRequest("Invalid token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = principal.Identity?.Name;
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);


            var token = _jwtService.GenerateTokenAsync(userId ?? "", username ?? "", roles);
            return Ok(token);
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult Test() => Ok($"Authenticated as {User.Identity?.Name}");

        [HttpGet("admin-only")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AdminOnly() => Ok("You are an admin.");

        [HttpGet("employee-only")]
        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeOnly() => Ok("You are an employee.");
    }
}
