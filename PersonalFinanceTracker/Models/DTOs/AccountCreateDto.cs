using System.ComponentModel.DataAnnotations;
namespace PersonalFinanceTracker.Models.DTOs
{
    public class AccountCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Main Checking", "Emergency Fund"

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } // e.g., "Checking", "Savings", "Credit Card"

        // The initial amount of money in the account.
        [Required]
        [Range(0, 1000000000, ErrorMessage = "Balance must be a non-negative number.")]
        public decimal InitialBalance { get; set; }
    }
}
