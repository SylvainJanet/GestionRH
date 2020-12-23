using GenericRepositoryAndService.Service;
using MiseEnSituation.Filters;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
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
        private readonly IGenericService<TrainingCourse> _trainingService;
        private readonly IGenericService<Employee> _employeeService;
        private readonly IGenericService<Post> _postService;
        private readonly IGenericService<Company> _companyService;
        private readonly IGenericService<Address> _adressService;
        private readonly IGenericService<CheckUp> _checkUpService;

        public ManagerController()
        {
            //_rhService = new RhService(new UserRepository(db));
            _trainingService = new TrainingCourseService(new TrainingCourseRepository(db));
            _employeeService = new EmployeeService(new EmployeeRepository(db));
            _postService = new PostService(new PostRepository(db));
            _companyService = new CompanyService(new CompanyRepository(db));
            _adressService = new AddressService(new AddressRepository(db));
            _checkUpService = new CheckUpService(new CheckUpRepository(db));
        }

        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Formations()
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