namespace PersonalFinanceTracker.Models.DTOs
{
    public class AccountReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public decimal InitialBalance { get; set; }

        // IMPORTANT: This field will be calculated in the controller 
        // by summing all associated transactions. It is not stored in the DB.
        public decimal CurrentBalance { get; set; }
    }
}
