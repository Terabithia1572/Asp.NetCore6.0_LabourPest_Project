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
            optionsBuilder.UseSqlServer("server=.;database=LabourPestDB;integrated security=true;");
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
       
        
    }
}
