using MailFlow.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MailFlow.Context
{
    public class MailContext : IdentityDbContext<AppUser>
    {
        public MailContext(DbContextOptions<MailContext> options) : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }
    }
}
