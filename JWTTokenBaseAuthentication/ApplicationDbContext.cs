using JWTTokenBaseAuthentication.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTTokenBaseAuthentication
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
    }

    public class ApplicationDBContext : ApplicationDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}

namespace JwtTokenBaseAuthentication
{
    public class ApplicationDBContext : JWTTokenBaseAuthentication.ApplicationDbContext
    {
        public ApplicationDBContext(DbContextOptions<JWTTokenBaseAuthentication.ApplicationDbContext> options) : base(options) { }
    }
}

