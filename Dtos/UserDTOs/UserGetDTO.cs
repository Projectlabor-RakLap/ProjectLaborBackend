namespace ProjectLaborBackend.Dtos.UserDTOs
{
    public class UserGetDTO
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public ICollection<string> WarehouseName { get; set; }
    }
}
