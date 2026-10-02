using JWTTokenBaseAuthentication.DTO;
using JWTTokenBaseAuthentication.Models;
using JWTTokenBaseAuthentication.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTTokenBaseAuthentication.Services
{
    public class UserService : IUserInterface
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

     
        public async Task<UserResponseDto> Register(UserRegisterDto userRegisterDto)
        {


            // here we are using mannual mapping


            //we can map userRequestDto dto user entity

            var user = new User
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                Role = userRegisterDto.Role,
                Username= userRegisterDto.Username,
                // here we can convert to has password before saving the database
                Password = BCrypt.Net.BCrypt.HashPassword( userRegisterDto.Password),
                ConfirmPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.ConfirmPassword),
            };
            //Add user  to database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new UserResponseDto
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                Role = userRegisterDto.Role,
                Username = userRegisterDto.Username,
            };

        }




        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // We Can Find that user with help of username

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == loginRequestDto.Username);


            // if User not found that we can throw the error

            if (user == null)
            {
                throw new Exception("User Not Fount");
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password);

            // if the passwrod is not valid that throw the error

            if (!isValid)
            {
                throw new Exception("Invalid Password");
            }

            var token = GenerateToken(user);
            return new LoginResponseDto
            {
                Token= token,
                User= new UserResponseDto
                {
                  Id=user.Id,
                  FirstName = user.FirstName,
                  LastName = user.LastName,
                  Username=user.Username,   
                  Role = user.Role,
                }
            };

        }


        //private method
        #region private method

        private string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");



            // here we convert the secret key to byte array
          //  [7,90]

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));


            //key + algorithm
            var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            //Payload
            // here we can save the user information or claims inside the token



            var claim = new[]
            {

                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };


            //Signature


            //

            var token = new JwtSecurityToken(

                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                claims: claim,
                signingCredentials: creds
                );

            //Convert token object to string and reture


            return new JwtSecurityTokenHandler().WriteToken(token); 


        }
        #endregion

    }
}
