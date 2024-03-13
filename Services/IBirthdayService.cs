using Congratulator.Models;

namespace Congratulator.Services
{
    public interface IBirthdayService
    {
        BirthdayModel GetById(string id);
        ResultDTO СreateUser (string fullName, string birthdayDate, IFormFile? file);
        ResultDTO DeleteUser (string id);
        IEnumerable<BirthdayModelDTO> GetAllBirthdays();
        BirthdayModel GetByName(string userName);
        ResultDTO Update(string id, string? name = null, string? birthday = null, string? profileImageUri = null);
        IEnumerable<BirthdayModelDTO> GetUpcomingBirthdays();
        IEnumerable<BirthdayModel> GetTodayBirthdays();
        IEnumerable<BirthdayModel> GetBirthdaysToNotificate(int daysToNotificate);

    }
}
