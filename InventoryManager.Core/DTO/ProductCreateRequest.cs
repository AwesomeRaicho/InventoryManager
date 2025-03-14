using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductCreateRequest
    {
        [Required]
        [StringLength(22, MinimumLength = 3)]
        public string? ProductNumber { get; set; }
        [Required]
        [StringLength(22, MinimumLength = 3)]
        public string? ProductName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Product requires a Product Type Id.")]
        public string? ProductTypeId {  get; set; }   

        public List<string>? PropertyIds {  get; set; }

    }
}
