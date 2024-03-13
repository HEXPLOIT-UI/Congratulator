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
            return _collection.Find(b => true).ToList().Select(birthday => new BirthdayModelDTO
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

            var filter = Builders<BirthdayModel>.Filter.Gte(x => x.BirthdayDate, today) &
                         Builders<BirthdayModel>.Filter.Lt(x => x.BirthdayDate, nextWeek);
            var birthdays = _collection.Find(filter).ToList();
            var birthdayViewModels = birthdays.Select(birthday => new BirthdayModelDTO
            {
                Id = birthday.Id.ToString(),
                FullName = birthday.FullName,
                BirthdayDate = birthday.BirthdayDate,
                ProfileImageUri = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", birthday.Id.ToString() + ".jpg")) ? birthday.Id.ToString() + ".jpg" : "default.jpg"
            });
            return birthdayViewModels;
        }

        public IEnumerable<BirthdayModel> GetTodayBirthdays()
        {
            var today = DateTime.Today;
            var filter = Builders<BirthdayModel>.Filter.Eq(x => x.BirthdayDate, today);
            var todayBirthdays = _collection.Find(filter).ToList();
            return todayBirthdays;
        }

        public IEnumerable<BirthdayModel> getBirthdaysToNotificate(int daysToNotificate)
        {
            var targetDate = DateTime.Today.AddDays(daysToNotificate);
            var filter = Builders<BirthdayModel>.Filter.Eq(x => x.BirthdayDate, targetDate);
            var docs = _collection.Find(filter).ToList();
            return docs;
        }
    }
}
