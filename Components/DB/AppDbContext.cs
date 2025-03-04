using ChatRoom.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Components.DB
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.File)
                .WithMany(f => f.Messages)
                .HasForeignKey(m => m.FileId)
                .OnDelete(DeleteBehavior.SetNull); // Если файл удален, сообщение останется
        }
    }
}
