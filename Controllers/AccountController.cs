using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController: Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user != null)
                {
                    await _signInManager.SignOutAsync();  // kullanıcı daha önce giriş yapmışsa tarayıcısından bunu sıfırlama işlemi

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);   // yeni cookie oluşturma

                    if(result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);  // başarısız girişi tekrar 5'e alındı
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home");
                    }
                    else if(result.IsLockedOut)
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"Your account has been locked, please try again after {timeLeft.Minutes} minutes later...");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your password is incorrect.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "An account could not be found with this email address.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // clear cookies
            return RedirectToAction("Login", "Account");
        }


        public IActionResult Create()
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name)
                .Where(name => name != null) // Null değerleri filtreler
                .Cast<string>()              // Nullable olmayan türe dönüştürür
                .ToList();

            var model = new CreateViewModel
            {
                AllRoles = roles // adding roles to models 
            };
            return View(model);
        }

        [HttpPost]
public async Task<IActionResult> Create(CreateViewModel model)
{
    if (ModelState.IsValid)
    {
        // Sanitize username: Remove non-alphanumeric characters
        var sanitizedUsername = new string(model.Email.Where(c => char.IsLetterOrDigit(c)).ToArray());

        if (string.IsNullOrWhiteSpace(sanitizedUsername))
        {
            ModelState.AddModelError("", "Username can only contain letters or digits.");
            return View(model);
        }

        var user = new AppUser
        {
            UserName = sanitizedUsername, // Use sanitized username
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber       
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var roleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Customer" });
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return RedirectToAction("Login", "Account");
        }

        foreach (IdentityError err in result.Errors)
        {
            ModelState.AddModelError("", err.Description);
        }
    }

    return View(model);
}


         public IActionResult CreatePersonnel()
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name)
                .Where(name => name != null) // Null değerleri filtreler
                .Cast<string>()              // Nullable olmayan türe dönüştürür
                .ToList();

            var model = new CreateViewModel
            {
                AllRoles = roles // adding roles to models
            };
            return View(model);
        }

        [HttpPost]
public async Task<IActionResult> CreatePersonnel(CreateViewModel model)
{
    if (ModelState.IsValid)
    {
        // Sanitize username: Remove non-alphanumeric characters
        var sanitizedUsername = new string(model.Email.Where(c => char.IsLetterOrDigit(c)).ToArray());

        if (string.IsNullOrWhiteSpace(sanitizedUsername))
        {
            ModelState.AddModelError("", "Username can only contain letters or digits.");
            return View(model);
        }

        var user = new AppUser
        {
            UserName = sanitizedUsername, // Use sanitized username
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Field = model.Field
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Check if "Personnel" role exists, if not, create it
            var roleExists = await _roleManager.RoleExistsAsync("Personnel");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Personnel" });
            }

            await _userManager.AddToRoleAsync(user, "Personnel"); // Assign the "Personnel" role

            return RedirectToAction("Login", "Account");
        }

        foreach (IdentityError err in result.Errors)
        {
            ModelState.AddModelError("", err.Description);
        }
    }

    return View(model);
}




    }
}