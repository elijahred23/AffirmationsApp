using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class CategoriesController : Controller
{
    
    private readonly AppDbContext _context;
    private readonly AuditService _auditService;


    public CategoriesController(AppDbContext context, AuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }


    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var username = User.Identity.Name;

        category.CreatedAt = DateTime.UtcNow;

        _context.Add(category);
        await _context.SaveChangesAsync();
    
        await _auditService.Log(
            userId,
            username,
            "Create",
            "Category",
            category.Id,
            $"Created category: {category.Name}"
        );

        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if(category == null) return NotFound();

        return View(category);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        _context.Update(category);

        await _context.SaveChangesAsync();

        await _auditService.Log(
            int.Parse(User.FindFirst("UserId").Value),
            User.Identity.Name,
            "Edit",
            "Category",
            category.Id,
            $"Updated category: {category.Name}"
        );

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if(category == null) return NotFound();

        return View(category);
   }

   [HttpPost, ActionName("Delete")]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if(category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            await _auditService.Log(
                int.Parse(User.FindFirst("UserId").Value),
                User.Identity.Name,
                "Delete",
                "Category",
                category.Id,
                $"Deleted category: {category.Name}"
            );
        }

        return RedirectToAction(nameof(Index));
    }
}