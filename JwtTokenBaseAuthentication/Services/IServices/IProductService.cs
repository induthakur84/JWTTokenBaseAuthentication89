using JWTTokenBaseAuthentication.DTO;

namespace JWTTokenBaseAuthentication.Services.IServices
{
    //interface  where we will define the methods for product service
    //we are able to achieve the abstration with the interface and we are able to achieve the loose coupling with the help of interface because we are not directly using the implementation of the method we are using the interface and we are able to change the implementation of the method without changing the code of the method because we are using the interface and we are able to achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    //We are able to implement dependency injection with the help of interface because we are not directly using the implementation of the method we are using the interface and we are able to change the implementation of the method without changing the code of the method because we are using the interface and we are able to achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    // in c#, we are able to achieve the multiple inheritance with the help of interface because we can implement multiple interfaces in a class and we can achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    public interface IProductService
    {
        Task<ProductResponseDto> Create(ProductCreateDto productCreateDto);
        Task<List<ProductResponseDto>> GetAll();
        Task<ProductResponseDto> Update(ProductUpdateDto productUpdateDto);
        Task<string> Delete(int id);
        Task<ProductResponseDto> GetById(int id);
    }
}
