using InventoryManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;


namespace InventoryManager.Core.Services
{
    public class Product_PropertyService : IProduct_PropertyService
    {

        private readonly IRepository<Product_Property> _product_PropertyRespository;
        private readonly IRepository<Product> _productRespository;
        private readonly IRepository<PropertyInstance> _propertyInstanceRespository;

        public Product_PropertyService(IRepository<Product_Property> product_PropertyRepository, IRepository<Product> productRepository, IRepository<PropertyInstance> propertyInstanceRepository)
        {
            _product_PropertyRespository = product_PropertyRepository;
            _productRespository = productRepository;
            _propertyInstanceRespository= propertyInstanceRepository;
        }


        public async Task<bool> Create(string productId, List<string> propertyInstanceIds)
        {
            if(string.IsNullOrEmpty(productId) || propertyInstanceIds.Count <= 0)
            {
                return false;
            }

            if(!Guid.TryParse(productId, out Guid parsedProductId))
            {
                return false;
            }

            var dbProduct = await _productRespository.Find(e => e.Id == parsedProductId);

            if (dbProduct == null)
            {
                return false;
            }

            var validId = new List<Guid>();

            foreach (var propertyInstanceId in propertyInstanceIds)
            {
                if(!Guid.TryParse(propertyInstanceId, out Guid parsedPropertyId))
                {
                    continue;
                }

                var dbProperty = await _propertyInstanceRespository.Find(e => e.Id == parsedPropertyId);

                if (dbProperty == null)
                {
                    continue;
                }

                validId.Add(parsedPropertyId);
            }

            foreach(var propertyInstanceId in validId)
            {
                var newEntety = new Product_Property()
                {
                    ProductId = parsedProductId,
                    PropertyId = propertyInstanceId
                };

                await _product_PropertyRespository.Create(newEntety);
            }

            return true;

        }

        public async Task<List<PropertyInstanceResponse>> GetByProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return new List<PropertyInstanceResponse>();
            }

            if (!Guid.TryParse(productId, out Guid parsedProductId))
            {
                return new List<PropertyInstanceResponse>();
            }

            var dbProduct = await _productRespository.Find(e => e.Id == parsedProductId);

            if (dbProduct == null)
            {
                return new List<PropertyInstanceResponse>();
            }

            var query = _product_PropertyRespository.GetQueryable();

            var properties = query.Where(e => e.ProductId == parsedProductId).ToList();

            return properties.Select(e => new PropertyInstanceResponse()
            {
                Id = e.PropertyId.ToString(),
                Name = e.Property != null ? e.Property.Name : string.Empty,
                PropertyTypeId = e.PropertyId.ToString(),
                PropertyTypeName = e.Property != null && e.Property.PropertyType != null ? e.Property.PropertyType.Name : string.Empty,
            }).ToList();


        }



        public Task<bool> Delete(string productId, List<string> propertyInstanceIds)
        {
            throw new NotImplementedException();
        }
    }
}
