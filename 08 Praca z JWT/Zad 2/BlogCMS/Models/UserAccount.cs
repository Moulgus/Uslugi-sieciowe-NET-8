using System.ComponentModel.DataAnnotations;

namespace BlogCMS.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Admin";
    }
}
