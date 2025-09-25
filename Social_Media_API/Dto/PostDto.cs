namespace Social_Media_API.Dto
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
