namespace ProjectLaborBackend.Dtos.UserDTOs
{
    public class UserPatchDTO
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public bool? IsVerified { get; set; }
    }
}
