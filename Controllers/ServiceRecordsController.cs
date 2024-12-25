using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class ServiceRecordsController : Controller
    {
        private readonly IdentityContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public ServiceRecordsController(IdentityContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager;
            _roleManager = roleManager;
        }

         public async Task<IActionResult> Create()
{
    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null)
    {
        return Unauthorized();
    }

    // Check if the current user is in the 'Customer' role
    var isCustomer = await _userManager.IsInRoleAsync(currentUser, "Customer");

    // Fetch customers and personnel for dropdowns only if the user is not a Customer
    if (!isCustomer)
    {
        var customerRole = await _roleManager.FindByNameAsync("Customer");
        if (customerRole == null)
        {
            return NotFound("Customer role not found.");
        }

        var customers = await _context.Users
            .Where(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == customerRole.Id))
            .ToListAsync();
        ViewBag.Customers = customers;
    }

    // Fetch personnel for dropdown
    var personnelRole = await _roleManager.FindByNameAsync("Personnel");
    if (personnelRole == null)
    {
        return NotFound("Personnel role not found.");
    }

    var personnelList = await _context.Users
        .Where(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == personnelRole.Id))
        .ToListAsync();
    ViewBag.Personnel = personnelList;

    // Pass the current customer to the view if the user is a Customer
    ViewBag.IsCustomer = isCustomer;
    if (isCustomer)
    {
        ViewBag.CurrentCustomer = currentUser;
    }

    return View();
}



        [HttpPost]
        public async Task<IActionResult> Create(ServiceRecord model)
        {
            if (ModelState.IsValid)
            {
                // Yeni servis kaydını ekle
                _context.ServiceRecords.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Eğer model geçerli değilse, formu yeniden göster
            return View(model);
        }

        public async Task<IActionResult> Index()
{
    var currentUser = await _userManager.GetUserAsync(User);

    if (currentUser == null)
    {
        return Unauthorized();
    }

    IQueryable<ServiceRecord> serviceRecordsQuery = _context.ServiceRecords
        .Include(sr => sr.Customer)
        .Include(sr => sr.Personnel);

    if (User.IsInRole("Customer"))
    {
        // customers can reach only their own records
        serviceRecordsQuery = serviceRecordsQuery.Where(sr => sr.CustomerId == currentUser.Id);
    }
    else if (User.IsInRole("Personnel"))
    {
        // personnels can reach only their own records
        serviceRecordsQuery = serviceRecordsQuery.Where(sr => sr.PersonnelId == currentUser.Id);
    }
    // admin reaches all
    var serviceRecords = await serviceRecordsQuery.ToListAsync();
    return View(serviceRecords);
}


        public async Task<IActionResult> Edit(int id)
        {
            // Fetch the service record by id, including related customer and personnel
            var serviceRecord = await _context.ServiceRecords
                .Include(sr => sr.Customer)
                .Include(sr => sr.Personnel)
                .FirstOrDefaultAsync(sr => sr.ServiceRecordId == id);

            if (serviceRecord == null)
            {
                return NotFound("Service record not found.");
            }

            // Fetch customers and personnel for the dropdowns
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            if (customerRole != null)
            {
                ViewBag.Customers = await _context.Users
                    .Where(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == customerRole.Id))
                    .ToListAsync();
            }

            var personnelRole = await _roleManager.FindByNameAsync("Personnel");
            if (personnelRole != null)
            {
                ViewBag.Personnel = await _context.Users
                    .Where(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == personnelRole.Id))
                    .ToListAsync();
            }

            return View(serviceRecord);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceRecord model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // If validation fails, redisplay the form
            }

            // Fetch the existing service record
            var serviceRecord = await _context.ServiceRecords
                .FirstOrDefaultAsync(sr => sr.ServiceRecordId == model.ServiceRecordId);

            if (serviceRecord == null)
            {
                return NotFound("Service record not found.");
            }

            // Update the service record details
            serviceRecord.CustomerId = model.CustomerId;
            serviceRecord.PersonnelId = model.PersonnelId;
            serviceRecord.VehicleModel = model.VehicleModel;
            serviceRecord.Complaint = model.Complaint;

            _context.ServiceRecords.Update(serviceRecord); // Mark the record as updated
            await _context.SaveChangesAsync(); // Save changes to the database

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Fetch the service record by ID
            var serviceRecord = await _context.ServiceRecords
                .FirstOrDefaultAsync(sr => sr.ServiceRecordId == id);

            if (serviceRecord == null)
            {
                return NotFound("Service record not found.");
            }

            // Remove the service record
            _context.ServiceRecords.Remove(serviceRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
