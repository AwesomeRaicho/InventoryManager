using InventoryManager.Core.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IProduct_PropertyService
    {
        public Task<bool> Create(string productId, List<string> propertyInstanceIds);

        public Task<bool> Delete(string productId, List<string> propertyInstanceIds);

        public Task<List<PropertyInstanceResponse>> GetByProductId(string productId);
    }
}
