//------------------------------------------------------------------------------
// VERSION 1.0 - Antonio Gonzalez
// History - Antonio Gonzalez - 13/10/2017 - Integrar Expresiones Regulares
// History - Beatriz Juárez - 20/10/2017 - Validación PreRender con command items 
// History - Antonio Gonzalez - 28/11/2017 - Se añade carácteres a expresiones regulares y codigo al buscar RadGrid en página
// History - Antonio Gonzalez - 20/03/2018 - Se añaden estándares a InactionWMS. Solución a error de permisos boton de exportar
// History - Antonio Gonzalez - 24/04/2018 - Se optimiza la búsqueda de RadGrid en toda la página
// History - Antonio Gonzalez - 26/04/2018 - Se añade UniqueIdControl
// History - Antonio Gonzalez - 27/04/2018 - Se resuelve error en botones deshabilitados Excel y PDF
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.ComponentModel;
using System.Xml;
using System.Text.RegularExpressions;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class PageEstandar : Page
    {
        string nombreArchivo = string.Empty;
        string tituloPagina = string.Empty;
        protected string tituloPanel = string.Empty;
        protected bool permisoConsultar = false;
        protected bool permisoAgregar = false;
        protected bool permisoEditar = false;
        protected bool permisoEliminar = false;
        protected bool permisoExportar = false;
        protected const string busqueda = "busqueda";
        protected const string consTituloPanel = "tituloPanel";
        protected bool isExport = false;
        const string defaultAspx = "Default.aspx";

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.Path.Substring(Request.Path.LastIndexOf("/") + 1).Equals("Login.aspx")) //Cerrar sesión
            {
                Session.Clear();
                return;
            }
            else if (Session["Logged"] == null) //No hay sesión, redirige a Login
                Response.Redirect("Login.aspx?redirect=" + Request.Path.Substring(Request.Path.LastIndexOf("/") + 1));

            Session["Tab"] = tituloPagina = "Inicio";
            nombreArchivo = Request.Path.Substring(Request.Path.LastIndexOf("/") + 1);
            if (Session["Permisos"] != null)
            {
                if (((DataTable)Session["Permisos"]).Select("Archivo =  '" + nombreArchivo + "'").Count() > 0)
                {
                    DataTable dt = ((DataTable)Session["Permisos"]).Select("Archivo =  '" + nombreArchivo + "'").CopyToDataTable();
                    tituloPagina = dt.Rows[0]["NombreModulo"].ToString();
                    permisoConsultar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Consultar"].ToString()));
                    permisoAgregar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Agregar"].ToString()));
                    permisoEditar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Editar"].ToString()));
                    permisoEliminar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Eliminar"].ToString()));
                    permisoExportar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Exportar"].ToString()));
                    if (!permisoConsultar)
                        Response.Redirect(defaultAspx);
                }
                else if (!nombreArchivo.Equals(defaultAspx))
                    Response.Redirect(defaultAspx);

                if (!Page.IsPostBack)
                    Page.Title = tituloPagina;
            }

            //Localizar RadGrid en el Page
            Control contentPh = Master.FindControl("ContentSection");
            if (contentPh != null)
                BuscarGrid(contentPh);
        }

        #region Estándares RadGrid
        private void EstandarizarGrid(RadGrid grid)
        {
            //Traduce filtros y textos del RadGrid
            TraducirGrid(grid);
            //Asigna eventos generales
            grid.NeedDataSource += new GridNeedDataSourceEventHandler(RadGridDefault_NeedDataSourece);
            grid.ItemCreated += new GridItemEventHandler(RadGridDefault_ItemCreated);
            grid.ItemCommand += new GridCommandEventHandler(RadGridDefault_ItemCommand);
            grid.ItemDataBound += new GridItemEventHandler(RadGridDefault_ItemDataBound);
            grid.PdfExporting += new OnGridPdfExportingEventHandler(RadGridDefault_PdfExporting);
            grid.PreRender += new EventHandler(RadGridDefault_PreRender);
            grid.ExportSettings.Pdf.ForceTextWrap = true;
        }

        private void TraducirGrid(RadGrid grid)
        {
            //Oculta opciones del menú
            GridFilterMenu menu = grid.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "IsEmpty" ||
                    menu.Items[i].Text == "NotIsEmpty" ||
                    menu.Items[i].Text == "IsNull" ||
                    menu.Items[i].Text == "NotIsNull")
                    menu.Items.RemoveAt(i);
                else
                    i++;
            }
            //Traduce tooltip del botón Filtro
            foreach (GridColumn column in grid.Columns)
                grid.MasterTableView.GetColumn(column.UniqueName).FilterImageToolTip = "Menú filtros";
            //Traduce filtros
            foreach (RadMenuItem item in menu.Items)
            {
                switch (item.Value)
                {
                    case "NoFilter":
                        item.Text = "Sin filtro";
                        break;
                    case "Contains":
                        item.Text = "Contiene";
                        break;
                    case "DoesNotContain":
                        item.Text = "No contiene";
                        break;
                    case "StartsWith":
                        item.Text = "Empieza con";
                        break;
                    case "EndsWith":
                        item.Text = "Termina con";
                        break;
                    case "EqualTo":
                        item.Text = "Es igual a";
                        break;
                    case "NotEqualTo":
                        item.Text = "No es igual a";
                        break;
                    case "GreaterThan":
                        item.Text = "Mayor que";
                        break;
                    case "LessThan":
                        item.Text = "Menor que";
                        break;
                    case "GreaterThanOrEqualTo":
                        item.Text = "Mayor o igual que";
                        break;
                    case "LessThanOrEqualTo":
                        item.Text = "Menor o igual que";
                        break;
                    case "Between":
                        item.Text = "Está entre";
                        break;
                    case "NotBetween":
                        item.Text = "No está entre";
                        break;
                    case "IsEmpty":
                        item.Text = "Es espacio en blanco";
                        item.Remove();
                        break;
                    case "NotIsEmpty":
                        item.Text = "Es es espacio en blanco";
                        item.Remove();
                        break;
                    case "IsNull":
                        item.Text = "Es valor nulo";
                        item.Remove();
                        break;
                    case "NotIsNull":
                        item.Text = "No es valor nulo";
                        item.Remove();
                        break;
                }
            }
            //Traduce toltips accesibles
            grid.FilterMenu.ToolTip = "Filtro";
            grid.ClientSettings.ClientMessages.DragToGroupOrReorder = "Arrastre para agrupar o reordenar";
            grid.ClientSettings.ClientMessages.DropHereToReorder = "Suelte aquí para reordenar";
            grid.ClientSettings.ClientMessages.DragToResize = "Arrastre para redimensionar";
            grid.SortingSettings.SortedAscToolTip = "Orden ascendente";
            grid.SortingSettings.SortedDescToolTip = "Orden descendente";
            grid.SortingSettings.SortToolTip = "Ordenar";
            grid.GroupingSettings.UnGroupButtonTooltip = "Desagrupar";
            grid.GroupingSettings.UnGroupTooltip = "Arrastre fuera de la barra para desagrupar";
        }

        private void SetAdvancedPagerStyle(GridPagerItem item)
        {
            //Traduce el Pager del RadGrid y asigna tooltips
            Label changePageSizeLabel = item.FindControl("ChangePageSizeLabel") as Label;
            Label goToPageLabel = item.FindControl("GoToPageLabel") as Label;
            Label pageOfLabel = item.FindControl("PageOfLabel") as Label;
            RadNumericTextBox changePageSizeTextBox = item.FindControl("ChangePageSizeTextBox") as RadNumericTextBox;
            RadNumericTextBox goToPageTextBox = item.FindControl("GoToPageTextBox") as RadNumericTextBox;
            Button goToPageLinkButton = item.FindControl("GoToPageLinkButton") as Button;
            Button changePageSizeLinkButton = item.FindControl("ChangePageSizeLinkButton") as Button;
            if (changePageSizeLabel != null)
                changePageSizeLabel.Text = "&nbsp;Filas:";
            if (goToPageLabel != null)
                goToPageLabel.Text = "Página:";
            if (pageOfLabel != null)
                pageOfLabel.Text = "de " + item.Paging.PageCount.ToString();
            if (changePageSizeTextBox != null)
            {
                changePageSizeTextBox.MaxValue = 1000;
                changePageSizeTextBox.ToolTip = "Filas por página";
                changePageSizeTextBox.Attributes.Add("data-toggle", "tooltip");
            }
            if (goToPageTextBox != null)
            {
                goToPageTextBox.ToolTip = "Página";
                goToPageTextBox.Attributes.Add("data-toggle", "tooltip");
            }
            if (goToPageLinkButton != null)
            {
                goToPageLinkButton.Text = "Ver";
                goToPageLinkButton.ToolTip = "Ver página";
                goToPageLinkButton.CssClass = "btn btn-primary btn-sm";
                goToPageLinkButton.Attributes.Add("data-toggle", "tooltip");
            }
            if (changePageSizeLinkButton != null)
            {
                changePageSizeLinkButton.Text = "Cambiar";
                changePageSizeLinkButton.ToolTip = "Cambiar número de filas por página";
                changePageSizeLinkButton.CssClass = "btn btn-primary btn-sm";
                changePageSizeLinkButton.Attributes.Add("data-toggle", "tooltip");
            }
        }

        private void SetTooltipFilterMenu(GridFilteringItem filterItem, RadGrid radgrid)
        {
            //Traduce el tooltip del botón Calendario en las columnas DateTime
            foreach (GridColumn column in radgrid.Columns)
            {
                if (column is GridDateTimeColumn)
                {
                    RadDatePicker picker = (RadDatePicker)filterItem[column.UniqueName].Controls[0];
                    picker.DatePopupButton.ToolTip = "Abrir calendario";
                    picker.DatePopupButton.Attributes.Add("data-toggle", "tooltip");
                    //Agrega el atributo data-toggle para mostrar el tooltip en los botones de filtro
                    Button btn = (Button)filterItem[column.UniqueName].Controls[2];
                    btn.Attributes.Add("data-toggle", "tooltip");
                }
                else if (column is GridBoundColumn)
                {
                    //Agrega el atributo data-toggle para mostrar el tooltip en los botones de filtro
                    Button btn = (Button)filterItem[column.UniqueName].Controls[1];
                    btn.Attributes.Add("data-toggle", "tooltip");
                }
            }
        }

        private void SetTooltipDataItem(GridDataItem dataItem, RadGrid grid)
        {
            foreach (GridColumn column in grid.MasterTableView.RenderColumns)
                if (column is GridBoundColumn)
                    if (column.Display)
                        dataItem[column.UniqueName].ToolTip = dataItem[column.UniqueName].Text;
            //dataItem[column.UniqueName].Attributes.Add("data-toggle", "tooltip");
        }

        private void SetTooltipGridHeader(GridHeaderItem gridHeader)
        {
            TableRow table = gridHeader as TableRow;
            foreach (TableCell cell in table.Cells)
                foreach (Control itemCtrl in cell.Controls)
                    if (itemCtrl is LinkButton)//Agrega tooltip al encabezado de la columna
                    {
                        LinkButton btn = itemCtrl as LinkButton;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
                    else if (itemCtrl is CheckBox)//Agrega tooltip al checkbox seleccionar todo
                    {
                        CheckBox cbx = itemCtrl as CheckBox;
                        cbx.InputAttributes.Add("data-toggle", "tooltip");
                    }
                    else if (itemCtrl is Button)//Agrega tooltip al boton orden asc/desc
                    {
                        Button btn = itemCtrl as Button;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
        }

        private void SetTooltips(object sender, GridItemEventArgs e)
        {
            //Traduce y agrega tooltips al pager
            if (e.Item is GridPagerItem)
                SetAdvancedPagerStyle((GridPagerItem)e.Item);
            //Agrega tooltips al encabezado
            else if (e.Item is GridHeaderItem)
                SetTooltipGridHeader((GridHeaderItem)e.Item);
            //Agrega tooltips a los filtros
            else if (e.Item is GridFilteringItem)
                SetTooltipFilterMenu((GridFilteringItem)e.Item, (RadGrid)sender);
            //Asigna un tooltip a los Items del RadGrid
            else if (e.Item is GridDataItem)
                SetTooltipDataItem((GridDataItem)e.Item, (RadGrid)sender);
        }
        #endregion

        #region Eventos RadGrid
        //Secuencia de eventos (excepto ItemCommand y PdfExporting)
        private void RadGridDefault_NeedDataSourece(object sender, GridNeedDataSourceEventArgs e)
        {
            RadGrid grid = (RadGrid)sender;
            DataTable dt = grid.DataSource == null ? new DataTable() : (DataTable)grid.DataSource;
            //Si hay registros asigna el Paginado "Advanced", de lo contrario oculta el Encabezado y el Paginado
            if (dt.Rows.Count > 0)
            {
                grid.MasterTableView.PagerStyle.PagerTextFormat = "{4} {3} filas de {5}";
                grid.MasterTableView.PagerStyle.Mode = GridPagerMode.Advanced;
            }
        }

        private void RadGridDefault_ItemCreated(object sender, GridItemEventArgs e)
        {
            //Oculta fila de filtros al exportar
            if (isExport && e.Item is GridFilteringItem)
                e.Item.Visible = false;
        }

        private void RadGridDefault_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //Asigna tooltips al RadGrid unicamente cuando no se exporta
            if (!isExport)
                SetTooltips(sender, e);
        }

        private void RadGridDefault_PreRender(object sender, EventArgs e)
        {
            RadGrid grid = (RadGrid)sender;
            // Si el grid no tiene command items el evento no aplica
            if (grid.MasterTableView.GetItems(GridItemType.CommandItem).Count() == 0)
                return;
            //Muestra/Oculta los botones del RadGrid deacuerdo a los permisos
            GridCommandItem comandos = (GridCommandItem)grid.MasterTableView.GetItems(GridItemType.CommandItem)[0];
            string[] arrayComandos = { "btnNuevo", "btnEditar", "btnEliminar", "btnExcel", "btnPdf", "btnFiltrar" };
            bool[] arrayPermisos = { permisoAgregar, permisoEditar, permisoEliminar, permisoExportar, permisoExportar };

            for (int i = 0; i < arrayComandos.Length; i++)
            {
                LinkButton linkBtn = comandos.FindControl(arrayComandos[i]) as LinkButton;
                if (linkBtn != null)
                    //Agrega un atributo GID para identificar el RadGrid desde donde se llama al botón Filtrar
                    if (arrayComandos[i].Equals("btnFiltrar"))
                        linkBtn.Attributes.Add("GID", grid.ClientID);
                    else
                    {
                        //Aplica los permisos al botón
                        linkBtn.Visible = arrayPermisos[i];
                        //Si no hay registros deshabilita los botones de exportar
                        if ((linkBtn.ID.Equals("btnExcel") || linkBtn.ID.Equals("btnPdf")) && (grid.MasterTableView.Items.Count == 0))
                        {
                            linkBtn.Enabled = false;
                            linkBtn.Attributes.Add("disabled", "disabled");
                        }
                    }
            }

            //Toltips para los objetos que estén en el GrouoPanel
            foreach (TableCell cell in grid.GroupPanel.GroupPanelItems)
            {
                cell.Attributes.Add("data-placement", "bottom");
                cell.Attributes.Add("data-toggle", "tooltip");
                foreach (Control ctrl in cell.Controls)
                    if (ctrl is LinkButton)//Agrega tooltip al encabezado de la columna
                    {
                        LinkButton btn = ctrl as LinkButton;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
                    else if (ctrl is Button)//Agrega tooltip al boton orden asc/desc
                    {
                        Button btn = ctrl as Button;
                        btn.Attributes.Add("data-toggle", "tooltip");
                    }
            }
        }

        private void RadGridDefault_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.Equals(RadGrid.ExportToExcelCommandName) || (e.CommandName == RadGrid.ExportToPdfCommandName))
                FormatoExportar((RadGrid)sender);
            else if (e.CommandName == RadGrid.PageCommandName)
            {
                string pagina;
                string filas;
                string totalFilas;
                RadNumericTextBox goToPageTextBox = e.Item.FindControl("GoToPageTextBox") as RadNumericTextBox;
                RadNumericTextBox changePageSizeTextBox = e.Item.FindControl("ChangePageSizeTextBox") as RadNumericTextBox;
                if (goToPageTextBox != null)
                    pagina = goToPageTextBox.Value.ToString();
                if (changePageSizeTextBox != null)
                    filas = changePageSizeTextBox.Value.ToString();

                //totalFilas = HfTotalFilas.Value;

                //HfPagina.Value = HfFilas.Value.Equals(filas) ? pagina : "1";
                //HfFilas.Value = filas;
            }
            else if (e.CommandName == RadGrid.FilterCommandName)
            {
                //HfPagina.Value = "1";
            }
        }

        private void RadGridDefault_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            //Elimina los carácteres ilegales al exportar el Grid a PDF
            string text = e.RawHTML;
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            e.RawHTML = new string(validXmlChars);
            //Modifica la cadena HTML al exportar el RadGrid para asignar el formato estándar
            e.RawHTML = e.RawHTML.Replace("border=\"1\"", " border=\"0\"");
            e.RawHTML = "<div style='text-align: center; font-family: Arial Unicode MS; font-size: 11pt; line-height: 16px;'>" + ViewState[consTituloPanel].ToString() + "</div><br />" + e.RawHTML;
        }
        #endregion

        #region Métodos generales
        ///<summary>Busca el RadGrid en cascada dentro de todos los controles de un control en específico.</summary>
        ///<param name="control">Es el control donde se busca el grid</param> 
        private void BuscarGrid(Control control)
        {
            if (control.Controls.OfType<RadGrid>().Count() > 0)
                foreach (Control grid in control.Controls.OfType<RadGrid>())
                    EstandarizarGrid((RadGrid)grid);
            else
                foreach (Control noGrid in control.Controls)
                    BuscarGrid(noGrid);
        }
        ///<summary>Actualiza el titulo al Panel visible con un DataBind().</summary>
        ///<param name="descripcion">Es el titulo para mostrar</param> 
        public void TituloPanel(string descripcion)
        {
            if (ViewState[consTituloPanel] == null)
                ViewState.Add(consTituloPanel, string.Empty);
            ViewState[consTituloPanel] = tituloPanel = tituloPagina + descripcion;
            DataBind(); //Asigna los valores de las variables referenciadas en HTML
        }
        ///<summary>Limpia los filtros del RadGrid proporcionado.</summary>
        ///<param name="grid">Es el RadGrid a limpiar.</param> 
        public void LimpiarFiltros(RadGrid grid)
        {
            foreach (GridColumn column in grid.MasterTableView.OwnerGrid.Columns)
            {
                column.CurrentFilterFunction = GridKnownFunction.NoFilter;
                column.CurrentFilterValue = string.Empty;
            }
            grid.MasterTableView.FilterExpression = string.Empty;
        }
        ///<summary>Aplica el formato estándar al exportar un RadGrid proporcionado.</summary>
        ///<param name="grid">Es el RadGrid a exportar.</param> 
        public void FormatoExportar(RadGrid grid)
        {
            isExport = true;
            grid.ExportSettings.FileName = ViewState[consTituloPanel].ToString();
            grid.ExportSettings.ExportOnlyData = true;
            grid.ExportSettings.IgnorePaging = true;
            grid.ExportSettings.OpenInNewWindow = true;
            grid.MasterTableView.Caption = "<b>" + ViewState[consTituloPanel].ToString() + "</b>";
        }
        ///<summary>Regresa el estatus de los artículos requeridos en el evento ItemsRequested de un RadComboBox.</summary>
        ///<param name="requeridos">Es la cantidad de registros solicitados.</param> 
        ///<param name="total">Es el total de registros que existen.</param> 
        public string EstatusArticulosRequeridos(int requeridos, int total)
        {
            if (total <= 0)
                return "Sin resultados";
            return String.Format("<b>1</b> a <b>{0}</b> resultados de <b>{1}</b>", requeridos, total);
        }
        ///<summary>Regresa el Id del lado del Cliente de un control específico dentro de un RadGrid.</summary>
        ///<param name="grid">Es el grid donde se enuentra el control.</param> 
        ///<param name="control">Es el control que se busca.</param> 
        public string ClientIdControl(RadGrid grid, string control)
        {
            if (grid.MasterTableView.GetItems(GridItemType.CommandItem).Count() > 0)
            {
                var ctrl = grid.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl(control);
                if (ctrl == null)
                    return string.Empty;
                else
                    return ctrl.ClientID;
            }
            else
                return string.Empty;
        }
        ///<summary>Regresa el UniqueId de un control específico dentro de un RadGrid.</summary>
        ///<param name="grid">Es el grid donde se enuentra el control.</param> 
        ///<param name="idControl">Es el control que se busca.</param> 
        public string UniqueIdControl(RadGrid grid, string idControl)
        {
            if (grid.MasterTableView.GetItems(GridItemType.CommandItem).Count() > 0)
            {
                var ctrl = grid.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl(idControl);
                if (ctrl == null)
                    return string.Empty;
                else
                    return ctrl.UniqueID;
            }
            else
                return string.Empty;
        }
        ///<summary>Asigna el estilo segun la condición para habilitar o deshabilitar un control RadComboBox.</summary>
        ///<param name="control">Es el control al que se le asigna la condición.</param> 
        ///<param name="enabled">Es la condición del control.</param> 
        public void EnableControl(RadComboBox control, bool enabled)
        {
            control.Enabled = enabled;
            if (enabled)
                control.Attributes.Remove("disabled");
            else
                control.Attributes.Add("disabled", "disabled");
        }
        ///<summary>Asigna el estilo segun la condición para habilitar o deshabilitar un control TextBox.</summary>
        ///<param name="control">Es el control al que se le asigna la condición.</param> 
        ///<param name="enabled">Es la condición del control.</param> 
        public void EnableControl(TextBox control, bool enabled)
        {
            control.Enabled = enabled;
            if (enabled)
                control.Attributes.Remove("disabled");
            else
                control.Attributes.Add("disabled", "disabled");
        }
        ///<summary>Agrega artículos a un control RadCombobox con datos filtrados del DataTable que coinciden con el dato ingresado por el usuario en una solicitud ItemsRequested.</summary>
        ///<param name="cmb">Es el control RadCombobox a poblar.</param> 
        ///<param name="dt">Es el DataTable con todos los datos a filtrar.</param> 
        ///<param name="campoTexto">Es el campo del DataTable a filtrar.</param> 
        ///<param name="campoValor">Es valor a buscar en el DataTable.</param> 
        ///<param name="e">Es el evento RadComboBoxItemsRequestedEventArgs donde se guarda el mensaje de estado de la solicitud del RadComboBox.</param>
        public void LlenarCmbRequested(RadComboBox cmb, DataTable dt, string campoTexto, string campoValor, RadComboBoxItemsRequestedEventArgs e)
        {
            e.Message = EstatusArticulosRequeridos(0, 0);

            if (dt != null)
                if (dt.Select(campoTexto + " LIKE '%" + e.Text + "%'").Count() > 0)
                {
                    dt = dt.Select(campoTexto + " LIKE '%" + e.Text + "%'").CopyToDataTable();
                    int itemOffset = e.NumberOfItems;
                    int endOffset = Math.Min(itemOffset + 10, dt.Rows.Count);
                    e.EndOfItems = endOffset == dt.Rows.Count;

                    for (int i = itemOffset; i < endOffset; i++)
                        cmb.Items.Add(new RadComboBoxItem(dt.Rows[i][campoTexto].ToString(), dt.Rows[i][campoValor].ToString()));
                    e.Message = EstatusArticulosRequeridos(endOffset, dt.Rows.Count);
                }
        }
        ///<summary>Comprueba si la cadena del objeto esta vacía.</summary>
        ///<param name="str">Es el objeto del que se obtiene la cadena a evaluar.</param> 
        public bool IsEmpty(object str) { return str.ToString().Trim().Equals(string.Empty); }

        #endregion

        #region Modals
        ///<summary>Oculta el modal activo.</summary>
        ///<param name="idModal">Es el ID del control que contiene el modal.</param> 
        public void OcultarModal(string idModal)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "OcultarModal", "<script> $('#" + idModal + "').modal('hide');</script>", false);
        }
        ///<summary>Muestra el modal programado en un botón.</summary>
        ///<param name="triggerBtn">Es el botón que activa el modal.</param> 
        public void MostrarModal(string triggerBtn)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "MostrarModal", "<script> document.getElementById('" + triggerBtn + "').click(); </script> ", false);
        }

        #endregion

        #region Alertas
        ///<summary>Muestra una alerta de éxito.</summary>
        ///<param name="mensaje">Es el mensaje para mostrar en la alerta.</param> 
        public void AlertSuccess(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "", "<script> document.getElementById('btnSuccess').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnSuccess').click(); </script> ", false);
        }
        ///<summary>Muestra una alerta de error.</summary>
        ///<param name="mensaje">Es el mensaje para mostrar en la alerta.</param> 
        public void AlertError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "", "<script> document.getElementById('btnError').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnError').click(); </script> ", false);
        }
        ///<summary>Muestra una alerta de confirmación.</summary>
        ///<param name="mensaje">Es el mensaje para mostrar en la alerta.</param> 
        ///<param name="btnUniqueId">Es el UniqueId del botón que ejecuta el proceso al confirmar la alerta.</param> 
        public void AlertQuestion(string mensaje, string btnUniqueId)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "",
                "<script> document.getElementById('btnOk').setAttribute('onclick', \"javascript:__doPostBack('" + btnUniqueId + "', '')\"); document.getElementById('btnQuestion').setAttribute('data-whatever', '" + mensaje + "'); document.getElementById('btnQuestion').click(); </script> ", false);
        }
        #endregion

        #region Expresiones Regulares
        ///<summary>Expresión regular para validar sólo texto, ñ y vocales con acentos.</summary>
        public Regex rexTexto = new Regex(@"^([A-z ÑñÁáÉéÍíÓóÚúÜü])+$");
        ///<summary>Expresión regular para validar correo electrónico.</summary>
        public Regex rexCorreo = new Regex(@"^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$");
        ///<summary>Expresión regular para validar números.</summary>
        public Regex rexNum = new Regex(@"^[0-9]*$");
        ///<summary>Expresión regular para validar números. con hasta 5 decimales</summary>
        public Regex rexDecimal = new Regex(@"^[0-9]{0,9}(\.[0-9]{0,5})?$");
        ///<summary>Expresión regular para validar sólo texto, ñ, vocales con acentos y números.</summary>
        public Regex rexTextNum = new Regex(@"^([A-z ÑñÁáÉéÍíÓóÚúÜü\d])+$");
        ///<summary>Expresión regular para validar sólo texto, ñ, vocales con acentos, números y puntos.</summary>
        public Regex rexCustom = new Regex(@"^([A-z ÑñÁáÉéÍíÓóÚúÜü.\d])+$");
        ///<summary>Expresión regular para validar sólo texto, ñ, vocales con acentos, números, puntos y carácteres {+,-,/}.</summary>
        public Regex rexSpecial = new Regex(@"^([A-z ÑñÁáÉéÍíÓóÚúÜü.+-/\-\d])+$");
        #endregion
    }
}