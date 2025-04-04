
namespace Application.DTOs.WarehouseItem
{
    public class TopItemsDto
    {
        public List<WarehouseItemDto> TopHighItems { get; set; } = new List<WarehouseItemDto>();
        public List<WarehouseItemDto> TopLowItems { get; set; } = new List<WarehouseItemDto>();
    }
}
