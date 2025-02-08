using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IProductService
    {
        public Task<bool> CreateProduct(ProductCreateRequest productCreateRequest);
        public Task<ProductResponse?> GetById(string id);
        public Task<List<ProductResponse>?> GetAllProducts(ProductGetRequest productGetRequest);

    }
}
