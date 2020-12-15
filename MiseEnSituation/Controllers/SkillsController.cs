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
        private MyDbContext db = new MyDbContext();
        private ISkillService _skillService;
        private ITrainingCourseService _TrainingCourseService;

        public SkillsController()
        {
            _skillService = new SkillService(new SkillRepository(db));
            _TrainingCourseService = new TrainingCourseService(new TrainingCourseRepository(db));
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
            return View();
        }

        // POST: Skills/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "Id,Description")] Skill skill, int?[] TrainingCourses)
        {
            if (ModelState.IsValid)
            {
                List<TrainingCourse> tcs = TrainingCourses != null ? _TrainingCourseService.FindManyByIdExcludes(TrainingCourses) : null;
                //_skillService.Save(skill,tcs);
                return RedirectToAction("Index");
            }
            ViewBag.TrainingCourses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
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
            ViewBag.Courses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
            return View(skill);
        }

        // POST: Skills/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit([Bind(Include = "Id,Description")] Skill skill, int?[] Courses)
        {
            if (ModelState.IsValid)
            {
                foreach (TrainingCourse trainingCourse in _TrainingCourseService.GetAllExcludes(1, int.MaxValue, null, t => !Courses.Contains(t.Id) && t.TrainedSkills.Count() == 1 && t.TrainedSkills.Where(s => s.Id == skill.Id).Count() == 1))
                {
                    _TrainingCourseService.Delete(trainingCourse);
                }
                List<TrainingCourse> tcs = Courses != null ? _TrainingCourseService.FindManyByIdExcludes(Courses) : null;
                //_skillService.Update(skill,tcs);
                return RedirectToAction("Index");
            }
            ViewBag.Courses = new MultiSelectList(_TrainingCourseService.GetAllExcludes(), "Id", "Name", null);
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
            foreach (TrainingCourse trainingCourse in _TrainingCourseService.GetAllExcludes(1, int.MaxValue, null, t => t.TrainedSkills.Count() == 1 && t.TrainedSkills.Where(s => s.Id == id).Count() == 1))
            {
                _TrainingCourseService.Delete(trainingCourse);
            }
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
