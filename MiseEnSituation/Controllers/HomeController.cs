using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    public class HomeController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private IUserService _userService;

        public HomeController()
        {
            _userService = new UserService(new UserRepository(db));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            //utilisiation du IUserService
            try
            {
                User foundedUser = _userService.CheckLogin(u.Email, u.Password);
                if (foundedUser!=null)
                {
                    Session["user_id"] = foundedUser.Id;
                    Session["user_connected"] = true;
                    return RedirectToAction("Index", "Admin"); // /Admin/Index
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
            }
            return View("Index",u);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}