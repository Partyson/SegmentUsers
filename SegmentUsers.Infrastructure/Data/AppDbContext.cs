using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SegmentUsers.Domain.Entities;
using SegmentUsers.Infrastructure.ModelsConfiguration;

namespace SegmentUsers.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<Admin, IdentityRole<Guid>, Guid>
    {
        public DbSet<Segment> Segments { get; set; }
        
        public DbSet<VkUser> VkUsers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SegmentConfiguration());
        }
    }
}