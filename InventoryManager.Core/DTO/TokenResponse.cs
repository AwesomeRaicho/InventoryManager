using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class TokenResponse
    {
        [Required]
        public string? AccessToken { get; set; }
        
        [Required]
        public string? RefreshToken { get; set; }
        
        [Required]
        public DateTime ExpiresAt { get; set; }

        
    }
}
