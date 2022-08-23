using Excel.Integration.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Excel.Integration.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logon(string username, string password)
        {
            if (username.ToControl() || password.ToControl())
            {
                ViewBag.Error = "Kullanıcı adı veya şifre alanı zorunludur";
                return View("Login");
            }

            if (username == "Login.UserName".AppSettingsVal() && password == "Login.Password".AppSettingsVal())
            {
                SessionHelper.Login(username, password);
                return RedirectToAction("Index", "Home");

            }

            ViewBag.Error = "Kullanıcı adı veya şifre alanı hatalı";
            return View("Login");
        }

        public ActionResult LogOut()
        {
            SessionHelper.LogOut();
            return RedirectToAction("Login");
        }
    }

}