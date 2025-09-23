using ProjectLaborBackend.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectLaborBackend.Dtos.Warehouse
{
    public class WarehouseGetDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
    }
}
