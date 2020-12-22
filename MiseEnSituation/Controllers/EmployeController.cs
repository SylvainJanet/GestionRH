using GenericRepositoryAndService.Service;
using MiseEnSituation.Filters;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    [AdminFilter]
    public class EmployeController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        private readonly IGenericService<Employee> _employeeService;
        private readonly IGenericService<Post> _postService;
        private readonly IGenericService<Company> _companyService;

        public EmployeController()
        {
            _employeeService = new EmployeeService(new EmployeeRepository(db));
            _postService = new PostService(new PostRepository(db));
            _companyService = new CompanyService(new CompanyRepository(db));
        }
        // GET: Employe
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Formation(/*int id*/)
        {
            //var query = "SELECT * FROM TrainingCourse WHERE Id = " + id;
            //var result = query.ExecuteSQL();
            return View();
        }

        [HttpGet]
        public ActionResult Profil(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Password,ProPhone,BirthDate,PersonalPhone,PersonalAdress")] Employee employee)
        {
            
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