using Application.DTOs.Common;
using Application.DTOs.Result;
using Application.DTOs.WarehouseItem;

namespace Application.Interfaces
{
    public interface IWarehouseItemService
    {
        Task<Result<WarehouseItemDto>> GetWarehouseItemAsync(int id);
        Task<Result<List<WarehouseItemDto>>> GetWarehouseItemsAsync(PagingParams pagingParams, string? filter);
        Task<Result<WarehouseItemDto>> CreateWarehouseItemAsync(CreateWarehouseItemDto dto);
        Task<Result<WarehouseItemDto>> UpdateWarehouseItemAsync(int id, UpdateWarehouseItemDto dto);
        Task<Result<bool>> DeleteWarehouseItemAsync(int id);
        Task<Result<TopItemsDto>> GetTopItemsAsync();
    }
}
