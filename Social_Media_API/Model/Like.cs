using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_API.Model
{
    public enum Reaction
    {
        Love=1,
        Hahaha=2,
        Care=3,
        Angrey=4,
        Sad=5,
        Woooooow=6
    }
    public class Like
    {
        public int LikeId { get; set; }
        [ForeignKey("Post")]
        public DateTime CreatAt { get; set; }
        public Reaction Reaction { get; set; }
        public int PostId { get; set; }
        [ForeignKey("User")]
      
        public string UserId { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }

    }
}
