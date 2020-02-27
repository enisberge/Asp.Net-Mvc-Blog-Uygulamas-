using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class DatabaseContext : DbContext
    {
        public DbSet<EvernoteUser> EvernoteUsers { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Liked> Likes { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new MyInitializer());
            this.Configuration.LazyLoadingEnabled = true;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //FluentAPI annatation tarafının alternatifi
            //ilişkili tabloların silinebilmesi için
            modelBuilder.Entity<Note>()
                .HasMany(i => i.Comments)
                .WithRequired(c => c.Note)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Note>()
                .HasMany(i => i.Likes)
                .WithRequired(c => c.Note)
                .WillCascadeOnDelete(true);


        }
    }
}
