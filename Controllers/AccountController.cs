using Microsoft.AspNetCore.Mvc;
using PersonalityTest.Models;

namespace PersonalityTest.Controllers
{
    public class AccountController : Controller
    {
        private const string validUser = "student";
        private const string validPass = "1234";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (model.Username == validUser && model.Password == validPass)
            {
                HttpContext.Session.SetString("User", model.Username);
                return RedirectToAction("Instructions", "Test");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
