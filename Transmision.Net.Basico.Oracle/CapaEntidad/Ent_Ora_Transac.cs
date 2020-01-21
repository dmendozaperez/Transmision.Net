using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transmision.Net.Basico.Oracle.CapaEntidad
{
    public class Ent_Ora_Transac
    {
        public Decimal RTL_LOC_ID { get; set; }
        public Decimal WKSTN_ID { get; set; }
        public Decimal TRANS_SEQ { get; set; }
        public DateTime BUSINESS_DATE { get; set; }
        public string NUMDOC { get; set; }
        public Decimal TOTAL { get; set; }
        public string DOCUMENT_TYPCODE { get; set; }
    }
}
