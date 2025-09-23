namespace Social_Media_API.Dto
{
    public class CreatePostDto
    {
       
        public string? Content { get; set; }
        
        public IFormFile? ImageUrl { get; set; }
        

    }
}
