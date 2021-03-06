﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GenericRepositoryAndService.Service;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;

namespace MiseEnSituation.Controllers
{
    //[AdminFilter]
    [RoutePrefix("Companies")]
    [Route("{action=index}")]
    public class CompanyController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly IGenericService<Company> _companyService;
        private readonly IGenericService<Address> _adressService;
        public CompanyController()
        {
            _companyService = new CompanyService(new CompanyRepository(db));
            _adressService = new AddressService(new AddressRepository(db));
        }

        // GET: Company
        public ActionResult Index()
        {

            return View(_companyService.GetAll(true, true, 1,5241));
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
        [HttpGet]
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
            _adressService.Save(company.Adress);
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
            var addressSave = _companyService.FindByIdIncludes(company.Id).Adress;
            _adressService.Save(company.Adress);
            if (ModelState.IsValid)
            {

                _companyService.Update(company);
                _adressService.Delete(addressSave);
                return RedirectToAction("Index");
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

            Address address = _adressService.FindByIdIncludes(company.Adress.Id);
            

            _companyService.Delete(company);
            _adressService.Delete(address);
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
