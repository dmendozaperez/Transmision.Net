using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalSolucion
{
    public class Variables
    {
        public static string _venta_cab = "FFACTC02";
        public static string _venta_det = "FFACTD02";
        public static string _venta_pago = "FNOTAA02";
        public static string _path_default = @"D:\POS";
        public static string _codigo_empresa_tda = "02";
        public static string _path_envia= @"D:\POS\TMP";


        public static string _conexion_vfpoledb
        {
            //get { return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path_default + ";Extended Properties=FoxPro 2.x;"; }
            get { return "Provider=VFPOLEDB;Data Source=" + _path_default + ";Exclusive=No"; }
        }
        public static string _conexion
        {
            //get { return "Provider=VFPOLEDB.1;Data Source=" + _path_default + ";Exclusive=No"; }
            get { return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path_default + ";Extended Properties=dBASE IV;"; }
        }
        public static string _conexion_envia
        {
            //get { return "Provider=VFPOLEDB.1;Data Source=" + _path_envia + ";Exclusive=No"; }
            get { return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path_envia + ";Extended Properties=dBASE IV;"; }
        }
    }
}
