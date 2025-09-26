using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Social_Media_API.Model
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [AllowNull]
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? PostFormFile { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }

    }
}

