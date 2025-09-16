using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_API.Model
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }

    }
}
