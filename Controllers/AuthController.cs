using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;


public class AuthController: Controller
{
    private readonly UsersDbContext _context;
    private readonly AppDbContext _appDbContext;

    public AuthController(UsersDbContext context, AppDbContext appDbContext)
    {
        _context = context;
        _appDbContext = appDbContext;
    }

    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]

    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
        {
            ModelState.AddModelError("", "Invalid login");
            return View();
        }

        var hasher = new PasswordHasher<string>();

        var result = hasher.VerifyHashedPassword(
            user.Username,
            user.PasswordHash,
            password
        );
        
        if(result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid login");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserId", user.Id.ToString())
        };

        var permissions = await _appDbContext.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Permission.Name)
            .ToListAsync();
        
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);


        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Affirmations");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }

}