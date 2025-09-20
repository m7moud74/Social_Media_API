using Microsoft.AspNetCore.Identity;
using Social_Media_API.Dto;
using System.ComponentModel.DataAnnotations.Schema;
namespace Social_Media_API.Model
{
    public class User:IdentityUser
    {
        public string ProfilePictureUrl { get; set; }
        [NotMapped]
        public IFormFile? formFile { get; set; }
       public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }
        public virtual ICollection<Friendship> FriendRequestsSent { get; set; }   
        public virtual ICollection<Friendship> FriendRequestsReceived { get; set; }

        public static implicit operator User(UserDto v)
        {
            throw new NotImplementedException();
        }
    }
}
