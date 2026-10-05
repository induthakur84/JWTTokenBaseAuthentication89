using JWTTokenBaseAuthentication.DTO;
using JWTTokenBaseAuthentication.Models;
using JWTTokenBaseAuthentication.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace JWTTokenBaseAuthentication.Services
{
    public class ProductService : IProductInterface
    {

        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProductResponseDto> Create(ProductCreateDto productCreateDto)
        {
            var product = new Product
            {
                Name = productCreateDto.Name,
                Description = productCreateDto.Description,
            };



            _context.Products.Add(product);

            await _context.SaveChangesAsync();


            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
            };
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IList<ProductResponseDto>> GetAll()
        {
            return
                await _context.Products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description

                }).ToListAsync();
        }

        public async Task<ProductResponseDto> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return null;
            }
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
            };
        }

        public async Task<ProductResponseDto> Update(ProductUpdateDto productUpdateDto)
        {
           var exitingProduct= await _context.Products.FindAsync(productUpdateDto.Id);
            if (exitingProduct == null) 
            {
                return null;

            }


            exitingProduct.Name = productUpdateDto.Name;
             exitingProduct.Description = productUpdateDto.Description;
            await _context.SaveChangesAsync();
            return new ProductResponseDto
            {
                Id = exitingProduct.Id,
                Name = exitingProduct.Name,
                Description = exitingProduct.Description,
            };
        }
    }
}
