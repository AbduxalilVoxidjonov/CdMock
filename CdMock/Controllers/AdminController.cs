namespace CdMock.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 20;

            var usersQuery = _userManager.Users.AsQueryable();

            // Qidiruv
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.Email.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            // Umumiy soni
            var totalUsers = await usersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Pagination
            var users = await usersQuery
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Har bir user uchun rolni olish
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "User";
            }

            ViewBag.UserRoles = userRoles;
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User topilmadi!";
                return RedirectToAction("Index");
            }

            // Eski rollarni olib tashlash
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Yangi rolni qo'shish
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Success"] = $"{user.UserName} uchun rol {newRole}ga o'zgartirildi!";
            return RedirectToAction("Index");
        }
    }
}
