using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models.DTOs;
using PersonalFinanceTracker.Models;
using System.Security.Claims;
using ApplicationDbContext = PersonalFinanceTracker.Models.ApplicationDbContext;

namespace PersonalFinanceTracker.Controllers
{
    [Authorize] // 🔒 Protects all methods in this controller
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Helper method to get the ID of the currently authenticated user
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        //GET : api/Accounts
        //Retrieves all accounts for the authenticated user

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountReadDto>>> GetAccounts()
        {
            var userId = GetCurrentUserId();

            // Fetch accounts and include the related Transactions data for calculation
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Include(a => a.Transactions) // Include transactions to calculate balance
                .ToListAsync();

            // Map and calculate the balance for each account
            var accountDtos = accounts.Select(a => new AccountReadDto
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type,
                InitialBalance = a.InitialBalance,
                // 💸 Calculation: Sum of InitialBalance and all transaction amounts
                CurrentBalance = a.InitialBalance + a.Transactions.Sum(t => t.Amount)
            }).ToList();

            return Ok(accountDtos);
        }

        //GET: api/Accounts/5
        //Retrieves a specific account by ID for the authenticated user

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountReadDto>> GetAccount(int id)
        {
            var userId = GetCurrentUserId();

            // Fetch account including related transactions, ensuring ownership
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return NotFound();
            }

            // Map and calculate balance
            var readDto = new AccountReadDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                InitialBalance = account.InitialBalance,
                CurrentBalance = account.InitialBalance + account.Transactions.Sum(t => t.Amount)
            };

            return Ok(readDto);
        }

        //POST: api/Accounts
        //Creates a new account for the authenticated user

        [HttpPost]
        public async Task<ActionResult<AccountReadDto>> PostAccount(AccountCreateDto createDto)
        {
            var userId = GetCurrentUserId();
            var account = new Account
            {
                Name = createDto.Name,
                Type = createDto.Type,
                InitialBalance = createDto.InitialBalance,
                UserId = userId
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            var readDto = new AccountReadDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                InitialBalance = account.InitialBalance,
                CurrentBalance = account.InitialBalance // No transactions yet
            };
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, readDto);
        }

        //PUT : api/Accounts/5
        //Updates an existing account for the authenticated user

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, AccountCreateDto updateDto)
        {
            var userId = GetCurrentUserId();
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                return NotFound();
            }
            account.Name = updateDto.Name;
            account.Type = updateDto.Type;
            account.InitialBalance = updateDto.InitialBalance;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AccountExists(id))
            {
                return NotFound();
            }

            return NoContent(); // 204 Success, no content returned
        }

        //DELETE: api/Accounts/5
        //Deletes an account for the authenticated user

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = GetCurrentUserId();
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                return NotFound();
            }
            _context.Accounts.Remove(account);
            // Deletion will be RESTRICTED by the database if there are still 
            // Transactions associated with this Account (per your OnModelCreating fix).
            // This is good; it prevents corrupted financial history.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle the restriction error gracefully
                return BadRequest(new { error = "Cannot delete account because it still contains transactions." });
            }

            return NoContent(); // 204 Success, no content returned
        }

        // Add this private helper method to the AccountsController class
        private bool AccountExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Accounts.Any(a => a.Id == id && a.UserId == userId);
        }
    }
}
