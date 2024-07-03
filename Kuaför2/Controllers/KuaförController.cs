using Kuaför2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kuaför2.Controllers
{
    public class KuaförController : Controller
    {
        KuaförEntities25 db = new KuaförEntities25();

        public ActionResult Kuaförekle()
        {
            Class1 model = new Class1();
            List<İl> sehirList = db.İl.OrderBy(f => f.İlAdı).ToList();
            model.SehirList = (from s in sehirList
                               select new SelectListItem
                               {
                                   Text = s.İlAdı,
                                   Value = s.ID.ToString()

                               }).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Kuaförekle(FormCollection datafc)
        {
            string eposta = datafc["eposta"];
            Kuaför Kf = new Kuaför();
            Kf.AD = datafc["ad"];
            Kf.il = (string)TempData["il"];
            Kf.ilçe = (string)TempData["ilçe"];
            Kf.Adres = datafc["adres"];
            Kf.Eposta = datafc["eposta"];
            Kf.Telefon_numarası = datafc["tel"];
            Kf.kapanış_saati = datafc["kapanışsaati"];
            Kf.açılış_saati = datafc["açılışsaati"];
            Kf.şifre = datafc["şifre"];
            Kf.Konum = datafc["konum"];
            Kf.puan = 0;

            db.Kuaför.Add(Kf);
            db.SaveChanges();


            return RedirectToAction("KuaförGiriş");
        }
        [HttpGet]
        public ActionResult KuaförGiriş()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KuaförGiriş(Kuaför k, FormCollection dtn)
        {
            var eposta = dtn["AD"];
            var sifre = dtn["şifre"];
            var kuaför = db.Kuaför.FirstOrDefault(x => x.Eposta == eposta && x.şifre ==
            sifre);
            if (kuaför != null)
            {
                TempData["kuaförrid"] = kuaför.ID;
                FormsAuthentication.SetAuthCookie(kuaför.Eposta, false);
                Session["kuaföreposta"] = kuaför.Eposta;

                return RedirectToAction("KuaförBilgi");

            }

            ViewBag.hata = "Hatalı Eposta veya şifre girdiniz!";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("kuaföreposta");
            FormsAuthentication.SignOut();
            return RedirectToAction("KuaförGiriş");
        }

        public ActionResult KuaförBilgi()
        {
            string eposta = (string)Session["kuaföreposta"];
            List<Kuaför> KuaförBilgiler = db.Kuaför.Where(i => i.Eposta == eposta).ToList(); // Assuming you are using Identity framework

            return View(KuaförBilgiler);
        }
        [HttpPost]
        public ActionResult KuaförBilgi(FormCollection datafc)
        {
            string eposta = (string)Session["kuaföreposta"];
            Kuaför Kf = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
            Kf.AD = datafc["ad"];
            Kf.Adres = datafc["adres"];
            Kf.Eposta = datafc["eposta"];
            Kf.Telefon_numarası = datafc["tel"];
            Kf.kapanış_saati = datafc["kapanışsaati"];
            Kf.açılış_saati = datafc["açılışsaati"];
            db.SaveChanges();

            return RedirectToAction("KuaförBilgi");
        }

    }
}
