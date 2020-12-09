﻿using MiseEnSituation.Models;
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
        public ActionResult Account()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            //utilisiation du IUserService
            try
            {
                User foundedUser = _userService.CheckLogin(u.Email, u.Password, u.Type);
                if (foundedUser != null && u.Type == UserType.ADMIN)
                {
                    Session["user_id"] = foundedUser.Id;
                    Session["user_connected"] = true;
                    //ViewBag.UserType = UserType.ADMIN;
                    return RedirectToAction("Index", "Admin"); // /Admin/Index
                    
                }

                //else
                //{
                //    u.Type.Equals(UserType.EMPLOYEE);
                //    ViewBag.UserType = UserType.EMPLOYEE;
                //    return RedirectToAction("Index", "Employee"); // /Employee/Index
                //    u.Type.Equals(UserType.RH);
                //    ViewBag.UserType = UserType.RH;
                //    return RedirectToAction("Index", "RH"); // /RH/Index
                //    u.Type.Equals(UserType.MANGAGER);
                //    ViewBag.UserType = UserType.MANGAGER;
                //    return RedirectToAction("Index", "Manager"); // /Manager/Index

                //}
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