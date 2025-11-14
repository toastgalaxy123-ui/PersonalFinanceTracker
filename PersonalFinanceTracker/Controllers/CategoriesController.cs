using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models.DTOs;
using PersonalFinanceTracker.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ApplicationDbContext = PersonalFinanceTracker.Models.ApplicationDbContext;
using Microsoft.AspNet.Identity;
using Microsoft.Identity.Client;

namespace PersonalFinanceTracker.Controllers
{
    [Authorize] // 🔒 Protects all methods in this controller
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Helper method to get the ID of the currently authenticated user
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        //GET : api/Categories
        //Retrieves all categories for the authenticated user

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories()
        {
            var userId = GetCurrentUserId();
            // Fetch categories owned by the current user and map them to the Read DTO
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsExpense = c.IsExpense
                })
                .ToListAsync();

            return Ok(categories);
        }

        //GET: api/Categories/5
        //Retrieves a specific category by ID for the authenticated user

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategory(int id)
        {
            var userId = GetCurrentUserId();

            var category = await _context.Categories
                .Where(c => c.Id == id && c.UserId == userId) // Crucial security check
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                IsExpense = category.IsExpense
            });
        }

        //POST: api/Categories
        //Creates a new category for the authenticated user

        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateCategory(CategoryCreateDto categoryDto)
        {
            var userId = GetCurrentUserId();
            // 1. Map DTO to Entity and assign ownership
            var category = new Category
            {
                Name = categoryDto.Name,
                IsExpense = categoryDto.IsExpense,
                UserId = userId // Assign the category to the current user
            };

            // 2. Save to database
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // 3. Return 201 Created response with the new resource details
            var readDto = new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                IsExpense = category.IsExpense
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, readDto);
        }

        //PUT: api/Categories/5
        //Updates an existing category for the authenticated user

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryCreateDto categoryDto)
        {
            var userId = GetCurrentUserId();

            var category = await _context.Categories
                .Where(c => c.Id == id && c.UserId == userId) // Crucial security check
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            // Update the entity properties
            category.Name = categoryDto.Name;
            category.IsExpense = categoryDto.IsExpense;

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CategoryExists(id))
            {
                return NotFound();
            }

            return NoContent(); // 204 Success, no content returned
        }

        //DELETE: api/Categories/5
        //Deletes a category for the authenticated user

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetCurrentUserId();

            var category = await _context.Categories
                .Where(c => c.Id == id && c.UserId == userId) // Crucial security check
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);

            // The EF Core Restrict behavior (which you configured) will prevent 
            // deletion if transactions still use this category. If that happens, 
            // SaveChangesAsync will throw an exception, which you would ideally handle
            // to return a friendly error message (e.g., "Cannot delete category with active transactions").

            await _context.SaveChangesAsync();

            return NoContent(); // 204 Success, no content returned
        }


        // Helper method to check existence
        private bool CategoryExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Categories.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}


        
