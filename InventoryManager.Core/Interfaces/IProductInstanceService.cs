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

        public Task<Result<ProductInstanceResponse>> GetProductInstanceById(string? id);

        public Task<Result<List<ProductInstanceResponse>>> GetAllProductInstances(ProductInstanceGetRequest productInstanceGetRequest);

        public Task<Result<ProductInstanceResponse>> UpdateProductInstance(ProductInstancePutRequest productInstancePutRequest);

        public Task<Result<ProductInstanceResponse>> DeleteProductInstance(string id);
 

        
        public Task<Result<List<ProductInstanceResponse>>> GetByProductId(ProductInstanceGetRequest propertyInstanceGetRequest);
    }
}

