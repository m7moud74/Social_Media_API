using Social_Media_API.Model;

namespace Social_Media_API.Dto.Like_DTO
{
    public class CreateLikeDto
    {
       
        public int PostId {  get; set; }
        public Reaction Reaction { get; set; }
        public DateTime CrateAt { get; set; }

    }
}
