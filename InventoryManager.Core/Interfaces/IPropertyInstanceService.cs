using InventoryManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IPropertyInstanceService
    {
        public Task<Result<PropertyInstanceResponse>> CreatePropertyInstance(PropertyInstanceCreateRequest propertyInstanceCreateRequest);

        public Task<Result<PropertyInstanceResponse>> GetPropertyInstance(string id);

        public Task<Result<PropertyInstanceResponse>> UpdatePropertyInstance(PropertyInstancePutRequest propertyInstancePutRequest);

        public Task<Result<bool>> DeletePropertyInstance(string id);

        public Task<Result<List<PropertyInstanceResponse>>> GetAllPropertyInstance(PropertyInstanceGetRequest propertyInstanceGetRequest);

        public Task<Result<List<PropertyInstanceResponse>>> GetPropertyInstancesByProductTypeId(PropertyInstanceGetRequest propertyInstanceGetRequest);
    }
}
