﻿using System;
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
    [RoutePrefix("CheckUpReports")]
    [Route("{action=index}")]
    public class CheckUpReportsController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly IGenericService<CheckUpReport> _checkUpReport;
        private readonly IGenericService<TrainingCourse> trainingCourseService;
        public CheckUpReportsController()
        {
            _checkUpReport = new CheckUpReportService(new CheckUpReportRepository(db));
            trainingCourseService = new TrainingCourseService(new TrainingCourseRepository(db));
           
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

            IEnumerable<SelectListItem> finished =  new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            IEnumerable<SelectListItem> wished =  new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.FinishedCourses = finished;
            ViewBag.WishedCourses = wished;
            
            return View(new CheckUpReport());
        }

        // POST: CheckUpReports/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content,TrainedCourses,WishedCourses")] CheckUpReport checkUpReport, int?[] FinishedCourses,int?[] WishedCourses)
        {
            List<TrainingCourse> _finishedCourses;
            List<TrainingCourse> _whishedCourses;
            if (FinishedCourses != null)
            {
                _finishedCourses = trainingCourseService.FindManyByIdExcludes(FinishedCourses);
                checkUpReport.FinishedCourses = _finishedCourses;
            }
            if (WishedCourses!=null)
            {
                _whishedCourses = trainingCourseService.FindManyByIdExcludes(WishedCourses);
                checkUpReport.WishedCourses = _whishedCourses;
            }
            else
            {
                checkUpReport.FinishedCourses = null;
            }
            // _checkUpReport.Update(checkUpReport, _finishedCourses, _whishedCourses);
            
            
            if (ModelState.IsValid )
            {
                _checkUpReport.Save(checkUpReport);
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
            IEnumerable<SelectListItem> finished = new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            IEnumerable<SelectListItem> wished = new MultiSelectList(trainingCourseService.GetAllExcludes(), "Id", "Name", null);
            ViewBag.FinishedCourses = finished;
            ViewBag.WishedCourses = wished;
            return View(checkUpReport);
        }

        // POST: CheckUpReports/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,TrainedCourses,WishedCourses")] CheckUpReport checkUpReport, int?[] FinishedCourses, int?[] WishedCourses)
        {
            List<TrainingCourse> _finishedCourses;
            List<TrainingCourse> _whishedCourses;
            if (FinishedCourses != null)
            {
                _finishedCourses = trainingCourseService.FindManyByIdExcludes(FinishedCourses);
                checkUpReport.FinishedCourses = _finishedCourses;
            }
            if (WishedCourses != null)
            {
                _whishedCourses = trainingCourseService.FindManyByIdExcludes(WishedCourses);
                checkUpReport.WishedCourses = _whishedCourses;
            }
            else
            {
                checkUpReport.FinishedCourses = null;
            }


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

            CheckUpReport checkUpReport = _checkUpReport.FindByIdExcludes(id);
            _checkUpReport.Delete(checkUpReport);
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
