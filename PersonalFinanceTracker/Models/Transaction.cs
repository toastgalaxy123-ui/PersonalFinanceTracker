using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date  { get; set; }
        public decimal Amount { get; set; }//Positive for income, negative for expense
        public string Description { get; set; }
        public string Notes { get; set; }

        // Relationships 

        // Foreign Key to Account(One transaction belongs to one account)
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        // Foreign Key to Category(One transaction belongs to one category)
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }


    }
}
