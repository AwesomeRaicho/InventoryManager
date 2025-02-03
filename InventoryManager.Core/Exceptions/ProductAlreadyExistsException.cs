using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Exceptions
{
    public class ProductAlreadyExistsException : Exception
    {
        public string? ProductName { get;}
        public string? ProductNumber { get;}

        public ProductAlreadyExistsException(string message, string? productName, string? productNumber) : base(message)
        {
            ProductName = productName;
            ProductNumber = productNumber;
        }



    }
}




