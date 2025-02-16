using InventoryManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IProductInstanceService
    {
        public Task<Result<ProductInstanceResponse>> CreateProductInstance(ProductInstanceCreateRequest productInstanceCreateRequest);
    }
}
