using Application.DTOs.Common;
using Application.DTOs.Warehouse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Extensions;


namespace HappyCompanyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            var result = await _warehouseService.GetWarehouseAsync(id);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses([FromQuery] PagingParams pagingParams, [FromQuery] string? filter)
        {
            var result = await _warehouseService.GetWarehousesAsync(pagingParams, filter);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto dto)
        {
            var result = await _warehouseService.CreateWarehouseAsync(dto);
            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto dto)
        {
            var result = await _warehouseService.UpdateWarehouseAsync(id, dto);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            return result.ToActionResult();
        }
    }
}
