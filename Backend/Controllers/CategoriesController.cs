using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuProyecto.Data;
using TuProyecto.DTOs;
using TuProyecto.Models;

namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET /api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = $"Categoría con ID {id} no encontrada" });

            return Ok(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count
            });
        }

        // POST /api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryCreateDto dto)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return BadRequest(new { message = "Ya existe una categoría con ese nombre" });

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0
            });
        }

        // PUT /api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound(new { message = $"Categoría con ID {id} no encontrada" });

            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);

            if (exists)
                return BadRequest(new { message = "Ya existe otra categoría con ese nombre" });

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = $"Categoría con ID {id} no encontrada" });

            if (category.Products.Any())
                return BadRequest(new { message = $"No se puede eliminar, tiene {category.Products.Count} producto(s) asociado(s)" });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}