using System.ComponentModel.DataAnnotations;

namespace Social_Media_API.Dto.Acoount
{
    public class DeleteAccountDTO
    {
        [Required(ErrorMessage = "Password is required to delete account")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        public string Password { get; set; } = string.Empty;


        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}
