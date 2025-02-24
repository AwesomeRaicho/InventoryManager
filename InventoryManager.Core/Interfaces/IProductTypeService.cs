using InventoryManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IProductTypeService
    {
        public Task<Result<ProductTypeResponse>> CreateProductType(ProductTypeCreateRequest productTypeCreateRequest);
        public Task<Result<ProductTypeResponse>> GetById(string id);
        public Task<Result<List<ProductTypeResponse>>> GetAllProductTypes(ProductTypeGetRequest productTypeGetRequest);
        public Task<Result<ProductTypeResponse>> UpdateProductType(ProductTypePutRequest productTypePutRequest);

        public Task<Result<bool>> DeleteProductType(string id);
    }
}
