namespace JwtTokenBaseAuthentication.DTO
{
    public class UserRegisterDto
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        //securely store the password in the database.
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
