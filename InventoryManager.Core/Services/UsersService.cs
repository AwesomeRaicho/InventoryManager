using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserResponse>> CreateUser(UserCreateRequest createRequest)
        {
            if(string.IsNullOrEmpty(createRequest.UserName))
            {
                return Result<UserResponse>.Failure("UserName cannot be null or empty");
            }

            if (string.IsNullOrEmpty(createRequest.Password))
            {
                return Result<UserResponse>.Failure("Password cannot be null or empty");
            }

            var dbUserCheck = await _userManager.FindByNameAsync(createRequest.UserName);

            if(dbUserCheck != null)
            {
                return Result<UserResponse>.Failure("User name already exists, please try a different one.");
            }

            var newUser = new ApplicationUser()
            {
                UserName = createRequest.UserName,
                NormalizedUserName = createRequest.UserName.ToUpper(),
                
            };

            var result = await _userManager.CreateAsync(newUser, createRequest.Password);

            if(result.Succeeded) 
            {
                var createdUser = await _userManager.FindByNameAsync(createRequest.UserName);
                if(createdUser != null)
                {
                    var userResponse = new UserResponse()
                    {
                        Id = createdUser.Id.ToString(),
                        Name = createdUser.UserName,
                    };

                    return Result<UserResponse>.Success(userResponse);
                }
                return Result<UserResponse>.Failure("Could not get created user, pklease refresh an try again.");
            }
            else
            {
                string? errors = null;
                
                foreach(var error in result.Errors)
                {
                    errors += $"• {error.Description} ";
                }

                return Result<UserResponse>.Failure(errors ?? "Something went wrong when creating user.");

            }

        }

        public async Task<Result<List<UserResponse>>> GetAllUsers()
        {
            var users = await _userManager.Users.Where(e =>  e.UserName != null).ToListAsync();

            if(users.Any())
            {
                var responseList = new List<UserResponse>();

                foreach(var user in users)
                {
                    var userRes = new UserResponse()
                    {
                        Id = user.Id.ToString(),
                        Name = user.UserName,
                        
                    };

                    responseList.Add(userRes);
                }

                return Result<List<UserResponse>>.Success(responseList);
            }
            else
            {
                return Result<List<UserResponse>>.Success(new List<UserResponse>());
            }
        }

        public async Task<Result<bool>> DeleteUser(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return Result<bool>.Failure("User ID cannot be null or blank.");
            }

            if(!Guid.TryParse(userId, out var ParsedId))
            {
                return Result<bool>.Failure("User ID is not the correct format.");
            }

            var dbUser = await _userManager.FindByIdAsync(userId);

            if(dbUser != null)
            {
                await _userManager.DeleteAsync(dbUser);
                return Result<bool>.Success(true);
            }
            else
            {
                return Result<bool>.Failure("User ID does not exist.");
            }
        }

        public async Task<Result<UserResponse>> CheckUserAndPassword(TokenRequest request)
        {
            if(request == null)
            {
                return Result<UserResponse>.Failure("Request cannot be null.");
            }
            if(string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return Result<UserResponse>.Failure("User name and password need to be provided.");

            }

            var dbUser = await _userManager.FindByNameAsync(request.UserName);

            if( dbUser == null )
            {
                return Result<UserResponse>.Failure("User name does not match an existing record.");

            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(dbUser, request.Password, false);

            var roles = await _userManager.GetRolesAsync(dbUser);

            var response = new UserResponse()
            {
                Id = dbUser.Id.ToString(),
                Name = request.UserName,
                Roles = roles,
            };

            if(signInResult.Succeeded)
            {
                return Result<UserResponse>.Success(response);
            }
            else
            {
                return Result<UserResponse>.Failure("User name or password does not match an existing record.");
            }

        }

        public async Task<Result<bool>> UpdateRefreshToken(string userId, string refreshToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Result<bool>.Failure("Invalid user id format.");
            }
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userGuid);

            if( user == null )
            {
                return Result<bool>.Failure("Could not find user with provided user id.");
            }

            user.CurrentRefreshToken = refreshToken;

            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(2);

            await _userManager.UpdateAsync(user);

            return Result<bool>.Success(true);

        }

        public async Task<Result<bool>> RemoveRefreshToken(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Result<bool>.Failure("Invalid user id format.");
            }
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userGuid);

            if (user == null)
            {
                return Result<bool>.Failure("Could not find user with provided user id.");
            }

            user.CurrentRefreshToken = null;

            user.RefreshTokenExpiresAt = null;

            await _userManager.UpdateAsync(user);

            return Result<bool>.Success(true);
        }

        public Result<bool> ValidateActiveRefreshToken(string userId, string refreshToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Result<bool>.Failure("Invalid user id format.");
            }

            var user = _userManager.Users.FirstOrDefault(u => u.Id == userGuid && u.CurrentRefreshToken == refreshToken);

            if (user == null)
            {
                return Result<bool>.Failure("No matching refresh token found.");
            }

            if (user.RefreshTokenExpiresAt == null)
            {
                return Result<bool>.Failure("Refresh token expiration date is missing.");
            }

            if (DateTime.UtcNow >= user.RefreshTokenExpiresAt.Value)
            {
                return Result<bool>.Failure("Refresh token has expired.");
            }

            return Result<bool>.Success(true);

        }
    }
}
