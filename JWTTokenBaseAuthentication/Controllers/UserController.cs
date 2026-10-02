using JWTTokenBaseAuthentication.DTO;
using JWTTokenBaseAuthentication.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace JWTTokenBaseAuthentication.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserInterface _userInterface;

        public UserController(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        [HttpPost("Register")]
       
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var result = await _userInterface.Register(userRegisterDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                var result = await _userInterface.Login(loginRequestDto);
                return Ok(new
                {
                    message = "Login Successful",
                    token = result.Token,
                    user = result.User

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Login Failed",
                    error = ex.Message

                });
            }
        }
    }
}
