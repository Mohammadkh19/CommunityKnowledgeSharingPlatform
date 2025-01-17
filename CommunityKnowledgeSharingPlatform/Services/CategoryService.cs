using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Categories Category)> AddCategoryAsync(CategoryDTO categoryDto)
        {
            try
            {
                var category = new Categories
                {
                    CategoryName = categoryDto.CategoryName
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return (true, "Category added successfully", category);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while adding the category: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, Categories Category)> UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryDto.CategoryId);
                if (category == null)
                {
                    return (false, "Category not found", null);
                }

                // Update the category's properties
                category.CategoryName = categoryDto.CategoryName;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return (true, "Category updated successfully", category);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while updating the category: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return (false, $"Category with ID {id} not found");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return (true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting the category: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, List<Categories> Categories)> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                return (true, "Categories fetched successfully", categories);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while fetching categories: {ex.Message}", null);
            }
        }
    }
}
