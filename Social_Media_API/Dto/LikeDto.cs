namespace Social_Media_API.Dto
{
    public class LikeDto
    {
        public int LikeId { get; set; }
       
        public int PostId { get; set; }
        public UserDto LikeUserDto { get; set; }=new UserDto();

    }
}
