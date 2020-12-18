using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;

namespace MiseEnSituation.Controllers
{
    public class CheckUpController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly ICheckUpService _checkUpService;
        private readonly IUserService userService;
        private readonly IEmployeeService employeeService;

        public CheckUpController()
        {
            _checkUpService = new CheckUpService(new CheckUpRepository(db));
            userService = new UserService(new UserRepository(db));
            employeeService = new EmployeeService(new EmployeeRepository(db));
        }
        // GET: CheckUp
        public ActionResult Index()
        {
            return View(_checkUpService.FindAll(1,5241));
        }

        // GET: CheckUp/Details/5
        public ActionResult Details(int? id)
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

        // GET: CheckUp/Create
        public ActionResult Create()
        {
           
            
            ViewBag.Employee = employeeService.GetAllByExcludes(c => c.Type.ToString().Equals(UserType.EMPLOYEE.ToString()));
            ViewBag.Manager = employeeService.GetAllByExcludes(c => c.Type.ToString().Equals(UserType.MANAGER.ToString()));
            ViewBag.RH = employeeService.GetAllByExcludes(c => c.Type.ToString().Equals(UserType.RH.ToString()));
            return View(new CheckUp());
        }

        // POST: CheckUp/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,EmployeeId,ManagerId,RHId")] CheckUp checkUp)
        {
            if (checkUp.EmployeeId.HasValue)
            {
                checkUp.Employee =employeeService.FindById(false, true,checkUp.EmployeeId);
            }
            if (checkUp.ManagerId.HasValue)
            {
                checkUp.Manager = employeeService.FindById(false, true, checkUp.ManagerId);
            }
            if (checkUp.RHId.HasValue)
            {
                checkUp.RH = employeeService.FindById(false, true, checkUp.RHId);
            }
            ModelState.Remove("checkUp.Employee");
            ModelState.Remove("checkUp.Manager");
           
            if (ModelState.IsValid)
            {
                _checkUpService.Save(checkUp);
                return RedirectToAction("Index");
            }

            return View(checkUp);
        }

        // GET: CheckUp/Edit/5
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
            ViewBag.Employee = employeeService.GetAllByExcludes(c => c.Type.Equals(UserType.EMPLOYEE));
            ViewBag.Manager = employeeService.GetAllByExcludes(c => c.Type.Equals(UserType.MANAGER));
            ViewBag.RH = employeeService.GetAllByExcludes(c => c.Type.Equals(UserType.RH));
            return View(checkUp);
        }

        // POST: CheckUp/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Employee,Manager,RH")] CheckUp checkUp)
        {
            if (checkUp.EmployeeId.HasValue)
            {
                checkUp.Employee = employeeService.FindById(false, true, checkUp.EmployeeId);
            }
            if (checkUp.ManagerId.HasValue)
            {
                checkUp.Manager = employeeService.FindById(false, true, checkUp.ManagerId);
            }
            if (checkUp.RHId.HasValue)
            {
                checkUp.RH = employeeService.FindById(false, true, checkUp.RHId);
            }
            ModelState.Remove("checkUp.Employee");
            ModelState.Remove("checkUp.Manager");

            if (ModelState.IsValid)
            {
                _checkUpService.Save(checkUp);
                return RedirectToAction("Index");
            }

            return View(checkUp);
        }

        // GET: CheckUp/Delete/5
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
