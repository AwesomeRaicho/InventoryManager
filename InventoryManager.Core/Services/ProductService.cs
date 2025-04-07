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
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace InventoryManager.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductType> _productTypeRepository;
        private readonly IProduct_PropertyService _productPropertyService;
        private readonly IPropertyInstanceService _propertyInstanceService;
        private readonly IRepository<ProductInstance> _productInstanceRepository;

        public ProductService(IRepository<Product> productRepository, IRepository<ProductType> productTypeRepository, IProduct_PropertyService productPropertyService, IPropertyInstanceService propertyInstanceService, IRepository<ProductInstance> productInstanceRepository)
        {
            _productRepository = productRepository;
            _productTypeRepository = productTypeRepository;
            _productPropertyService = productPropertyService;
            _propertyInstanceService = propertyInstanceService;
            _productInstanceRepository = productInstanceRepository;
        }

        /// <summary>
        /// Turn Product entity into ProductResponse DTO
        /// </summary>
        /// <param name="product">Product entity</param>
        /// <returns></returns>
        public ProductResponse GetProductResponse(Product product)
        {
            

            return new ProductResponse()
            {
                Id = product.Id.ToString(),
                ConcurrencyStamp = product.ConcurrencyStamp,
                Price = product.Price,
                ProductName = product.ProductName,
                ProductNumber = product.ProductNumber,
                StockAmount = product.StockAmount,
                ProductTypeName = product.ProductType != null ? product.ProductType.Name : "",
                ProductTypeId = product.ProductTypeId.ToString(),
            };
        }




        public async Task<Result<ProductResponse>> CreateProduct(ProductCreateRequest productCreateRequest)
        {
            if (productCreateRequest == null)
            {
                return Result<ProductResponse>.Failure("Product data cannot be empty.");
            }

            var productDb = await _productRepository.Find(e =>
                e.ProductNumber == productCreateRequest.ProductNumber ||
                e.ProductName == productCreateRequest.ProductName);

            if (productDb != null)
            {
                return Result<ProductResponse>.Failure("Product already exists. Either the name or product number is in use.");
            }


            if (!Guid.TryParse(productCreateRequest.ProductTypeId, out var productTypeId))
            {
                return Result<ProductResponse>.Failure("Invalid Product Type ID.");
            }

            var productType = await _productTypeRepository.Find(e => e.Id == productTypeId);

            if (productType == null)
            {
                return Result<ProductResponse>.Failure("Product Type selected does not exist.");
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

            if(productCreateRequest.PropertyIds != null && productCreateRequest.PropertyIds.Any())
            {
                await _productPropertyService.Create(newProduct.Id.ToString(), productCreateRequest.PropertyIds);
            }


            var response = this.GetProductResponse(newProduct);

            var list = await _productPropertyService.GetByProductId(newProduct.Id.ToString());

            if(list.Any())
            {
                response.Properties = list;
            }


            return Result<ProductResponse>.Success(response);
        }


        public List<ProductResponse>? GetProductResponseList(List<Product> products)
        {
            if(products == null || products.Count <= 0)
            {
                return null;
            }

            return products.Select(p => new ProductResponse()
            {
                Id = p.Id.ToString(),
                Price = p.Price,
                ProductName = p.ProductName,
                ProductNumber = p.ProductNumber,
                ProductTypeId = p.ProductTypeId.ToString(),
                ConcurrencyStamp = p.ConcurrencyStamp,
                ProductTypeName = p.ProductType?.Name ?? string.Empty,
                StockAmount = p.StockAmount
            }).ToList();


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

        public async Task<List<ProductResponse>?> GetAllProducts(ProductGetRequest productGetRequest)
        {
            if(productGetRequest == null)
            {
                throw new ArgumentException("¨ProductGetRequest cannot be null.");
            };

            var query = _productRepository.GetQueryable();

            if(productGetRequest.ProductId != null && Guid.TryParse(productGetRequest.ProductId, out Guid requestId))
            {
                query = query.Where(e => e.Id == requestId);
            };

            if (!string.IsNullOrEmpty(productGetRequest.SearchText))
            {
                string searchTextLower = productGetRequest.SearchText.ToLower();

                query = query.Where(e =>
                    (e.ProductName != null && e.ProductName.ToLower().Contains(searchTextLower)) ||
                    (e.ProductType != null && e.ProductType.Name != null && e.ProductType.Name.ToLower().Contains(searchTextLower)) ||
                    (e.ProductNumber != null && e.ProductNumber.ToLower().Contains(searchTextLower)));
            }

            var products = (List<Product>)await _productRepository.FindAll(query, productGetRequest.PageNumber, productGetRequest.PageSize);

            if(products.Count <= 0)
            {
                return null;
            }

            return this.GetProductResponseList(products);
        }

        public async Task<Result<ProductResponse>> UpdateProduct(ProductPutRequest productPutRequest)
        {
            if (!Guid.TryParse(productPutRequest.Id, out Guid parsedProductId))
            {
                return Result<ProductResponse>.Failure("Invalid Product Id format.");
            }

            var dbProduct = await _productRepository.GetEntityById(productPutRequest.Id);
            if (dbProduct == null)
            {
                return Result<ProductResponse>.Failure($"Product with Id {productPutRequest.Id} does not exist.");
            }

            if(productPutRequest.ConcurrencyStamp == null)
            {
                return Result<ProductResponse>.Failure("ConcurrencyStamp is required when updating data.");
            }
            if (!dbProduct.ConcurrencyStamp.SequenceEqual(productPutRequest.ConcurrencyStamp))
            {
                return Result<ProductResponse>.Failure("Concurrency conflict: The product was modified by another process.");
            }

            Guid? parsedTypeId = null;
            if (!string.IsNullOrEmpty(productPutRequest.ProductTypeId))
            {
                if (!Guid.TryParse(productPutRequest.ProductTypeId, out Guid parsedProductType))
                {
                    return Result<ProductResponse>.Failure("Invalid ProductTypeId format.");
                }

                var dbProductType = await _productTypeRepository.GetEntityById(productPutRequest.ProductTypeId);
                if (dbProductType == null)
                {
                    return Result<ProductResponse>.Failure($"ProductType with Id {productPutRequest.ProductTypeId} does not exist.");
                }

                parsedTypeId = parsedProductType;
            }


            // Update product details
            dbProduct.ProductNumber = productPutRequest.ProductNumber;
            dbProduct.ProductName = productPutRequest.ProductName;
            dbProduct.ConcurrencyStamp = productPutRequest.ConcurrencyStamp;
            dbProduct.ProductTypeId = parsedTypeId;
            dbProduct.Price = productPutRequest.Price;

            //update properties
            var props = await _propertyInstanceService.GetAllPropertyInstancesByProductId(dbProduct.Id.ToString());
            if(productPutRequest.PropertyIds != null && productPutRequest.PropertyIds.Count > 0)
            {
                var dbIds = new List<string?>();
                if (props.Value != null)
                {
                    dbIds = props.Value.Where(e => e.Id != null).Select(e =>  e.Id).ToList();
                }


                var removeList = dbIds.Where(e => e != null && !productPutRequest.PropertyIds.Contains(e)).ToList();
                
                var addList = productPutRequest.PropertyIds.Where(e => !dbIds.Contains(e)).ToList();

                if(removeList != null && removeList.Count > 0 )
                {
                    await _productPropertyService.DeleteRange(productPutRequest.Id, removeList);

                }

                if(addList != null  && addList.Count > 0 )
                {
                    await _productPropertyService.CreateRange(productPutRequest.Id, addList);

                }
            }else if(props.IsSuccess && props.Value != null && props.Value.Count > 0)
            {
                var removeList = props.Value.Select(e => e.Id).ToList();
                if (removeList != null && removeList.Count > 0)
                {
                    await _productPropertyService.DeleteRange(productPutRequest.Id, removeList);

                }
            }
            

            

            

            var updateResult = await _productRepository.Update(dbProduct);
            if (updateResult == null)
            {
                return Result<ProductResponse>.Failure("Failed to update product.");
            }

            var response = GetProductResponse(updateResult);
            return response != null
                ? Result<ProductResponse>.Success(response)
                : Result<ProductResponse>.Failure("Failed to generate response DTO.");
        }

        public async Task<Result<bool>> DeleteProduct(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<bool>.Failure("Id cannot be null");
            }

            if(!Guid.TryParse(id, out var productId))
            {
                return Result<bool>.Failure("Id provided is incorrect format.");
            }

            var response = await _productRepository.GetEntityById(id);
            
            if (response == null)
            {
                return Result<bool>.Failure("Id does not exist.");

            }


            var query = _productInstanceRepository.GetQueryable();

            var dbList = await query.Where(e => e.ProductId == productId).ToListAsync();

            await _productInstanceRepository.RemoveRange(dbList);

            await _productRepository.Delete(id);

            return Result<bool>.Success(true);

        }

        public async Task<Result<List<ProductResponse>>> GetAllByProductTypeId(ProductGetRequest productGetRequest)
        {
            if (productGetRequest == null || string.IsNullOrEmpty(productGetRequest.ProductTypeId))
            {
                return Result<List<ProductResponse>>.Failure("Product Type Id cannot be null.");
            }

            if(!Guid.TryParse(productGetRequest.ProductTypeId, out var parsedProductTypeId))
            {
                return Result<List<ProductResponse>>.Failure("Product Type Id is not the correct format.");
            }

            var productType = await _productTypeRepository.Find(e => e.Id == parsedProductTypeId);

            if(productType == null)
            {
                return Result<List<ProductResponse>>.Failure("Product Type Id does not exist.");
            }

            productGetRequest.PageNumber = productGetRequest.PageNumber < 0 ? 0 : productGetRequest.PageNumber;

            productGetRequest.PageSize = productGetRequest.PageSize < 20 ? 20 : productGetRequest.PageSize > 100 ? 100 : productGetRequest.PageSize;

            
            var query = _productRepository.GetQueryable();

            query = query.Where(e => e.ProductTypeId == parsedProductTypeId);

            query = query.OrderBy(e => e.ProductName);

            var productList = await query.Skip(productGetRequest.PageNumber * productGetRequest.PageSize).Take(productGetRequest.PageSize).ToListAsync();

            var response = productList.Select(e => new ProductResponse()
            {
                Id = e.Id.ToString(),
                ProductName = e.ProductName,
                ConcurrencyStamp = e.ConcurrencyStamp,
                Price = e.Price,
                ProductNumber = e.ProductNumber,
                ProductTypeId = e.ProductTypeId.ToString(),
                StockAmount = e.StockAmount,
                ProductTypeName = e.ProductType != null ? e.ProductType.Name : null,
            }).ToList();


            for (int i = 0; i < response.Count; i++)
            {
                string? pId = response[i].Id;
                if(pId == null)
                {
                    continue;
                }

                var propList = await _propertyInstanceService.GetAllPropertyInstancesByProductId(pId);
                if(propList.IsSuccess && propList.Value != null)
                {
                    if(propList.Value.Any())
                    {
                        response[i].Properties = propList.Value;
                    }
                }
            }


            return Result<List<ProductResponse>>.Success(response);
        }

        public async Task<bool> AddToStock(string? productId)
        {
            if(string.IsNullOrEmpty(productId))
            {
                return false;
            }

            var dbProduct = await _productRepository.Find(e => e.Id == Guid.Parse(productId));

            if(dbProduct == null)
            {
                return false;
            }

            dbProduct.StockAmount++;

            await _productRepository.Update(dbProduct);
            return true;

        }
        public async Task<bool> SubtrackToStock(string? productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return false;
            }

            var dbProduct = await _productRepository.GetEntityById(productId);

            if (dbProduct == null)
            {
                return false;
            }

            if (dbProduct.StockAmount > 0)
            {
                dbProduct.StockAmount--;
            }

            await _productRepository.Update(dbProduct);
            return true;
        }

    }
}
