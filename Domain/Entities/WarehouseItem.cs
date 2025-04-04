using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        public string ItemName { get; set; }
        public string SkuCode { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? MsrpPrice { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
