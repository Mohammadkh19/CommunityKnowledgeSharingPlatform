using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface ICategoryService
    {
        Task<(bool Success, string Message, Categories Category)> AddCategoryAsync(CategoryDTO category);
        Task<(bool Success, string Message, Categories Category)> UpdateCategoryAsync(CategoryDTO category);
        Task<(bool Success, string Message)> DeleteCategoryAsync(int id);
        Task<(bool Success, string Message, List<Categories> Categories)> GetAllCategoriesAsync();
    }
}
