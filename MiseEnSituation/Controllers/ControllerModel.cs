//using MiseEnSituation.Controllers;
//using MiseEnSituation.Repositories;
//using System.Collections.Generic;
//using System.Net;
//using System.Web.Mvc;

//namespace WebApp.Controllers
//{
//    [RoutePrefix("class")]
//    [Route("{action=index}")]
//    public class ClassController : Controller
//    {
//        private MyDbContext db = new MyDbContext();
//        #region Services
//        //private IClassService _ClassService;
//        //private IClassManyToManyService _ClassClassManyToMany;
//        //private IClassOneRequiredToManyService _ClassOneRequiredToManyService;
//        //private IClassOneToOneRequiredService _ClassOneToOneRequiredService;
//        //private IClassManyToOneRequiredService _ClassManyToOneService;
//        //private IClassOneRequiredToManyService _ClassOneRequiredToManyService
//        //#endregion

//        public ClassController()
//        {
//            #region Services Init
//            //_ClassService = new ClassService(new ClassRepository(db));
//            //_ClassManyToManyService = new ClassManyToManyService(new ClassManyToManyRepository(db));
//            //_ClassOneRequiredToManyService = new ClassOneRequiredToManyService(new ClassOneRequiredToManyRepository(db));            
//            //_ClassOneToOneRequiredService = new ClassOneToOneRequiredService(new _ClassOneToOneRequiredRepository(db));
//            //_ClassManyToOneRequiredService = new ClassManyToOneRequiredService(new ClassManyToOneRepository(db));
//            //_ClassOneRequiredToManyService = new ClassOneRequiredToManyService(new ClassOneRequiredToManyRepository(db));
//            #endregion
//        }

//        [HttpGet]
//        [Route("{page?}/{maxByPage?}/{searchField?}")]
//        public ActionResult Index(int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string SearchField = "")
//        {
//            var elements = 0;
//            //List<Class> elements = _ClassService.FindAllIncludes(page, maxByPage, SearchField);
//            //ViewBag.NextExist = _ClassService.NextExist(page, maxByPage, SearchField);
//            ViewBag.Page = page;
//            ViewBag.MaxByPage = maxByPage;
//            ViewBag.SearchField = SearchField;
//            return View("Index", elements);
//        }

//        [HttpGet]
//        [Route("Details/{id}")]
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var element = 0;
//            //Class element = _ClassService.FindByIdIncludes(id);
//            if (element == null)
//            {
//                return HttpNotFound();
//            }
//            return View(element);
//        }

//        [HttpGet]
//        [Route("Create")]
//        public ActionResult Create()
//        {
//            //ViewBag.ClassManyToMany = new MultiSelectList(_ClassManyToManyService.GetAllExcludes(), "Id", "Name", null);
//            //ViewBag.ClassOneToOneRequiredId = new SelectList(_ClassOneToOneRequired.GetAllExcludes(1, int.MaxValue, null, c => c.Class == null), "Id", "Name");
//            //ViewBag.ManyToOneRequiredOwnerId = new SelectList(_ClassManyToOneRequired.GetAllExcludes(), "Id", "Name");
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Route("Create")]
//        #region ClassManyToMany
//        //public ActionResult Create([Bind(Include = "Id,Name")] Class element, int?[] Owners)
//        #endregion
//        #region OneRequiredToMany
//        //public ActionResult Create([Bind(Include = "Id,Name")] Class element, int?[] Owners)
//        #endregion
//        #region ManyToOneRequired
//        //public ActionResult Edit([Bind(Include = "Id,Name")] Class element, int? OwnerId)
//        #endregion
//        #region ClassOneToOneRequired
//        //public ActionResult Create([Bind(Include = "Id, Name")] Class element, int? Id)
//        #endregion
//        public ActionResult Create(object element)
//        {
//            #region ClassManyToMany
//            //if (ModelState.IsValid)
//            //{
//            //    List<ClassManyToMany> owners =  Owners != null ? _ClassManyToManyService.FindManyByIdExcludes(Owners) : null;
//            //    _ClassService.Save(person, owners);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion
//            #region ClassManyToOneRequired
//            //ModelState.Remove("Owner");
//            //if (ModelState.IsValid && OwnerId.HasValue)
//            //{
//            //    element.Owner = _ClassManyToOneRequiredService.FindByIdExcludes(OwnerId);
//            //    _ClassService.Save(element, element.Owner);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion
//            #region ClassOneToOneRequired
//            //ModelState.Remove("Owner");
//            //if (ModelState.IsValid && Id.HasValue)
//            //{
//            //    element.Owner = _ClassOneToOneRequiredService.FindByIdExcludesTracked(Id);
//            //    _classService.Save(element, element.Owner);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion
//            #region ClassOneRequiredToMany
//            //if (ModelState.IsValid)
//            //{
//            //    _ClassService.Save(element);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            //ViewBag.ClassManyToMany = new MultiSelectList(_ClassManyToManyService.GetAllExcludes(), "Id", "Name", null, Class.ClassManyToMany.Select(i => i.Id));
//            //ViewBag.ClassOneToOneRequiredId = new SelectList(_ClassOneToOneRequired.GetAllExcludes(1, int.MaxValue, null, c => c.Class == null), "Id", "Name");
//            //ViewBag.ManyToOneRequiredOwnerId = new SelectList(_ClassManyToOneRequired.GetAllExcludes(), "Id", "Name", Class.OwnerId);
//            //return View(person);
//            return View(element);
//        }

