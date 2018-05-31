using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transmision.Net.Basico
{
    public class FFCGUD02
    {
        public string gtc_tipo { get; set; }
        public string gtc_alm { get; set; }
        public string gtc_nume { get; set; }
        public DateTime gtc_femi { get; set; }
        public string gtc_semi { get; set; }
        public string gtc_gudis { get; set; }
        public string gtc_tndcl { get; set; }
        public string gtc_estad { get; set; }
        public DateTime gtc_frect { get; set; }
        public Int32 gtc_cal { get; set; }
        public decimal gtc_calm { get; set; }
        public Int32 gtc_acc { get; set; }
        public decimal gtc_accm { get; set; }
        public Int32 gtc_caj { get; set; }
        public decimal gtc_cajm { get; set; }
        public string gtc_glosa { get; set; }

        public List<FFDGUD02> gtd_lista { get; set; }

    }
}
