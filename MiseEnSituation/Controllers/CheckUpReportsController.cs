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
    [RoutePrefix("CheckUpReports")]
    [Route("{action=index}")]
    public class CheckUpReportsController : Controller
    {
        private MyDbContext db = new MyDbContext();
        ICheckUpReportService _checkUpReport;
        ITrainingCourseService trainingCourseService;
        ITrainingCourseService _TrainedCoursesService;
        public CheckUpReportsController()
        {
            _checkUpReport = new CheckUpReportService(new CheckUpReportRepository(db));
            trainingCourseService = new TrainingCourseService(new TrainingCourseRepository(db));
            _TrainedCoursesService = new TrainingCourseService(new TrainingCourseRepository(db));
        }
        // GET: CheckUpReports
        public ActionResult Index()
        {
            return View(_checkUpReport.FindAllExcludes());
        }

        // GET: CheckUpReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUpReport checkUpReport = _checkUpReport.FindByIdExcludes(id);
            if (checkUpReport == null)
            {
                return HttpNotFound();
            }
            return View(checkUpReport);
        }

        // GET: CheckUpReports/Create
        public ActionResult Create()
        {
            ViewBag.FinishedCourses = new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.WishedCourses = new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            
            return View();
        }

        // POST: CheckUpReports/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content,TrainedCourses,WishedCourses")] CheckUpReport checkUpReport, int?[] FinishedCourses,int?[] WishedCourses)
        {
            if (ModelState.IsValid && FinishedCourses!= null && WishedCourses!=null)
            {
                List<TrainingCourse> _finishedCourses = _TrainedCoursesService.FindManyByIdExcludes(FinishedCourses);
                List<TrainingCourse>_whishedCourses = _TrainedCoursesService.FindManyByIdExcludes(WishedCourses);
                _checkUpReport.Update(checkUpReport, _finishedCourses, _whishedCourses);
                return RedirectToAction("Index");
            }

            return View(checkUpReport);
        }

        // GET: CheckUpReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUpReport checkUpReport = _checkUpReport.FindByIdExcludes(id);
            if (checkUpReport == null)
            {
                return HttpNotFound();
            }
            return View(checkUpReport);
        }

        // POST: CheckUpReports/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content")] CheckUpReport checkUpReport)
        {
            if (ModelState.IsValid)
            {
                _checkUpReport.Update(checkUpReport);
                return RedirectToAction("Index");
            }
            return View(checkUpReport);
        }

        // GET: CheckUpReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUpReport checkUpReport = _checkUpReport.FindByIdExcludes(id);
            if (checkUpReport == null)
            {
                return HttpNotFound();
            }
            return View(checkUpReport);
        }

        // POST: CheckUpReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _checkUpReport.Delete(id);
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
