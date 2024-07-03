using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
    public class Degerlendirme : Controller
    {
        // GET: Değerlendirme
       

            internal List<Degerlendirme> degerlendirme;
            public müşteri müşterivm { get; set; }

            public Kuaför kuaförvm { get; set; }

            public List<Kuaför> kuaför { get; set; }
            public değerlendirme değerlendirmevm { get; set; }
        }
    
}