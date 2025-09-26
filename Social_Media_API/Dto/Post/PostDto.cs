using Social_Media_API.Dto.Acoount;
using Social_Media_API.Dto.Comment;
using Social_Media_API.Dto.Like;

namespace Social_Media_API.Dto.Post
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
