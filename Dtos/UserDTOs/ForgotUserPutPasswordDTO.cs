using System.ComponentModel.DataAnnotations;

namespace ProjectLaborBackend.Dtos.UserDTOs
{
    public class ForgotUserPutPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
