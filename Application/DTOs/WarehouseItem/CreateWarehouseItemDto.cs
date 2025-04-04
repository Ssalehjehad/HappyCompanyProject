
namespace Application.DTOs.WarehouseItem
{
    public class CreateWarehouseItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int Qty { get; set; } = 1;
        public decimal CostPrice { get; set; }
        public decimal? MsrpPrice { get; set; }
        public int WarehouseId { get; set; }
    }
}
