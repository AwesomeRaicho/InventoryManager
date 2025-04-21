using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;




namespace InventoryManager.Core.Interfaces
{
    public interface IJwtService
    {
        public TokenResponse GenerateTokenAsync(string userId, string username, IEnumerable<string> roles);

        public string? GenerateRefreshToken();

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
