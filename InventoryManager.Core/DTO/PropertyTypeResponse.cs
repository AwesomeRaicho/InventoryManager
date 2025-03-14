using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class PropertyTypeResponse
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }

    }
}
