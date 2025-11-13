using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PersonalFinanceTracker.Models
{
    public class User : IdentityUser
    {
        //PersonalFinanceTracker specific properties
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //Navigation properties for the user
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Category> Categories { get; set; }


    }


}
