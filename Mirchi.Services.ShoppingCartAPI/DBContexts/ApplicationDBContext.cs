using Microsoft.EntityFrameworkCore;
using Mirchi.Services.ShoppingCartAPI.Models;

namespace Mirchi.Services.ShoppingCartAPI.DBContexts
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
    }
}
