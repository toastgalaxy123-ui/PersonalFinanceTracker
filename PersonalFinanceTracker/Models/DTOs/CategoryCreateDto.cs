using System.ComponentModel.DataAnnotations;
namespace PersonalFinanceTracker.Models.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(50)] // Good practice to limit string length
        public string Name { get; set; } // e.g., "Groceries", "Salary"

        // Optional: Defaults to true (an expense category) if not provided
        public bool IsExpense { get; set; } = true;
    }
}
