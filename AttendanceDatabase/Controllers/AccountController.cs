using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using AttendanceDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AttendanceDatabase.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly AppDbContext _context;
        

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        
        public IActionResult ManageAccounts()
        {
            if (HttpContext.Session.GetString("_Role") == "Admin")
            {
                var accounts = _context.Accounts.ToList();
                return View(accounts);
            }
            else
            {
                return NotFound();
            }
            
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("_Role") == "Admin")
            {
                
                return View();
            }
            else
            {
                return NotFound();
            }
                
        }
        [HttpPost]
       
        /*[HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // var accounts = _context.Accounts.ToList();
                    var accountToBeDeleted = _context.Accounts.SingleOrDefault(c => c.Id == id);
                    _context.Accounts.Remove(accountToBeDeleted);
                    _context.SaveChanges();
                    var accounts = _context.Accounts.ToList();
                    return View("~/Views/Accounts/ManageAccounts.cshtml", accounts);
                }

                else
                {

                    TempData["ErrorMessage"] = "Please correct the errors and try again.";
                    return View("~/Views/Accounts/ManageAccounts.cshtml");
                }
                
                
                
            }
            catch(Exception ex) {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View("~/Views/Accounts/ManageAccounts.cshtml");
            }
            
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(int? itemid)
        {
            if (HttpContext.Session.GetString("_Role") == "Admin")
            {
                var account = _context.Accounts.Find(itemid); // Find the account by ID

                if (account == null)
                {
                    return NotFound(); // If the account is not found, return 404
                }

                _context.Accounts.Remove(account); // Remove the account from the database
                _context.SaveChanges(); // Save the changes to the database

                return RedirectToAction("ManageAccounts"); // Redirect back to the ManageAccounts page
            }
            else
            {
                return NotFound();
            }

            
        }
        
        




        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (HttpContext.Session.GetString("_Role") == "Admin")
            {
                try
                {
                    if (ModelState.IsValid)
                    {


                        
                        _context.Accounts.Add(account);
                        _context.SaveChanges();
                        TempData["SuccessMessage"] = "Account created successfully.";
                        return RedirectToAction("ManageAccounts");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please correct the errors and try again.";
                        return View(account);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return View(account);
                }
            }
            else
            {
                return NotFound();
            }
                
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int itemid)
        {
            if (HttpContext.Session.GetString("_Role") == "Admin")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        

                        Account account = _context.Accounts.Find(itemid);
                        


                        if (account == null)
                        {
                            return NotFound();
                        }
                        _context.Accounts.Remove(account);
                        _context.SaveChanges();


                        return View("Create", account);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please correct the errors and try again.";
                        return RedirectToAction("ManageAccounts");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return RedirectToAction("ManageAccounts");
                }
            }
            else
            {
                return NotFound();
            }
                
        }

    }
}
