using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityKnowledgeSharingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addCategory")]
        public async Task<IActionResult> AddCategory(CategoryDTO category)
        {
            var (success, message, categoryData) = await _categoryService.AddCategoryAsync(category);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, category = categoryData });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateCategory")]
        public async Task<IActionResult> UpdateCategory(CategoryDTO category)
        {
            var (success, message, categoryData) = await _categoryService.UpdateCategoryAsync(category);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message, category = categoryData });
        }



        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var (success, message, categories) = await _categoryService.GetAllCategoriesAsync();

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, categories });
        }
    }
}
