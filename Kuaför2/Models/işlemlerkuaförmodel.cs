using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
    public class işlemlerkuaförmodel : Controller
    {
        // GET: islemlerkuaförmodel
       
            // GET: işlemlerkuaförmodel
            public randevu randevvm { get; set; }
            public Kuaför kuaförvm { get; set; }
            public işlemler işlemvm { get; set; }
            public randevu_işlem randevu_İşlemvm { get; set; }
        
    }
}