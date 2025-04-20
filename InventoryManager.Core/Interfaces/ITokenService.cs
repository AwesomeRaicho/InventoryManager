using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Models;

namespace InventoryManager.Core.Interfaces
{
    public interface ITokenService
    {

        public string GenerateAccessToken(string userName, IList<string> roles);
        public TokenResponse GenerateToken(string userName, IList<string> roles);
    }
}
