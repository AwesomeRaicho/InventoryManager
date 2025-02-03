using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using InventoryManager.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }


        public async Task<bool> CreateProduct(ProductCreateRequest productCreateRequest)
        {
            if(true)
            {
                throw new ProductAlreadyExistsException("Product baing added may already exist, any of these two values may already be used.", productCreateRequest.ProductName, productCreateRequest.ProductNumber);
            }


            if(productCreateRequest == null)
            {
                throw new ArgumentNullException($"Product data cannot be empty");
            }

            var productDb = await _repository.Find(e => e.ProductNumber == productCreateRequest.ProductNumber || e.ProductName == productCreateRequest.ProductName);

            


        }


        public Product GetById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} cannot be null or empty");
            }

            return new Product();
        }

    }
}
