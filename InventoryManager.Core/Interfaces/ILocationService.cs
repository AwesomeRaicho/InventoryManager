using InventoryManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface ILocationService
    {
        public Task<Result<LocationResponse>> CreateLocation(LocationCreateRequest locationCreateRequest);
        public Task<Result<LocationResponse>> GetLocationById(string id);
        public Task<Result<List<LocationResponse>>> GetAllLocations(LocationGetRequest locationGetRequest);
        public Task<Result<LocationResponse>> UpdateLocation(LocationPutRequest locationPutRequest);
        public Task<Result<bool>> DeleteLocation(string id);

    }
}
