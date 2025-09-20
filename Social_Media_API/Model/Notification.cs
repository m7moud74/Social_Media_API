namespace Social_Media_API.Model
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Type { get; set; }    
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
