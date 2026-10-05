using JWTTokenBaseAuthentication.DTO;

namespace JWTTokenBaseAuthentication.Services.IServices
{
    public interface IProductInterface
    {

        Task<ProductResponseDto>Create(ProductCreateDto productCreateDto);
        Task<ProductResponseDto> Update(ProductUpdateDto productUpdateDto);

        Task<IList<ProductResponseDto>> GetAll();

        Task<ProductResponseDto> GetById(int id);
        Task<bool> Delete(int id);  
    }
}
