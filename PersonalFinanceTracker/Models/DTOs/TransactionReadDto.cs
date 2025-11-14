namespace PersonalFinanceTracker.Models.DTOs
{
    public class TransactionReadDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Notes { get; set; }

        // --- Related Data for Display ---

        public int AccountId { get; set; }
        public string AccountName { get; set; } // The name of the account

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // The name of the category
    }
}
