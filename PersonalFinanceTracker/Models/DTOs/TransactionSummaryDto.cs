namespace PersonalFinanceTracker.Models.DTOs
{
    public class TransactionSummaryDto
    {
        public string GroupName { get; set; } // e.g., "Groceries", "March 2025"
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal NetFlow { get; set; } // Total Income - Total Expense
    }
}
