using Application.DTOs.Common;
using Application.DTOs.Result;
using Application.DTOs.Warehouse;
using Application.Interfaces;
using Application.Store;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly InventoryContext _context;
        public WarehouseService(InventoryContext context)
        {
            _context = context;
        }

        public async Task<Result<WarehouseDto>> GetWarehouseAsync(int id)
        {
            var result = new Result<WarehouseDto>(null);
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse not found." };
                    return result;
                }

                var itemsCount = await _context.WarehouseItems.CountAsync(i => i.WarehouseId == id);

                result.Data = new WarehouseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Address = warehouse.Address,
                    City = warehouse.City,
                    Country = warehouse.Country,
                    ItemsCount = itemsCount
                };
                result.StatusCode = StatusCode.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting warehouse with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving the warehouse." };
            }
            return result;
        }

        public async Task<Result<List<WarehouseDto>>> GetWarehousesAsync(PagingParams pagingParams, string? filter)
        {
            var result = new Result<List<WarehouseDto>>(null);
            try
            {
                var query = _context.Warehouses.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter))
                    query = query.Where(w => w.Name.Contains(filter));

                query = pagingParams.SortDesc ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name);

                var warehouses = await query
                    .Skip(pagingParams.PageIndex * pagingParams.PageSize)
                    .Take(pagingParams.PageSize)
                    .ToListAsync();

                var dtoList = new List<WarehouseDto>();
                foreach (var warehouse in warehouses)
                {
                    var itemsCount = await _context.WarehouseItems.CountAsync(i => i.WarehouseId == warehouse.Id);
                    dtoList.Add(new WarehouseDto
                    {
                        Id = warehouse.Id,
                        Name = warehouse.Name,
                        Address = warehouse.Address,
                        City = warehouse.City,
                        Country = warehouse.Country,
                        ItemsCount = itemsCount
                    });
                }

                result.Data = dtoList;
                result.StatusCode = StatusCode.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting warehouses");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving warehouses." };
            }
            return result;
        }

        public async Task<Result<WarehouseDto>> CreateWarehouseAsync(CreateWarehouseDto dto)
        {
            var result = new Result<WarehouseDto>(null);
            try
            {
                if (await _context.Warehouses.AnyAsync(w => w.Name == dto.Name))
                {
                    result.StatusCode = StatusCode.AlreadyExist;
                    result.ErrorMessages = new List<string> { "Warehouse name already exists." };
                    return result;
                }

                var warehouse = new Domain.Entities.Warehouse
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    City = dto.City,
                    Country = dto.Country
                };

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync();

                result.Data = new WarehouseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Address = warehouse.Address,
                    City = warehouse.City,
                    Country = warehouse.Country,
                    ItemsCount = 0
                };
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse created successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating warehouse");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while creating the warehouse." };
            }
            return result;
        }

        public async Task<Result<WarehouseDto>> UpdateWarehouseAsync(int id, UpdateWarehouseDto dto)
        {
            var result = new Result<WarehouseDto>(null);
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse not found." };
                    return result;
                }

                warehouse.Name = dto.Name;
                warehouse.Address = dto.Address;
                warehouse.City = dto.City;
                warehouse.Country = dto.Country;
                await _context.SaveChangesAsync();

                var itemsCount = await _context.WarehouseItems.CountAsync(i => i.WarehouseId == warehouse.Id);
                result.Data = new WarehouseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Address = warehouse.Address,
                    City = warehouse.City,
                    Country = warehouse.Country,
                    ItemsCount = itemsCount
                };
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse updated successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating warehouse with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while updating the warehouse." };
            }
            return result;
        }

        public async Task<Result<bool>> DeleteWarehouseAsync(int id)
        {
            var result = new Result<bool>(false);
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "Warehouse not found." };
                    return result;
                }

                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
                result.Data = true;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Warehouse deleted successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting warehouse with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while deleting the warehouse." };
            }
            return result;
        }
    }
}
