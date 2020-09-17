using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using TREMEC_EtiquetaInventario_WS.Negocios;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class ReporteProduccion : PageEstandar
    {
        #region Definicion de objetos y variables
        NumerosParte numerosParte = new NumerosParte();
        ReportesProductividad produccion = new ReportesProductividad();
        ControlExcepciones excepcion = new ControlExcepciones();
        static string nombreArchivo = string.Empty;
        static string tituloPagina = string.Empty;
        protected static string encFecha = string.Empty;
        protected static string encUbicacion = string.Empty;
        //protected static string tituloPanel = string.Empty;
        //static bool permisoConsultar = false;
        //static bool permisoExportar = false;
        bool isExport = false;
        static bool busqueda = false;
        static List<int> listaItemsSel = new List<int>();
        static List<int> listaItemsSelDetalle = new List<int>();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            #region Codigo que se encuentra en el PageEstandar
            if (Session["Logged"] == null) //No hay sesión
                Response.Redirect("Login.aspx?redirect=" + Request.Path.Substring(Request.Path.LastIndexOf("/") + 1));
            else if (!Page.IsPostBack)
                Session["Tab"] = "Inicio";

            if (!Page.IsPostBack)
            {
                nombreArchivo = Request.Path.Substring(Request.Path.LastIndexOf("/") + 1);
                if (Session["Permisos"] != null)
                {
                    DataTable dt = ((DataTable)Session["Permisos"]).Select("Archivo =  '" + nombreArchivo + "'").CopyToDataTable();
                    tituloPagina = dt.Rows[0]["NombreModulo"].ToString();
                    permisoConsultar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Consultar"].ToString()));
                    if (!permisoConsultar)
                        Response.Redirect("Default.aspx");
                    permisoExportar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Exportar"].ToString()));
                    Page.Title = tituloPagina;
                    //busqueda = false;
                }
                TituloPanel(string.Empty);
                listaItemsSel = new List<int>();
                listaItemsSelDetalle = new List<int>();

                //RadDatePicker1.SelectedDate = DateTime.Today;
                //RadDatePicker1.DataBind();
            }
            #endregion
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
            //if(Page.IsPostBack)
                dt = produccion.ObtenerRegistros(ref mensaje);
            
            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            RadGrid1.DataSource = dt == null ? new DataTable() : dt;
        }

        protected void RadGrid1_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            e.RawHTML = e.RawHTML.Replace("border=\"1\"", " border=\"0\"");
            e.RawHTML = "<div style='text-align: center; font-family: Arial Unicode MS; font-size: 11pt; line-height: 16px;'>" + tituloPanel + "</div><br />" + e.RawHTML;
            //"Reporte Productividad por Recurso"
        }

        protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.Equals(RadGrid.ExportToExcelCommandName))
            {
                ConfigureExport(RadGrid1);
            }
            else if (e.CommandName == RadGrid.ExportToPdfCommandName)
            {
                RadGrid1.MasterTableView.GetColumn("NumParte").HeaderStyle.Width = 140;
                RadGrid1.MasterTableView.GetColumn("Descripcion").HeaderStyle.Width = 180;
                RadGrid1.MasterTableView.GetColumn("Lote").HeaderStyle.Width = 140;
                RadGrid1.MasterTableView.GetColumn("TipoEtiqueta").HeaderStyle.Width = 120;
                RadGrid1.MasterTableView.GetColumn("Cantidad").HeaderStyle.Width = 80;
                RadGrid1.MasterTableView.GetColumn("Usuario").HeaderStyle.Width = 100;
                RadGrid1.MasterTableView.GetColumn("FechaActualiza").HeaderStyle.Width = 130;
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
            if (listaItemsSel.Count > 0)
                foreach (GridDataItem item in RadGrid1.Items) {
                    item.Selected = listaItemsSel.Contains(Convert.ToInt32(item["IdProduccion"].Text));
                }
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
                    item["NumParte"].Width = 150;
                    item["Descripcion"].Width = 180;
                    item["Lote"].Width = 150;
                    item["TipoEtiqueta"].Width = 150;
                    item["Cantidad"].Width = 80;
                    item["Usuario"].Width = 100;
                    item["FechaActualiza"].Width = 150;
                }
        }

        //private void GuardarItemsSeleccionados(ref List<int> lista, RadGrid grid, string idColumna)
        //{
        //    lista = new List<int>();
        //    if (grid.MasterTableView.Items.Count > 0)
        //        foreach (GridDataItem item in grid.SelectedItems)
        //            lista.Add(Convert.ToInt32(item[idColumna].Text));
        //    
        //}

        private void GuardarItemsSeleccionados(RadGrid radgrid){
            listaItemsSel = new List<int>();
            foreach (GridDataItem item in radgrid.SelectedItems)
                listaItemsSel.Add(Convert.ToInt32(item["IdProduccion"].Text));
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

        //private void TituloPanel(string descripcion)
        //{
        //    tituloPanel = tituloPagina + descripcion;
        //    DataBind();
        //}
    }
}