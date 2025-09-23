using System.ComponentModel.DataAnnotations;

namespace ProjectLaborBackend.Dtos.UserDTOs
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
