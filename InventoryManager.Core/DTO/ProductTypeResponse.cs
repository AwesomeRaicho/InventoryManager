using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductTypeResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }

    }
}
