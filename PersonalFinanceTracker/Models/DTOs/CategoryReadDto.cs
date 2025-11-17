namespace PersonalFinanceTracker.Models.DTOs
{
    public class CategoryReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsExpense { get; set; }
    }
}
