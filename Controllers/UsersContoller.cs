using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class UsersController:Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usersWithCustomerRole = new List<AppUser>();

            // Get all users
            var allUsers = _userManager.Users;

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Check if the user has the "Customer" role
                if (roles.Contains("Customer"))
                {
                    usersWithCustomerRole.Add(user);
                }
            }

            return View(usersWithCustomerRole);
        }

        public async Task<IActionResult> Index2()
        {
            var usersWithPersonnelRole = new List<AppUser>();

            // Get all users
            var allUsers = _userManager.Users;

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Check if the user has the "Customer" role
                if (roles.Contains("Personnel"))
                {
                    usersWithPersonnelRole.Add(user);
                }
            }

            return View(usersWithPersonnelRole);
        }

    
        public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id); 

            if(user != null)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();  // assign list to role

                return View(new EditViewModel{
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Field = user.Field,
                    SelectedRoles = await _userManager.GetRolesAsync(user)
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {
            if(id != model.Id)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if(user != null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Field = model.Field;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user);  // remove users password
                        await _userManager.AddPasswordAsync(user, model.Password);  // assign the password from the model to the user 
                    }

                    if(result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));   // _userManager üzerinden kullanıcının rollerini kullanıcıdan silme işlemi
                        if(model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);  // kullanıcıya yeni roller ekleme işlemi (SelectedRoles listesinden)
                        }
                        return RedirectToAction("Index");
                    }

                    foreach(IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user= await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}