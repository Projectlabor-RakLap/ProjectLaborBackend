using ProjectLaborBackend.Dtos.Product;
using ProjectLaborBackend.Dtos.Warehouse;

namespace ProjectLaborBackend.Dtos.Stock
{
    public class StockGetDTO
    {
        public int Id { get; set; }
        public int StockInWarehouse { get; set; }
        public int StockInStore { get; set; }
        public int WarehouseCapacity { get; set; }
        public int StoreCapacity { get; set; }
        public double Price { get; set; }
        public double TransportCost { get; set; }
        public double StorageCost { get; set; }
        public string Currency { get; set; }
        public ProductGetDTO Product { get; set; }
        public WarehouseGetDTO Warehouse { get; set; }
    }
}
