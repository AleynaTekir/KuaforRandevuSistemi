using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
   
        public class Class1
        {
            // GET: Class1
            public Class1()
            {
                this.IlceList = new List<SelectListItem>();
                IlceList.Add(new SelectListItem { Text = "---ilçe Seçiniz---", Value = "" });
            }
            public int SehirID { get; set; }
            public int IlceID { get; set; }
            public string İl { get; set; }
            public string ilçe { get; set; }
            public List<SelectListItem> SehirList { get; set; }
            public List<SelectListItem> IlceList { get; set; }
            public List<Kuaför> KuaförList { get; set; }

        }
    
}