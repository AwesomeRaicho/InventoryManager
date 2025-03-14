using InventoryManager.Core.DTO;
using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IPropertyTypeService
    {
        public Task<Result<PropertyTypeResponse>> Create(PropertyTypeCreateRequest CreateRequest);

        public Task<Result<bool>> Delete(string id);

        public Task<Result<List<PropertyTypeResponse>>> GetAll(PropertyTypeGetRequest GetRequest);

        public Task<Result<PropertyTypeResponse>> GetById(string id);

        public Task<Result<PropertyTypeResponse>> Update(PropertyTypePutRequest PutRequest);
    }
}
