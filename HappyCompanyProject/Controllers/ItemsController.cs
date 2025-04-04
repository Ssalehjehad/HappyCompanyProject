using Application.DTOs.Common;
using Application.DTOs.WarehouseItem;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Application.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HappyCompanyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IWarehouseItemService _itemService;
        public ItemsController(IWarehouseItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var result = await _itemService.GetWarehouseItemAsync(id);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] PagingParams pagingParams, [FromQuery] string? filter)
        {
            var result = await _itemService.GetWarehouseItemsAsync(pagingParams, filter);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateWarehouseItemDto dto)
        {
            var result = await _itemService.CreateWarehouseItemAsync(dto);
            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateWarehouseItemDto dto)
        {
            var result = await _itemService.UpdateWarehouseItemAsync(id, dto);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _itemService.DeleteWarehouseItemAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("topitems")]
        public async Task<IActionResult> GetTopItems()
        {
            var result = await _itemService.GetTopItemsAsync();
            return result.ToActionResult();
        }
    }
}
