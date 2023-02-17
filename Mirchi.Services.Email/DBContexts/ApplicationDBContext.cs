using Microsoft.EntityFrameworkCore;
using Mirchi.Services.Email.Models;

namespace Mirchi.Services.Email.DBContexts
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<EmailLog> EmailLogs { get; set; }        
    }
}
