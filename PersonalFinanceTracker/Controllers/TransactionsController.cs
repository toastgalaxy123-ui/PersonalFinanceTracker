using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Helper method to get the ID of the currently authenticated user
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        //Helper method for security validation
        private async Task<IActionResult?> ValidateForeignKeys(string userId, int accountId, int categoryId)
        {
            // 1. Check if the Account belongs to the current user
            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Id == accountId && a.UserId == userId);

            if (!accountExists)
            {
                return NotFound(new { error = "Account not found or access denied." });
            }

            // 2. Check if the Category belongs to the current user
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == categoryId && c.UserId == userId);

            if (!categoryExists)
            {
                return NotFound(new { error = "Category not found or access denied." });
            }

            return null; // Validation passed
        }

        //GET : api/Transactions
        //Retrieves all transactions for the authenticated user

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionReadDto>>> GetTransactions()
        {
            var userId = GetCurrentUserId();
            // Fetch transactions, including Account and Category data for DTO mapping
            var transactions = await _context.Transactions
                .Where(t => t.Account.UserId == userId) // Filter by the account owner
                .Include(t => t.Account)
                .Include(t => t.Category)
                .OrderByDescending(t => t.Date) // Display newest first
                .ToListAsync();

            // Map the Entity to the Read DTO
            var dtos = transactions.Select(t => new TransactionReadDto
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Description = t.Description,
                Notes = t.Notes,
                AccountId = t.AccountId,
                AccountName = t.Account.Name,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            }).ToList();

            return Ok(dtos);
        }

        //GET: api/Transactions/5
        //Retrieves a specific transaction by ID for the authenticated user

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionReadDto>> GetTransaction(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.Account.UserId == userId) // Crucial security check
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync();
            if (transaction == null)
            {
                return NotFound();
            }
            // Map to Read DTO
            return Ok(new TransactionReadDto
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Notes = transaction.Notes,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account.Name,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name
            });

        }

        //POST: api/Transactions
        //Creates a new transaction for the authenticated user

        [HttpPost]
        public async Task<IActionResult> PostTransaction(TransactionCreateUpdateDto transactionDto)
        {
            var userId = GetCurrentUserId();
            // 1. Validate Foreign Keys (crucial security check)
            var validationResult = await ValidateForeignKeys(userId, transactionDto.AccountId, transactionDto.CategoryId);
            if (validationResult != null)
            {
                // Fix: Return the IActionResult as an ActionResult<TransactionReadDto>
                return validationResult;
            }

            // 2. Map DTO to Entity
            var transaction = new Transaction
            {
                Date = transactionDto.Date,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description,
                Notes = transactionDto.Notes,
                AccountId = transactionDto.AccountId,
                CategoryId = transactionDto.CategoryId
            };

            // 3. Save to database
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // 4. Fetch the created transaction with navigation properties for the Read DTO
            var createdTransaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            // Map the created entity to the Read DTO and return 201
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, new TransactionReadDto
            {
                Id = createdTransaction.Id,
                Date = createdTransaction.Date,
                Amount = createdTransaction.Amount,
                Description = createdTransaction.Description,
                Notes = createdTransaction.Notes,
                AccountId = createdTransaction.AccountId,
                AccountName = createdTransaction.Account.Name,
                CategoryId = createdTransaction.CategoryId,
                CategoryName = createdTransaction.Category.Name
            });
        }

        //PUT : api/Transactions/5
        //Updates an existing transaction for the authenticated user

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, TransactionCreateUpdateDto transactionDto)
        {
            var userId = GetCurrentUserId();
            // 1. Find existing transaction and ensure ownership
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.Account.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            // 2. Validate new foreign keys (Account and Category must still belong to the user)
            var validationResult = await ValidateForeignKeys(userId, transactionDto.AccountId, transactionDto.CategoryId);
            if (validationResult != null)
            {
                return validationResult;
            }

            // 3. Update properties
            transaction.Date = transactionDto.Date;
            transaction.Amount = transactionDto.Amount;
            transaction.Description = transactionDto.Description;
            transaction.Notes = transactionDto.Notes;
            transaction.AccountId = transactionDto.AccountId;
            transaction.CategoryId = transactionDto.CategoryId;

            // 4. Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id)) return NotFound();
                throw; // Re-throw if it's a different concurrency error
            }

            return NoContent(); // 204 Success

        }

        //DELETE: api/Transactions/5
        //Deletes a specific transaction by ID for the authenticated user

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.Account.UserId == userId) // Crucial security check
                .FirstOrDefaultAsync();
            if (transaction == null)
            {
                return NotFound();
            }
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent(); // 204 Success
        }

        // Helper method to check existence
        private bool TransactionExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Transactions.Any(t => t.Id == id && t.Account.UserId == userId);
        }
    }
}
