using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

[Authorize]
public class AdminController: Controller
{
    private readonly AppDbContext _context;
    private readonly UsersDbContext _usersContext;

    public AdminController(AppDbContext context, UsersDbContext usersContext)
    {
        _context = context;
        _usersContext = usersContext;
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
}