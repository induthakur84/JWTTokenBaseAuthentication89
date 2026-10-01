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


    }
}
