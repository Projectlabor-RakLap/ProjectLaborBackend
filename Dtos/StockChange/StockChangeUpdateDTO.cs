namespace ProjectLaborBackend.Dtos.StockChange
{
    public class StockChangeUpdateDTO
    {
        public int? Quantity { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ProductId { get; set; }
    }
}
