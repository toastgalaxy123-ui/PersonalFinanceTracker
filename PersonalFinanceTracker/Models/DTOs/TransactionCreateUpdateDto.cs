using System.ComponentModel.DataAnnotations;
namespace PersonalFinanceTracker.Models.DTOs
{
    public class TransactionCreateUpdateDto
    {
        [Required]
        public DateTime Date { get; set; }

        // Amount can be positive (income/deposit) or negative (expense/withdrawal).
        [Required]
        [Range(0.01, 1000000000, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } // e.g., "Starbucks coffee", "Monthly Salary"

        [MaxLength(500)]
        public string Notes { get; set; }

        // --- Foreign Key DTOs ---

        // Required: Which account the money came from or went to.
        [Required]
        public int AccountId { get; set; }

        // Required: How to classify the transaction (e.g., Groceries, Rent).
        [Required]
        public int CategoryId { get; set; }
    }
}
