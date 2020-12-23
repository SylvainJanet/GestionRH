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
        private readonly IGenericService<TrainingCourse> _trainingService;
        private readonly IGenericService<CheckUp> _checkUpService;


        public EmployeController()
        {

            _employeeService = new EmployeeService(new EmployeeRepository(db));
            _postService = new PostService(new PostRepository(db));
            _companyService = new CompanyService(new CompanyRepository(db));
            _trainingService = new TrainingCourseService(new TrainingCourseRepository(db));
            _checkUpService = new CheckUpService(new CheckUpRepository(db));

        }
        // GET: Employe
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Formations ()
        {
            User user = (User)Session["user"];
            List<TrainingCourse> lstTc = _trainingService.GetAllByIncludes(t => t.Id == user.Id);
            Session["lstTc"] = lstTc;
            return RedirectToAction("Formations", "ListAllOptionForUser"); // /Admin/Index             
            //return View();
        }

        [HttpGet]
        public ActionResult CheckUps()
        {
            User user = (User)Session["user"];
            List<CheckUp> lstCU = _checkUpService.GetAllByIncludes(t => t.Id == user.Id);
            Session["lstCu"] = lstCU;
            return RedirectToAction("CheckUps", "ListAllOptionForUser"); // /Admin/Index             
            //return View();
        }


        [HttpGet]
        public ActionResult Profil()
        {
            //Employee user = (Employee)Session["user"];
            return RedirectToAction("profils", "ListAllOptionForUser"); // /Admin/Index          
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