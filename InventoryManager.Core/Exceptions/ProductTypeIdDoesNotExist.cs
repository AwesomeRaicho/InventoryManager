using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Exceptions
{
    public class ProductTypeIdDoesNotExist : Exception
    {
        public string? ProductTypeId { get; }

        public ProductTypeIdDoesNotExist(string message, string? productTypeId) : base(message)
        {
            ProductTypeId = productTypeId;
        }
    }
}
