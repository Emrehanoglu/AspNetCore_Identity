using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public static class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            //bakalım bu rol tabloda var mı
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");

            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasBasicRole)
            {
                //AspNetRoles tablosunda yok o zaman ekleyelim
                await roleManager.CreateAsync(new AppRole { Name = "BasicRole"});

                //rolü ekledik ama id bilgisi elimizde değil, alalım...
                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                await AddReadPermission(basicRole, roleManager);
            }

            if (!hasAdvancedRole)
            {
                //AspNetRoles tablosunda yok o zaman ekleyelim
                await roleManager.CreateAsync(new AppRole { Name = "AdvancedRole" });

                //rolü ekledik ama id bilgisi elimizde değil, alalım...
                var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

                await AddReadPermission(basicRole, roleManager);
                await AddUpdateAndCreatePermission(basicRole, roleManager);
            }
            if (!hasAdminRole)
            {
                //AspNetRoles tablosunda yok o zaman ekleyelim
                await roleManager.CreateAsync(new AppRole { Name = "AdminRole" });

                //rolü ekledik ama id bilgisi elimizde değil, alalım...
                var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;

                await AddReadPermission(basicRole, roleManager);
                await AddUpdateAndCreatePermission(basicRole, roleManager);
                await AddDeletePermission(basicRole, roleManager);
            }
        }

        public static async Task AddReadPermission(AppRole appRole, RoleManager<AppRole> roleManager)
        {
            //AspNetRoleClaims tablosuna BasicRole ile ilişkili kayıtlar atıldı.
            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Stock.Read));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Order.Read));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Catalog.Read));
        }

        public static async Task AddUpdateAndCreatePermission(AppRole appRole, RoleManager<AppRole> roleManager)
        {
            //AspNetRoleClaims tablosuna BasicRole ile ilişkili kayıtlar atıldı.
            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Stock.Create));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Order.Create));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Catalog.Create));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Stock.Update));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Order.Update));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Catalog.Update));
        }

        public static async Task AddDeletePermission(AppRole appRole, RoleManager<AppRole> roleManager)
        {
            //AspNetRoleClaims tablosuna BasicRole ile ilişkili kayıtlar atıldı.
            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Stock.Delete));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Order.Delete));

            await roleManager.AddClaimAsync(appRole,
                new Claim("Permission", Core.Permissions.Permission.Catalog.Delete));
        }
    }
}
