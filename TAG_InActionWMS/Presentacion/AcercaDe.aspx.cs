using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class AcercaDe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logged"] == null) //No hay sesión
                Response.Redirect("Login.aspx?redirect=" + Request.Path.Substring(Request.Path.LastIndexOf("/") + 1));
            else if (!Page.IsPostBack)
                Session["Tab"] = "Inicio";

        }
    }
}