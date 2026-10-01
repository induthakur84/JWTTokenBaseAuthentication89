namespace JWTTokenBaseAuthentication.DTO
{
    public class UserRegisterDto
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}
