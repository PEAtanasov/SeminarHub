using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Security.Claims;

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
