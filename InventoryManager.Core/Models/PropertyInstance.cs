using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Models
{
    public class PropertyInstance
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        
        public byte[]? ConcurrencyStamp { get; set; }

        // Foreign Key
        public Guid? PropertyTypeId { get; set; }

        // Navigation property
        public PropertyType? PropertyType { get; set; }

        //one-to-many
        public List<ProductInstance_Property>? ProductInstance_Property { get; set; }
    }
}
