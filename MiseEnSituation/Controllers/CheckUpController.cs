using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MiseEnSituation.Filters;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Services;

namespace MiseEnSituation.Controllers
{
    [AdminFilter]
    [RoutePrefix("CheckUps")]
    [Route("{action=index}")]
    public class CheckUpController : Controller
    {
        private MyDbContext db = new MyDbContext();

        private ICheckUpService _checkUpService;

        public CheckUpController()
        {
            _checkUpService = new CheckUpService(new CheckUpRepository(db));
        }
      
        // GET: CheckUp
        [HttpGet] //localhost:xxx/checkup/1/15
        [Route("{page?}/{maxByPage?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE)
        {
            List<CheckUp> lstCheckUps = _checkUpService.FindAll(page, maxByPage);
            ViewBag.NextExist = _checkUpService.NextExist(page, maxByPage);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;

            return View("Index",lstCheckUps);
        }

        // GET: CheckUp/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUp checkUp =_checkUpService.Find(id);
            if (checkUp == null)
            {
                return HttpNotFound();
            }
            return View(checkUp);
        }

        // GET: CheckUp/Create
        [HttpGet]
        [Route("Create")]
        public ActionResult Create(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE)
        {
            UserService userService = new UserService(new UserRepository(db));
            List<User> users = userService.FindAll(page, maxByPage, "");

            List<SelectListItem> employe = new List<SelectListItem>();
            List<SelectListItem> manager = new List<SelectListItem>();
            List<SelectListItem> rh = new List<SelectListItem>();

            foreach (var item in users)
            {
                if (item.Type.Equals(UserType.EMPLOYEE))
                {

                    employe.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.Id) });
                }
                else if (item.Type.Equals(UserType.MANAGER))
                {
                    manager.Add(new SelectListItem { Text = item.Name, Value=Convert.ToString(item.Id )});
                }
                else if (item.Type.Equals(UserType.RH))
                {
                    rh.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.Id) });
                }

            }

            ViewBag.EmployeeList = employe;
            ViewBag.ManagerList = manager;
            ViewBag.RHList = rh;

            return View();
        }

        // POST: CheckUp/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "Id,Date,Employee,Manager,RH")] CheckUp checkUp)
        {
            if (ModelState.IsValid)
            {
                _checkUpService.Save(checkUp);
                return RedirectToAction("Index");
            }

            return View(checkUp);
        }

        // GET: CheckUp/Edit/5
        [HttpGet]
        [Route("Edit/{id?}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUp checkUp = _checkUpService.Find(id);
            if (checkUp == null)
            {
                return HttpNotFound();
            }
            return View(checkUp);
        }

        // POST: CheckUp/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit([Bind(Include = "Id,Date,Employee,Manager,RH")] CheckUp checkUp)
        {
            if (ModelState.IsValid)
            {
                _checkUpService.Update(checkUp);
                return RedirectToAction("Index");
            }
            return View(checkUp);
        }

        // GET: CheckUp/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUp checkUp = _checkUpService.Find(id);
            if (checkUp == null)
            {
                return HttpNotFound();
            }
            return View(checkUp);
        }

        // POST: CheckUp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult DeleteConfirmed(int id)
        {
            _checkUpService.Remove(id);
            return RedirectToAction("Index");
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
