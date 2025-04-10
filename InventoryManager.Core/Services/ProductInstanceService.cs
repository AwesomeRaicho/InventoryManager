using InventoryManager.Core.DTO;
using InventoryManager.Core.Enums;
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InventoryManager.Core.Services
{
    public class ProductInstanceService : IProductInstanceService
    {

        private readonly IRepository<ProductInstance> _productInstanceRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<Product_Property> _productInstance_PropertyRepository;
        private readonly IRepository<PropertyInstance> _propertyInstanceRepository;
        private readonly IProductService _productService;

        public ProductInstanceService(IRepository<ProductInstance> productInstanceRepo, IRepository<Product> productRepo, IRepository<Location> locationRepo, IRepository<Product_Property> product_PropertyRepository, IRepository<PropertyInstance> PropertyInstanceRepo, IProductService productService)
        {
            _productInstanceRepository = productInstanceRepo;
            _productRepository = productRepo;
            _locationRepository = locationRepo;
            _productInstance_PropertyRepository = product_PropertyRepository;
            _propertyInstanceRepository = PropertyInstanceRepo;
            _productService = productService;
        }

        private ProductInstanceResponse? GetResponseDTO(ProductInstance productinstance)
        {
            if(productinstance == null) { return null; };

            var reponse = new ProductInstanceResponse()
            {
                Barcode = productinstance.Barcode,
                ConcurrencyStamp = productinstance.ConcurrencyStamp,
                Id = productinstance.Id.ToString(),
                Status = productinstance.Status,
                LocationId = productinstance.LocationId.ToString(),
                LocationName = productinstance.Location != null ? productinstance.Location.Name : null,
                ProductId = productinstance.ProductId.ToString(),
                ProductName = productinstance.Product != null ? productinstance.Product.ProductName : null,
                EntryDate = productinstance.EntryDate,
                PurchasePrice = productinstance.PurchasePrice,
            };

            return reponse;
        }

        private async Task AddPropertiesToInstance(ProductInstance instanceEntity, List<string> propertyIds)
        {
            if (instanceEntity == null)
                throw new ArgumentNullException(nameof(instanceEntity));

            // Check if the ProductInstance exists
            var entity = await _productInstanceRepository.Find(e => e.Id == instanceEntity.Id);
            if (entity == null) return;

            // Parse string IDs to Guid
            var validIds = propertyIds
                .Select(id => Guid.TryParse(id, out Guid parsedId) ? parsedId : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList();

            if (!validIds.Any()) return;

            // Get only valid PropertyInstance IDs that exist in the DB
            var existingPropertyIds = await _propertyInstanceRepository
                .GetQueryable()
                .Where(e => validIds.Contains(e.Id))  // Filter to ensure properties exist
                .Select(e => e.Id)
                .ToListAsync();

            if (!existingPropertyIds.Any()) return;

            // Get all relationships for this specific ProductInstance
            var existingRelations = await _productInstance_PropertyRepository
                .GetQueryable()
                .Where(e => e.ProductId == entity.Id)
                .ToListAsync();

            var existingPropertyIdsInDb = existingRelations.Select(r => r.PropertyId).ToList();

            // Identify relationships to remove (those in the DB but NOT in the incoming list)
            var relationsToRemove = existingRelations
                .Where(rel => !existingPropertyIds.Contains(rel.PropertyId))
                .ToList();

            if (relationsToRemove.Any())
            {
                await _productInstance_PropertyRepository.RemoveRange(relationsToRemove);
            }

            // Identify new relationships to add (those in the incoming list but NOT in the DB)
            var newRelations = existingPropertyIds
                .Except(existingPropertyIdsInDb)
                .Select(propertyId => new Product_Property
                {
                    ProductId = entity.Id,
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
                Status = productInstanceCreateRequest.Status,
                LocationId = verifiedLocatioinId,
                    
            };

            var dbEntity = await _productInstanceRepository.Create(productInstance);

            //properties
            if(productInstanceCreateRequest.PropertyIds  != null && productInstanceCreateRequest.PropertyIds.Any())
            {
                await this.AddPropertiesToInstance(dbEntity, productInstanceCreateRequest.PropertyIds);
            }

            await _productService.AddToStock(dbEntity.ProductId.ToString());

            var resDTO = this.GetResponseDTO(dbEntity);

            if(resDTO != null)
            {
                return Result<ProductInstanceResponse>.Success(resDTO);
            }

            return Result<ProductInstanceResponse>.Failure("Could not return created Product instance, please validate in all view.");

        }

        public async Task<Result<ProductInstanceResponse>> GetProductInstanceById(string? id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<ProductInstanceResponse>.Failure("Id cannot be null"); 
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<ProductInstanceResponse>.Failure($"{nameof(id)} is not valid");
            }

            var query = await _productInstanceRepository.GetQueryable().Where(x => x.Id == parsedId).Include(x => x.Product).FirstOrDefaultAsync();

            if(query == null)
            {
                return Result<ProductInstanceResponse>.Failure($"Product was not found or matches any existing products.");
            }

            var responseEntity = this.GetResponseDTO(query);

            if(responseEntity == null)
            {
                return Result<ProductInstanceResponse>.Failure("Could not create product response.");   
            }

            return Result<ProductInstanceResponse>.Success(responseEntity);

        }

        public async Task<Result<List<ProductInstanceResponse>>> GetAllProductInstances(ProductInstanceGetRequest productInstanceGetRequest)
        {
            if(productInstanceGetRequest == null)
            {
                return Result<List<ProductInstanceResponse>>.Failure("Get request cannot be null.");
            }

            var parseResult = Guid.TryParse(productInstanceGetRequest.ProductId, out Guid parsedProductId);
            
            bool productExists = false;
            
            if (!string.IsNullOrEmpty(productInstanceGetRequest.ProductId))
            {
                if(!parseResult)
                {
                    return Result<List<ProductInstanceResponse>>.Failure("Product id is not correct.");
                }

                var entity = await _productRepository.GetEntityById(productInstanceGetRequest.ProductId);

                if(entity == null)
                {
                    return Result<List<ProductInstanceResponse>>.Failure("Product Id provided does not belong to an existing product.");
                }

                productExists = true;
            }

            //making sure page 1 will  be provided always
            productInstanceGetRequest.PageNumber = productInstanceGetRequest.PageNumber <= 1 ? 0 : productInstanceGetRequest.PageNumber;

            //keeping the page size between 20 and 1000
            productInstanceGetRequest.PageSize =  productInstanceGetRequest.PageSize < 20 ? 20 : productInstanceGetRequest.PageSize > 1000 ? 1000 : productInstanceGetRequest.PageSize;


            //do Query
            var query =  _productInstanceRepository.GetQueryable();

            // Fugure out column order
            if(productInstanceGetRequest.OrderBy == OrderBy.Desc)
            {
                if(productInstanceGetRequest.OrderByColumn == "status")
                {
                    query = query.OrderByDescending(x => x.Status);
                }else
                {
                    query = query.OrderByDescending(e =>  e.EntryDate);
                }

            }else
            {
                if(productInstanceGetRequest.OrderByColumn == "status")
                {
                    query = query.OrderBy(x => x.Status);
                }
                else
                {
                    query = query.OrderBy(e => e.EntryDate);
                }
            }

            //search parameters

            if (productExists)
            {
                query = query.Where(e => e.Product != null && e.Product.ProductTypeId == parsedProductId);
            }
            
            if(!string.IsNullOrEmpty(productInstanceGetRequest.SearchText))
            {
                query = query.Where(e => (e.Barcode != null && e.Barcode.Contains(productInstanceGetRequest.SearchText)) || (e.Product != null && e.Product.ProductName != null && e.Product.ProductName.Contains(productInstanceGetRequest.SearchText)));
            }

            var entities = await query.Skip((int)(productInstanceGetRequest.PageNumber * productInstanceGetRequest.PageSize)).Take((int)productInstanceGetRequest.PageSize).ToListAsync();

            if (entities.Any())
            {
                var responses = entities.Select(e => new ProductInstanceResponse()
                {
                    Barcode = e.Barcode,
                    ConcurrencyStamp = e.ConcurrencyStamp,
                    EntryDate = e.EntryDate,
                    Id = e.Id.ToString(),
                    LocationId = e.LocationId.ToString(),
                    LocationName = e.Location != null ? e.Location.Name : null,
                    ProductId = e.ProductId.ToString(),
                    ProductName = e.Product != null ? e.Product.ProductName : null,
                    Status = e.Status
                }).ToList();

                return Result<List<ProductInstanceResponse>>.Success(responses);
            }else
            {
                return Result<List<ProductInstanceResponse>>.Failure("No matching prodcuts found based on your search text or product Id"); 
            }
        }

        public async Task<Result<ProductInstanceResponse>> UpdateProductInstance(ProductInstancePutRequest productInstancePutRequest)
        {
            if (productInstancePutRequest == null)
            {
                return Result<ProductInstanceResponse>.Failure("Put Request cannot be null."); 
            }

            if(string.IsNullOrEmpty(productInstancePutRequest.Id))
            {
                return Result<ProductInstanceResponse>.Failure("Product instance ID cannot be null.");
            }

            if(!Guid.TryParse(productInstancePutRequest.Id, out Guid parsedId))
            {
                return Result<ProductInstanceResponse>.Failure("Product instance ID id not the correct format.");
            }

            var entity = await _productInstanceRepository.Find(e =>  e.Id == parsedId);

            if (entity == null)
            {
                return Result<ProductInstanceResponse>.Failure("Product instance ID does not match any product.");
            }

            //check concurrency
            if (productInstancePutRequest.ConcurrencyStamp == null)
            {
                return Result<ProductInstanceResponse>.Failure("ConcurrencyStamp is required when updating data.");
            }
            if (entity.ConcurrencyStamp != null && !entity.ConcurrencyStamp.SequenceEqual(productInstancePutRequest.ConcurrencyStamp))
            {
                return Result<ProductInstanceResponse>.Failure("Concurrency conflict: The product was modified by another process.");
            }

            //ProductID
            if (!Guid.TryParse(productInstancePutRequest.ProductId, out Guid parsedProductId))
            {
                return Result<ProductInstanceResponse>.Failure("Product Id is not the correct format.");
            }
            else
            {
                var product = await _productRepository.Find(e => e.Id == parsedProductId);

                if (product == null)
                {
                    return Result<ProductInstanceResponse>.Failure("Product Id does not match an existing Product.");
                }
            }

            //Location ID
            if (!Guid.TryParse(productInstancePutRequest.LocationId, out Guid parsedLocationId))
            {
                return Result<ProductInstanceResponse>.Failure("Location id is not the correct format.");
            }
            else
            {
                var location = await _locationRepository.Find(e => e.Id == parsedLocationId);

                if (location == null)
                {
                    return Result<ProductInstanceResponse>.Failure("Location id provided does not exist");
                } 
            }
            
            //Update retreived entity and sent to the update method in the reposotory
            entity.Barcode = productInstancePutRequest.Barcode;
            entity.Status = productInstancePutRequest.Status;
            entity.PurchasePrice = productInstancePutRequest.PurchasePrice;
            entity.ProductId = parsedProductId;
            entity.LocationId = parsedLocationId;

            if (productInstancePutRequest.ProductInstance_Property != null && productInstancePutRequest.ProductInstance_Property.Any())
            {
                await this.AddPropertiesToInstance(entity, productInstancePutRequest.ProductInstance_Property);
            }

            var updatedEntity = await _productInstanceRepository.Update(entity);

            var responseDTO = this.GetResponseDTO(updatedEntity);

            if (responseDTO == null)
            {
                return Result<ProductInstanceResponse>.Failure("Could not return data, please go to all view.");
            }

            return Result<ProductInstanceResponse>.Success(responseDTO);
        }
        public async Task<Result<ProductInstanceResponse>> DeleteProductInstance(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<ProductInstanceResponse>.Failure("Id cannot be null");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<ProductInstanceResponse>.Failure("Id is not the correct format");
            }

            var entity = await _productInstanceRepository.Find(e => e.Id == parsedId);

            if(entity == null)
            {
                return Result<ProductInstanceResponse>.Failure("Id provided does not belong to an existing product");
            }
            await _productService.SubtrackToStock(entity.ProductId.ToString());

            
            var isDeleted = await _productInstanceRepository.Delete(id);

            return isDeleted ?
                Result<ProductInstanceResponse>.Success(new ProductInstanceResponse()) :
                Result<ProductInstanceResponse>.Failure("Could not delete from DB.");
        }


        public async Task<Result<List<ProductInstanceResponse>>> GetByProductId(ProductInstanceGetRequest productInstanceGetRequest)
        {
            if (productInstanceGetRequest == null)
            {
                return Result<List<ProductInstanceResponse>>.Failure("Get request cannot be null.");
            }

            if(string.IsNullOrEmpty(productInstanceGetRequest.ProductId))
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id cannot be null.");
            }

            if(!Guid.TryParse(productInstanceGetRequest.ProductId, out Guid parsedProductId))
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id is not the correc format.");
            }

            var dbProduct = await _productRepository.Find(e => e.Id == parsedProductId);

            if(dbProduct == null)
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id does not exist.");
            }

            productInstanceGetRequest.PageNumber = productInstanceGetRequest.PageNumber < 0 ? 0 : productInstanceGetRequest.PageNumber;

            productInstanceGetRequest.PageSize = productInstanceGetRequest.PageSize < 20 ? 20 : productInstanceGetRequest.PageSize > 100 ? 100 : productInstanceGetRequest.PageSize;

            var query = _productInstanceRepository.GetQueryable();

            if(productInstanceGetRequest.OrderBy == OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Location);
            }
            else
            {
                query = query.OrderByDescending(e => e.Location);
            }

            query = query.Where(e => e.ProductId == parsedProductId && e.Status != "Sold");

            var dbList = await query.Skip(productInstanceGetRequest.PageSize * productInstanceGetRequest.PageNumber).Take(productInstanceGetRequest.PageSize).Include(e => e.Location).ToListAsync(); 


            
            if(!dbList.Any())
            {
                return Result<List<ProductInstanceResponse>>.Success(new List<ProductInstanceResponse>());
            }




            var response = dbList.Select(e => new ProductInstanceResponse()
            {
                Barcode = e.Barcode,
                Id = e.Id.ToString(),
                EntryDate = e.EntryDate,
                LocationId = e.LocationId.ToString(),
                Status = e.Status,
                ConcurrencyStamp = e.ConcurrencyStamp,
                LocationName = e.Location != null ?  e.Location.Name : null,
                ProductId = e.ProductId.ToString(),
                ProductName = e.Product != null ? e.Product.ProductName : null,
                PurchasePrice = e.PurchasePrice,
            }).ToList();

            return Result<List<ProductInstanceResponse>>.Success(response);

        }

        public async Task<Result<List<ProductInstanceResponse>>> GetSoldByProductId(ProductInstanceGetRequest productInstanceGetRequest)
        {
            if (productInstanceGetRequest == null)
            {
                return Result<List<ProductInstanceResponse>>.Failure("Get request cannot be null.");
            }

            if (string.IsNullOrEmpty(productInstanceGetRequest.ProductId))
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id cannot be null.");
            }

            if (!Guid.TryParse(productInstanceGetRequest.ProductId, out Guid parsedProductId))
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id is not the correc format.");
            }

            var dbProduct = await _productRepository.Find(e => e.Id == parsedProductId);

            if (dbProduct == null)
            {
                return Result<List<ProductInstanceResponse>>.Failure("Product Id does not exist.");
            }

            productInstanceGetRequest.PageNumber = productInstanceGetRequest.PageNumber < 0 ? 0 : productInstanceGetRequest.PageNumber;

            productInstanceGetRequest.PageSize = productInstanceGetRequest.PageSize < 20 ? 20 : productInstanceGetRequest.PageSize > 100 ? 100 : productInstanceGetRequest.PageSize;

            var query = _productInstanceRepository.GetQueryable();

            if (productInstanceGetRequest.OrderBy == OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Location);
            }
            else
            {
                query = query.OrderByDescending(e => e.Location);
            }

            query = query.Where(e => e.ProductId == parsedProductId && e.Status == "Sold");

            var dbList = await query.Skip(productInstanceGetRequest.PageSize * productInstanceGetRequest.PageNumber).Take(productInstanceGetRequest.PageSize).Include(e => e.Location).ToListAsync();



            if (!dbList.Any())
            {
                return Result<List<ProductInstanceResponse>>.Success(new List<ProductInstanceResponse>());
            }




            var response = dbList.Select(e => new ProductInstanceResponse()
            {
                Barcode = e.Barcode,
                Id = e.Id.ToString(),
                EntryDate = e.EntryDate,
                LocationId = e.LocationId.ToString(),
                Status = e.Status,
                ConcurrencyStamp = e.ConcurrencyStamp,
                LocationName = e.Location != null ? e.Location.Name : null,
                ProductId = e.ProductId.ToString(),
                ProductName = e.Product != null ? e.Product.ProductName : null,
                PurchasePrice = e.PurchasePrice,
                SoldBy = e.SoldBy,
            }).ToList();

            return Result<List<ProductInstanceResponse>>.Success(response);

        }
    }
}
