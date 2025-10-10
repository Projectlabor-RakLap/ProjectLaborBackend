namespace ProjectLaborBackend.Dtos.Stock
{
    public class StockCreateDTO
    {
        public int StockInWarehouse { get; set; }
        public int StockInStore { get; set; }
        public int WarehouseCapacity { get; set; }
        public int StoreCapacity { get; set; }
        public double Price { get; set; }
        public double TransportCost { get; set; }
        public double StorageCost { get; set; }
        public string Currency { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
    }
}
