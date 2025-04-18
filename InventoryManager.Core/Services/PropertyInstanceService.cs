using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace InventoryManager.Core.Services
{
    public class PropertyInstanceService : IPropertyInstanceService
    {
        private readonly IRepository<PropertyInstance> _propertyInstanceRepository;
        private readonly IRepository<PropertyType> _propertyTypeRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Product_Property> _productPropertyRepository;



        public PropertyInstanceService(IRepository<PropertyInstance> propertyInstancerepo, IRepository<PropertyType> propertyTypeRepo, IRepository<Product> productRepository, IRepository<Product_Property> productPropertyRepository)
        {
            _propertyInstanceRepository = propertyInstancerepo;
            _propertyTypeRepository = propertyTypeRepo;
            _productRepository = productRepository;
            _productPropertyRepository = productPropertyRepository;
        }

        private PropertyInstanceResponse GetPropertyInstanceResponse(PropertyInstance propertyIntance)
        {


            return new PropertyInstanceResponse()
            {
                Id = propertyIntance.Id.ToString(),
                Name = propertyIntance.Name,
                ConcurrencyStamp = propertyIntance.ConcurrencyStamp,
                PropertyTypeId = propertyIntance.PropertyTypeId.ToString(),
                PropertyTypeName = propertyIntance.PropertyType != null && !string.IsNullOrEmpty(propertyIntance.PropertyType.Name) ? propertyIntance.PropertyType.Name : "",
                
            };
        }

        public async Task<Result<PropertyInstanceResponse>> CreatePropertyInstance(PropertyInstanceCreateRequest propertyInstanceCreateRequest)
        {
            if (propertyInstanceCreateRequest == null)
            {
                return Result<PropertyInstanceResponse>.Failure("Property Instance cannot be null when creating.");
            }

            if(string.IsNullOrEmpty(propertyInstanceCreateRequest.Name) || string.IsNullOrEmpty(propertyInstanceCreateRequest.PropertyTypeId))
            {
                return Result<PropertyInstanceResponse>.Failure("neither PropertyType ID or propertyInstance Name can be blank or null.");
            }

            if(!Guid.TryParse(propertyInstanceCreateRequest.PropertyTypeId, out var parsedPropertyTypeId))
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyType Id is not the correc format.");
            }

            var dbPropertyType = await _propertyTypeRepository.Find(e => e.Id == parsedPropertyTypeId);

            if(dbPropertyType == null)
            {
                return Result<PropertyInstanceResponse>.Failure("Property type Id does not match with an existing entity.");
            }

            var dbPropertyInstance = await _propertyInstanceRepository.Find(e => e.Name == propertyInstanceCreateRequest.Name.Trim());

            if (dbPropertyInstance != null)
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyInstance name already exists");
            }

            var newPropertyInstance = new PropertyInstance()
            {
                Id = Guid.NewGuid(),
                Name = propertyInstanceCreateRequest.Name,
                PropertyTypeId = parsedPropertyTypeId,
            };

            var savedPropertyInstance = await _propertyInstanceRepository.Create(newPropertyInstance);

            var response = this.GetPropertyInstanceResponse(savedPropertyInstance);

            return Result<PropertyInstanceResponse>.Success(response);
        }

        public async Task<Result<bool>> DeletePropertyInstance(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<bool>.Failure("Product Instance Id cannot be null.");
            }

            if(!Guid.TryParse(id, out var parsedId))
            {
                return Result<bool>.Failure("Id id not the correct format");
            }

            var dbPropertyInstance = await _propertyInstanceRepository.Find(e => e.Id == parsedId);

            if(dbPropertyInstance == null)
            {
                return Result<bool>.Failure("Product Instance ID does not belong to an existing entity");
            }

            await _propertyInstanceRepository.Delete(id);

            return Result<bool>.Success(true);

        }

        public async Task<Result<List<PropertyInstanceResponse>>> GetAllPropertyInstance(PropertyInstanceGetRequest propertyInstanceGetRequest)
        {
            if(propertyInstanceGetRequest == null)
            {
                return Result<List<PropertyInstanceResponse>>.Failure("Property Instance get request cannot be null.");
            }

            propertyInstanceGetRequest.PageIndex = propertyInstanceGetRequest.PageIndex < 0 ? 0 : propertyInstanceGetRequest.PageIndex;

            propertyInstanceGetRequest.PageSize = propertyInstanceGetRequest.PageSize < 20 ? 20 : propertyInstanceGetRequest.PageSize > 1000 ? 1000 : propertyInstanceGetRequest.PageSize;

            var query = _propertyInstanceRepository.GetQueryable();

            if(!string.IsNullOrEmpty(propertyInstanceGetRequest.SearchText))
            {
                query = query.Where(e => e.Name != null && e.Name.Contains(propertyInstanceGetRequest.SearchText));
            }

            if(propertyInstanceGetRequest.OrderBy == Enums.OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Name);
            }else
            {
                query = query.OrderByDescending(e => e.Name);
            }

            var dbList = await query.Skip(propertyInstanceGetRequest.PageIndex * propertyInstanceGetRequest.PageSize).Take(propertyInstanceGetRequest.PageSize).ToListAsync();

            if (!dbList.Any())
            {
                return Result<List<PropertyInstanceResponse>>.Success(new List<PropertyInstanceResponse>()); 
            }

            var response = dbList.Select(e => new PropertyInstanceResponse()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                ConcurrencyStamp = e.ConcurrencyStamp,
                PropertyTypeId = e.PropertyTypeId.ToString(),
                PropertyTypeName = e.PropertyType != null ? e.PropertyType.Name : null,
            }).ToList();

            return Result<List<PropertyInstanceResponse>>.Success(response);
        }

        public async Task<Result<PropertyInstanceResponse>> GetPropertyInstance(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<PropertyInstanceResponse>.Failure("Property instance cannot be null.");
            }

            if(!Guid.TryParse(id, out var parsedId))
            {
                return Result<PropertyInstanceResponse>.Failure("Property Id is not the correct format.");
            }

            var dbEntity = await _propertyInstanceRepository.Find(e => e.Id == parsedId);

            if(dbEntity == null)
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyInstance ID does not belong to an existing entity.");
            }


            var response = this.GetPropertyInstanceResponse(dbEntity);

            return Result<PropertyInstanceResponse>.Success(response);


        }

        public async Task<Result<PropertyInstanceResponse>> UpdatePropertyInstance(PropertyInstancePutRequest propertyInstancePutRequest)
        {
            if(propertyInstancePutRequest == null)
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyInstance update request cannot be null.");
            }

            if(string.IsNullOrEmpty(propertyInstancePutRequest.Name) || string.IsNullOrEmpty(propertyInstancePutRequest.Id) || propertyInstancePutRequest.ConcurrencyStamp == null)
            {
                return Result<PropertyInstanceResponse>.Failure("All fields are required to Update a PropertyInstance");
            }

            if (!Guid.TryParse(propertyInstancePutRequest.Id, out var parsedId))
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyInstance ID is not the correct Format.");
            }

            var dbEntity = await _propertyInstanceRepository.Find(e => e.Id == parsedId);
            if (dbEntity == null)
            {
                return Result<PropertyInstanceResponse>.Failure("PropertyInstance ID does not belong to an existing entity.");
            }

            if(dbEntity.ConcurrencyStamp != null && propertyInstancePutRequest.ConcurrencyStamp != null && !propertyInstancePutRequest.ConcurrencyStamp.SequenceEqual(dbEntity.ConcurrencyStamp))
            {
                return Result<PropertyInstanceResponse>.Failure("Concurrency issue: entity may have ben changed since last acquired.");
            }

            dbEntity.Name = propertyInstancePutRequest.Name;


            await _propertyInstanceRepository.Update(dbEntity);

            var response = this.GetPropertyInstanceResponse(dbEntity);

            return Result<PropertyInstanceResponse>.Success(response);

        }

        public async Task<Result<List<PropertyInstanceResponse>>> GetPropertyInstancesByPropertyTypeId(PropertyInstanceGetRequest propertyInstanceGetRequest)
        {
            if(propertyInstanceGetRequest == null)
            {
                return Result<List<PropertyInstanceResponse>>.Failure("PropertyInstanceGetRequest cannot be null.");
            }

            if(string.IsNullOrEmpty(propertyInstanceGetRequest.PropertyTypeId))
            {
                return Result<List<PropertyInstanceResponse>>.Failure("Property type ID cannot be null.");
            }

            if(!Guid.TryParse(propertyInstanceGetRequest.PropertyTypeId, out Guid parsedPropertyTypeId))
            {
                return Result<List<PropertyInstanceResponse>>.Failure("Product type ID is not the correct format.");
            }

            propertyInstanceGetRequest.PageIndex = propertyInstanceGetRequest.PageIndex < 0 ? 0 : propertyInstanceGetRequest.PageIndex;

            propertyInstanceGetRequest.PageSize = propertyInstanceGetRequest.PageSize < 20 ? 20 : propertyInstanceGetRequest.PageSize > 100 ? 100 : propertyInstanceGetRequest.PageSize;

            var query = _propertyInstanceRepository.GetQueryable();

            query = query.Where(e => e.PropertyTypeId == parsedPropertyTypeId);

            if(propertyInstanceGetRequest.OrderBy == Enums.OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Name);
            }else
            {
                query = query.OrderByDescending(e => e.Name);
            }

            var dbList = await query.Skip(propertyInstanceGetRequest.PageSize * propertyInstanceGetRequest.PageIndex).Take(propertyInstanceGetRequest.PageSize).ToListAsync();

            if (!dbList.Any())
            {
                return Result<List<PropertyInstanceResponse>>.Success(new List<PropertyInstanceResponse>());
            }

            var responses = dbList.Select(e => new PropertyInstanceResponse()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                PropertyTypeId = e.PropertyTypeId.ToString(),
                ConcurrencyStamp = e.ConcurrencyStamp,
                PropertyTypeName = e.PropertyType != null ? e.PropertyType.Name : null,
            }).ToList();

            return Result<List<PropertyInstanceResponse>>.Success(responses);

        }

        public async Task<Result<Dictionary<string, List<PropertyInstanceResponse>>>> GetAllPropertyTypesWithInstances()
        {
            Dictionary<string, List<PropertyInstanceResponse>> responseDictionary = new Dictionary<string, List<PropertyInstanceResponse>>();

            var query = _propertyTypeRepository.GetQueryable();
            query = query.OrderBy(e => e.Name);
            var propertyTypes = await query.ToArrayAsync();

            if (propertyTypes.Any())
            {
                foreach (var propType in propertyTypes)
                {
                    var key = propType.Name ?? "";

                    var instancesQuery = _propertyInstanceRepository.GetQueryable()
                        .Where(e => e.PropertyTypeId == propType.Id);

                    var propertyInstances = await instancesQuery.ToListAsync();
                    var propertyInstancesRes = propertyInstances.Select(e => new PropertyInstanceResponse()
                    {
                        Id = e.Id.ToString(),
                        Name = e.Name,
                        PropertyTypeId = e.PropertyTypeId.ToString(),
                        ConcurrencyStamp = propType.ConcurrencyStamp,
                        PropertyTypeName = propType.Name,
                    }).ToList();

                    responseDictionary.Add(key, propertyInstancesRes);
                }
            }

            return Result<Dictionary<string, List<PropertyInstanceResponse>>>.Success(responseDictionary);
        }

        public async Task<Result<List<PropertyInstanceResponse>>> GetAllPropertyInstancesByProductId(string ProductId)
        {
            if (string.IsNullOrEmpty(ProductId))
            {
                return Result<List<PropertyInstanceResponse>>.Failure("ProductId cannot be null.");
            }

            if(!Guid.TryParse(ProductId, out Guid parsedProductId))
            {
                return Result<List<PropertyInstanceResponse>>.Failure("ProductId is not the correct formast.");
            }

            var dbProduct = await _productRepository.Find(e => e.Id == parsedProductId);

            if(dbProduct == null)
            {
                return Result<List<PropertyInstanceResponse>>.Failure("ProductId does not belong to an existing product.");
            }

            //all propertyintances that have the productId
            var query = _productPropertyRepository.GetQueryable();

            var dbProperties = await query.Where(e => e.ProductId == parsedProductId).Select(e => new PropertyInstanceResponse
            {
                Name = e.Property != null ? e.Property.Name : null,
                Id = e.Property != null ? e.Property.Id.ToString() : null,
                ConcurrencyStamp = e.Property != null ? e.Property.ConcurrencyStamp : null,
                PropertyTypeId = e.Property != null ? e.Property.PropertyTypeId.ToString() : null,
                PropertyTypeName = e.Property != null && e.Property.PropertyType != null ? e.Property.PropertyType.Name : null,
            }).ToListAsync();

            return Result<List<PropertyInstanceResponse>>.Success(dbProperties);


        }

   


    }
}


