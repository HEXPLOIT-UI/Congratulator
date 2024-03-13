using Congratulator.Models;
using Congratulator.Repositories;
using MongoDB.Bson;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Congratulator.Services
{
    public class BirthdayService : IBirthdayService
    {
        private readonly BirthdayRepository _myRepository; 

        public BirthdayService(BirthdayRepository birthdayRepository)
        {
            _myRepository = birthdayRepository;
        }

        public ResultDTO СreateUser(string fullName, string birthdayDate, IFormFile? file)
        {
            var model = new BirthdayModel {Id = ObjectId.GenerateNewId(), BirthdayDate = DateTimeOffset.Parse(birthdayDate), FullName = fullName };
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    return new ResultDTO() { result = false, comment = "Допускаются только файлы изображений в формате .jpg или .jpeg" };
                }
                if (file.ContentType.ToLowerInvariant() != "image/jpeg")
                {
                    return new ResultDTO() { result = false, comment = "Допускаются только файлы изображений в формате .jpg или .jpeg" };
                }
                using var image = Image.Load(file.OpenReadStream());
                (int width, int height) newSize = image.Width == image.Height ? (512, 512)
                                            : image.Height > image.Width ? (512, 768)
                                            : (768, 512);
                image.Mutate(x => x.Resize(newSize.width, newSize.height));
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.Id.ToString()+".jpg");
                image.Save(path);
            }
            _myRepository.Add(model);
            return new ResultDTO() { result = true, comment = "Пользователь создан"};
        }

        public ResultDTO DeleteUser(string id)
        {
            var model = _myRepository.GetById(id);
            if (model == null)
            {
                return new ResultDTO() { result = false, comment = "Такого пользователя не существует" };
            }
            else 
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.Id.ToString() + ".jpg");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                _myRepository.Delete(model);
                return new ResultDTO() { result = true, comment = "Пользователь удален" };
            }
        }

        public IEnumerable<BirthdayModelDTO> GetAllBirthdays()
        {
            return _myRepository.GetAllBirthdays();
        }

        public BirthdayModel GetByName(string userName)
        {
            return _myRepository.GetByName(userName);
        }


        public ResultDTO Update(string id, string? name = null, string? birthday = null, string? profileImageUri = null)
        {
            int count = 0;
            string result = "Были обновлены: ";
            var model = _myRepository.GetById(id);
            if (model == null)
            {
                return new ResultDTO() { result = false, comment = "Пользователь не найден" };
            }
            if (!string.IsNullOrEmpty(name) && model.FullName != name)
            {
                model.FullName = name;
                count++;
                result += "ФИО, ";
            }
            if (!string.IsNullOrEmpty(birthday) && model.BirthdayDate.ToString("yyyy-MM-dd") != DateTimeOffset.Parse(birthday).ToString("yyyy-MM-dd"))
            {
                Console.WriteLine(model.BirthdayDate.ToString("yyyy-MM-dd") + " " + DateTimeOffset.Parse(birthday).ToString("yyyy-MM-dd"));

                model.BirthdayDate = DateTimeOffset.Parse(birthday);
                count++;
                result += "Дата рождения, ";
            }

            if (count == 0)
            {
                return new ResultDTO() { result = false, comment = "Не было передано ни одного нового параметра" };
            }
            else {

                _myRepository.Update(id, model);
                return new ResultDTO() { result = true, comment = result};
            }
        }


        public IEnumerable<BirthdayModelDTO> GetUpcomingBirthdays()
        {
            return _myRepository.GetUpcomingBirthdays();
        }

        public BirthdayModel GetById(string id)
        {
            return _myRepository.GetById(id);
        }

        public IEnumerable<BirthdayModel> GetTodayBirthdays()
        {
            return _myRepository.GetTodayBirthdays();
        }

        IEnumerable<BirthdayModel> IBirthdayService.GetBirthdaysToNotificate(int daysToNotificate)
        {
            return _myRepository.getBirthdaysToNotificate(daysToNotificate);
        }
    }
}
