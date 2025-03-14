using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Models
{
    public class Product_Property
    {
        public Guid ProductId { get; set; }
        public Guid PropertyId { get; set; }

        //Navigation Properties
        public Product? Product { get; set; }
        public PropertyInstance? Property { get; set; }
    }
}
