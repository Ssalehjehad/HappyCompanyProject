
namespace Application.DTOs.WarehouseItem
{
    public class WarehouseItemDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int Qty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? MsrpPrice { get; set; }
        public int WarehouseId { get; set; }
    }
}
