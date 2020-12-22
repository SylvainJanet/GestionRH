using System;
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
    //[AdminFilter]
    [RoutePrefix("Posts")]
    [Route("{action=index}")]
    public class PostController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        private readonly IGenericService<Post> _postService;
        private readonly IGenericService<Company> _companyService;

        public List<Post> LstPost { get; set; }
        public PostController()
        {
            _postService = new PostService(new PostRepository(db));
            _companyService = new CompanyService(new CompanyRepository(db));

        }
        // GET: Post
        public ActionResult Index()
        {
            return View(_postService.GetAll(true,true,1,8541));
        }


        [HttpGet]
        [Route("{page?}/{maxByPage?}/{searchField?}")]
        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
        {
            LstPost = _postService.FindAllIncludes(page, maxByPage, SearchField);

            ViewBag.NextExist = _postService.NextExist(page, maxByPage, SearchField);
            ViewBag.Page = page;
            ViewBag.MaxByPage = maxByPage;
            ViewBag.SearchField = SearchField;
            return View("Index", LstPost);
        }
        // GET: Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = _postService.FindByIdIncludes(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Post/Create
        [HttpGet]
        public ActionResult Create()
        {
            
            ViewBag.CompanyList = _companyService.FindAllExcludes(1, 5412, "");
            //companyService.FindAllIncludes(page, maxByPage, SearchField);

            return View(new Post());
        }

        // POST: Post/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,HiringDate,ContractType,EndDate,WeeklyWorkLoad,FileForContract,CompanyId")] Post post)
        {

            if (post.CompanyId.HasValue)
            {
                post.Company = _companyService.FindByIdIncludes(post.CompanyId);
            }
            ModelState.Remove("Company");

            if (ModelState.IsValid)
            {
                _postService.Save(post);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }

            return View(post);
        }

        // GET: Post/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = _postService.FindByIdIncludes(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyList = _companyService.FindAllExcludes(1, 5412, "");
            return View(post);
        }

        // POST: Post/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,HiringDate,ContractType,EndDate,WeeklyWorkLoad,FileForContract,CompanyId")] Post post)
        {
            if (post.CompanyId.HasValue)
            {
                post.Company = _companyService.FindByIdIncludes(post.CompanyId);
            }
            if (ModelState.IsValid)
            {
                _postService.Save(post);
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Post/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = _postService.FindByIdIncludes(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = _postService.FindByIdIncludes(id);
            _postService.Delete(post);
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
