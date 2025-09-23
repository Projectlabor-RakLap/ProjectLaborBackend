namespace ProjectLaborBackend.Dtos.Stock
{
    public class StockUpdateDto
    {
        public int? StockInWarehouse { get; set; }
        public int? StockInStore { get; set; }
        public int? WarehouseCapacity { get; set; }
        public int? StoreCapacity { get; set; }
        public double? Price { get; set; }
        public string? Currency { get; set; }
    }
}
