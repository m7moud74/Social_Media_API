using Social_Media_API.Dto.Acoount;
using Social_Media_API.Model;
using System.Text.Json.Serialization;

namespace Social_Media_API.Dto.Like
{
    public class LikeDto
    {
        public DateTime CreateAt { get; set; }
        public int PostId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Reaction Reaction { get; set; }
        public UserDto LikeUserDto { get; set; }=new UserDto();

    }
}
