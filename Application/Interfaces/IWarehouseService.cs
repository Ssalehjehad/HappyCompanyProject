using Application.DTOs.Common;
using Application.DTOs.Result;
using Application.DTOs.Warehouse;

namespace Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<Result<WarehouseDto>> GetWarehouseAsync(int id);
        Task<Result<List<WarehouseDto>>> GetWarehousesAsync(PagingParams pagingParams, string? filter);
        Task<Result<WarehouseDto>> CreateWarehouseAsync(CreateWarehouseDto dto);
        Task<Result<WarehouseDto>> UpdateWarehouseAsync(int id, UpdateWarehouseDto dto);
        Task<Result<bool>> DeleteWarehouseAsync(int id);
    }
}
