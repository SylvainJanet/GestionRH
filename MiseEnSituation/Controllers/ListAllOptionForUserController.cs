using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    public class ListAllOptionForUserController : Controller
    {
        // GET: ListAllOptionForUser
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Formations()
        {
            ViewBag.lstTc = Session["lstTc"];
            return View();
        }

        [HttpGet]
        public ActionResult CheckUps()
        {
            ViewBag.User = (User)Session["user"];
            return View();
        }


        [HttpGet]
        public ActionResult Profils(int? id)
        {
            var user = (Employee)Session["user"];
            ViewBag.User = user;
            return View();
        }
    }
}