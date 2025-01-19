using Microsoft.AspNetCore.Identity;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Configuration
{
    public class DataInitializerService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public DataInitializerService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task InitializeData()
        {
            if (!_context.Users.Any())
            {
                await CreateUsers();
                await CreateTrainings();
            }
        }

        private async Task CreateUsers()
        {
            var admin = new User { UserName = "admin", Email = "admin@poczta.pl", IsAdmin = true, Name = "Jestem", Lastname = "Główny" };
            await _userManager.CreateAsync(admin, "admin");

            for (int i = 1; i <= 10; i++)
            {
                var employee = new User { UserName = $"employee{i}", Email = $"employee{i}@poczta.pl", IsAdmin = false, Name = "Pracuję-Tu", Lastname = $"{i} lat" };
                await _userManager.CreateAsync(employee, "pracownik");
            }
        }

        private async Task CreateTrainings()
        {
            for (int i = 1; i <= 20; i++)
            {
                var training = new Training
                {
                    Title = $"Training {i}",
                    Description = $"Description for Training {i}",
                    Link = $"https://example.com/training{i}",
                    DueDate = DateTime.Now.AddDays(30)
                };
                _context.Trainings.Add(training);
            }
            await _context.SaveChangesAsync();
        }
    }

}
