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
    [RoutePrefix("User")]
    [Route("{action=index}")]
    public class UserController : Controller
    {
        private MyDbContext db = new MyDbContext();

        private IGenericService<Employee> _employeeService;  
        private IGenericService<Post> _postService;  
        private IGenericService<Company> _companyService;  

        public UserController()
        {
            _employeeService = new EmployeeService(new EmployeeRepository(db));
            _postService = new PostService(new PostRepository(db));
            _companyService = new CompanyService(new CompanyRepository(db));
        }
        // GET: User
        
        public ActionResult Index()
        {
            return View(_employeeService.GetAll(false, true, 1, 857458));
        }
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        // GET: User/Details/5
        [HttpGet]
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
        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.Post = _postService.GetAll(false, true, 1, 85241);
            ViewBag.Company = _companyService.GetAll(false, true, 1, 85241);
            return View();
        }

        // POST: User/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Password,ProPhone,Type,BirthDate,PersonalPhone,IsManager,CompagnyId,PostId")] Employee employee)
        {
            employee.CreationDate = DateTime.Now;
            if (employee.CompagnyId.HasValue)
            {
                employee.Company = _companyService.FindByIdIncludes(employee.CompagnyId);
            }
            if (employee.PostId.HasValue)
            {
                employee.Post = _postService.FindByIdIncludes(employee.PostId);
            }
            if (ModelState.IsValid)
            {
                _employeeService.Save(employee);
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: User/Edit/5
        [HttpGet]
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

            ViewBag.Post = _postService.GetAll(false, true, 1, 85241);
            ViewBag.Company = _companyService.GetAll(false, true, 1, 85241);
            return View(employee);
        }

        // POST: User/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Password,CreationDate,ProPhone,Type,BirthDate,PersonalPhone,IsManager,CompagnyId,PostId")] Employee employee)
        {
            employee.CreationDate = DateTime.Now;
            if (employee.CompagnyId.HasValue)
            {
                employee.Company = _companyService.FindByIdIncludes(employee.CompagnyId);
            }
            if (employee.PostId.HasValue)
            {
                employee.Post = _postService.FindByIdIncludes(employee.PostId);
            }
            if (ModelState.IsValid)
            {
                _employeeService.Save(employee);
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: User/Delete/5
        [HttpGet]
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = _employeeService.FindByIdIncludes(id);
            _employeeService.Delete(employee);
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
