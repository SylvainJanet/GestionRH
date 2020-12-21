using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;

namespace MiseEnSituation.Controllers
{
    [RoutePrefix("addresses")]
    [Route("{action=index}")]
    public class AddressesController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private readonly IAddressService _AddressService;

        public AddressesController()
        {
            _AddressService = new AddressService(new AddressRepository(db));
        }

        [HttpGet]
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            List<Address> elements = _AddressService.FindAllIncludes(page, maxByPage, SearchField);
            ViewBag.NextExist = _AddressService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", elements);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = _AddressService.FindByIdIncludes(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            return View(new Address());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "Number,Street,City,ZipCode,Country")] Address address)
        {
            if (ModelState.IsValid)
            {
                _AddressService.Save(address);
                return RedirectToAction("Index");
            }

            return View(address);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = _AddressService.FindByIdIncludes(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Number,Street,City,ZipCode,Country")] Address address)
        {
            if (ModelState.IsValid)
            {
                _AddressService.Update(address); 
                return RedirectToAction("Index");
            }
            return View(address);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = _AddressService.FindByIdIncludes(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           Address address= _AddressService.FindByIdIncludes(id);
            _AddressService.Delete(address);
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
