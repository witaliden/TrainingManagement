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
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user => user.OwnsOne(
                u => u.UserPasswordOptions)
            );

            modelBuilder.Entity<UserTraining>()
                .HasKey(ut => new
                {
                    ut.UserId,
                    ut.TrainingId
                });

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTrainings)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.Training)
                .WithMany(t => t.UserTrainings)
                .HasForeignKey(ut => ut.TrainingId);

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.ActionType).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

        }
    }

}
