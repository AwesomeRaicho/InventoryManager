using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductGetRequest
    {
        public string? SearchText { get; set; }
        
        public string? ProductId { get; set; }
        
        [StringLength(22, MinimumLength = 3)]
        public string? ProductNumber { get; set; }
        
        [StringLength(22, MinimumLength = 3)]
        public string? ProductName { get; set; }



    }
}
