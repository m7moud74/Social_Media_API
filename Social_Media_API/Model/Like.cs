using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_API.Model
{
    public enum Raction
    {
        Love,
        Hahaha,
        Care,
        Angrey,
        Sad,
        Woooooow
    }
    public class Like
    {
        public int LikeId { get; set; }
        [ForeignKey("Post")]
        public DateTime CreatAt { get; set; }
        public Raction Raction { get; set; }
        public int PostId { get; set; }
        [ForeignKey("User")]
      
        public string UserId { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }

    }
}
