using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_API.Model
{
    public class Like
    {
        public int LikeId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ForeignKey("User")]
      
        public string UserId { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }

    }
}
