namespace ProjectLaborBackend.Dtos.StockChange
{
    public class StockChangeGetDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime ChangeDate { get; set; }
        public int ProductId { get; set; }
    }
}
