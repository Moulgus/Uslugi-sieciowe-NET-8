using System.ComponentModel.DataAnnotations;

namespace BlogCMS.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Url]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime Published { get; set; } = DateTime.Now;
    }
}
