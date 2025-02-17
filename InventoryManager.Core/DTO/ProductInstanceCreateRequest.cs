using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductInstanceCreateRequest
    {
        [Required]
        public string? Barcode { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? LocationId { get; set; }
        public string? Status { get; set; }

        [Required]
        public string? ProductId { get; set; }
        public List<string>? PropertyIds { get; set; }
    }
}
