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
        UserService userService;
        EmployeeService employeeService; 

        private ICheckUpService _checkUpService;

        public CheckUpController()
        {
            employeeService = new EmployeeService(new EmployeeRepository(db));
            userService = new UserService(new UserRepository(db));
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
            
            //List<Employee> employee = employeeService.GetAllIncludesTracked(page, maxByPage, null, e => e.Type == UserType.EMPLOYEE);
            //List<Employee> manager = employeeService.GetAllIncludesTracked(page, maxByPage, null, e => e.Type == UserType.MANAGER);
            //List<Employee> rh = employeeService.GetAllIncludesTracked(page, maxByPage, null, e => e.Type == UserType.RH);

            //List<User> manager = userService.FindByType(UserType.MANAGER);
            //List<User> rh = userService.FindByType(UserType.RH);
            //List<User> employee = userService.FindByType(UserType.EMPLOYEE);
           
            ViewBag.Employee = userService.FindByType(UserType.EMPLOYEE); 
            ViewBag.Manager = userService.FindByType(UserType.MANAGER); ;
            ViewBag.RH = userService.FindByType(UserType.RH); ;

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
            if (checkUp.Employee.Id.HasValue)
            {
                checkUp.Employee = (Employee)userService.Find(checkUp.Employee.Id);//employeeService.FindByIdExcludes(checkUp.Employee.Id);
            }
            if (checkUp.Manager.Id.HasValue)
            {
                checkUp.Manager = (Employee)userService.Find(checkUp.Manager.Id);//employeeService.FindByIdExcludes(checkUp.Manager.Id);
            }
            if (checkUp.Employee.Id.HasValue)
            {
                checkUp.RH = (Employee)userService.Find(checkUp.RH.Id);//employeeService.FindByIdExcludes(checkUp.RH.Id);
            }
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
