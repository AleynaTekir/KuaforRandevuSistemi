using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
    public class değerlendirmemodel : Controller
    {
        // GET: Değerlendirmemodel
       
            internal List<değerlendirmemodel> müşterimodels;
            public müşteri müşterivm { get; set; }
            public Kuaför kuaförvm { get; set; }
            public değerlendirme değerlendirmevm { get; set; }
        }
   
}