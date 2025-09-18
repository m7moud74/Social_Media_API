using System.ComponentModel.DataAnnotations;

namespace Social_Media_API.Dto
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }=string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public UserDto CommentUserDto { get; set; }=new UserDto();
    }
}
