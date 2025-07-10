using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Application.Data;
using Practice_Application.Models;
using Practice_Application.ViewModels;
using System.Security.Claims;

namespace Practice_Application.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly dbContext _dbcontext;
        public AuthenticationController(dbContext context)
        {
            _dbcontext = context;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Name,Email,Password")] User user)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Check if the user already exists
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(u => user.Email == u.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists!" });
            }

            //Encrypt the password here
            PasswordHasher<User> hasher = new();
            user.Password = hasher.HashPassword(user, user.Password);
           
            //Save the user to the database here
            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();
            return Ok(new {message="Signup successful!"});
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Retrieve the user from the database
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(u => user.Email == u.Email);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found!" });
            }
            //Verify the password
            PasswordHasher<User> hasher = new();
            var passwordVerificationResult = hasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid password!" });
            }
            //Login successful

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Name),
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim("Role", "Admin"), // You can add more claims as needed
            };

            var Identity = new ClaimsIdentity(claims,Environment.GetEnvironmentVariable("AuthCookieName"));
            var principal = new ClaimsPrincipal(Identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = true, // Set to true if you want the cookie to persist across browser sessions
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(7) // Set the expiration time for the cookie
            };

            await HttpContext.SignInAsync(Environment.GetEnvironmentVariable("AuthCookieName"), principal, props);

            return Ok(new { message = "Login successful!" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
        
            await HttpContext.SignOutAsync("AuthCookie");
            return RedirectToAction("Index","Home");
        }
    }
}
