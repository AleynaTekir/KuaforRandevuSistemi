using Kuaför2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Controllers
{
    public class IlilceController : Controller
    {
        // GET: Ilce
        KuaförEntities25 db = new KuaförEntities25();
        // GET: Ililce
        public ActionResult liste()
        {

            Class1 model = new Class1();
            List<İl> sehirList = db.İl.OrderBy(f => f.İlAdı).ToList();
            model.SehirList = (from s in sehirList
                               select new SelectListItem
                               {
                                   Text = s.İlAdı,
                                   Value = s.ID.ToString()

                               }).ToList();
            islemmodel by = new islemmodel();



            model.KuaförList = db.Kuaför.ToList();

            return View(model);

        }
        [HttpPost]
        public JsonResult IlceList(int id)
        {

            List<İlçe> ilceList = db.İlçe.Where(k => k.İlID == id).OrderBy(f => f.ID).ToList();
            List<SelectListItem> itemList = (from i in ilceList
                                             select new SelectListItem
                                             {
                                                 Value = i.ID.ToString(),
                                                 Text = i.İlçeAdı
                                             }).ToList();
            var Il = db.İl.Where(x => x.ID == id).FirstOrDefault();
            TempData["il"] = Il.İlAdı;
            return Json(itemList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult list(int id)
        {


            var ilceList = db.İlçe.Where(k => k.ID == id).FirstOrDefault();

            TempData["ilçe"] = ilceList.İlçeAdı;
            var p = ilceList;
            var values = from x in db.Kuaför select x;

            return RedirectToAction("list");
        }
        [HttpPost]
        public ActionResult liste(FormCollection dtn)
        {
            Class1 model = new Class1();
            var ilçe = TempData["ilçe"];
            var il = TempData["il"];

            List<İl> sehirList = db.İl.OrderBy(f => f.İlAdı).ToList();
            model.SehirList = (from s in sehirList
                               select new SelectListItem
                               {
                                   Text = s.İlAdı,
                                   Value = s.ID.ToString()

                               }).ToList();


            model.KuaförList = db.Kuaför.Where(k => k.il == il && k.ilçe == ilçe).ToList();

            return View(model);
        }
    }
}
