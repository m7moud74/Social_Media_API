namespace Social_Media_API.Dto
{
    public class LikeDto
    {
        public DateTime CreateAt { get; set; }
        public int PostId { get; set; }
        public UserDto LikeUserDto { get; set; }=new UserDto();

    }
}
