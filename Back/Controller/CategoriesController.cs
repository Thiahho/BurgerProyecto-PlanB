using Back.Data;
using Back.Dtos;
using Back.Models;
using Back.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Back.Controller
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CatalogCacheService _catalogCache;

        public CategoriesController(AppDbContext context, CatalogCacheService catalogCache)
        {
            _context = context;
            _catalogCache = catalogCache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            return await _context.Categories
                .OrderBy(c => c.SortOrder)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, SortOrder = c.SortOrder })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CreateUpdateCategoryDto categoryDto)
        {
            var category = new Category { Name = categoryDto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            _catalogCache.Invalidate();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, new CategoryDto { Id = category.Id, Name = category.Name, SortOrder = category.SortOrder });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CreateUpdateCategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();
            _catalogCache.Invalidate();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                return BadRequest("Cannot delete category with associated products.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _catalogCache.Invalidate();

            return NoContent();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderCategories([FromBody] List<ReorderCategoryDto> reorderList)
        {
            try
            {
                foreach (var item in reorderList)
                {
                    var category = await _context.Categories.FindAsync(item.CategoryId);
                    if (category != null)
                    {
                        category.SortOrder = item.SortOrder;
                    }
                }

                await _context.SaveChangesAsync();
                _catalogCache.Invalidate();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al reordenar categorías", details = ex.Message });
            }
        }
    }

}
