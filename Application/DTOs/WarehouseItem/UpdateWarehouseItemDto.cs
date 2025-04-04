
namespace Application.DTOs.WarehouseItem
{
    public class UpdateWarehouseItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int Qty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? MsrpPrice { get; set; }
    }
}
