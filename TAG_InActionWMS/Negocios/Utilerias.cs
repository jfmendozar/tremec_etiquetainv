using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class Utilerias
    {
        public static bool IsDate(object _value)
        {
            try
            {
                Convert.ToDateTime(_value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string get_FormatoFecha(string cadena)
        {
            DateTime cadfecha = Convert.ToDateTime(cadena);
            string fecha = String.Format("{0:dd-MM-yyyy}", cadfecha);
            return fecha;
        }
    }
}