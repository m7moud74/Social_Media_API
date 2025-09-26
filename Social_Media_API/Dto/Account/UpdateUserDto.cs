namespace Social_Media_API.Dto.Account
{
    public class UpdateUserDto
    {
        public string ?Name { get; set; } = string.Empty;
        public string ?Email { get; set; } = string.Empty;
        public string ?NewPassword { get; set; } = string.Empty;
        public string ?CurrPassword { get; set; } =string.Empty;
        public IFormFile? ImageUrl { get; set; }
    }
}
