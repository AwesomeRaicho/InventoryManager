using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Services
{
    public class ProductInstanceService : IProductInstanceService
    {

        private readonly IRepository<ProductInstance> _productInstanceRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<ProductInstance_Property> _productInstance_PropertyRepository;
        private readonly IRepository<PropertyInstance> _propertyInstanceRepository; 

        public ProductInstanceService(IRepository<ProductInstance> productInstanceRepo, IRepository<Product> productRepo, IRepository<Location> locationRepo, IRepository<ProductInstance_Property> product_PropertyRepository, IRepository<PropertyInstance> PropertyInstanceRepo)
        {
            _productInstanceRepository = productInstanceRepo;
            _productRepository = productRepo;
            _locationRepository = locationRepo;
            _productInstance_PropertyRepository = product_PropertyRepository;
            _propertyInstanceRepository = PropertyInstanceRepo;
        }

        private ProductInstanceResponse GetResponseDTO(ProductInstance productinstance)
        {
            //NEED TO RETURN INTO A RESPONSE DTO

            return new ProductInstanceResponse();
        }

        private async Task AddPropertiesToInstance(ProductInstance instanceEntity, List<string> propertyIds)
        {

            if (instanceEntity == null)
                throw new ArgumentNullException(nameof(instanceEntity));

            // check the ProductInstance exists
            var entity = await _productInstanceRepository.Find(e => e.Id == instanceEntity.Id);
            if (entity == null) return;

            // parsable strings IDs to Guid
            var validIds = propertyIds
                .Select(id => Guid.TryParse(id, out Guid parsedId) ? parsedId : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList();

            if (!validIds.Any()) return;

            // Get only existing property IDs from DB
            var existingPropertyIds = await _propertyInstanceRepository
                .GetQueryable()
                .Where(e => validIds.Contains(e.Id))
                .Select(e => e.Id)
                .ToListAsync();

            if (!existingPropertyIds.Any()) return;

            // Get already existing relationships (avoid duplicates)
            var existingRelations = await _productInstance_PropertyRepository
                .GetQueryable()
                .Where(e => e.ProductInstanceId == entity.Id && existingPropertyIds.Contains(e.PropertyId))
                .Select(e => e.PropertyId)
                .ToListAsync();

            // Create new relationships, filtering out existing ones
            var newRelations = existingPropertyIds
                .Except(existingRelations) 
                .Select(propertyId => new ProductInstance_Property
                {
                    ProductInstanceId = entity.Id,
                    PropertyId = propertyId
                })
                .ToList();

            if (newRelations.Any())
            {
                await _productInstance_PropertyRepository.AddRange(newRelations);
            }
        }


        public async Task<Result<ProductInstanceResponse>> CreateProductInstance(ProductInstanceCreateRequest productInstanceCreateRequest)
        {
            if(productInstanceCreateRequest == null) throw new ArgumentNullException(nameof(productInstanceCreateRequest), "Request cannot be null.");

            //check for barcode and uniquness
            if(!string.IsNullOrEmpty(productInstanceCreateRequest.Barcode))
            {
                bool unique = await _productInstanceRepository.IsUnique(e => e.Barcode == productInstanceCreateRequest.Barcode);

                if (!unique)
                {
                    return Result<ProductInstanceResponse>.Failure("Barcode already used.");
                }

            }else
            {
                return Result<ProductInstanceResponse>.Failure("Barcode is required.");
            }


            //productId
            if(!Guid.TryParse(productInstanceCreateRequest.ProductId, out Guid parsedProductId))
            {
                return Result<ProductInstanceResponse>.Failure("Product Id is not the correct format.");
            }else
            {
                var product = await _productRepository.Find(e => e.Id == parsedProductId);

                if (product == null)
                {
                    return Result<ProductInstanceResponse>.Failure("Product Id does not exist.");
                }
            }


            //LocationId
            Guid? verifiedLocatioinId = null;
            if(!string.IsNullOrEmpty(productInstanceCreateRequest.LocationId))
            {
                if(!Guid.TryParse(productInstanceCreateRequest.LocationId, out Guid parsedLocationId))
                {
                    return Result<ProductInstanceResponse>.Failure("Location id is not the correct format.");
                }else
                {
                    var location = await _locationRepository.Find(e => e.Id == parsedLocationId);

                    if (location == null)
                    {
                        return Result<ProductInstanceResponse>.Failure("Location id provided does not exist");
                    }

                    verifiedLocatioinId = parsedLocationId;
                }
            }


            var productInstance = new ProductInstance()
            {
                Id = Guid.NewGuid(),
                Barcode = productInstanceCreateRequest.Barcode,
                ProductId = parsedProductId,
                EntryDate = DateTime.UtcNow,
                PurchasePrice = productInstanceCreateRequest.PurchasePrice,
                Status = "Available",
                LocationId = verifiedLocatioinId,
                    
            };


            var dbEntity = await _productInstanceRepository.Create(productInstance);

            //properties
            if(productInstanceCreateRequest.PropertyIds  != null && productInstanceCreateRequest.PropertyIds.Any())
            {
                await this.AddPropertiesToInstance(dbEntity, productInstanceCreateRequest.PropertyIds);
            }

            var resDTO = this.GetResponseDTO(dbEntity);

            return Result<ProductInstanceResponse>.Success(resDTO);
        }

    }
}
