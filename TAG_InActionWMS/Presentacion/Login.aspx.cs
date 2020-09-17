using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using TREMEC_EtiquetaInventario_WS.Negocios;
using System.Reflection;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class Login : PageEstandar
    {
        Inicio login = new Inicio();
        //Colocar el rango de versión actualización a mostrar
        //  (1)    Versión principal
        //  (2)    Versión secundaria 
        //  (3)    Número de compilación
        //  (4)    Revisión
        protected string version = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logged"] != null)
                Session.Clear();

            txtUsuario.Focus();
            if (!Page.IsPostBack)
                DataBind();

#if DEBUG
            txtUsuario.Text = "admin";
            txtPassword.Text = "Dataware";
            LinkButton1_Click(null, null);
#endif
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2300);
            DataTable dt = new DataTable();
            string mensaje = string.Empty;
            if (txtUsuario.Text.Trim().Equals(string.Empty) || txtPassword.Text.Trim().Equals(string.Empty))
            {
                AlertError("Para iniciar sesión debe ingresar usuario y contraseña");
                return;
            }
            dt = login.DatosSesion(txtUsuario.Text.Trim(), txtPassword.Text.Trim(), ref mensaje);
            if (!mensaje.Equals(string.Empty))
                AlertError(mensaje);
            else
            {
                if (dt != null) //Sesión existe
                {
                    //Iniciar sesión
                    Session.Add("Logged", true);
                    Session.Add("IdUsuario", Convert.ToInt32(dt.Rows[0]["IdUsuario"].ToString()));
                    Session.Add("Nombre", dt.Rows[0]["Nombre"].ToString());
                    Session.Add("IdTipoUsuario", dt.Rows[0]["IdTipoUsuario"].ToString());
                    Session.Add("IdPerfil", Convert.ToInt32(dt.Rows[0]["IdPerfil"].ToString()));
                    CreaMenu();

                    if (Session["Menu"] != null)
                        if (Request.QueryString["redirect"] == null)
                            Response.Redirect("Default.aspx");
                        else
                            Response.Redirect(Request.QueryString["redirect"]);
                }
                else
                    AlertError("Usuario o contraseña no válido");
            }
        }

        private void CreaMenu()
        {
            DataTable dt = new DataTable();
            DataTable dwModulos = new DataTable();
            string mensaje = string.Empty;
            string filterExpression = string.Empty;
            LiteralControl literal = new LiteralControl();
            dt = login.DatosPermisos((int)Session["IdUsuario"], ref mensaje);
            if (!mensaje.Equals(string.Empty))
            {
                AlertError(mensaje);
                return;
            }
            //Consulta modulos padre (encabezados de menu)
            filterExpression = "IdModuloPadre IS NULL";
            if (dt.Select(filterExpression).Count() > 0)
            {
                Session.Add("Permisos", dt);
                dwModulos = dt.Select("IdModuloPadre IS NULL AND Consultar = '1'", "Orden ASC").CopyToDataTable();
                literal.Text += @"<ul class='nav navbar-nav'>";
                foreach (DataRow rowModulos in dwModulos.Rows)
                {
                    string file = rowModulos["Archivo"].ToString();
                    literal.Text += "<li class='dropdown'><a href='" + file + "' class='dropdown-toggle" + (file.Length > 0 ? " disabled" : "") + "' data-toggle='dropdown'>" + rowModulos["RibbonName"].ToString();
                    //Consulta submodulos de modulos padre
                    filterExpression = "IdModuloPadre = '" + rowModulos["IdModulo"].ToString() + "'";
                    if (dt.Select(filterExpression).Count() > 0)
                    {
                        DataTable dtModulos2 = new DataTable();
                        dtModulos2 = dt.Select("IdModuloPadre = '" + rowModulos["IdModulo"].ToString() + "' AND Consultar = '1'", "Orden ASC").CopyToDataTable();
                        literal.Text += @"&nbsp;&nbsp;<span class='caret'></span></a>";
                        literal.Text += @"<ul class='dropdown-menu'>";
                        foreach (DataRow rowModulos2 in dtModulos2.Rows)
                        {
                            //Consulta submodulos de submodulos
                            filterExpression = "IdModuloPadre = '" + rowModulos2["IdModulo"].ToString() + "'";
                            if (dt.Select(filterExpression).Count() > 0)
                            {
                                DataTable dtModulos3 = new DataTable();
                                dtModulos3 = dt.Select("IdModuloPadre = '" + rowModulos2["IdModulo"].ToString() + "' AND Consultar = '1'", "Orden ASC").CopyToDataTable();
                                literal.Text += "<li class='dropdown-submenu'><a href='" + rowModulos2["Archivo"].ToString() + "'><img src='../img/" + rowModulos2["Icono"].ToString() + "' style='height: 1.5em;' />&nbsp;&nbsp;" + rowModulos2["RibbonName"].ToString() + @"</a>
                                    <ul class='dropdown-menu'>";
                                foreach (DataRow rowModulos3 in dtModulos3.Rows)
                                    literal.Text += "<li><a href='" + rowModulos3["Archivo"].ToString() + "'><img src='../img/" + rowModulos3["Icono"].ToString() + "' style='height: 1.5em;' />&nbsp;&nbsp;" + rowModulos3["RibbonName"].ToString() + "</a></li>";

                                literal.Text += "</ul></li>";
                            }
                            else
                                literal.Text += "<li><a href='" + rowModulos2["Archivo"].ToString() + "'><img src='../img/" + rowModulos2["Icono"].ToString() + "' style='height: 1.5em;' />&nbsp;&nbsp;" + rowModulos2["RibbonName"].ToString() + "</a></li>";
                        }
                        literal.Text += "</ul>";
                    }
                    else
                        literal.Text += "</a>";

                    literal.Text += "</li>";
                }
                literal.Text += "</ul>";
            }

            //Crear opciones para perfil y crea logo del cliente
            literal.Text += @"<ul class='nav navbar-nav navbar-right'>
                <li class='dropdown'>
                    <a href='#' class='dropdown-toggle' data-toggle='dropdown'>" + Session["Nombre"] + @"&nbsp;&nbsp;<span class='caret'></span></a>
                    <ul class='dropdown-menu'>
                        <li><a href='AcercaDe.aspx'><span class='glyphicon glyphicons-info-sign'></span>&nbsp;&nbsp;Acerca de</a></li>
                        <li role='separator' class='divider'></li>
                        <li><a href='Login.aspx'><span class='glyphicon glyphicons-log-out'></span>&nbsp;&nbsp;Cerrar Sesión</a></li>
                    </ul>
                </li>
                <a class='navbar-brand' href='Default.aspx' style='float: right !important;'>
                    <img src='../img/logo_cliente.png' style='padding-top: 15px; height: 40px; width: 70px;'>
                </a>
            </ul>";

            Session["Menu"] = literal.Text;
        }
    }
}