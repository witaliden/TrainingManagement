using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Models;

namespace TrainingManagement.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Training> Trainings { get; set; }
        public DbSet<UserTraining> UserTrainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTraining>()
                .HasKey(ut => new { ut.UserId, ut.TrainingId });

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTrainings)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.Training)
                .WithMany(t => t.UserTrainings)
                .HasForeignKey(ut => ut.TrainingId);
        }
    }

}
