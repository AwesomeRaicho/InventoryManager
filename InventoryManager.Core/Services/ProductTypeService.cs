using InventoryManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections;


namespace InventoryManager.Core.Services
{
    public class ProductTypeService : IProductTypeService
    {

        private readonly IRepository<ProductType> _productTypeRepository;


        public ProductTypeService(IRepository<ProductType> productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }


        private ProductTypeResponse? GetProductTypeResponse(ProductType productType)
        {
            if (productType == null)
            {
                return null;
            }

            var response = new ProductTypeResponse()
            {
                Id = productType.Id.ToString(),
                Name = productType.Name,
                ConcurrencyStamp = productType.ConcurrencyStamp,
            };

            return response;
        }




        public async Task<Result<ProductTypeResponse>> CreateProductType(ProductTypeCreateRequest productTypeCreateRequest)
        {
            if(productTypeCreateRequest == null || productTypeCreateRequest.Name == null || string.IsNullOrEmpty(productTypeCreateRequest.Name))
            {
                return Result<ProductTypeResponse>.Failure("Product type name must be provided.");
            }

            var dbEntity = await _productTypeRepository.Find(e => e.Name == productTypeCreateRequest.Name);

            if (dbEntity != null)
            {
                return Result<ProductTypeResponse>.Failure("Product type name already exists.");
            }

            var newEntity = new ProductType()
            {
                Id = Guid.NewGuid(),
                Name = productTypeCreateRequest.Name,
            };

            var createdEntity = await _productTypeRepository.Create(newEntity);

            var responseDto = this.GetProductTypeResponse(createdEntity);

            if(responseDto == null)
            {
                return Result<ProductTypeResponse>.Failure("could not get response, but data was succesfully added.");
            }


            return Result<ProductTypeResponse>.Success(responseDto);
        }

        //Get 
        public async Task<Result<ProductTypeResponse>> GetById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<ProductTypeResponse>.Failure("Product Type ID must be provided.");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<ProductTypeResponse>.Failure("Product Type ID provided is not the correct format.");
            }

            var dbEntity = await _productTypeRepository.GetEntityById(id);

            if (dbEntity == null)
            {
                return Result<ProductTypeResponse>.Failure($"Product Type with ID: {id} does not exist.");
            }

            var response = this.GetProductTypeResponse(dbEntity);

            if (response == null)
            {
                return Result<ProductTypeResponse>.Failure("Failed to create response object. The data might be incomplete or corrupted.");
            }

            return Result<ProductTypeResponse>.Success(response);
        }


        //GetAll
        public async Task<Result<List<ProductTypeResponse>>> GetAllProductTypes(ProductTypeGetRequest productTypeGetRequest)
        {
            if (productTypeGetRequest == null)
            {
                return Result<List<ProductTypeResponse>>.Failure("Request cannot be null.");
            }

            productTypeGetRequest.PageIndex = productTypeGetRequest.PageIndex < 0 ? 0 : productTypeGetRequest.PageIndex;

            productTypeGetRequest.PageSize = productTypeGetRequest.PageSize < 20 ? 20 : productTypeGetRequest.PageSize > 1000 ? 1000 : productTypeGetRequest.PageSize;



            var query = _productTypeRepository.GetQueryable();

            if (!string.IsNullOrEmpty(productTypeGetRequest.SearchText))
            {
                query = query.Where(e => e.Name != null && e.Name.Contains(productTypeGetRequest.SearchText));
            }

            if (productTypeGetRequest.OrderBy == Enums.OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Name);
            }
            else
            {
                query = query.OrderByDescending(e => e.Name);
            }

            List<ProductType> dbList = await query
                .Skip(productTypeGetRequest.PageSize * productTypeGetRequest.PageIndex)
                .Take(productTypeGetRequest.PageSize)
                .ToListAsync();


            var response = dbList.Any() ? 
                dbList.Select(e => new ProductTypeResponse()
                {
                    Id = e.Id.ToString(),
                    Name = e.Name,
                    ConcurrencyStamp = e.ConcurrencyStamp,
                }).ToList()
                : null;

            return Result<List<ProductTypeResponse>>.Success(response ?? new List<ProductTypeResponse>());
        }
        //update

        public async Task<Result<ProductTypeResponse>> UpdateProductType(ProductTypePutRequest productTypePutRequest)
        {
            if (productTypePutRequest == null)
            {
                return Result<ProductTypeResponse>.Failure("Editing product type cannot be null.");
            }

            if (string.IsNullOrEmpty(productTypePutRequest.Name) || string.IsNullOrEmpty(productTypePutRequest.Id) || productTypePutRequest.ConcurrencyStamp == null)
            {
                return Result<ProductTypeResponse>.Failure("Id, Name and ConcurrencyStamp are required to edit data.");
            }

            if(!Guid.TryParse(productTypePutRequest.Id, out Guid parsedId))
            {
                return Result<ProductTypeResponse>.Failure("product type id is not the correct format.");
            }


            var dbEntity = await _productTypeRepository.Find(e => e.Id == parsedId);

            if(dbEntity == null)
            {
                return Result<ProductTypeResponse>.Failure("product type does not exist.");
            }

            if (dbEntity.ConcurrencyStamp != null && !productTypePutRequest.ConcurrencyStamp.SequenceEqual(dbEntity.ConcurrencyStamp))
            {
                return Result<ProductTypeResponse>.Failure("Concurrency conflict detected: The entity has been modified by another process. Please reload the latest data and retry your update.");
            }


            dbEntity.Name = productTypePutRequest.Name;

            var updatedResponse = await _productTypeRepository.Update(dbEntity);

            var response = this.GetProductTypeResponse(updatedResponse);


            if (response == null)
            {
                return Result<ProductTypeResponse>.Failure("Failed to create response object. The data might be incomplete or corrupted.");
            }

            return Result<ProductTypeResponse>.Success(response);

        }

        //delete
        public async Task<Result<bool>> DeleteProductType(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<bool>.Failure($"ProductType does not exist.");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<bool>.Failure($"ProductType Id is not the correc format.");
            }

            var dbEntity = await _productTypeRepository.Find(e => e.Id ==parsedId);


            if(dbEntity == null)
            {
                return Result<bool>.Failure($"ProductType does not exist.");
            }

            await _productTypeRepository.Delete(id);

            return Result<bool>.Success(true);

        }

    }
}
