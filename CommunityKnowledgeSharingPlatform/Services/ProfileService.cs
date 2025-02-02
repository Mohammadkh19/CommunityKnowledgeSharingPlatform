using System;
using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class ProfileService : IProfileService
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<(bool Success, string Message)> AddProfileInfoAsync(ProfileDTO profileDto, string userId)
        {
            try
            {
                // Check if the user already has a profile
                var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (existingProfile != null)
                {
                    return (false, "Profile already exists for this user.");
                }

                // Handle profile picture upload
                string picturePath = null;

                if (profileDto.ProfilePicture != null && profileDto.ProfilePicture.Length > 0)
                {
                    picturePath = await SaveProfilePictureAsync(profileDto.ProfilePicture);
                }

                // Map ProfileDTO to Profiles entity
                var profile = new Profiles
                {
                    FirstName = profileDto.FirstName,
                    LastName = profileDto.LastName,
                    Bio = profileDto.Bio,
                    Address = profileDto.Address,
                    ProfilePicturePath = picturePath,
                    UserId = userId
                };

                // Add the profile
                await _context.Profiles.AddAsync(profile);
                await _context.SaveChangesAsync();

                return (true, "Profile added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<(bool Success, string Message)> UpdateProfileInfoAsync(ProfileDTO profileDto, string userId)
        {
            try
            {
                // Find the profile
                var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (existingProfile == null)
                {
                    return (false, "Profile not found.");
                }

                // Update the profile
                existingProfile.FirstName = profileDto.FirstName;
                existingProfile.LastName = profileDto.LastName;
                existingProfile.Bio = profileDto.Bio;
                existingProfile.Address = profileDto.Address;

                // Handle profile picture upload
                if (profileDto.ProfilePicture != null && profileDto.ProfilePicture.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingProfile.ProfilePicturePath))
                    {
                        DeleteProfilePicture(existingProfile.ProfilePicturePath); // Clean up old picture
                    }
                    existingProfile.ProfilePicturePath = await SaveProfilePictureAsync(profileDto.ProfilePicture);
                }


                _context.Profiles.Update(existingProfile);
                await _context.SaveChangesAsync();

                return (true, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<Profiles> GetProfileByUserAsync(string username)
        {
            // Fetch the profile from the database based on the user 
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.User.UserName == username);

            if (profile == null)
            {
                return null; // No profile found for the given user
            }

            // Map the profile entity to ProfileDTO
            return new Profiles
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Bio = profile.Bio,
                Address = profile.Address,
                ProfilePicturePath = profile.ProfilePicturePath
            };
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile profilePicture)
        {
            // Generate a unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(profilePicture.FileName);

            // Define the upload path (e.g., wwwroot/uploads/profile-pictures)
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads/profile-pictures");

            // Ensure the upload directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Combine the file name with the upload path
            var filePath = Path.Combine(uploadPath, fileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // Return the relative path for the database (e.g., /uploads/profile-pictures/filename.jpg)
            return $"/uploads/profile-pictures/{fileName}";
        }


        private void DeleteProfilePicture(string picturePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, picturePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}
