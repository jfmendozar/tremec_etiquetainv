using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using TREMEC_EtiquetaInventario_WS.Negocios;
using Telerik.Web.UI;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class Cat_NumerosParte : PageEstandar
    {
        #region Definicion de objetos y variables
        Inicio inicio = new Inicio();
        Usuarios usuarios = new Usuarios();
        Perfiles perfiles = new Perfiles();
        NumerosParte numerosParte = new NumerosParte();
        ControlExcepciones excepcion = new ControlExcepciones();
        static DataTable dtAcciones;// = new DataTable();
        static List<int> listItemSelected = new List<int>();
        static string nombreArchivo = string.Empty;
        static string tituloPagina = string.Empty;
        //protected static string tituloPanel = string.Empty;
        //static bool permisoConsultar = false;
        //static bool permisoAgregar = false;
        //static bool permisoEditar = false;
        //static bool permisoEliminar = false;
        //static bool permisoExportar = false;
        bool isExport = false;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logged"] == null) //No hay sesión
                Response.Redirect("Login.aspx?redirect=" + Request.Path.Substring(Request.Path.LastIndexOf("/") + 1));
            else if (!Page.IsPostBack)
                Session["Tab"] = "Inicio";

            if (!Page.IsPostBack)
            {
                nombreArchivo = Request.Path.Substring(Request.Path.LastIndexOf("/") + 1);
                if (Session["Permisos"] != null)
                {
                    DataTable dt = null;
                    try
                    {
                        dt = ((DataTable)Session["Permisos"]).Select("Archivo =  '" + nombreArchivo + "'").CopyToDataTable();
                    }
                    catch (Exception)
                    {
                        (this.Master as Principal).AlertError("Tiene otra sesión activa.");
                        Response.Redirect("Default.aspx");
                        return;
                    }

                    tituloPagina = dt.Rows[0]["NombreModulo"].ToString();
                    permisoConsultar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Consultar"].ToString()));
                    if (!permisoConsultar)
                        Response.Redirect("Default.aspx");
                    permisoAgregar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Agregar"].ToString()));
                    permisoEditar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Editar"].ToString()));
                    permisoEliminar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Eliminar"].ToString()));
                    permisoExportar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Exportar"].ToString()));
                    Page.Title = tituloPagina;
                }
                TituloPanel(string.Empty);
                listItemSelected = new List<int>();
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
            RadGrid1.Rebind();
        }

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new DataTable();
            string mensaje = "";
            if (Page.IsPostBack)
                dt = numerosParte.NumerosParteConsultar(ref mensaje);

            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            RadGrid1.DataSource = dt == null ? new DataTable() : dt;
        }

        protected void RadGrid1_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            e.RawHTML = e.RawHTML.Replace("border=\"1\"", " border=\"0\"");
            e.RawHTML = "<div style='text-align: center; font-family: Arial Unicode MS; font-size: 11pt; line-height: 16px;'>" + tituloPanel + "</div><br />" + e.RawHTML;
        }

        protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
        {
            string mensaje = string.Empty;
            GuardarItemsSeleccionados(RadGrid1);
            
            if (e.CommandName.Equals(RadGrid.ExportToExcelCommandName))
            {
                ConfigureExport(RadGrid1);
            }
            else if (e.CommandName == RadGrid.ExportToPdfCommandName)
            {
                RadGrid1.MasterTableView.GetColumn("NumParte").HeaderStyle.Width = 180;
                RadGrid1.MasterTableView.GetColumn("Descripcion").HeaderStyle.Width = 450;
                RadGrid1.MasterTableView.GetColumn("Usuario").HeaderStyle.Width = 100;
                RadGrid1.MasterTableView.GetColumn("FechaActualiza").HeaderStyle.Width = 120;
                ConfigureExport(RadGrid1);
            }
        }

        protected void RadGrid1_PreRender(object sender, EventArgs e)
        {
            GridCommandItem itemsRadGrid1 = (GridCommandItem)RadGrid1.MasterTableView.GetItems(GridItemType.CommandItem)[0];
            LinkButton btnExcel = itemsRadGrid1.FindControl("btnExcel") as LinkButton;
            btnExcel.Enabled = btnExcel.Visible = permisoExportar;
            LinkButton btnPdf = itemsRadGrid1.FindControl("btnPdf") as LinkButton;
            btnPdf.Enabled = btnPdf.Visible = permisoExportar;
            //Agrega un atributo GID para identificar el RadGrid desde donde se llama al botón Filtrar
            LinkButton btnFiltrar = itemsRadGrid1.FindControl("btnFiltrar") as LinkButton;
            if (btnFiltrar != null)
                btnFiltrar.Attributes.Add("GID", RadGrid1.ClientID);
            //Si no hay registros deshabilita los botones de exportar
            if (RadGrid1.MasterTableView.Items.Count == 0)
            {
                btnExcel.Attributes.Add("disabled", "disabled");
                btnPdf.Attributes.Add("disabled", "disabled");
                btnExcel.Enabled = btnPdf.Enabled = false;
            }
            //Selecciona las filas que estaban seleccionadas antes del postback
            if (listItemSelected.Count > 0)
                foreach (GridDataItem item in RadGrid1.Items)
                    item.Selected = listItemSelected.Contains(Convert.ToInt32(item["IdNumParte"].Text));
        }

        protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //Asigna tooltips cuando no exporta
            if (!isExport)
                (this.Master as Principal).SetTooltips(sender, e);
        }

        protected void RadGrid1_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (isExport)
                //Oculta fila de filtros al exportar
                if (e.Item is GridFilteringItem)
                    e.Item.Visible = false;
                //Asigna tamaño a las columnas al exportar a Excel
                else if (e.Item is GridDataItem)
                {
                    GridDataItem item = e.Item as GridDataItem;
                    item["NumParte"].Width = 180;
                    item["Descripcion"].Width = 500;
                    item["Usuario"].Width = 100;
                    item["FechaActualiza"].Width = 100;
                    
                   
                }
        }

        private void GuardarItemsSeleccionados(RadGrid radgrid)
        {
            listItemSelected = new List<int>();
            foreach (GridDataItem item in radgrid.SelectedItems)
                listItemSelected.Add(Convert.ToInt32(item["IdNumParte"].Text));
        }
        
        private void ConfigureExport(RadGrid radgrid)
        {
            isExport = true;
            radgrid.ExportSettings.FileName = tituloPanel;
            radgrid.ExportSettings.ExportOnlyData = true;
            radgrid.ExportSettings.IgnorePaging = true;
            radgrid.ExportSettings.OpenInNewWindow = true;
            radgrid.MasterTableView.Caption = "<b>" + tituloPanel + "</b>";
        }
    }
}