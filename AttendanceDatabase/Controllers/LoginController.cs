using Microsoft.AspNetCore.Mvc;
using AttendanceDatabase.Models;


namespace AttendanceDatabase.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext? _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            
            try
            {
                var accounts = _context?.Accounts.ToList();
                if (ModelState.IsValid)
                {
                    
                    Account? validAccount = accounts.Find(x => x.Username == Username && x.Password == Password);
                    if (validAccount != null)
                    {
                        HttpContext.Session.SetString("_Name", validAccount.Username);
                        HttpContext.Session.SetString("_Role", validAccount.role);
                        if(validAccount.role == "Admin"){
                            return RedirectToAction("Admin_Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Staff_Index", "Home");
                        }
                        
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Incorrect Username or Password";
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }

           



           
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }
    }
}
