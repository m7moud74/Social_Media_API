using Social_Media_API.Dto.Account_DTO;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_API.Dto.Comment_DTO
{
    public class CommentDto
    {

        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }=string.Empty;

        public DateTime CreatedAt { get; set; }
        [Display(Name = "Post Id")]
        public int PostId { get; set; }
        public UserDto CommentUserDto { get; set; }=new UserDto();
    }
}
