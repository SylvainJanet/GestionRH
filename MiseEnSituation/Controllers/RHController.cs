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
        // GET: RH
        [AdminFilter]
        //[RhFilter]
        [RoutePrefix("Rh")]
        [Route("{action=index}")]
        public class RHController : Controller
        {
            private readonly MyDbContext db = new MyDbContext();
        //private IRhService _rhService;
        private readonly IGenericService<TrainingCourse> _trainingService;
        private readonly IGenericService<Employee> _employeeService;
        private readonly IGenericService<Post> _postService;
        private readonly IGenericService<Company> _companyService;
        private readonly IGenericService<Address> _adressService;
        private readonly IGenericService<CheckUp> _checkUpService;
        public RHController()
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

        // GET: Users/Create
        [HttpGet]
            [Route("Create")]
            public ActionResult Create()
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
