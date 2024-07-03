using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
    public class RAndevu : Controller
    {
        // GET: RAndevu
        public List<string> saatler;
        public randevu_işlem randevu_İşlemvm { get; set; }
        public randevu randevvm { get; set; }
        public Kuaför kuaförvm { get; set; }
        public işlemler işlemvm { get; set; }
        public müşteri müşterivm { get; set; }
    }
}