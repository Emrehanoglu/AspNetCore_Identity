using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers;

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

    [HttpGet]
    [Route("RoleCreate")]
    public IActionResult RoleCreate()
    {
        return View();
    }

    [HttpPost]
    [Route("RoleCreate")]
    public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
    {
        var result = await _roleManager.CreateAsync(new AppRole { Name = request.Name});

        if (!result.Succeeded)
        {
            foreach (IdentityError item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
                return View();
            }
        }

        return RedirectToAction("Index","Roles", new {area="Admin"});
    }
}
