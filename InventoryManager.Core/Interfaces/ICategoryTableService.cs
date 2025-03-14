using InventoryManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    /// <summary>
    /// Generic class service for tables that have a single column string value, other than ConcurrencyStamp and ID
    /// </summary>
    /// <typeparam name="TEntity">Entity name in DB</typeparam>
    /// <typeparam name="TCreateRequestDTO">Create Request DTO</typeparam>
    /// <typeparam name="TGetRequestDTO">Get Request DTO</typeparam>
    /// <typeparam name="TPutRequestDTO">Put Request DTO</typeparam>
    /// <typeparam name="TResponseDTO">Response DTO</typeparam>
    public interface ICategoryTableService<
    TEntity,
    TCreateRequestDTO,
    TGetRequestDTO,
    TPutRequestDTO,
    TResponseDTO>
    where TEntity : class
    where TCreateRequestDTO : class
    where TGetRequestDTO : class
    where TPutRequestDTO : class
    where TResponseDTO : class
    {
        Task<Result<TResponseDTO>> Create(TCreateRequestDTO CreateRequest);
        Task<Result<TResponseDTO>> GetById(string id);
        Task<Result<List<TResponseDTO>>> GetAll(TGetRequestDTO GetRequest);
        Task<Result<TResponseDTO>> Update(TPutRequestDTO PutRequest);
        Task<Result<bool>> Delete(string id);
    }
}