//        [HttpGet]
//        [Route("Edit/{id?}")]
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var element = 0;
//            //Class element = _ClassService.FindByIdIncludes(id);
//            if (element == null)
//            {
//                return HttpNotFound();
//            }
//            //ViewBag.ClassManyToMany = new MultiSelectList(_ClassManyToManyRequired.GetAllExcludes(), "Id", "Name", null, Class.ClassManyToMany.Select(i => i.Id));
//            //ViewBag.ManyToOneRequiredOwnerId = new SelectList(_ClassManyToOneRequired.GetAllExcludes(), "Id", "Name", Class.OwnerId);
//            //TempData["OneRequiredToOne"] = element.OneRequireToOne;
//            //TempData["OneRequiredToMany"] = element.OneRequiredToMany;
//            //TempData.Keep();
//            return View(element);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Route("Edit")]
//        #region ClassManyToMany
//        //public ActionResult Edit([Bind(Include = "Id,Name")] Class element, int?[] Owners)
//        #endregion
//        #region ClassOneRequiredToMany
//        //public ActionResult Edit([Bind(Include = "Id,Name")] Class element)
//        #endregion
//        #region ClassManyToOneRequired
//        //public ActionResult Edit([Bind(Include = "Id,Name")] Class element, int? OwnerId)
//        #endregion
//        #region ClassOneToOneRequired
//        //public ActionResult Edit([Bind(Include = "Id,Name")] Class element)
//        #endregion
//        public ActionResult Edit()
//        {
//            #region ClassManyToMany
//            //if (ModelState.IsValid)
//            //{
//            //    foreach (ClassManyRequiredToMany el in _ClassManyRequiredToManyService.GetAllExcludes(1, int.MaxValue, null, t => !Owners.Contains(t.Id) && t.Owners.Count() == 1 && t.Owners.Where(s => s.Id == element.Id).Count() == 1))
//            //    {
//            //        _ClassManyRequiredToManyService.Delete(el);
//            //    }
//            //    List<ClassManyToMany> owners =  Owners != null ? _ClassManyToManyService.FindManyByIdExcludes(Owners) : null;
//            //    _ClassService.Update(person, owners);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            #region ClassOneRequiredToMany
//            //if (ModelState.IsValid)
//            //{
//            //    List<ClassOneRequiredToMany> objs = TempData["ClassOneRequiredToMany"] as List<ClassOneRequiredToMany>
//            //    _ClassService.Update(element, objs);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            #region ClassManyToOneRequired
//            //ModelState.Remove("Owner");
//            //if (ModelState.IsValid && OwnerId.HasValue)
//            //{
//            //    element.Owner = _ClassManyToOneRequiredService.FindByIdExcludes(OwnerId);
//            //    _ClassService.Update(element, element.Owner);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            #region ClassOneToOneRequired
//            //ModelState.Remove("Owner");
//            //if (ModelState.IsValid)
//            //{
//            //    element.Owner = _ClassOneToOneRequiredService.FindByIdExcludes(element.Id);
//            //    _ClassService.Update(element, element.Owner);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            #region ClassOneRequiredToOne
//            //if (ModelState.IsValid)
//            //{
//            //    ClassOneRequiredToOne obj = TempData["ClassOneRequiredToOne"] as ClassOneRequiredToOne;
//            //    _PersonService.Update(element, obj);
//            //    return RedirectToAction("Index");
//            //}
//            #endregion

//            //ViewBag.ClassManyToMany = new MultiSelectList(_ClassManyToManyService.GetAllExcludes(), "Id", "Name", null, Class.ClassManyToMany.Select(i => i.Id));
//            //ViewBag.ManyToOneRequiredOwnerId = new SelectList(_ClassManyToOneRequired.GetAllExcludes(), "Id", "Name", Class.OwnerId);
//            #region ClassOneRequiredToMany, ClassOneRequiredToOne
//            //TempDate.Keep()
//            #endregion
//            //return View(element);
//            return View();
//        }

//        [HttpGet]
//        [Route("Delete/{id}")]
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var element = 0;
//            //Class element = _ClassService.FindByIdIncludes(id);
//            if (element == null)
//            {
//                return HttpNotFound();
//            }
//            return View(element);
//        }

//        // POST: People/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        [Route("Delete/{id}")]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            //foreach (ClassOneRequiredToMany elementOneRequiredToMany in _ClassOneRequiredToManyService.GetAllExcludes(1, int.MaxValue, null, c => c.OwnerId == id))
//            //{
//            //    _ClassOneRequiredToManyService.Delete(elementOneRequiredToMany);
//            //}
//            //foreach (ClassManyRequiredToMany el in _ManyRequiredToManyService.GetAllExcludes(1, int.MaxValue, null, t => t.Owners.Count() == 1 && t.Owners.Where(s => s.Id == id).Count() == 1))
//            //{
//            //    _ManyRequiredToManyService.Delete(el);
//            //}
//            //_ClassnService.Delete(id);
//            return RedirectToAction("Index");
//        }

//        [HttpGet]
//        [Route("Search")]
//        public ActionResult Search([Bind(Include = ("page, maxByPage, SearchField"))] int page = 1, int maxByPage = MyConstants.MAX_BY_PAGE, string searchField = "")
//        {
//            if (searchField.Trim().Equals(""))
//                return RedirectToAction("Index");
//            return Index(page, maxByPage, searchField);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
