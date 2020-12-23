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

        public ActionResult Formation()
        {
            ViewBag.lsTc = Session["lstTc"];
            return View();
        }

        [HttpGet]
        public ActionResult CheckUp()
        {
            ViewBag.lsTc = Session["lstCU"];
            return RedirectToAction("CheckUp", "ListAllOptionForUser"); // /Admin/Index             
            //return View();
        }


        [HttpGet]
        public ActionResult Profil(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //Employee employee = _employeeService.FindByIdIncludes(id);
            //if (employee == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(employee);
            return View();
        }
    }
}