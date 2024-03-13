using Congratulator.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Congratulator.Repositories
{
    public class BirthdayRepository
    {
        private readonly IMongoCollection<BirthdayModel> _collection;

        public BirthdayRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BirthdayModel>("birthdayCollection");
        }

        public IEnumerable<BirthdayModelDTO> GetAllBirthdays()
        {
            return _collection.Find(Builders<BirthdayModel>.Filter.Empty).ToList().Select(birthday => new BirthdayModelDTO
            {
                Id = birthday.Id.ToString(),
                FullName = birthday.FullName,
                BirthdayDate = birthday.BirthdayDate,
                ProfileImageUri = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", birthday.Id.ToString() + ".jpg")) ? birthday.Id.ToString() + ".jpg" : "default.jpg"
            });
        }

        public BirthdayModel GetByName(string userName)
        {
            return _collection.Find(b => b.FullName == userName).FirstOrDefault();
        }
        public BirthdayModel GetById(string id)
        {
            var objectId = ObjectId.TryParse(id, out ObjectId objId) ? objId : ObjectId.Empty;
            return _collection.Find(b => b.Id == objectId).FirstOrDefault();
        }

        public void Add(BirthdayModel birthday)
        {
            _collection.InsertOne(birthday);
        }

        public void Update(string id, BirthdayModel birthday)
        {
            _collection.ReplaceOne(b => b.Id == GetById(id).Id, birthday);
        }

        public void Delete(BirthdayModel birthdayModel)
        {
            _collection.DeleteOne(b => b == birthdayModel);
        }

        public IEnumerable<BirthdayModelDTO> GetUpcomingBirthdays()
        {
            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);
            var birthdays = _collection.Find(_ => true).ToList();

            var upcomingBirthdays = birthdays.Where(birthday =>
            {
                var thisYearBirthday = new DateTime(today.Year, birthday.BirthdayDate.Month, birthday.BirthdayDate.Day);
                var nextYearBirthday = new DateTime(today.Year + 1, birthday.BirthdayDate.Month, birthday.BirthdayDate.Day);
                return (thisYearBirthday >= today && thisYearBirthday < nextWeek) ||
                       (today.Month == 12 && nextWeek.Month == 1 && nextYearBirthday < nextWeek);
            })
            .Select(birthday => new BirthdayModelDTO
            {
                Id = birthday.Id.ToString(),
                FullName = birthday.FullName,
                BirthdayDate = birthday.BirthdayDate,
                ProfileImageUri = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", birthday.Id.ToString() + ".jpg")) ? birthday.Id.ToString() + ".jpg" : "default.jpg"
            });

            return upcomingBirthdays;
        }


        public IEnumerable<BirthdayModel> GetTodayBirthdays()
        {
            var today = DateTime.Today;
            var allBirthdays = _collection.Find(Builders<BirthdayModel>.Filter.Empty).ToList();
            var todayBirthdays = allBirthdays.Where(birthday =>birthday.BirthdayDate.Month == today.Month && birthday.BirthdayDate.Day == today.Day).ToList();
            return todayBirthdays;
        }


        public IEnumerable<BirthdayModel> getBirthdaysToNotificate(int daysToNotificate)
        {
            var targetDate = DateTime.Today.AddDays(daysToNotificate);
            var allBirthdays = _collection.Find(_ => true).ToList();
            var relevantBirthdays = allBirthdays.Where(birthday =>
            {
                var adjustedBirthday = new DateTime(DateTime.Today.Year, birthday.BirthdayDate.Month, birthday.BirthdayDate.Day);
                if (adjustedBirthday < DateTime.Today)
                {
                    adjustedBirthday = adjustedBirthday.AddYears(1);
                }
                return adjustedBirthday == targetDate;
            });

            return relevantBirthdays;
        }

    }
}
