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
    //[AdminFilter]
    [RoutePrefix("Companies")]
    [Route("{action=index}")]
    public class CompanyController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private IGenericService<Company> _companyService;
        public CompanyController()
        {
            _companyService = new CompanyService(new CompanyRepository(db));
        }

        // GET: Company
        public ActionResult Index()
        {

            return View(db.Companies.ToList());
        }

        [HttpGet]
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            List<Company> lstCompanies = _companyService.FindAllIncludes(page, maxByPage, SearchField);

            ViewBag.NextExist = _companyService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", lstCompanies);

        }

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.FindByIdIncludes(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Company/Create
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Adress")] Company company)
        {
            if (ModelState.IsValid)
            {
                _companyService.Save(company);
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Company/Edit/5
        [HttpGet]
        [Route("Edit/{id?}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.FindByIdIncludes(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit([Bind(Include = "Id,Name,Adress")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                _companyService.Update(company);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return View(company);
        }

        // GET: Company/Delete/5
        [HttpGet]
        [Route("Delete/{id?}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyService.FindByIdIncludes(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = _companyService.FindByIdIncludes(id);
            _companyService.Delete(company);
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
