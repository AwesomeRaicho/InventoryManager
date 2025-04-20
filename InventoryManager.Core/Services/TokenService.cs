using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, string> _tokenStore = new Dictionary<string, string>();


        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateAccessToken(string userName, IList<string> roles)
        {
            throw new NotImplementedException();
        }

        public TokenResponse GenerateToken(string userName, IList<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}
