using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Grocery Shopping"

        public bool IsExpense { get; set; } // true for expense, false for income

        //Relationships
        //Foreign Key to the user
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; } // Navigation property to the user

        //Navigation to the transactions in this category
        public ICollection<Transaction> Transactions { get; set; }
    }
}
