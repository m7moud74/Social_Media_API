using Social_Media_API.Dto.Account_DTO;
using Social_Media_API.Dto.Comment_DTO;
using Social_Media_API.Dto.Like_DTO;

namespace Social_Media_API.Dto.Post_DTO
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
       public UserDto PostUserDto { get; set; }= new UserDto();
        public List<CommentDto> Comments { get; set; }=new List<CommentDto>();
        public List<LikeDto> Likes { get; set; } = new List<LikeDto>();



    }
}
