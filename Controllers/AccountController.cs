using Microsoft.AspNetCore.Mvc;
using PersonalityTest.Models;

namespace PersonalityTest.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Simple credentials check (you can change these values)
            if (model.Username == "student" && model.Password == "1234")
            {
                HttpContext.Session.SetString("UserLoggedIn", "true");
                HttpContext.Session.SetString("Username", model.Username);

                // Redirect to your Test controller's Instructions or Questions page
                return RedirectToAction("Instructions", "Test");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}