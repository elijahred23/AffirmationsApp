using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

[Authorize]
public class AdminController: Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
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
}