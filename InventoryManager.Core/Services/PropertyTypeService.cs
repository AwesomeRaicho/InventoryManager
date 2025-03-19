using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;
using InventoryManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Core.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private readonly IRepository<PropertyType> _propertyTypeRepository;
        private readonly IRepository<PropertyInstance> _propertyIntanceRepository;

        public PropertyTypeService(IRepository<PropertyType> propertyTypeService, IRepository<PropertyInstance> propertyIntanceRepository)
        {
            _propertyTypeRepository = propertyTypeService;
            _propertyIntanceRepository = propertyIntanceRepository;
        }

        private PropertyTypeResponse GetPropertyTypeResponse(PropertyType propertyType)
        {
            return new PropertyTypeResponse()
            {
                Id = propertyType.Id.ToString(),
                Name = propertyType.Name,
                ConcurrencyStamp = propertyType.ConcurrencyStamp,
            };

        }

        public async Task<Result<PropertyTypeResponse>> Create(PropertyTypeCreateRequest CreateRequest)
        {
            if (CreateRequest == null || string.IsNullOrEmpty(CreateRequest.Name))
            {
                return Result<PropertyTypeResponse>.Failure("Product Type name cannot be null.");
            }

            var dbEntity = await _propertyTypeRepository.Find(e => e.Name == CreateRequest.Name);

            if(dbEntity != null)
            {
                return Result<PropertyTypeResponse>.Failure("Property type name already exist.");
            }

            var toAddEntity = new PropertyType()
            { 
                Name = CreateRequest.Name,
                Id = Guid.NewGuid(),
            };

            var newEntity = await _propertyTypeRepository.Create(toAddEntity);
            
            var ResponseDto = this.GetPropertyTypeResponse(newEntity);

            return Result<PropertyTypeResponse>.Success(ResponseDto);
            
        }

        public async Task<Result<bool>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Result<bool>.Failure("Property Type Id need to be provided.");
            }

            if (!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<bool>.Failure("Property Type Id is not the correct format.");
            }

            var dbEntity = await _propertyTypeRepository.Find(e => e.Id == parsedId);

            if( dbEntity == null )
            {
                return Result<bool>.Failure("Product type Id does not belong to an existing entity.");
            }

            var propertyInstances = await _propertyIntanceRepository.GetQueryable().Where(e => e.PropertyTypeId == parsedId).ToListAsync();

            if( propertyInstances.Any() )
            {
                await _propertyIntanceRepository.RemoveRange(propertyInstances);
            }

            await _propertyTypeRepository.Delete(id);

            return Result<bool>.Success(true);


        }

        public async Task<Result<List<PropertyTypeResponse>>> GetAll(PropertyTypeGetRequest getRequest)
        {
            if(getRequest == null )
            {
                return Result<List<PropertyTypeResponse>>.Failure("ProductypeRequest cannot be null.");
            }

            getRequest.PageSize = getRequest.PageSize < 20 ? 20 : getRequest.PageSize > 1000 ? 1000 : getRequest.PageSize;

            getRequest.PageIndex = getRequest.PageIndex < 0 ? 0 : getRequest.PageIndex;

            var query = _propertyTypeRepository.GetQueryable();

            if(!string.IsNullOrEmpty(getRequest.SearchText))
            {
                query = query.Where(e => e.Name != null && e.Name.Contains(getRequest.SearchText));
            }

            if(getRequest.OrderBy == Enums.OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Name);
            }else
            {
                query = query.OrderByDescending(e => e.Name);
            }

            var dbList = await query.Skip(getRequest.PageSize * getRequest.PageIndex).Take(getRequest.PageSize).ToListAsync();

            if(!dbList.Any())
            {
                return Result<List<PropertyTypeResponse>>.Success(new List<PropertyTypeResponse>());
            }

            var responsList = dbList.Select(e => new PropertyTypeResponse()
            {
                ConcurrencyStamp = e.ConcurrencyStamp,
                Id = e.Id.ToString(),
                Name = e.Name,
                
            }).ToList();

            return Result<List<PropertyTypeResponse>>.Success(responsList);

        }

        public async Task<Result<PropertyTypeResponse>> GetById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<PropertyTypeResponse>.Failure("Property type Id needs to be provided.");
            }

            if(!Guid.TryParse(id, out Guid ParsedId))
            {
                return Result<PropertyTypeResponse>.Failure("Property type Id is not the correct format.");
            }

            var dbEntity = await _propertyTypeRepository.Find(e => e.Id == ParsedId);

            if(dbEntity == null)
            {
                return Result<PropertyTypeResponse>.Failure("Property type Id doe not belong to an existing Entity.");
            }

            var response = this.GetPropertyTypeResponse(dbEntity);

            return Result<PropertyTypeResponse>.Success(response);
        }

        public async Task<Result<PropertyTypeResponse>> Update(PropertyTypePutRequest PutRequest)
        {
            if (PutRequest == null)
            {
                return Result<PropertyTypeResponse>.Failure("PropertyType request cannot be null.");
            }

            if (string.IsNullOrEmpty(PutRequest.Id) || string.IsNullOrEmpty(PutRequest.Name))
            {
                return Result<PropertyTypeResponse>.Failure("PropertyType update request requires ID and Name.");
            }

            var dbEntityName = await _propertyTypeRepository.Find(e => e.Name == PutRequest.Name);

            if (dbEntityName != null)
            {
                return Result<PropertyTypeResponse>.Failure("Property type name already exists.");

            }

            if (!Guid.TryParse(PutRequest.Id, out Guid parsedId))
            {
                return Result<PropertyTypeResponse>.Failure("PropertyType ID is not the correct format.");
            }

            var dbEntity = await _propertyTypeRepository.Find(e => e.Id == parsedId);

            if (dbEntity == null)
            {
                return Result<PropertyTypeResponse>.Failure("PropertyType ID does not belong to an existing propertyType");
            }

            if (PutRequest.ConcurrencyStamp == null)
            {
                return Result<PropertyTypeResponse>.Failure("PropertyType request cannot be null.");
            }

            if (dbEntity.ConcurrencyStamp != null && !dbEntity.ConcurrencyStamp.SequenceEqual(PutRequest.ConcurrencyStamp))
            {
                return Result<PropertyTypeResponse>.Failure("Concurrency issue: you are trying to update an old version of this product please get updated version and try to update again.");
            }

            dbEntity.Name = PutRequest.Name;

            await _propertyTypeRepository.Update(dbEntity);

            var response = this.GetPropertyTypeResponse(dbEntity);

            return Result<PropertyTypeResponse>.Success(response);
        }
    }
}
