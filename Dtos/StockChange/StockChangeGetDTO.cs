using ProjectLaborBackend.Dtos.Product;
namespace ProjectLaborBackend.Dtos.StockChange
{
    using ProjectLaborBackend.Entities;
    public class StockChangeGetDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime ChangeDate { get; set; }
        public ProductGetNoPicDTO Product { get; set; }
    }
}
