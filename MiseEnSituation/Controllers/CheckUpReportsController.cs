using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;

namespace MiseEnSituation.Controllers
{
    public class CheckUpReportsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: CheckUpReports
        public ActionResult Index()
        {
            return View(db.CheckUpReports.ToList());
        }

        // GET: CheckUpReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckUpReport checkUpReport = db.CheckUpReports.Find(id);
            if (checkUpReport == null)
            {
                return HttpNotFound();
            }
            return View(checkUpReport);
        }

        // GET: CheckUpReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CheckUpReports/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content")] CheckUpReport checkUpReport)
        {
            if (ModelState.IsValid)
            {
                db.CheckUpReports.Add(checkUpReport);
                db.SaveChanges();
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
            CheckUpReport checkUpReport = db.CheckUpReports.Find(id);
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
                db.Entry(checkUpReport).State = EntityState.Modified;
                db.SaveChanges();
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
            CheckUpReport checkUpReport = db.CheckUpReports.Find(id);
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
            CheckUpReport checkUpReport = db.CheckUpReports.Find(id);
            db.CheckUpReports.Remove(checkUpReport);
            db.SaveChanges();
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
