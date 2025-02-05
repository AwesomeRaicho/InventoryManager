using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using InventoryManager.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductType> _productTypeRepository;

        public ProductService(IRepository<Product> productRepository, IRepository<ProductType> productTypeRepository)
        {
            _productRepository = productRepository;
            _productTypeRepository = productTypeRepository;
        }

        /// <summary>
        /// Turn Product entity into ProductResponse DTO
        /// </summary>
        /// <param name="product">Product entity</param>
        /// <returns></returns>
        public ProductResponse? GetProductResponse(Product product)
        {
            if(product == null)
            {
                return null;
            }

            return new ProductResponse()
            {
                Id = product.Id,
                ConcurrencyStamp = product.ConcurrencyStamp,
                Price = product.Price,
                ProductName = product.ProductName,
                ProductNumber = product.ProductNumber,
                StockAmount = product.StockAmount,
                ProductTypeName = product.ProductType != null ? product.ProductType.Name : "",
                ProductTypeId = product.ProductTypeId.ToString(),
            };
        }


        public async Task<bool> CreateProduct(ProductCreateRequest productCreateRequest)
        {

            if(productCreateRequest == null)
            {
                throw new ArgumentNullException($"Product data cannot be empty");
            }

            var productDb = await _productRepository.Find(e => e.ProductNumber == productCreateRequest.ProductNumber || e.ProductName == productCreateRequest.ProductName);

            if(productDb != null )
            {
                throw new ProductAlreadyExistsException("Product being added may already exist, any of these two values may already be in used.", productCreateRequest.ProductName, productCreateRequest.ProductNumber);
            }

            if(!string.IsNullOrEmpty(productCreateRequest.ProductTypeId))
            {
                var productType = await _productTypeRepository.Find(e => e.Id == Guid.Parse(productCreateRequest.ProductTypeId));

                if(productType == null)
                {
                    throw new Exception("Product Type selected does not exists");
                }
            }

            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Price = productCreateRequest.Price,
                ProductName = productCreateRequest.ProductName,
                ProductNumber = productCreateRequest.ProductNumber,
                ProductTypeId = !string.IsNullOrEmpty(productCreateRequest.ProductTypeId) ? Guid.Parse(productCreateRequest.ProductTypeId) : null,
            };

            await _productRepository.Create(newProduct);

            return true;
        }


        public async Task<ProductResponse?> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} cannot be null or empty");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return null;
            }

            var product = await _productRepository.Find(e => e.Id == parsedId, e => e.Include(e => e.ProductType));

            if(product == null)
            {
                return null;
            }

            return GetProductResponse(product);


        }
 

    }
}
