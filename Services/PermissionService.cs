using Microsoft.EntityFrameworkCore;

public class PermissionService
{
    private readonly AppDbContext _context;
    public PermissionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermission(int userId, string permissionName)
    {
        return await _context.UserPermissions
        .Include(up => up.Permission)
        .AnyAsync(up =>
            up.UserId == userId &&
            up.Permission.Name == permissionName
        );
    }
}