using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuaför2.Models
{
    public class randev : Controller
    {
        // GET: randev
        internal List<randev> Randev;
        public randevu randevvm { get; set; }
        public Kuaför kuaförvm { get; set; }
        public işlemler işlemvm { get; set; }
        public müşteri müşterivm { get; set; }
    }
}