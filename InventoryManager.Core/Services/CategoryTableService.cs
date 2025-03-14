using InventoryManager.Core.DTO;
using InventoryManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Services
{
    public class CategoryTableService<TEntity, TCreateRequestDTO, TGetRequestDTO, TPutRequestDTO, TResponseDTO> : ICategoryTableService<TEntity, TCreateRequestDTO, TGetRequestDTO, TPutRequestDTO, TResponseDTO>
    where TEntity : class
    where TCreateRequestDTO : class
    where TGetRequestDTO : class
    where TPutRequestDTO : class
    where TResponseDTO : class
    {
        public Task<Result<TResponseDTO>> Create(TCreateRequestDTO CreateRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<TResponseDTO>>> GetAll(TGetRequestDTO GetRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TResponseDTO>> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TResponseDTO>> Update(TPutRequestDTO PutRequest)
        {
            throw new NotImplementedException();
        }
    }
}
