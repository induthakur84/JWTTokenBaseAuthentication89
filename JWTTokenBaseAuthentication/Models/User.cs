namespace JWTTokenBaseAuthentication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password {  get; set; }

        public string ConfirmPassword { get; set; }
        public string Role {  get; set; }   

    }
}
