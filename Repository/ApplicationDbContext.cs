using Microsoft.EntityFrameworkCore;

namespace TrainingManagement.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<TrainingManagement.Models.Training> Trainings { get; set; }
        public DbSet<TrainingManagement.Models.User> Users { get; set; }
        public DbSet<TrainingManagement.Models.UserTraining> UserTrainings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrainingManagement.Models.UserTraining>()
                .HasKey(ut => new { ut.UserId, ut.TrainingId });
            modelBuilder.Entity<TrainingManagement.Models.UserTraining>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTrainings)
                .HasForeignKey(ut => ut.UserId);
            modelBuilder.Entity<TrainingManagement.Models.UserTraining>()
                .HasOne(ut => ut.Training)
                .WithMany(t => t.UserTrainings)
                .HasForeignKey(ut => ut.TrainingId);
        }
    }
}
