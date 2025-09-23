using System.ComponentModel.DataAnnotations;

namespace ProjectLaborBackend.Dtos.UserDTOs
{
    public class UserPutPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
