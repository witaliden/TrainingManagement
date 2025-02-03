using TrainingManagement.Models;
using TrainingManagement.Repository;
using System.Threading.Tasks;
using TrainingManagement.Models.enums;


    

namespace TrainingManagement.ActivityLogging
{
    public class UserActivityLogger : IUserActivityLogger
    {
        private readonly ApplicationDbContext _context;

        public UserActivityLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userName, UserActionType actionType, bool isSuccess, string description)
        {
            var log = new UserActivityLog
            {
                UserName = userName,
                Timestamp = DateTime.UtcNow,
                ActionType = actionType.ToString(),
                Description = description,
                IsSuccess = isSuccess
            };

            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}