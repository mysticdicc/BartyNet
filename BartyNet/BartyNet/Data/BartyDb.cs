using Microsoft.EntityFrameworkCore;
using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;

namespace BartyNet.Data
{
    public partial class BartyDb : DbContext
    {
        public BartyDb() { }
        public BartyDb(DbContextOptions<BartyDb> options)
            : base(options)
        {
        }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ThumbnailImage> Thumbnails { get; set; }
        public virtual DbSet<WebsitePost> WebsitePosts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("images");

                entity.Property(e => e.Id);
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.LocalPath)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(e => e.RemotePath)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(e => e.FileExtension)
                    .HasMaxLength(25)
                    .IsRequired();
            });

            modelBuilder.Entity<ThumbnailImage>(entity =>
            {
                entity.ToTable("thumbnail_images");

                entity.Property(e => e.Id);
                entity.Property(e => e.ThumbnailName)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.LocalPath)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(e => e.RemotePath)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(e => e.FileExtension)
                    .HasMaxLength(25)
                    .IsRequired();
            });

            modelBuilder.Entity<WebsitePost>(entity =>
            {
                entity.ToTable("website_posts");

                entity.Property(e => e.Id);
                entity.Property(e => e.LastSubmit)
                    .IsRequired();
                entity.Property(e => e.Body)
                    .IsUnicode(false)
                    .IsRequired();
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();
                entity.Property(e => e.PostTypeIs)
                    .IsRequired()
                    .HasMaxLength(50);


                entity.HasMany(e => e.Images)
                    .WithOne(e => e.WebsitePost)
                    .HasForeignKey(e => e.PostId)
                    .IsRequired(false);

                entity.HasOne(e => e.ThumbnailImage)
                    .WithOne(e => e.ThumbnailPost)
                    .HasForeignKey<ThumbnailImage>(e => e.PostId)
                    .IsRequired(false);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
