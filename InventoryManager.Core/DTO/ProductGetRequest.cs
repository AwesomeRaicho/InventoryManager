using InventoryManager.Core.Enums;
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
        [StringLength(22, MinimumLength = 3)]
        public string? SearchText { get; set; }
        public string? ProductId { get; set; }
        public string? ProductType { get; set; }
        public string? ProductTypeId { get; set;}
        
        public int? PageNumber { get; set; } = 0;
        
        [Range(20, 100)]
        public int? PageSize { get; set;} = 0;
        public OrderBy OrderBy { get; set; } = OrderBy.Desc;
    }
}
