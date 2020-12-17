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
    [RoutePrefix("Skills")]
    [Route("{action=index}")]
    public class SkillsController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly ISkillService _skillService;
        private readonly ITrainingCourseService _TrainingCourseService;
        private readonly IPostService _PostService;
        private readonly IEmployeeService _EmployeeService;

        public SkillsController()
        {
            _skillService = new SkillService(new SkillRepository(db));
            _TrainingCourseService = new TrainingCourseService(new TrainingCourseRepository(db));
            _PostService = new PostService(new PostRepository(db));
            _EmployeeService = new EmployeeService(new EmployeeRepository(db));
        }

        // GET: Users
        [HttpGet] //localhost:xxx/users/1/15
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            List<Skill> lstSkills = _skillService.FindAllIncludes(page, maxByPage, SearchField);
            ViewBag.NextExist = _skillService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", lstSkills);
        }

        // GET: Users/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = _skillService.FindByIdIncludes(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        // GET: Skills/Create
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            ViewBag.TrainingCourses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.Posts = new MultiSelectList(_PostService.GetAllExcludes(), "Id", "Description", null);
            ViewBag.Employees = new MultiSelectList(_EmployeeService.GetAllExcludes(), "Id", "Name", null);
            return View(new Skill());
        }

        // POST: Skills/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "Id,Description")] Skill skill, object[] TrainingCourses, object[] Posts, object[] Employees)
        {
            if (ModelState.IsValid)
            {
                List<TrainingCourse> tcs = TrainingCourses != null ? _TrainingCourseService.FindManyByIdExcludes(TrainingCourses) : null;
                List<Post> posts = Posts != null ? _PostService.FindManyByIdExcludes(Posts) : null;
                List<Employee> employees = Employees != null ? _EmployeeService.FindManyByIdExcludes(Employees) : null;
                skill.Courses = tcs;
                skill.Posts = posts;
                skill.Employees = employees;
                _skillService.Save(skill);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "";
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    ViewBag.ErrorMessage += error.ErrorMessage + "<br/>";
                }
            }
            ViewBag.TrainingCourses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.Posts = new MultiSelectList(_PostService.GetAllExcludes(), "Id", "Description", null);
            ViewBag.Employees = new MultiSelectList(_EmployeeService.GetAllExcludes(), "Id", "Name", null);
            return View(skill);
        }

        // GET: Skills/Edit/5
        [HttpGet]
        [Route("Edit/{id?}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = _skillService.FindByIdIncludes(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            ViewBag.TrainingCourses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.Posts = new MultiSelectList(_PostService.GetAllExcludes(), "Id", "Description", null);
            ViewBag.Employees = new MultiSelectList(_EmployeeService.GetAllExcludes(), "Id", "Name", null);
            return View(skill);
        }

        // POST: Skills/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit([Bind(Include = "Id,Description")] Skill skill, object[] TrainingCourses, object[] Posts, object[] Employees)
        {
            if (ModelState.IsValid)
            {
                List<TrainingCourse> tcs = TrainingCourses != null ? _TrainingCourseService.FindManyByIdExcludes(TrainingCourses) : null;
                List<Post> posts = Posts != null ? _PostService.FindManyByIdExcludes(Posts) : null;
                List<Employee> employees = Employees != null ? _EmployeeService.FindManyByIdExcludes(Employees) : null;
                skill.Courses = tcs;
                skill.Posts = posts;
                skill.Employees = employees;
                _skillService.Update(skill);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "";
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    ViewBag.ErrorMessage += error.ErrorMessage + "<br/>";
                }
            }
            ViewBag.TrainingCourses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.Posts = new MultiSelectList(_PostService.GetAllExcludes(), "Id", "Description", null);
            ViewBag.Employees = new MultiSelectList(_EmployeeService.GetAllExcludes(), "Id", "Name", null);
            return View(skill);
        }

        // GET: Skills/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = _skillService.FindByIdIncludes(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult DeleteConfirmed(int id)
        {
            _skillService.Delete(id);
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
