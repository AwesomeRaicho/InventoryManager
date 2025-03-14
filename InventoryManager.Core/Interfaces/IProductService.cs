using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IProductService
    {
        public Task<Result<ProductResponse>> CreateProduct(ProductCreateRequest productCreateRequest);
        public Task<ProductResponse?> GetById(string id);
        public Task<List<ProductResponse>?> GetAllProducts(ProductGetRequest productGetRequest);
        public Task<Result<ProductResponse>> UpdateProduct(ProductPutRequest productPutRequest);
        public Task<Result<bool>> DeleteProduct(string id);
        public Task<Result<List<ProductResponse>>> GetAllByProductTypeId(ProductGetRequest productGetRequest);
    }
}
