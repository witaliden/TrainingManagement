using TrainingManagement.Models.enums;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.ActivityLogging
{
    public interface IUserActivityLogger
    {
        Task LogActivityAsync(string userName, UserActionType actionType, bool isSuccess, string description);
    }
}