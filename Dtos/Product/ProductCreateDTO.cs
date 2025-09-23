using ProjectLaborBackend.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectLaborBackend.Dtos.Product
{
    public class ProductCreateDTO
    {
        public string EAN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
