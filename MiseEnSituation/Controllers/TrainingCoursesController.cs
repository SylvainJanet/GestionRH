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
    [RoutePrefix("TrainingCourse")]
    [Route("{action=index}")]
    public class TrainingCoursesController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private ITrainingCourseService _trainingCourseService;
        //private EmployeeService _employeeService;
        private ISkillService _skillService;

        public TrainingCoursesController()
        {
            _trainingCourseService = new TrainingCourseService(new TrainingCourseRepository(db));
            //_employeeService = new EmployeeService(new EmployeeRepository(db));
            _skillService = new SkillService(new SkillRepository(db));
        }

        // GET: TrainingCourses
        [HttpGet] //localhost:xxx/users/1/15
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            List<TrainingCourse> lstSkills = _trainingCourseService.FindAllExcludes(page, maxByPage, SearchField);
            ViewBag.NextExist = _trainingCourseService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", lstSkills);
        }

        // GET: TrainingCourses/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = _trainingCourseService.FindByIdIncludes(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Create
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            //ViewBag.Employees = _employeeService.FindAll(1,int.MaxValue,"");
            ViewBag.Skills = _skillService.FindAllExcludes(1, int.MaxValue,"");
            return View();
        }

        // POST: TrainingCourses/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "Id,Name,StartingDate,EndingDate,DurationInHours,Price")] TrainingCourse trainingCourse,/* int[] EnrolledEmployees,*/ int?[] TrainedSkills)
        {
            if (ModelState.IsValid && TrainedSkills!=null)
            {
                List<Skill> skills = _skillService.FindManyByIdExcludes(TrainedSkills);
                _trainingCourseService.Save(trainingCourse,skills);
                return RedirectToAction("Index");
            }

            if (TrainedSkills == null)
                ViewBag.ErrorSkillsMessage = "At least one skill must be selected";
            ViewBag.Skills = _skillService.FindAllExcludes(1, int.MaxValue, "");
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Edit/5
        [HttpGet]
        [Route("Edit/{id?}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = _trainingCourseService.FindByIdIncludes(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Employees = _employeeService.FindAll(1,int.MaxValue,"");
            ViewBag.Skills = _skillService.FindAllExcludes(1, int.MaxValue, "");
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit([Bind(Include = "Id,Name,StartingDate,EndingDate,DurationInHours,Price",Exclude ="TrainedSkills")] TrainingCourse trainingCourse,/* int[] EnrolledEmployees,*/ int?[] EditTrainedSkills)
        {
            if (ModelState.IsValid && EditTrainedSkills != null)
            {
                List<Skill> skills = _skillService.FindManyByIdExcludes(EditTrainedSkills);
                _trainingCourseService.Update(trainingCourse, skills);
                return RedirectToAction("Index");
            }
            if (EditTrainedSkills == null)
                ViewBag.ErrorSkillsMessage = "At least one skill must be selected";
            ViewBag.Skills = _skillService.FindAllExcludes(1, int.MaxValue, "");
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = _trainingCourseService.FindByIdIncludes(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult DeleteConfirmed(int id)
        {
            _trainingCourseService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
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
