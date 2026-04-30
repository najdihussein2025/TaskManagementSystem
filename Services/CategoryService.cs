using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Models;
using TaskManagementSystem.DTOs.Category;

namespace TaskManagementSystem.Services{
    public class CategoryService : ICategoryService{

        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public async Task CreateAsync(CreateCategoryDto dto)
        {
            var existingCategories = await _categoryRepository.GetAllAsync();
            var duplicateExists = existingCategories.Any(c =>
                string.Equals(c.Name, dto.Name, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = string.IsNullOrWhiteSpace(dto.Color) ? "#3b82f6" : dto.Color
            };
            await _categoryRepository.AddAsync(category);
        }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color
            }).ToList();

        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null) return null;
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,

                Color = category.Color
            };
        }
        public async Task UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category is null) throw new KeyNotFoundException("Category not found");

            var existingCategories = await _categoryRepository.GetAllAsync();
            var duplicateExists = existingCategories.Any(c =>
                c.Id != dto.Id &&
                string.Equals(c.Name, dto.Name, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.Color = string.IsNullOrWhiteSpace(dto.Color) ? "#3b82f6" : dto.Color;
            await _categoryRepository.UpdateAsync(category);
        }
        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }


    }
}