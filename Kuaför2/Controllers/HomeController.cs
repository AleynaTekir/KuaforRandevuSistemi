using Kuaför2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kuaför2.Controllers
{
    public class HomeController : Controller
    {
        KuaförEntities25 db = new KuaförEntities25();


    
    public ActionResult HizmetKayit()
    {
        List<işlemler> işlemler = db.işlemler.ToList();
        return View(işlemler);
    }
    [HttpPost]
    public ActionResult HizmetKayit(int id, FormCollection dtn)
    {
        string eposta = (string)Session["kuaföreposta"];

        var sorgu = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
        int Kuaförid = sorgu.ID;

        işlem_ücret işlem_1 = db.işlem_ücret.Where(i => i.kuaför == Kuaförid && i.işlem == id).FirstOrDefault();
        if (işlem_1 != null)
        {
            ViewBag.mesaj = "Eklemek istediğini işlem zaten mevcut!";

        }
        else
        {
            işlem_ücret işlem_ = new işlem_ücret();
            işlem_.ücret = Convert.ToInt32(dtn["işlemfiyat2"]);
            işlem_.kuaför = Kuaförid;
            işlem_.işlem = id;
            db.işlem_ücret.Add(işlem_);
            db.SaveChanges();
            ViewBag.mesaj = "işleminiz kayıt olmuştur!";
        }
        List<işlemler> işlemler = db.işlemler.ToList();
        return View(işlemler);
    }
    [HttpPost]
    public ActionResult hizmetler(int id, FormCollection dtn)
    {
        int Kuaförid = Convert.ToInt32(TempData["kuaförid"]);

        işlem_ücret işlem_1 = db.işlem_ücret.Where(i => i.kuaför == Kuaförid && i.işlem == id).FirstOrDefault();


        var işlemfiyat = Convert.ToInt32(dtn["işlemfiyat"]);
        işlem_1.ücret = işlemfiyat;
        db.SaveChanges();

        return RedirectToAction("hizmetler");
    }
    public ActionResult Kuaförrandevu(string inputvalue)
    {
        var model = new ModelRandevu();
        string eposta = (string)Session["kuaföreposta"];

        var sorgu = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
        int id = sorgu.ID;
        model.Items = new List<SelectListItem>();
        List<SelectListItem> items = new List<SelectListItem>();
        //var sorgu = db.işlem.Where(i => i.kuaför == id).FirstOrDefault();
        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();



        model.CheckBoxValues = (List<SelectListItem>)(from s in işlem_ücret
                                                      join st in işlemler on s.işlem equals st.ID into st2
                                                      from st in st2.DefaultIfEmpty()

                                                      where (s.kuaför == id)

                                                      select new SelectListItem { Text = st.işlem_türü, Value = st.ID.ToString() }).ToList();





        for (int i = 1; i <= 19; i++)
        {

            string time = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(i * 30)).ToString(@"hh\:mm");
            model.Items.Add(new SelectListItem { Value = i.ToString(), Text = time, Disabled = true });
        }

        return View(model);
    }
    [HttpPost]
    public ActionResult Kuaförrandevu(FormCollection dtn, ModelRandevu fixedTime)
    {
        string eposta = (string)Session["kuaföreposta"];

        var sorgu1 = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
        int id = sorgu1.ID;
        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();
        var model = new ModelRandevu();


        model.CheckBoxValues = (List<SelectListItem>)(from s in işlem_ücret
                                                      join st in işlemler on s.işlem equals st.ID into st2
                                                      from st in st2.DefaultIfEmpty()

                                                      where (s.kuaför == id)

                                                      select new SelectListItem { Text = st.işlem_türü, Value = st.ID.ToString() }).ToList();





        string tel = dtn["tel"];
        var sorgu2 = db.müşteri.Where(i => i.Telefon_numarası == tel).FirstOrDefault();

        model.Items = new List<SelectListItem>();
        List<SelectListItem> items = new List<SelectListItem>();
        int idd = fixedTime.SelectedId;

        var tarih = dtn["tarih"];
        if (tarih != null)
        {
            model.tarih = tarih;
        }
        TempData["tarih"] = tarih;
        ViewBag.mesaj = tarih;
        var sorgu = db.randevu.Where(i => i.Kuaför == id && i.tarih == tarih).ToList();
        ;
        List<string> databaseValues = sorgu.Select(x => x.saat).ToList();
        List<DateTime> dateTimeList = new List<DateTime>();
        List<string> saatler = new List<string>();
        foreach (var item in databaseValues)
        {
            DateTime dateTime = DateTime.Parse(item);
            dateTimeList.Add(dateTime);

        }
        foreach (var item in dateTimeList)
        {
            string saat = item.ToString("HH:mm");
            saatler.Add(saat);


        }
        bool CompareListWithData(string time)
        {


            bool değişken1 = saatler.Contains(time);
            return değişken1;


        }
        for (int i = 1; i <= 19; i++)
        {

            string time = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(i * 30)).ToString(@"hh\:mm");
            model.Items.Add(new SelectListItem { Value = i.ToString(), Text = time, Disabled = CompareListWithData(time) });
        }
        randevu randevu = new randevu();
        dtn["tarih"] = tarih;
        string saati = null;
        bool randevuVarM = db.randevu.Any(r => r.tarih == tarih && r.saat == saati && r.Kuaför == id);
        if (!randevuVarM)
        {
            if (idd != 0)
            {

                saati = model.Items[idd - 1].Text;
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {


                    bool randevuVarMi = db.randevu.Any(r => r.tarih == tarih && r.saat == saati && r.Kuaför == id);


                    if (randevuVarMi)
                    {
                        transaction.Rollback();

                    }

                    else
                    {
                        randevu.müşteri = sorgu2.Id;
                        randevu.Kuaför = id;
                        randevu.tarih = tarih;
                        randevu.saat = saati.ToString();
                        db.randevu.Add(randevu);
                        db.SaveChanges();
                        ViewBag.tarih = tarih;
                        ViewBag.saat = saati.ToString();
                        idd = 0;
                        transaction.Commit();

                    }



                }
            }
        }

        if (fixedTime.SelectedValues != null)
        {
            int tutar = 0;
            var sorgu3 = db.randevu.Where(i => i.Kuaför == id && i.tarih == tarih && i.saat == saati).FirstOrDefault();
            List<string> işlemlist = new List<string>();
            List<int> selectedValues = fixedTime.SelectedValues;
            foreach (var item in selectedValues)
            {
                var sorgu5 = db.işlemler.Where(i => i.ID == item).FirstOrDefault();
                işlemlist.Add(sorgu5.işlem_türü);

                var sorguücret = db.işlem_ücret.Where(i => i.kuaför == id && i.işlem == item).FirstOrDefault();
                tutar = tutar + (int)sorguücret.ücret;
                randevu_işlem randevu_ = new randevu_işlem();
                randevu_.randevu = sorgu3.ID;
                randevu_.işlem = item;

                db.randevu_işlem.Add(randevu_);

                db.SaveChanges();
                ViewBag.değer = item;

            }
            int randevid = sorgu3.ID + 1;
            var sorgu4 = db.randevu.Where(i => i.ID == randevid).FirstOrDefault();

            db.SaveChanges();
            ViewBag.kayıt = "Randevunuz başarıyla oluşturulmuştur";
            ViewBag.işlemler = işlemlist;
            ViewBag.tutar = tutar.ToString() + "TL";
            randevu.tutar = tutar;
            ViewBag.tarih = tarih;
            ViewBag.saat = saati.ToString();
        }




        return View(model);

    }
    [HttpPost]
    public ActionResult Randevumusteri(int id, FormCollection dtn, ModelRandevu fixedTime)
    {
        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();
        var model = new ModelRandevu();


        model.CheckBoxValues = (List<SelectListItem>)(from s in işlem_ücret
                                                      join st in işlemler on s.işlem equals st.ID into st2
                                                      from st in st2.DefaultIfEmpty()

                                                      where (s.kuaför == id)

                                                      select new SelectListItem { Text = st.işlem_türü, Value = st.ID.ToString() }).ToList();





        string eposta = (string)Session["musterieposta"];
        var sorgu1 = db.müşteri.Where(i => i.eposta == eposta).FirstOrDefault();

        model.Items = new List<SelectListItem>();
        List<SelectListItem> items = new List<SelectListItem>();
        int idd = fixedTime.SelectedId;


        var tarih = dtn["tarih"];
        if (tarih != null)
        {
            model.tarih = tarih;
        }
        TempData["tarih"] = tarih;
        ViewBag.mesaj = tarih;
        var sorgu = db.randevu.Where(i => i.Kuaför == id && i.tarih == tarih).ToList();

        List<string> databaseValues = sorgu.Select(x => x.saat).ToList();
        List<DateTime> dateTimeList = new List<DateTime>();
        List<string> saatler = new List<string>();
        foreach (var item in databaseValues)
        {
            DateTime dateTime = DateTime.Parse(item);
            dateTimeList.Add(dateTime);

        }
        foreach (var item in dateTimeList)
        {
            string saat = item.ToString("HH:mm");
            saatler.Add(saat);


        }
        bool CompareListWithData(string time)
        {



            bool değişken1 = saatler.Contains(time);
            return değişken1;



        }
        for (int i = 1; i <= 19; i++)
        {

            string time = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(i * 30)).ToString(@"hh\:mm");
            model.Items.Add(new SelectListItem { Value = i.ToString(), Text = time, Disabled = CompareListWithData(time) });
        }
        randevu randevu = new randevu();
        dtn["tarih"] = tarih;
        string saati = null;
        bool randevuVarM = db.randevu.Any(r => r.tarih == tarih && r.saat == saati && r.Kuaför == id);
        if (!randevuVarM)
        {
            if (idd != 0)
            {

                saati = model.Items[idd - 1].Text;
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {

                    bool randevuVarMi = db.randevu.Any(r => r.tarih == tarih && r.saat == saati && r.Kuaför == id);


                    if (randevuVarMi)
                    {
                        transaction.Rollback();

                    }

                    else
                    {
                        randevu.müşteri = sorgu1.Id;
                        randevu.Kuaför = id;
                        randevu.tarih = tarih;
                        randevu.saat = saati.ToString();
                        db.randevu.Add(randevu);
                        db.SaveChanges();
                        ViewBag.tarih = tarih;
                        ViewBag.saat = saati.ToString();
                        idd = 0;
                        transaction.Commit();

                    }



                }
            }
        }

        if (fixedTime.SelectedValues != null)
        {
            int tutar = 0;
            var sorgu3 = db.randevu.Where(i => i.Kuaför == id && i.tarih == tarih && i.saat == saati).FirstOrDefault();
            List<string> işlemlist = new List<string>();
            List<int> selectedValues = fixedTime.SelectedValues;
            foreach (var item in selectedValues)
            {
                var sorgu5 = db.işlemler.Where(i => i.ID == item).FirstOrDefault();
                işlemlist.Add(sorgu5.işlem_türü);

                var sorgu2 = db.işlem_ücret.Where(i => i.kuaför == id && i.işlem == item).FirstOrDefault();
                tutar = tutar + (int)sorgu2.ücret;
                randevu_işlem randevu_ = new randevu_işlem();
                randevu_.randevu = sorgu3.ID;
                randevu_.işlem = item;

                db.randevu_işlem.Add(randevu_);

                db.SaveChanges();
                ViewBag.değer = item;

            }
            int randevid = sorgu3.ID + 1;
            var sorgu4 = db.randevu.Where(i => i.ID == randevid).FirstOrDefault();
            // db.randevu.Remove(sorgu4);
            db.SaveChanges();
            ViewBag.kayıt = "Randevunuz başarıyla oluşturulmuştur";
            ViewBag.işlemler = işlemlist;
            ViewBag.tutar = tutar.ToString() + "TL";
            randevu.tutar = tutar;
            ViewBag.tarih = tarih;
            ViewBag.saat = saati.ToString();
        }


        return View(model);

    }

    public ActionResult Randevumusteri(int id, string inputvalue)
    {
        var model = new ModelRandevu();
        model.Items = new List<SelectListItem>();
        List<SelectListItem> items = new List<SelectListItem>();

        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();



        model.CheckBoxValues = (List<SelectListItem>)(from s in işlem_ücret
                                                      join st in işlemler on s.işlem equals st.ID into st2
                                                      from st in st2.DefaultIfEmpty()

                                                      where (s.kuaför == id)

                                                      select new SelectListItem { Text = st.işlem_türü, Value = st.ID.ToString() }).ToList();





        for (int i = 1; i <= 19; i++)
        {

            string time = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(i * 30)).ToString(@"hh\:mm");
            model.Items.Add(new SelectListItem { Value = i.ToString(), Text = time, Disabled = true });
        }


        return View(model);

    }
    [HttpPost]
    public ActionResult saaati(ModelRandevu SelecValue)
    {

        int selectvalue = SelecValue.SelectedId;
        var değer = SelecValue.Items[selectvalue];


        string value = SelecValue.Items.ToString();
        return View();
    }

    [HttpPost]
    public JsonResult YourActionName(string inputValue)
    {
        TempData["tarih"] = inputValue;


        return Json(inputValue, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    public JsonResult saat(string inputData)
    {
        TempData["saat"] = inputData;


        return Json(inputData, JsonRequestBehavior.AllowGet);
    }

    public JsonResult IlceList(int id)
    {
        TempData["id"] = id;


        return Json(id, JsonRequestBehavior.AllowGet);
    }


    [HttpPost]
    public ActionResult Randevulist(FormCollection dtn)
    {

        string eposta = (string)Session["musterieposta"];
        var sorgu = db.müşteri.Where(i => i.eposta == eposta).FirstOrDefault();
        var sorgu2 = db.randevu.Where(i => i.müşteri == sorgu.Id).FirstOrDefault();
        var sorgu3 = db.Kuaför.Where(İ => İ.ID == sorgu2.Kuaför).FirstOrDefault();
        var sorgu5 = db.randevu.Where(i => i.tarih == sorgu2.tarih && i.saat == sorgu2.saat && i.Kuaför == sorgu3.ID).FirstOrDefault();




        değerlendirme değerlendirme1 = new değerlendirme();
        var sorgu6 = db.müşteri.Where(i => i.eposta == eposta).FirstOrDefault();
        değerlendirme1.Müşteri = sorgu6.Id;
        değerlendirme1.Kuaför = sorgu5.Kuaför;
        değerlendirme1.Yorum = dtn["yorum"];
        değerlendirme1.Puan = Convert.ToInt32(dtn["ratingValue"]);
        db.değerlendirme.Add(değerlendirme1);
        db.SaveChanges();
        var sorgus = db.değerlendirme.Where(i => i.Kuaför == sorgu5.Kuaför).Average(i => i.Puan);
        Kuaför K = db.Kuaför.Where(İ => İ.ID == sorgu2.Kuaför).FirstOrDefault();/* new Kuaför();*/
        K.puan = sorgus;
        db.SaveChanges();
        return RedirectToAction("Randevulist");
    }



    public ActionResult Randevulist()
    {



        List<müşteri> müşteri = db.müşteri.ToList();
        List<randevu> randevu = db.randevu.ToList();
        List<Kuaför> Kuaför = db.Kuaför.ToList();
        List<işlemler> işlem = db.işlemler.ToList();
        String eposta = (string)Session["musterieposta"];
        var sorgu = db.müşteri.Where(i => i.eposta == eposta).FirstOrDefault();
        var sorgu2 = db.randevu.Where(i => i.müşteri == sorgu.Id).FirstOrDefault();



        if (sorgu2 != null)
        {

            var sorgu4 = db.randevu.Where(i => i.müşteri == sorgu.Id).ToList();

            var Randev = from s in randevu
                         join st in Kuaför on s.Kuaför equals st.ID into st2
                         from st in st2.DefaultIfEmpty()

                         where (sorgu2.müşteri == s.müşteri)

                         select new randev
                         {

                             kuaförvm = st,
                             randevvm = s,

                         };




            return View(Tuple.Create(Randev));

        }
        else
        {
            ViewBag.mesaj = "henüz geçmiş bir randevunuz bulunmamaktadır.";
        }
        return View();
    }
    public ActionResult RandevularKuafor()
    {
        List<randevu_işlem> randevu_İşlems = db.randevu_işlem.ToList();
        List<müşteri> müşteri = db.müşteri.ToList();
        List<randevu> randevu = db.randevu.ToList();
        List<Kuaför> Kuaför = db.Kuaför.ToList();
        string eposta = (string)Session["kuaföreposta"];
        List<string> işlemler = new List<string>();
        var sorgu = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
        var sorgu2 = db.randevu.Where(i => i.Kuaför == sorgu.ID).FirstOrDefault();
        List<randevu> randevular = db.randevu.Where(i => i.Kuaför == sorgu.ID).ToList();
        foreach (var item in randevular)
        {
            List<randevu_işlem> işlem = db.randevu_işlem.Where(i => i.randevu == item.ID).ToList();
            foreach (var item2 in işlem)
            {
                var işlemçek = db.işlemler.Where(i => i.ID == item2.işlem).FirstOrDefault();
                işlemler.Add(işlemçek.işlem_türü);
            }


        }

        if (sorgu2 != null)
        {
            var Randevu = from s in randevu
                          join st in müşteri on s.müşteri equals st.Id into st2
                          from st in st2.DefaultIfEmpty()

                          where (sorgu.ID == s.Kuaför)

                          select new RAndevu
                          {
                              randevu_İşlemvm = db.randevu_işlem.Where(i => i.randevu == s.ID).FirstOrDefault(),

                              müşterivm = st,
                              randevvm = s,

                          };


            return View(Tuple.Create(Randevu));

        }
        else
        {
            ViewBag.mesaj = "henüz geçmiş bir randevunuz bulunmamaktadır.";
        }
        return View();
    }

    public ActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public ActionResult Index(FormCollection dtn)
    {

        return View();

    }
    public ActionResult değerlendirmedüzen()
    {
        List<değerlendirme> değerlendirmes = db.değerlendirme.ToList();
        List<Kuaför> kuaförs = db.Kuaför.ToList();
        List<müşteri> müşteris = db.müşteri.ToList();

        var eposta = Session["musterieposta"];
        var sorgu = db.müşteri.FirstOrDefault(i => i.eposta == eposta);
        var sorgu2 = değerlendirmes.Where(i => i.Müşteri == sorgu.Id).ToList();
        ViewBag.mesaj = "değerlendirmeniz bulunmamaktadır.";

        var degerlendirme = from s in değerlendirmes
                            join st in kuaförs on s.Kuaför equals st.ID into st2
                            from st in st2.DefaultIfEmpty()

                            where (s.Müşteri == sorgu.Id)
                            select new Degerlendirme
                            {
                                kuaförvm = st,
                                değerlendirmevm = s,

                            };
        return View(degerlendirme);
    }


    public ActionResult Kuaförkayıt()
    {
        return View();
    }
    public ActionResult Ekle()
    {
        return View();
    }
    [HttpPost]
    public ActionResult Ekle(FormCollection datafc)
    {
        müşteri m = new müşteri();
        m.Ad = datafc["ad"];
        m.Soyad = datafc["soyad"];
        m.eposta = datafc["e mail"];
        m.Telefon_numarası = datafc["tel"];
        m.şifre = datafc["şifre"];
        db.müşteri.Add(m);
        db.SaveChanges();

        return RedirectToAction("Musterigiris");
    }
    [HttpGet]
    public ActionResult Musterigiris()
    {
        return View();
    }


    [HttpPost]
    public ActionResult Musterigiris(müşteri k, FormCollection dtn)

    {
        var eposta = dtn["AD"];
        var sifre = dtn["şifre"];
        var musteri = db.müşteri.FirstOrDefault(x => x.eposta == eposta && x.şifre == sifre);
        if (musteri != null)
        {
            FormsAuthentication.SetAuthCookie(musteri.eposta, false);//k.ad
            Session["musterieposta"] = musteri.eposta;

            return RedirectToAction("Randevulist");

        }

        ViewBag.hata = "Hatalı Epost yada şifre girdiniz";
        return View();

    }
    public ActionResult MusteriLogout()
    {
        Session.Remove("musterieposta");
        FormsAuthentication.SignOut();

        return RedirectToAction("Musterigiris");
    }









    public ActionResult hizmetler()
    {
        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();
        List<Kuaför> Kuaför = db.Kuaför.ToList();
        string eposta = (string)Session["kuaföreposta"];
        var sorgu = db.Kuaför.Where(i => i.Eposta == eposta).FirstOrDefault();
        TempData["kuaförid"] = sorgu.ID;
        if (sorgu != null)
        {
            var joinAndViewModel = from s in işlem_ücret
                                   join st in işlemler on s.işlem equals st.ID into st2
                                   from st in st2.DefaultIfEmpty()

                                   where (s.kuaför == sorgu.ID)
                                   select new islemmodel
                                   {
                                       işlemlervm = st,
                                       işlemücretvm = s,


                                   }
                                   ;





            return View(joinAndViewModel);
        }
        return View();
    }
    public ActionResult Layout2()
    {


        return View();
    }



    public ActionResult Kuafördetay(int id)

    {

        değerlendirme değerlendirme1 = new değerlendirme();

        List<müşteri> müşteri = db.müşteri.ToList();
        List<Kuaför> Kuaförs = db.Kuaför.ToList();
        List<değerlendirme> değerlendirme = db.değerlendirme.ToList();
        List<işlemler> işlemler = db.işlemler.ToList();
        List<işlem_ücret> işlem_ücret = db.işlem_ücret.ToList();

        var sorgu = db.Kuaför.Where(i => i.ID == id).ToList();

        if (sorgu != null)
        {


            var islemmodel = from s in işlem_ücret
                             join st in işlemler on s.işlem equals st.ID into st2
                             from st in st2.DefaultIfEmpty()

                             where (s.kuaför == id)
                             select new islemmodel
                             {
                                 işlemlervm = st,
                                 işlemücretvm = s,

                             };

            var değerlendirmemodels = from s in değerlendirme
                                      join st in Kuaförs on s.Kuaför equals st.ID into st2
                                      from st in st2.DefaultIfEmpty()
                                      join t in müşteri on s.Müşteri equals t.Id into t2
                                      from t in t2.DefaultIfEmpty()
                                      where (s.Kuaför == id)
                                      select new değerlendirmemodel
                                      {
                                          kuaförvm = st,
                                          değerlendirmevm = s,
                                          müşterivm = t,

                                      };


            var sorgus = db.değerlendirme.Where(i => i.Kuaför == id).Average(i => i.Puan);


            ViewBag.QueryResult = Convert.ToInt32(sorgus);

            return View(Tuple.Create(islemmodel, sorgu, değerlendirmemodels));
        }
        return RedirectToAction("Kuafördetay");
    }




    [HttpPost]
    public ActionResult yorumguncelle(int id, FormCollection datafc)
    {
        değerlendirme değerlendirme = db.değerlendirme.Where(i => i.ID == id).FirstOrDefault();
        var yorum = datafc["yorum"];
        var puan = Convert.ToInt32(datafc["ratingValue"]);
        değerlendirme.Kuaför = değerlendirme.Kuaför;
        değerlendirme.Müşteri = değerlendirme.Müşteri;
        değerlendirme.Yorum = yorum;//datafc["yorum"];
        değerlendirme.Puan = puan;//Convert.ToInt32(datafc["ratingValue"]);


        db.SaveChanges();



        var sorgus = db.değerlendirme.Where(i => i.Kuaför == değerlendirme.Kuaför).Average(i => i.Puan);
        Kuaför K = db.Kuaför.Where(İ => İ.ID == değerlendirme.Kuaför).FirstOrDefault();/* new Kuaför();*/
        K.puan = sorgus;
        db.SaveChanges();


        return RedirectToAction("değerlendirmedüzen");
    }


    public ActionResult silişlem(int id)
    {

        var sorgu = db.işlem_ücret.Where(i => i.işlem == id).FirstOrDefault();
        db.işlem_ücret.Remove(sorgu);
        db.SaveChanges();
        return RedirectToAction("hizmetler");
    }

    public ActionResult silyorum(int id)
    {
        var sorgu7 = db.değerlendirme.Where(i => i.ID == id).FirstOrDefault();
        var kuaför = sorgu7.Kuaför;

        randevu randevu1 = new randevu();
        db.değerlendirme.Remove(sorgu7);
        db.SaveChanges();
        var sorgu6 = db.değerlendirme.Where(i => i.ID == id).FirstOrDefault();
        var sorgus = db.değerlendirme.Where(i => i.Kuaför == kuaför).Average(i => i.Puan);
        Kuaför K = db.Kuaför.Where(İ => İ.ID == kuaför).FirstOrDefault();/* new Kuaför();*/
        K.puan = sorgus;
        db.SaveChanges();
        return RedirectToAction("değerlendirmedüzen");
    }
    public ActionResult sil(int id)
    {
        var sorgu7 = db.randevu.Where(i => i.ID == id).FirstOrDefault();
        var sorgu8 = db.randevu_işlem.Where(i => i.randevu == id).ToList();
        randevu randevu1 = new randevu();
        db.randevu.Remove(sorgu7);
        foreach (var item in sorgu8)
        {
            db.randevu_işlem.Remove(item);
            db.SaveChanges();
        }
        db.SaveChanges();
        return RedirectToAction("Randevulist");
    }
    public ActionResult silkuaför(int id)
    {
        var sorguislem = db.randevu_işlem.Where(i => i.randevu == id).ToList();
        var sorgurandevu = db.randevu.Where(i => i.ID == id).FirstOrDefault();
        randevu randevu1 = new randevu();
        db.randevu.Remove(sorgurandevu);
        foreach (var item in sorguislem)
        {
            db.randevu_işlem.Remove(item);
            db.SaveChanges();
        }
        db.SaveChanges();
        return RedirectToAction("RandevularKuafor");

    }








    public ActionResult Değerlendirmekayit()
    {

        return View();
    }
    [HttpPost]
    public ActionResult Değerlendirmekayit(int id, FormCollection dtn)
    {



        var sorgurandevu = db.randevu.Where(i => i.ID == id).FirstOrDefault();




        değerlendirme değerlendirme1 = new değerlendirme();

        değerlendirme1.Müşteri = sorgurandevu.müşteri;
        değerlendirme1.Kuaför = sorgurandevu.Kuaför;
        değerlendirme1.Yorum = dtn["yorum"];
        değerlendirme1.Puan = Convert.ToInt32(dtn["ratingValue"]);
        db.değerlendirme.Add(değerlendirme1);
        db.SaveChanges();
        var sorgus = db.değerlendirme.Where(i => i.Kuaför == sorgurandevu.Kuaför).Average(i => i.Puan);
        Kuaför K = db.Kuaför.Where(İ => İ.ID == sorgurandevu.Kuaför).FirstOrDefault();
        K.puan = sorgus;
        db.SaveChanges();
        return RedirectToAction("değerlendirmedüzen");

    }
 }


}


