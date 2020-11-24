using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    public class TestDataController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: TestData
        public ActionResult Index()
        {
            //User u1 = new User() { Name = "Toto", Email = "toto@dawan.fr", Password = HashTools.ComputeSha256Hash("toto") };
            //User u2 = new User() { Name = "Tata", Email = "tata@dawan.fr", Password = HashTools.ComputeSha256Hash("tata") };
            //User u3 = new User() { Name = "Titi", Email = "titi@dawan.fr", Password = HashTools.ComputeSha256Hash("titi") };
            //User u4 = new User() { Name = "Tutu", Email = "tutu@dawan.fr", Password = HashTools.ComputeSha256Hash("tutu") };
            //User u5 = new User() { Name = "Tooto", Email = "tooto@dawan.fr", Password = HashTools.ComputeSha256Hash("tooto") };
            //User u6 = new User() { Name = "Taata", Email = "taata@dawan.fr", Password = HashTools.ComputeSha256Hash("taata") };
            //User u7 = new User() { Name = "Tiiti", Email = "tiiti@dawan.fr", Password = HashTools.ComputeSha256Hash("tiiti") };
            //User u8 = new User() { Name = "Tuutu", Email = "tuutu@dawan.fr", Password = HashTools.ComputeSha256Hash("tuutu") };
            //User u9 = new User() { Name = "Totoo", Email = "totoo@dawan.fr", Password = HashTools.ComputeSha256Hash("totoo") };
            //User u10 = new User() { Name = "Tataa", Email = "tataa@dawan.fr", Password = HashTools.ComputeSha256Hash("tataa") };
            //User u11 = new User() { Name = "Titii", Email = "titii@dawan.fr", Password = HashTools.ComputeSha256Hash("titii") };
            //User u12 = new User() { Name = "Tutuu", Email = "tutuu@dawan.fr", Password = HashTools.ComputeSha256Hash("tutuu") };
            //User u13 = new User() { Name = "Tootoo", Email = "tootoo@dawan.fr", Password = HashTools.ComputeSha256Hash("tootoo") };
            //User u14 = new User() { Name = "Taataa", Email = "taataa@dawan.fr", Password = HashTools.ComputeSha256Hash("taataa") };
            //User u15 = new User() { Name = "Tiitii", Email = "tiitii@dawan.fr", Password = HashTools.ComputeSha256Hash("tiitii") };
            //User u16 = new User() { Name = "Tuutuu", Email = "tuutuu@dawan.fr", Password = HashTools.ComputeSha256Hash("tuutuu") };


            //db.Users.Add(u1);
            //db.Users.Add(u2);
            //db.Users.Add(u3);
            //db.Users.Add(u4);
            //db.Users.Add(u5);
            //db.Users.Add(u6);
            //db.Users.Add(u7);
            //db.Users.Add(u8);
            //db.Users.Add(u9);
            //db.Users.Add(u10);
            //db.Users.Add(u11);
            //db.Users.Add(u12);
            //db.Users.Add(u13);
            //db.Users.Add(u14);
            //db.Users.Add(u15);
            //db.Users.Add(u16);
            //db.SaveChanges();
            return RedirectToAction("Index", "Home");
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