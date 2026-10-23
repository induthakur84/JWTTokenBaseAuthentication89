using JWTTokenBaseAuthentication.DTO;

namespace JWTTokenBaseAuthentication.Services.IServices
{

    //[AddScoped]
    //[AddSingleton]
    //[AddTransient]
    public interface IUserInterface
    {
        Task<UserResponseDto> Register(UserRegisterDto userRegisterDto);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
