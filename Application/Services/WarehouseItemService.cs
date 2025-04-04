using Application.DTOs.Common;
using Application.DTOs.Result;
using Application.DTOs.WarehouseItem;
using Application.Interfaces;
using Application.Store;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Application.Services
{
    public class WarehouseItemService : IWarehouseItemService
    {
        private readonly InventoryContext _context;
        public WarehouseItemService(InventoryContext context)
        {
            _context = context;
        }

        public async Task<Result<WarehouseItemDto>> GetWarehouseItemAsync(int id)
        {
            var result = new Result<WarehouseItemDto>(null);
            try
            {
                var item = await _context.WarehouseItems.FindAsync(id);
                if (item == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse item not found." };
                    return result;
                }
                result.Data = new WarehouseItemDto
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    SkuCode = item.SkuCode,
                    Qty = item.Quantity,
                    CostPrice = item.CostPrice,
                    MsrpPrice = item.MsrpPrice,
                    WarehouseId = item.WarehouseId
                };
                result.StatusCode = StatusCode.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting warehouse item with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving the warehouse item." };
            }
            return result;
        }

        public async Task<Result<List<WarehouseItemDto>>> GetWarehouseItemsAsync(PagingParams pagingParams, string? filter)
        {
            var result = new Result<List<WarehouseItemDto>>(null);
            try
            {
                var query = _context.WarehouseItems.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter))
                    query = query.Where(i => i.ItemName.Contains(filter));

                query = pagingParams.SortDesc ? query.OrderByDescending(i => i.ItemName) : query.OrderBy(i => i.ItemName);

                var items = await query
                    .Skip(pagingParams.PageIndex * pagingParams.PageSize)
                    .Take(pagingParams.PageSize)
                    .ToListAsync();

                var dtos = items.Select(item => new WarehouseItemDto
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    SkuCode = item.SkuCode,
                    Qty = item.Quantity,
                    CostPrice = item.CostPrice,
                    MsrpPrice = item.MsrpPrice,
                    WarehouseId = item.WarehouseId
                }).ToList();

                result.Data = dtos;
                result.StatusCode = StatusCode.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting warehouse items");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving warehouse items." };
            }
            return result;
        }

        public async Task<Result<WarehouseItemDto>> CreateWarehouseItemAsync(CreateWarehouseItemDto dto)
        {
            var result = new Result<WarehouseItemDto>(null);
            try
            {
                if (await _context.WarehouseItems.AnyAsync(i => i.WarehouseId == dto.WarehouseId && i.ItemName == dto.ItemName))
                {
                    result.StatusCode = StatusCode.AlreadyExist;
                    result.ErrorMessages = new List<string> { "An item with this name already exists in the warehouse." };
                    return result;
                }

                var item = new Domain.Entities.WarehouseItem
                {
                    ItemName = dto.ItemName,
                    SkuCode = dto.SkuCode,
                    Quantity = dto.Qty,
                    CostPrice = dto.CostPrice,
                    MsrpPrice = dto.MsrpPrice,
                    WarehouseId = dto.WarehouseId
                };

                _context.WarehouseItems.Add(item);
                await _context.SaveChangesAsync();

                result.Data = new WarehouseItemDto
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    SkuCode = item.SkuCode,
                    Qty = item.Quantity,
                    CostPrice = item.CostPrice,
                    MsrpPrice = item.MsrpPrice,
                    WarehouseId = item.WarehouseId
                };
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse item created successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating warehouse item");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while creating the warehouse item." };
            }
            return result;
        }

        public async Task<Result<WarehouseItemDto>> UpdateWarehouseItemAsync(int id, UpdateWarehouseItemDto dto)
        {
            var result = new Result<WarehouseItemDto>(null);
            try
            {
                var item = await _context.WarehouseItems.FindAsync(id);
                if (item == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse item not found." };
                    return result;
                }

                item.ItemName = dto.ItemName;
                item.SkuCode = dto.SkuCode;
                item.Quantity = dto.Qty;
                item.CostPrice = dto.CostPrice;
                item.MsrpPrice = dto.MsrpPrice;

                await _context.SaveChangesAsync();

                result.Data = new WarehouseItemDto
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    SkuCode = item.SkuCode,
                    Qty = item.Quantity,
                    CostPrice = item.CostPrice,
                    MsrpPrice = item.MsrpPrice,
                    WarehouseId = item.WarehouseId
                };
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse item updated successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating warehouse item with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while updating the warehouse item." };
            }
            return result;
        }

        public async Task<Result<bool>> DeleteWarehouseItemAsync(int id)
        {
            var result = new Result<bool>(false);
            try
            {
                var item = await _context.WarehouseItems.FindAsync(id);
                if (item == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse item not found." };
                    return result;
                }
                _context.WarehouseItems.Remove(item);
                await _context.SaveChangesAsync();
                result.Data = true;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse item deleted successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting warehouse item with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while deleting the warehouse item." };
            }
            return result;
        }

        public async Task<Result<TopItemsDto>> GetTopItemsAsync()
        {
            var result = new Result<TopItemsDto>(null);
            try
            {
                var topHighItems = await _context.WarehouseItems
                    .OrderByDescending(i => i.Quantity)
                    .Take(10)
                    .ToListAsync();

                var topLowItems = await _context.WarehouseItems
                    .OrderBy(i => i.Quantity)
                    .Take(10)
                    .ToListAsync();

                var dto = new TopItemsDto
                {
                    TopHighItems = topHighItems.Select(item => new WarehouseItemDto
                    {
                        Id = item.Id,
                        ItemName = item.ItemName,
                        SkuCode = item.SkuCode,
                        Qty = item.Quantity,
                        CostPrice = item.CostPrice,
                        MsrpPrice = item.MsrpPrice,
                        WarehouseId = item.WarehouseId
                    }).ToList(),

                    TopLowItems = topLowItems.Select(item => new WarehouseItemDto
                    {
                        Id = item.Id,
                        ItemName = item.ItemName,
                        SkuCode = item.SkuCode,
                        Qty = item.Quantity,
                        CostPrice = item.CostPrice,
                        MsrpPrice = item.MsrpPrice,
                        WarehouseId = item.WarehouseId
                    }).ToList()
                };

                result.Data = dto;
                result.StatusCode = StatusCode.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving top items");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving top items." };
            }
            return result;
        }
    }
}
