using MiseEnSituation.Filters;
using RepositoriesAndServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    [AdminFilter]
    public class EmployeController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        public EmployeController()
        {

        }
        // GET: Employe
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Formation()
        {
            return View();
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