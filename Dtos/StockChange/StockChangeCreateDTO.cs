namespace ProjectLaborBackend.Dtos.StockChange
{
    public class StockChangeCreateDTO
    {
        public int Quantity { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
    }
}
