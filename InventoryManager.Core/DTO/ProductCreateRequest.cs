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

        public Guid? ProductTypeId {  get; set; }   

    }
}
