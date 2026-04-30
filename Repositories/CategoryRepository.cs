using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Models;
using TaskManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Repositories{
         public class CategoryRepository : ICategoryRepository{ 
            private readonly AppDbContext _context;

         public CategoryRepository(AppDbContext context){
            _context = context;
         }

         public async Task<List<Category>> GetAllAsync(){
            return await _context.Categories.ToListAsync();
         }

         public async Task<Category?> GetByIdAsync(int id){
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
         }
         public async Task AddAsync(Category category){
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

         }
         public async Task UpdateAsync(Category category){
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

         }
         public async Task DeleteAsync(int id){
            var category= await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (category is null) return;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
                     
                     }


}
}