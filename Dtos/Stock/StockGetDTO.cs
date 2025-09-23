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
        public string Currency { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
    }
}
