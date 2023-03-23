using Microsoft.EntityFrameworkCore;
using NoteApplication.Models;

namespace NoteApplication.Data
{
    public class NoteAPIDbContext : DbContext
    {
        public NoteAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ForgetPassword> ForgetPasswords { get; set; }
    }
}
