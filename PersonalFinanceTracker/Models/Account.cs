using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations.Schema;
namespace PersonalFinanceTracker.Models

{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } //e.g., "My Checking Account"
        public string Type { get; set; } // e.g., Checking, Savings, Credit Card

        //Initial balance when the account is created
        public decimal InitialBalance { get; set; }

        //Relationships

        //Foreign Key to the user 
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        //Navigation property for transactions associated with this account
        public ICollection<Transaction> Transactions { get; set; }

    }
}
