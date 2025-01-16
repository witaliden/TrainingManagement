using TrainingManagement.Models;

namespace TrainingManagement.Repository
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if the database already contains trainings
            if (context.Trainings.Any())
            {
                return; // Database has been seeded
            }

            var trainings = new Training[]
            {
            new() { Title = "Introduction to ASP.NET Core", Description = "Learn the basics of ASP.NET Core", Date = DateTime.Now.AddDays(7) },
            new() { Title = "Advanced Entity Framework Core", Description = "Deep dive into EF Core", Date = DateTime.Now.AddDays(14) },
            new() { Title = "Web API Development", Description = "Build RESTful APIs with ASP.NET Core", Date = DateTime.Now.AddDays(21) }
            };

            context.Trainings.AddRange(trainings);
            context.SaveChanges();
        }
    }

}
