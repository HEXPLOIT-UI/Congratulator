using Congratulator.Models;
using Congratulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthdayController : ControllerBase
    {
        
        private readonly IBirthdayService m_birthdayService;

        public BirthdayController(IBirthdayService birthdayService)
        {
            m_birthdayService = birthdayService;
        }

        [HttpPost("CreateUser")]
        public ActionResult CreateUser([FromForm] string fullName, [FromForm] string birthdayDate, [FromForm] IFormFile? file = null)
        {
            return Ok(m_birthdayService.СreateUser(fullName, birthdayDate, file));
        }

        [HttpGet("GetUpcomingBirthdays")]
        public ActionResult<IEnumerable<BirthdayModel>> GetUpcomingBirthdays()
        {
            return Ok(m_birthdayService.GetUpcomingBirthdays());
        }

        [HttpGet("GetAllBirthdays")]
        public ActionResult<IEnumerable<BirthdayModel>> GetAllBirthdays()
        {
            return Ok(m_birthdayService.GetAllBirthdays());
        }

        [HttpPost("DeleteUser")]
        public ActionResult DeleteUser([FromForm]  string id)
        {
            return Ok(m_birthdayService.DeleteUser(id));
        }

        [HttpPost("UpdateUser")]
        public ActionResult UpdateUser(string id, string? fullName = null, string? birthdayDate = null, string? profileImageUri = null)
        {
            return Ok(m_birthdayService.Update(id, fullName, birthdayDate, profileImageUri));
        }

        [HttpGet("TodayBirthdays")]
        public ActionResult<IEnumerable<BirthdayModel>> getTodayBirthdays()
        {
            return Ok(m_birthdayService.GetTodayBirthdays());
        }

        [HttpGet("BirthdaysToNotificate")]
        public ActionResult<IEnumerable<BirthdayModel>> getBirthdaysToNotificate(int days)
        {
            return Ok(m_birthdayService.GetBirthdaysToNotificate(days));
        }
    }
}
