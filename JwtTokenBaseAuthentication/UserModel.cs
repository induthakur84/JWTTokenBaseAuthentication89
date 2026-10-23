namespace JwtTokenBaseAuthentication
{
    public class UserModel
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }    
    }



    //public class Userdto
    //{
      
    //    public string Name { get; set; }
      
    //}
}


//Dto -- we can for the security reason because we don't exposing our domain tables.


