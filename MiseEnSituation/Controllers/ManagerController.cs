using MiseEnSituation.Filters;
using RepositoriesAndServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    // GET: Manager
    [AdminFilter]
    //[ManagerFilter]
    [RoutePrefix("Manager")]
    [Route("{action=index}")]
    public class ManagerController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        //private IRhService _rhService;

        public ManagerController()
        {
            //_rhService = new RhService(new UserRepository(db));
        }

        public ActionResult Index()
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