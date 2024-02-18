using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

using SeminarHub.Data;
using SeminarHub.Data.Models;
using SeminarHub.Models;

using static Common.ValidationConstants.SeminarConstants;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext data;
        public SeminarController(SeminarHubDbContext context)
        {
            this.data = context;
        }
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await data.Seminars
                .AsNoTracking()
                .Select(s=> new SeminarViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DateAndTimeFormat),
                    Category = s.Category.Name,
                    Organizer = s.Organizer.UserName
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await GetCategoriesAsync();

            var model = new SeminarFormModel()
            {
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormModel model)
        {
            DateTime parsedDateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                DateAndTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDateAndTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid Date. Format must be: {DateAndTimeFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesAsync();
                return View(model);
            }

            var entity = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = parsedDateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId()
            };

            await data.Seminars.AddAsync(entity);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = GetUserId();

            ICollection<SeminarViewModel> currentUserSeminars = new List<SeminarViewModel>();

            var currentUserCreatedSeminars = await data.Seminars
                .Where(s => s.OrganizerId == userId)
                .AsNoTracking()
                .Select(s => new SeminarViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DateAndTimeFormat),
                    Organizer = s.Organizer.UserName,
                    Category = s.Category.Name
                })
                .ToListAsync();

            currentUserSeminars = currentUserCreatedSeminars;

            var currentUserJoinedSeminars = await data.SeminarsParticipants
                .Where(sp => sp.ParticipantId == userId)
                .AsNoTracking()
                .Select(s => new SeminarViewModel()
                {
                    Id = s.SeminarId,
                    Topic = s.Seminar.Topic,
                    Lecturer = s.Seminar.Lecturer,
                    DateAndTime = s.Seminar.DateAndTime.ToString(DateAndTimeFormat),
                    Organizer = s.Seminar.Organizer.UserName,
                    Category = s.Seminar.Category.Name
                })
                .ToListAsync();

            foreach (var seminar in currentUserJoinedSeminars)
            {
                currentUserSeminars.Add(seminar);
            }

            return View(currentUserSeminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var seminarToJoin = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s=>s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminarToJoin == null)
            {
                return BadRequest();
            }

            if (seminarToJoin.SeminarsParticipants.Any(sp=>sp.ParticipantId==GetUserId()))
            {
                return RedirectToAction(nameof(All));
            }

            seminarToJoin.SeminarsParticipants.Add(new SeminarParticipant()
            {
                SeminarId=seminarToJoin.Id,
                ParticipantId=GetUserId(),
            });

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var seminarToLeave = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminarToLeave == null)
            {
                return BadRequest();
            }

            if (!seminarToLeave.SeminarsParticipants.Any(sp => sp.ParticipantId == GetUserId()))
            {
                return BadRequest();
            }

            var entityToLeave = seminarToLeave.SeminarsParticipants.FirstOrDefault(ep => ep.ParticipantId == GetUserId());

            if (entityToLeave != null)
            {
                data.SeminarsParticipants.Remove(entityToLeave);
                await data.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }


        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<IList<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await data.Categories
                .AsNoTracking()
                .Select(e => new CategoryViewModel()
                {
                    Id = e.Id,
                    Name = e.Name
                })
                .ToListAsync();

            return categories;
        }
    }
}
