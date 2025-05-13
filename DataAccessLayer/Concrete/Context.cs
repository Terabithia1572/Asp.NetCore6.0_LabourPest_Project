using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  optionsBuilder.UseSqlServer("server=.;database=LabourPestDB;integrated security=true;");
            optionsBuilder.UseSqlServer("server=77.245.159.112\\MSSQLSERVER2022;database=LabourPest;user=LabourPest;password=Yunus6565*");

        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogComment> BlogsComments { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<HomePage> HomePages { get; set; }
        public DbSet<Image> Images { get; set; }      
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<WhoWeUs> WhoWeUs { get; set; }
        public DbSet<Writer> Writers { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogTag>()
                .HasKey(bt => new { bt.BlogID, bt.TagID });

            modelBuilder.Entity<BlogTag>()
                .HasOne(bt => bt.Blogs)
                .WithMany(b => b.BlogTags)
                .HasForeignKey(bt => bt.BlogID);

            modelBuilder.Entity<BlogTag>()
                .HasOne(bt => bt.Tags)
                .WithMany(t => t.BlogTags)
                .HasForeignKey(bt => bt.TagID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
