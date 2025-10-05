using Backend_AIHost.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<VPS> VPSes { get; set; }
        public DbSet<AIModel> AIModels { get; set; }
        public DbSet<UserContainer> UserContainers { get; set; }
        public DbSet<AIChat> AIChats { get; set; }
        public DbSet<UserImage> UserImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
            // Relacje i ograniczenia
            { /*
            builder.Entity<VPS>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Deployment>()
                .HasOne<VPS>()
                .WithMany()
                .HasForeignKey(d => d.VPSId)
                .OnDelete(DeleteBehavior.Cascade);
                */
            }
        }
    }
}
