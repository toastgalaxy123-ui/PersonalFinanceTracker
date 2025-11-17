using System.ComponentModel.DataAnnotations;
namespace PersonalFinanceTracker.Models.DTOs
{
    public class AuthResultDto
    {
        // The JWT token returned upon successful login/registration
        public string Token { get; set; }

        // Flag indicating if the operation succeeded
        public bool IsSuccess { get; set; }

        // List of errors if the operation failed (e.g., bad password, email already exists)
        public List<string> Errors { get; set; } = new List<string>();

        // Optional: User-friendly name to display in the frontend
        public string DisplayName { get; set; }
    }
}
