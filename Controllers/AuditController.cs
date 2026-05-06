using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuditController: Controller
{
    private readonly AppDbContext _context;

    public AuditController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AuditLogs()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(100)
            .ToListAsync(); 

        return View(logs);
    }


}