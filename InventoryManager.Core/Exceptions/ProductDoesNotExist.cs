using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Exceptions
{
    public class ProductDoesNotExist : Exception
    {
        public string? ProductId { get; }

        public ProductDoesNotExist(string message, string? productId) : base(message)
        { 
            ProductId = productId;
        }
    }
}
