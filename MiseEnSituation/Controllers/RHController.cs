using MiseEnSituation.Filters;
using RepositoriesAndServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
        // GET: RH
        [AdminFilter]
        //[RhFilter]
        [RoutePrefix("rh")]
        [Route("{action=index}")]
        public class RHController : Controller
        {
            private readonly MyDbContext db = new MyDbContext();
            //private IRhService _rhService;

            public RHController()
            {
                //_rhService = new RhService(new UserRepository(db));
            }

        public ActionResult Index()
        {
            return View();
        }

        // GET: Users/Create
        [HttpGet]
            [Route("Create")]
            public ActionResult Create()
            {
                return View();
            }


            // POST: Users/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Route("Delete/{id}")]
            //public ActionResult DeleteConfirmed(int id)
            //{
            //    _rhService.Remove(id);
            //    return RedirectToAction("Index");
            //}

            //[HttpGet] //localhost:xxx/users/1/15
            //[Route("{page?}/{maxByPage?}/{searchField?}")]
            //public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
            //{
            //    List<User> lstUsers = _userService.FindAll(page, maxByPage, SearchField);
            //    ViewBag.NextExist = _userService.NextExist(page, maxByPage, SearchField);
            //    ViewBag.Page = page;
            //    ViewBag.MaxByPage = maxByPage;
            //    ViewBag.SearchField = SearchField;
            //    return View("Index", lstUsers);
            //}

            //[HttpGet]
            //    //[ValidateAntiForgeryToken]
            //    [Route("Search")]
            //    public ActionResult Search([Bind(Include = ("page, maxByPage, SearchField"))] int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string searchField = "")
            //    {
            //        if (searchField.Trim().Equals(""))
            //            return RedirectToAction("Index");
            //        //List<User> lstUsers = _userService.Search(searchField);
            //        //ViewBag.Page = 1;
            //        //ViewBag.NextExist = false;
            //        //return View("Index", lstUsers);
            //        return Index(page, maxByPage, searchField);
            //    }

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
