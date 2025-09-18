using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Media_API.Dto
{
    public class UserDto
    {
        public string UserId { get; set; } =string.Empty;
        public string UserName { get; set; } = string.Empty;    
        public string ProfilePictureUrl { get; set; } = string.Empty;
        
    }
}
