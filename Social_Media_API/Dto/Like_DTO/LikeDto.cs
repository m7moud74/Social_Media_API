using Social_Media_API.Dto.Account_DTO;
using Social_Media_API.Model;
using System.Text.Json.Serialization;

namespace Social_Media_API.Dto.Like_DTO
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
