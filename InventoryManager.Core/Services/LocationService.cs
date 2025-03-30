using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Models;
using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Data.SqlTypes;

namespace InventoryManager.Core.Services
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Location> _locationRepository;

        public LocationService(IRepository<Location> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        private LocationResponse? GetLocationResponse(Location location)
        {
            if(location == null) return null;

            return new LocationResponse()
            {
                Id = location.Id.ToString(),
                Name = location.Name,
                ConcurrencyStamp = location.ConcurrencyStamp,
            };

        }

        public async Task<Result<LocationResponse>> CreateLocation(LocationCreateRequest locationCreateRequest)
        {
            if(locationCreateRequest == null || string.IsNullOrEmpty(locationCreateRequest.Name))
            {
                return Result<LocationResponse>.Failure("Location create request cannot be null.");
            }

            var dbEntity = await _locationRepository.Find(e =>  e.Name == locationCreateRequest.Name);

            if(dbEntity != null)
            {   
                return Result<LocationResponse>.Failure("Location Name already exists.");
            }

            var locationEntity = new Location()
            {
                Id = Guid.NewGuid(),
                Name = locationCreateRequest.Name,
            };
            
            var newLocationresponse = await _locationRepository.Create(locationEntity);

            var response = this.GetLocationResponse(newLocationresponse);

            if(response == null)
            {
                return Result<LocationResponse>.Failure("Something went wront when returning respose DTO of the newly created location. please validate in list to confirm it was created.");
            }

            return Result<LocationResponse>.Success(response);
        }

        public async Task<Result<LocationResponse>> GetLocationById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Result<LocationResponse>.Failure("Location ID cannot be null.");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<LocationResponse>.Failure("Location ID is not the correct format.");
            }

            var dbEntity = await _locationRepository.Find(e => e.Id == parsedId);

            if(dbEntity == null)
            {
                return Result<LocationResponse>.Failure("Location ID does not exist.");
            }

            var responseDTO = this.GetLocationResponse(dbEntity);
            
            if(responseDTO == null)
            {
                return Result<LocationResponse>.Failure("Could not return Response DTO of created location.");
            }

            return Result<LocationResponse>.Success(responseDTO);
        }

        public async Task<Result<List<LocationResponse>>> GetAllLocations(LocationGetRequest locationGetRequest)
        {
            if(locationGetRequest == null)
            {
                return Result<List<LocationResponse>>.Failure("Location request cannot be null."); 
            }

            var query = _locationRepository.GetQueryable();

            if(!string.IsNullOrEmpty(locationGetRequest.SearchText))
            {
                query = query.Where(e => e.Name != null && e.Name.Contains(locationGetRequest.SearchText));
            }

            locationGetRequest.PageSize = locationGetRequest.PageSize < 20 ? 20 : locationGetRequest.PageSize > 1000 ? 1000 : locationGetRequest.PageSize;

            locationGetRequest.PageIndex = locationGetRequest.PageIndex < 0 ? 0 : locationGetRequest.PageIndex;

            if(locationGetRequest.OrderBy == Enums.OrderBy.Asc)
            {
                query = query.OrderBy(e => e.Name);
            }else
            {
                query = query.OrderByDescending(e => e.Name);
            }

            var dbEntities = await query.Skip(locationGetRequest.PageSize * locationGetRequest.PageIndex).Take(locationGetRequest.PageSize).ToListAsync();

            if(!dbEntities.Any())
            {
                return Result<List<LocationResponse>>.Success(new List<LocationResponse>());
            }

            var response = dbEntities.Select(e => new LocationResponse()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                ConcurrencyStamp = e.ConcurrencyStamp,
            }).ToList();

            return Result<List<LocationResponse>>.Success(response); 

        }

        public async Task<Result<LocationResponse>> UpdateLocation(LocationPutRequest locationPutRequest)
        {
            if(locationPutRequest == null)
            {
                return Result<LocationResponse>.Failure("Location request cannot be null.");
            }
            if (string.IsNullOrEmpty(locationPutRequest.Name))
            {
                return Result<LocationResponse>.Failure("Location request requires a name.");
            }

            if(string.IsNullOrEmpty(locationPutRequest.Id))
            { 
                return Result<LocationResponse>.Failure("Location request requires ID to be provided.");
            }

            if (!Guid.TryParse(locationPutRequest.Id, out Guid parsedId))
            {
                return Result<LocationResponse>.Failure("Location ID is not the correct format.");
            }

            var dbEntity = await _locationRepository.Find(e => e.Id == parsedId);

            if (dbEntity == null)
            {
                return Result<LocationResponse>.Failure("Location does not exist.");
            }

            var dbEntityWithName = await _locationRepository.Find(e => e.Name == locationPutRequest.Name);

            if(dbEntityWithName != null)
            {
                return Result<LocationResponse>.Failure("Location name already exists.");
            }


            if (dbEntity.ConcurrencyStamp != null && !dbEntity.ConcurrencyStamp.SequenceEqual(dbEntity.ConcurrencyStamp))
            {   
                return Result<LocationResponse>.Failure("Concurrency error: location may have been updated, please refresh request to obtasined the most updated version of the location data.");
            }

            dbEntity.Name = locationPutRequest.Name;

            await _locationRepository.Update(dbEntity);

            var response = this.GetLocationResponse(dbEntity);

            if(response == null)
            {
                return Result<LocationResponse>.Failure("Could not retreive response DTO.");
            }

            return Result<LocationResponse>.Success(response);

        }

        public async Task<Result<bool>> DeleteLocation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Result<bool>.Failure("Id cannot be null when deleting a location.");
            }

            if(!Guid.TryParse(id, out Guid parsedId))
            {
                return Result<bool>.Failure("Location id is not the correct format.");
            }

            var dbEntity = await _locationRepository.Find(e => e.Id == parsedId);

            if(dbEntity == null)
            {
                return Result<bool>.Failure("Location does not exist.");
            }

            await _locationRepository.Delete(id);

            return Result<bool>.Success(true);
        }

    }
}
