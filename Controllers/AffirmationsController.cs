using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AffirmationsController : Controller
{
	private readonly AppDbContext _context;

	public AffirmationsController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);

		var affirmations = await _context.Affirmations
			.Include(a => a.Category)
			.ToListAsync();


		return View(affirmations);
    }
	public async Task<IActionResult> Create()
	{
		ViewBag.Categories = _context.Categories.ToList();

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(Affirmation affirmation)
	{
		_context.Add(affirmation);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null) return NotFound();

		var affirmation = await _context.Affirmations
			.Include(a => a.Category)
			.FirstOrDefaultAsync(m => m.Id == id);

		if(affirmation == null ) return NotFound();

		return View(affirmation);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var affirmation = await _context.Affirmations.FindAsync(id);

		if (affirmation != null)
		{
			_context.Affirmations.Remove(affirmation);
			await _context.SaveChangesAsync();
		}

		return RedirectToAction(nameof(Index));
	}
	public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

		var affirmation = await _context.Affirmations.FindAsync(id);

		if (affirmation == null) return NotFound();

		ViewBag.Categories = _context.Categories.ToList();

		return View(affirmation);
    }

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, Affirmation affirmation)
    {
        if (id != affirmation.Id) return NotFound();

		try
		{
			_context.Update(affirmation);
			await _context.SaveChangesAsync();
		} catch (DbUpdateConcurrencyException)
		{
			if(!_context.Affirmations.Any(e => e.Id == affirmation.Id))
			{
				return NotFound();
			} else
			{
				throw;
			}
		}
		return RedirectToAction(nameof(Index));
    }
	public async Task<IActionResult> Random()
    {
        var affirmation = await _context.Affirmations
			.Include(a => a.Category)
			.Where(a => a.IsActive)
			.OrderBy(a => Guid.NewGuid())
			.FirstOrDefaultAsync();

		return View(affirmation);
    }
	public async Task<IActionResult> Search(string query)
    {
        var results = await _context.Affirmations 
		.Include(a => a.Category)
		.Where(a => 
			string.IsNullOrEmpty(query) || 
			a.Text.Contains(query) ||
			a.Mood.Contains(query) ||
			a.Author.Contains(query) || 
			a.Category.Name.Contains(query)
			).ToListAsync();


		return PartialView("_AffirmationList", results); 
    }
}
