using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationApp.Models
{
    public class Response
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<YerlesimAyrinti> YerlesimAyrinti { get; set; }

    }

    public class YerlesimAyrinti
    {
        public string VagonAdi { get; set; }
        public int KisiSayisi { get; set; }

    }
}
