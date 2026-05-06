using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

[Authorize]
public class AdminController: Controller
{
    private readonly AppDbContext _context;
    private readonly UsersDbContext _usersContext;
    private readonly AuditService _auditService;


    public AdminController(AppDbContext context, UsersDbContext usersContext, AuditService auditService)
    {
        _context = context;
        _usersContext = usersContext;
        _auditService = auditService;
    }
    public async Task<IActionResult> Index()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
        {
            return Forbid();
        }

        var model = new AdminDashboardViewModel
        {
            TotalAffirmations = await _context.Affirmations.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            RecentAffirmations = await _context.Affirmations
                .OrderByDescending(a => a.Id)
                .Take(5)
                .Select(a => a.Text)
                .ToListAsync()
        };

        return View(model);
    }
    public async Task<IActionResult> Permissions()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();


        var users = await _usersContext.Users.ToListAsync();
        var permissions = await _context.Permissions.ToListAsync();
        var userPermissions = await _context.UserPermissions.ToListAsync();


        var model = users.Select(u => new UserPermissionViewModel
        {
            UserId = u.Id,
            Username = u.Username,
            Permissions = permissions.Select( p => new PermissionItemViewModel
            {
                PermissionId = p.Id,
                Name = p.Name,
                IsAssigned = userPermissions.Any(up => up.UserId == u.Id && up.PermissionId == p.Id)
            }).ToList()
        }).ToList();

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> TogglePermission(int userId, int permissionId)
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.PermissionId == permissionId);
        
        if(existing == null)
        {
            _context.UserPermissions.Add(new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }
        else
        {
            _context.UserPermissions.Remove(existing);
        }

        await _context.SaveChangesAsync();


        return RedirectToAction("Permissions");
    }
    public async Task<IActionResult> ManagePermissions()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var permissions = await _context.Permissions.ToListAsync();

        var usage = await _context.UserPermissions
            .GroupBy(up => up.PermissionId)
            .Select( g => new
            {
                PermissionId = g.Key,
                Count = g.Count()
            }).ToListAsync();

        var model = permissions.Select(p => new PermissionUsageViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            UserCount = usage.FirstOrDefault(u => u.PermissionId == p.Id)?.Count ?? 0
        }).ToList();

        return View(model);
    }
    public IActionResult CreatePermission()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreatePermission(Permission model)
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var username = User.Identity.Name;

        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        _context.Permissions.Add(model);
        await _context.SaveChangesAsync();

        await _auditService.Log(
            userId,
            username,
            "Create",
            "Permission",
            model.Id,
            $"Created permission: {model.Name}"
        );

        return RedirectToAction("ManagePermissions");
    }

    public async Task<IActionResult> EditPermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        return View(permission);
    }
    [HttpPost]
    public async Task<IActionResult> EditPermission(Permission model)
    {
        _context.Permissions.Update(model);
        await _context.SaveChangesAsync();
        
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var username = User.Identity.Name;

        await _auditService.Log(
            userId,
            username,
            "Edit",
            "Permission",
            model.Id,
            $"Updated permission: {model.Name}"
        );
        return RedirectToAction("ManagePermissions");
    }
    public async Task<IActionResult> DeletePermission(int id)
    {

        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var permission = await _context.Permissions.FindAsync(id);
        if(permission != null)
        {

            var related = await _context.UserPermissions
                .Where(up => up.PermissionId == id)
                .ToListAsync();

            _context.UserPermissions.RemoveRange(related);

            _context.Permissions.Remove(permission);

            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirst("UserId").Value);
            var username = User.Identity.Name;

            await _auditService.Log(
                userId,
                username,
                "Delete",
                "Permission",
                id,
                $"Deleted permission: {permission.Name}"
            );
        }
        return RedirectToAction("ManagePermissions");
    }
}