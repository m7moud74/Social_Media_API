namespace Social_Media_API.Dto.Post_DTO
{
    public class CreatePostDto
    {
       
        public string? Content { get; set; }
        
        public IFormFile? ImageUrl { get; set; }
        

    }
}
