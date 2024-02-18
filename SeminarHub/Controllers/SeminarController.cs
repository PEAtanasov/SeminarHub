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
