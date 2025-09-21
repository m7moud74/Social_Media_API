namespace Social_Media_API.Dto
{
    public class PostReadDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPic { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public UserDto PostUserDto { get; set; } = new UserDto();
     
       
    }
}
