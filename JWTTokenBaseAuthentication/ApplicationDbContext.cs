using JWTTokenBaseAuthentication.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTTokenBaseAuthentication
{
    public class ApplicationDbContext :DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
