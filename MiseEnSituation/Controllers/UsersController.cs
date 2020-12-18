using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GenericRepositoryAndService.Service;
using MiseEnSituation.Filters;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;

namespace MiseEnSituation.Controllers
{
    [AdminFilter]
    //[EmployeFilter]
    [RoutePrefix("Users")]
    [Route("{action=index}")]
    public class UsersController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly IGenericService<Employee> _employeeService;

        public UsersController()
        {
            _employeeService = new EmployeeService(new EmployeeRepository(db));
        }
        // GET: Employee
        public ActionResult Index()
        {
            return View(_employeeService.GetAll(false, true, 1, 857458));
        }

        [HttpGet]
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            List<Employee> lstEmployees = _employeeService.FindAllIncludes(page, maxByPage, SearchField);

            ViewBag.NextExist = _employeeService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", lstEmployees);
        }
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _employeeService.FindByIdIncludes(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        [HttpGet]
        [Route("Create")]
        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Password,CreationDate,ProPhone,Type,BirthDate,PersonalPhone,IsManager")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeService.Save(employee);
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _employeeService.FindByIdIncludes(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Password,CreationDate,ProPhone,Type,BirthDate,PersonalPhone,IsManager")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                _employeeService.Update(employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _employeeService.FindByIdIncludes(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = _employeeService.FindByIdIncludes(id);
            _employeeService.Delete(employee);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Search")]
        public ActionResult Search([Bind(Include = ("page, maxByPage, SearchField"))] int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string searchField = "")
        {
            if (searchField.Trim().Equals(""))
                return RedirectToAction("Index");
            return Index(page, maxByPage, searchField);
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
