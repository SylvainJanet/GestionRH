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
        public ActionResult Home()
        {
            return View();
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
        [HttpGet]
        [Route("Account")]
        public ActionResult Account()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account")]
        public ActionResult Account([Bind(Include = "Id,Name,Email,Password,ProPhone,Type")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Type = UserType.EMPLOYEE;
                _userService.Save(user);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            //utilisiation du IUserService
            try
            {

                User fu = _userService.CheckLogin(u.Email, u.Password, u.Type);
                        

                    if (fu != null && fu.Type.ToString().Equals(UserType.ADMIN.ToString()))
                    {
                        Session["user_id"] = fu.Id;
                        Session["user_connected"] = true;
                        return RedirectToAction("Index", "Admin"); // /Admin/Index             
                    }

                    else if(fu != null && fu.Type == UserType.MANAGER)
                    {
                        Session["user_id"] = fu.Id;
                        Session["user_connected"] = true;
                        return RedirectToAction("Index", "Manager"); // /Manager/Index             
                    }

                    else if (fu != null && fu.Type == UserType.EMPLOYEE)
                    {
                        Session["user_id"] = fu.Id;
                        Session["user_connected"] = true;
                        return RedirectToAction("Index", "Employee"); // /Employee/Index    
                }
                    else if (fu != null && fu.Type == UserType.RH)
                    {
                        Session["user_id"] = fu.Id;
                        Session["user_connected"] = true;
                        return RedirectToAction("Index", "RH"); // /RH/Index    
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
            return RedirectToAction("Home", "Home");
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