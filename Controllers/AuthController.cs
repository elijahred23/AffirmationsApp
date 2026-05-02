using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


public class AuthController: Controller
{
    private readonly UsersDbContext _context;

    public AuthController(UsersDbContext context)
    {
        _context = context;
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

        using var sha = SHA256.Create();

        var hash = Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password))
        );

        if(hash != user.PasswordHash)
        {
            ModelState.AddModelError("", "Invalid login");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserId", user.Id.ToString())
        };

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