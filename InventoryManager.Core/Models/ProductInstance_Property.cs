using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Models
{
    public class ProductInstance_Property
    {
        public Guid ProductInstanceId { get; set; }
        public Guid PropertyId { get; set; }

        //Navigation Properties
        public ProductInstance? ProductInstance { get; set; }
        public PropertyInstance? Property { get; set; }
    }
}
