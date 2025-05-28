using InventoryManager.Core.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IUsersService
    {
        public Task<Result<UserResponse>> CreateUser(UserCreateRequest createRequest);

        public Task<Result<List<UserResponse>>> GetAllUsers();
        public Task<Result<bool>> DeleteUser(string userId);

        public Task<Result<UserResponse>> CheckUserAndPassword(TokenRequest request);

        public Result<bool> ValidateActiveRefreshToken(string userId, string refreshToken);

        public Task<Result<bool>> UpdateRefreshToken(string userId, string refreshToken);

        public Task<Result<bool>> RemoveRefreshToken(string userId);

    }
}
