namespace ProjectLaborBackend.Dtos.Product
{
    public class ProductGetDTO
    {
        public int Id { get; set; }
        public string EAN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
