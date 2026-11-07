using AutoMapper;
using Buyora.Areas.Admin.ViewModels.User;
using BuyoraModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private const int pageSize = 10;
        private string CurrentUserId => _userManager.GetUserId(User)!;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
        {
            if(page<1)
            {
                page = 1;
            }
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var models = new List<DisplayUsersViewModel>();
            foreach (var user in users)
            {
                var model = _mapper.Map<DisplayUsersViewModel>(user);
                var roles = await _userManager.GetRolesAsync(user);
                model.Roles = roles.ToList();
                model.IsLocked = await _userManager.IsLockedOutAsync(user);
                models.Add(model);
            }

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;

            return View(models);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if(user.Id == CurrentUserId)
            {
                TempData["ErrorMessage"] = "You cannot lock your own account.";
                return RedirectToAction(nameof(Index));
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                TempData["SuccessMessage"] = "User unlocked successfully.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                await _userManager.UpdateSecurityStampAsync(user);       
                TempData["SuccessMessage"] = "User locked successfully.";
            }
            ViewBag.UserName = user.UserName;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            if(user.Id == CurrentUserId)
            {
                TempData["ErrorMessage"] = "You cannot manage your own roles.";
                return RedirectToAction(nameof(Index));
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var models = new List<ManageUserRolesViewModel>();
            foreach (var role in roles)
            {
                var model = _mapper.Map<ManageUserRolesViewModel>(role);
                bool isInRole = await _userManager.IsInRoleAsync(user, role.Name);
                model.IsSelected = isInRole;
                models.Add(model);
            }
            return View(models);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string userId,List<ManageUserRolesViewModel> models)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id == CurrentUserId)
            {
                TempData["ErrorMessage"] = "You cannot manage your own roles.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var model in models)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
                if (model.IsSelected && !isInRole)
                {
                     await _userManager.AddToRoleAsync(user, model.RoleName);
                    TempData["SuccessMessage"] = $"Role '{model.RoleName}' added to user successfully.";

                }
                else if(!model.IsSelected && isInRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    TempData["SuccessMessage"] = $"Role '{model.RoleName}' removed from user successfully.";
                }
                await _userManager.UpdateSecurityStampAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
