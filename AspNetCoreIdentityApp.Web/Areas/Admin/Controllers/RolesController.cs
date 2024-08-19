using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("Admin/Roles")]
public class RolesController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.Select(x => new RoleViewModel
        {
            Id = x.Id,
            Name = x.Name!
        }).ToListAsync();

        return View(roles);
    }

    [Authorize(Roles = "admin,role-action")]
    [HttpGet]
    [Route("RoleCreate")]
    public IActionResult RoleCreate()
    {
        return View();
    }

    [Authorize(Roles = "admin,role-action")]
    [HttpPost]
    [Route("RoleCreate")]
    public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
    {
        var result = await _roleManager.CreateAsync(new AppRole { Name = request.Name });

        if (!result.Succeeded)
        {
            foreach (IdentityError item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
                return View();
            }
        }

        return RedirectToAction("Index", "Roles", new { area = "Admin" });
    }

    [Authorize(Roles = "admin,role-action")]
    [HttpGet]
    [Route("RoleUpdate/{id}")]
    public async Task<IActionResult> RoleUpdate(string id)
    {
        var roleToUpdate = await _roleManager.FindByIdAsync(id);

        if (roleToUpdate == null)
        {
            throw new Exception("Güncellenecek rol bulunamamıştır.");
        }

        return View(new RoleUpdateViewModel
        {
            Id = roleToUpdate!.Id!,
            Name = roleToUpdate!.Name!
        });
    }

    [Authorize(Roles = "admin,role-action")]
    [HttpPost]
    [Route("RoleUpdate/{id}")]
    public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
    {
        var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

        if (roleToUpdate == null)
        {
            throw new Exception("Güncellenecek rol bulunamamıştır.");
        }

        roleToUpdate.Name = request.Name;

        await _roleManager.UpdateAsync(roleToUpdate);

        TempData["SuccessMessage"] = "Rol bilgisi güncellenmiştir.";

        return RedirectToAction("Index", "Roles", new { area = "Admin" });
    }

    [Authorize(Roles = "admin,role-action")]
    [Route("RoleDelete/{id}")]
    public async Task<IActionResult> RoleDelete(string id)
    {
        var roleToDelete = await _roleManager.FindByIdAsync(id);

        if (roleToDelete == null)
        {
            throw new Exception("Güncellenecek rol bulunamamıştır.");
        }

        var result = await _roleManager.DeleteAsync(roleToDelete);

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).First());
        }

        TempData["SuccessMessage"] = "Rol bilgisi silinmiştir.";

        return RedirectToAction("Index", "Roles", new { area = "Admin" });
    }

    [HttpGet]
    [Route("AssignRoleToUser/{id}")]
    public async Task<IActionResult> AssignRoleToUser(string id)
    {
        //kullanıcı id bilgisini ViewBag ile Post bloguna tasıyorum
        ViewBag.userId = id;

        var currentUser = (await _userManager.FindByIdAsync(id))!;

        var roles = await _roleManager.Roles.ToListAsync();

        var roleViewModelList = new List<AssignRoleToUserViewModel>();

        var userRoles = await _userManager.GetRolesAsync(currentUser!);

        foreach (var role in roles)
        {
            var assignRoleToUserViewModel = new AssignRoleToUserViewModel
            {
                Id = role.Id,
                Name = role.Name!
            };

            if (userRoles.Contains(role.Name))
            {
                assignRoleToUserViewModel.Exist = true;
            }

            roleViewModelList.Add(assignRoleToUserViewModel);
        }

        return View(roleViewModelList);
    }

    [HttpPost]
    [Route("AssignRoleToUser/{id}")]
    public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
    {
        var currentUser = (await _userManager.FindByIdAsync(userId))!;

        foreach(AssignRoleToUserViewModel role in requestList)
        {
            //checkBox işaretlenmişse kullanıcıya rol atandı
            if (role.Exist)
            {
                await _userManager.AddToRoleAsync(currentUser,role.Name);
            }

            //checkBox işaretlenmemişse kullanıcıdan rol silindi
            if (!role.Exist)
            {
                await _userManager.RemoveFromRoleAsync(currentUser, role.Name);
            }
        }

        return RedirectToAction("UserList","Home",new { area="Admin"});
    }
}
