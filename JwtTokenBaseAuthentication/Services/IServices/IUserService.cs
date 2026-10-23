using JWTTokenBaseAuthentication.DTO;

namespace JWTTokenBaseAuthentication.Services.IServices
{





    // in interface we just declear the method not giving the implementation of the method

    /// <summary>
    /// we are able to achieve the abstration with the interface and we are able to achieve the loose coupling with the help of interface because we are not directly using the implementation of the method we are using the interface and we are able to change the implementation of the method without changing the code of the method because we are using the interface and we are able to achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    /// </summary>
    /// 




    ///
    /// we can able to implement dependency injection with the help of interface because we are not directly using the implementation of the method we are using the interface and we are able to change the implementation of the method without changing the code of the method because we are using the interface and we are able to achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    ///



    // in c#, we are able to achieve the multiple inheritance with the help of interface because we can implement multiple interfaces in a class and we can achieve the polymorphism with the help of interface because we can have multiple implementation of the same method
    public interface IUserService
    {
        Task<UserResponseDto> Register(UserRegisterDto userRegisterDto);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
