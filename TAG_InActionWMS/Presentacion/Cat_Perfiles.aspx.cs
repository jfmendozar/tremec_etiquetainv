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
using System.Text;
using System.Web.Services;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class Cat_Perfiles : PageEstandar
    {
        #region Definicion de objetos y variables
        Inicio inicio = new Inicio();
        Perfiles perfiles = new Perfiles();
        ControlExcepciones excepcion = new ControlExcepciones();
        static DataTable dtAcciones;
        static List<int> listItemSelected = new List<int>();
        string pair1 = string.Empty, pair2 = string.Empty;
        static string nombreArchivo = string.Empty;
        static string tituloPagina = string.Empty;
        //protected static string tituloPanel = string.Empty;
        //static bool permisoConsultar = false;
        //static bool permisoAgregar = false;
        //static bool permisoEditar = false;
        //static bool permisoEliminar = false;
        //static bool permisoExportar = false;
        //bool isExport = false;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //{
            //    TituloPanel(string.Empty);
            //    listItemSelected = new List<int>();
            //}

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
                    permisoAgregar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Agregar"].ToString()));
                    permisoEditar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Editar"].ToString()));
                    permisoEliminar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Eliminar"].ToString()));
                    permisoExportar = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0]["Exportar"].ToString()));
                    Page.Title = tituloPagina;
                }
                TituloPanel(string.Empty);
                listItemSelected = new List<int>();
            }
            #endregion
        }

        #region Codigo que se encuentra en el PageEstandar
        //private void LimpiarFiltros(RadGrid radgrid)
        //{
        //    foreach (GridColumn column in radgrid.MasterTableView.OwnerGrid.Columns)
        //    {
        //        column.CurrentFilterFunction = GridKnownFunction.NoFilter;
        //        column.CurrentFilterValue = string.Empty;
        //    }
        //    radgrid.MasterTableView.FilterExpression = string.Empty;
        //}

        //private void TituloPanel(string descripcion)
        //{
        //    tituloPanel = tituloPagina + descripcion;
        //    DataBind();
        //}
        #endregion

        private void OcultarModal()
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "OcultarModal", "<script> $('#Modal1').modal('hide');</script>", false);
        }

        private void MostrarModal()
        {
            if (HfIdPerfil.Value.Length > 0)
                ScriptManager.RegisterStartupScript(this.Page, typeof(String), "MostrarModal", "<script> document.getElementById('btnEditarH').click(); </script> ", false);
            else
                ScriptManager.RegisterStartupScript(this.Page, typeof(String), "MostrarModal", "<script> document.getElementById('btnNuevoH').click(); </script> ", false);

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

        private void GuardarItemsSeleccionados(RadGrid radgrid)
        {
            listItemSelected = new List<int>();
            foreach (GridDataItem item in radgrid.SelectedItems)
                listItemSelected.Add(Convert.ToInt32(item["IdPerfil"].Text));
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
            RadGrid1.Rebind();
            CargarRadGrid2();
        }

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new DataTable();
            string mensaje = "";
            if (Page.IsPostBack)
                dt = perfiles.PerfilConsultar(ref mensaje);

            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            RadGrid1.DataSource = dt == null ? new DataTable() : dt;
        }

        protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
        {
            string mensaje = string.Empty;
            GuardarItemsSeleccionados(RadGrid1);
            if (e.CommandName.Equals("Nuevo"))
            {
                LimpiarCampos();
                MostrarModal();
            }
            else if (e.CommandName.Equals("Editar"))
            {
                DataTable dt = new DataTable();
                LimpiarCampos();
                foreach (GridDataItem item in RadGrid1.SelectedItems)
                {
                    HfIdPerfil.Value = item["IdPerfil"].Text.Trim();
                    TxtNombre.Text = item["Nombre"].Text.Trim();
                    TxtClave.Text = item["Clave"].Text.Trim();
                    dt = perfiles.PerfilModuloAccionConsultar(Convert.ToInt32(HfIdPerfil.Value), ref mensaje);

                    if (!mensaje.Equals(string.Empty))
                        (this.Master as Principal).AlertError(mensaje);
                    else
                        //Marca los permisos del perfil
                        CargarPermisos(dt);
                }

                MostrarModal();
            }
            else if (e.CommandName.Equals("Eliminar"))
            {
                foreach (GridDataItem item in RadGrid1.SelectedItems)
                    HfIdPerfil.Value = item["IdPerfil"].Text.Trim();

                perfiles.PerfilEliminar(Convert.ToInt32(Session["IdUsuario"]), Convert.ToInt32(HfIdPerfil.Value), ref mensaje);
                if (mensaje.Equals(ControlExcepciones.Ok))
                {
                    (this.Master as Principal).AlertSuccess("El perfil se eliminó.");
                    RadGrid1.Rebind();
                }
                else if (mensaje.Equals(ControlExcepciones.Existe))
                    (this.Master as Principal).AlertError("No se puede eliminar un perfil asignado a un usuario.");
                else
                    (this.Master as Principal).AlertError(mensaje);
            }
            else if (e.CommandName.Equals(RadGrid.ExportToExcelCommandName))
                ConfigureExport(RadGrid1);
            else if (e.CommandName == RadGrid.ExportToPdfCommandName)
            {
                RadGrid1.MasterTableView.GetColumn("Nombre").HeaderStyle.Width =
                RadGrid1.MasterTableView.GetColumn("Clave").HeaderStyle.Width = 290;
                ConfigureExport(RadGrid1);
            }
        }

        protected void RadGrid1_PreRender(object sender, EventArgs e)
        {
            GridCommandItem itemsRadGrid1 = (GridCommandItem)RadGrid1.MasterTableView.GetItems(GridItemType.CommandItem)[0];
            LinkButton btnNuevo = itemsRadGrid1.FindControl("btnNuevo") as LinkButton;
            btnNuevo.Enabled = btnNuevo.Visible = permisoAgregar;
            LinkButton btnEditar = itemsRadGrid1.FindControl("btnEditar") as LinkButton;
            btnEditar.Enabled = btnEditar.Visible = permisoEditar;
            LinkButton btnEliminar = itemsRadGrid1.FindControl("btnEliminar") as LinkButton;
            btnEliminar.Enabled = btnEliminar.Visible = permisoEliminar;
            LinkButton btnExcel = itemsRadGrid1.FindControl("btnExcel") as LinkButton;
            btnExcel.Enabled = btnExcel.Visible = permisoExportar;
            LinkButton btnPdf = itemsRadGrid1.FindControl("btnPdf") as LinkButton;
            btnPdf.Enabled = btnPdf.Visible = permisoExportar;
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
                    item.Selected = listItemSelected.Contains(Convert.ToInt32(item["IdPerfil"].Text));
        }

        protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //Asigna tooltips cuando no exporta
            if (!isExport)
                (this.Master as Principal).SetTooltips(sender, e);
            //Agrega un espacio a los items de la columna Clave para evitar formato en Excel
            else if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                item["Clave"].Text = String.Format("&nbsp;{0}", item["Clave"].Text);
            }
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
                    item["Nombre"].Width =
                    item["clave"].Width = 120;
                }
        }

        protected void RadGrid1_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            e.RawHTML = e.RawHTML.Replace("border=\"1\"", " border=\"0\"");
            e.RawHTML = "<div style='text-align: center; font-family: Arial Unicode MS; font-size: 11pt; line-height: 16px;'>" + tituloPanel + "</div><br />" + e.RawHTML;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string ModuloPermisos = string.Empty;
            string mensaje = string.Empty;
            int idPerfil = 0;
            int IdUsuarioSession = Convert.ToInt32(Session["IdUsuario"]);

            if (ValidarCampos())
            {
                //EDITAR
                if (HfIdPerfil.Value.Length > 0)
                    idPerfil = Convert.ToInt32(HfIdPerfil.Value);

                //AGREGAR PERMISOS al IdPerfil seleccionado
                int i = 0;
                string permisos = "";
                string[] sDatos = new string[RadGrid2.MasterTableView.Items.Count];

                foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
                {
                    bool agregar = false;
                    permisos = item["IdModulo"].Text;

                    foreach (DataRow drAccion in dtAcciones.Rows)
                    {
                        string sAccion = drAccion["Accion"].ToString().Replace(" ", "");
                        CheckBox cbAccion = ((CheckBox)(item[sAccion].Controls[0]));
                        permisos = permisos + "," + Convert.ToInt32(cbAccion.Checked);

                        if (cbAccion.Checked)
                            agregar = true;
                    }

                    if (!agregar)
                        continue;

                    sDatos[i] = permisos;
                    i++;
                    item.Selected = false;
                }

                //Elimina los valores items nulos
                sDatos = sDatos.Where(c => c != null).ToArray();
                //Unir la lista de registros con un   \r\n
                ModuloPermisos = string.Join("|", sDatos);

                //ACTUALIZA los permisos del perfil
                perfiles.PerfilModuloAccionAgregar(idPerfil, TxtNombre.Text.Trim(), TxtClave.Text.Trim(), ModuloPermisos, IdUsuarioSession, ref mensaje);

                //MOSTRAR RESULTADO
                if (mensaje.Equals(ControlExcepciones.Ok))
                {
                    OcultarModal();
                    (this.Master as Principal).AlertSuccess("El perfil se " + (HfIdPerfil.Value.Length > 0 ? "actualizó" : "agregó") + ".");
                    RadGrid1.Rebind();
                    HfIdPerfil.Value = string.Empty;
                }
                else if (mensaje.Equals(ControlExcepciones.Existe))
                    (this.Master as Principal).AlertError("El perfil \"" + TxtNombre.Text + "\" ya existe.");
                else
                    (this.Master as Principal).AlertError(mensaje);
            }
            else
                (this.Master as Principal).AlertError("Datos incompletos.");
        }

        private void CargarRadGrid2()
        {
            //DataTable dt = new DataTable();
            DataSet dsAccionesModulos = new DataSet();
            //DataTable dtAcciones = new DataTable();
            DataTable dtModulos = new DataTable();
            string mensaje = string.Empty;

            dsAccionesModulos = perfiles.ModuloConsultar(ref mensaje);

            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            //Dividir tablas de consulta
            dtAcciones = dsAccionesModulos.Tables[0];
            RadGrid3.DataSource = dtAcciones == null ? new DataTable() : dtAcciones;
            RadGrid3.DataBind();

            dtModulos = dsAccionesModulos.Tables[1];

            //Agregar columnas al RadGrid2
            GridCheckBoxColumn boundColumn1;
            foreach (DataRow drAccion in dtAcciones.Rows)
            {
                string sAccion = drAccion["Accion"].ToString();
                if (RadGrid2.MasterTableView.Columns.Contains(sAccion))
                    continue;

                boundColumn1 = new GridCheckBoxColumn();
                RadGrid2.MasterTableView.Columns.Add(boundColumn1);
                boundColumn1.DataField = sAccion;
                boundColumn1.ToolTip = sAccion;
                boundColumn1.HeaderText = sAccion;
                boundColumn1.UniqueName = sAccion.Replace(" ", "");
                boundColumn1.HeaderStyle.Width = 70;
                boundColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                boundColumn1.Visible = true;
            }

            RadGrid2.DataSource = dtModulos == null ? new DataTable() : dtModulos;
            RadGrid2.DataBind();
            CargarAcciones();
        }

        private void CargarAcciones()
        {
            DataTable dtModulosAcciones = null;
            string mensaje = string.Empty;

            try
            {
                //Obtener el conjunto de datos de dos tablas
                dtModulosAcciones = perfiles.ModuloAccionConsultar(ref mensaje);
                //Si ocurrio algún error, notificarlo
                if (!mensaje.Equals(string.Empty))
                {
                    (this.Master as Principal).AlertError(mensaje);
                    return;
                }

                //Obtneer la lista de acciones que no se encuentran en la tabla modulosAcciones
                List<DataRow> lAccionNo = new List<DataRow>();
                foreach (DataRow drAccion in dtAcciones.Rows)
                {
                    if (!dtModulosAcciones.Columns.Contains(drAccion["Accion"].ToString()))
                    {
                        lAccionNo.Add(drAccion);
                    }
                }
                //Eliminar permisos que no existen en los modulos
                foreach (DataRow noAccion in lAccionNo)
                {
                    GridColumn boundColumn1 = RadGrid2.MasterTableView.GetColumn(noAccion["Accion"].ToString().Replace(" ", ""));
                    RadGrid2.MasterTableView.Columns.Remove(boundColumn1);
                    dtAcciones.Rows.Remove(noAccion);
                }

                //Recorrer acciones y modulos
                foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
                {
                    foreach (DataRow drAccion in dtAcciones.Rows)
                    {
                        string sAccion = drAccion["Accion"].ToString();

                        try
                        {
                            bool permiso = (from DataRow permisos in dtModulosAcciones.Rows
                                            where permisos["IdModulo"].ToString().Equals(item["IdModulo"].Text)
                                            select Convert.ToBoolean(permisos[sAccion])).First<bool>();

                            CheckBox cbAccion = ((CheckBox)(item[sAccion.Replace(" ", "")].Controls[0]));
                            //cbAccion.ID = sAccion;
                            cbAccion.ToolTip = sAccion;
                            cbAccion.Attributes.Add("runat", "server");
                            cbAccion.Attributes.Add("onclick", "permisosClick(this, event);");
                            //cbAccion.CheckedChanged += new EventHandler(this.CbxMarcarAccion_CheckedChanged);
                            //cbAccion.AutoPostBack = true;
                            cbAccion.Checked = false;
                            cbAccion.Enabled = true;
                            cbAccion.Visible = permiso;
                        }
                        catch (Exception)
                        {
                            //Cacha el error, si la accion no existe en la tabla de modulos acciones.
                        }
                    }
                }
            }
            catch (Exception e) { string erro = e.Message; }
            finally
            {
                //Liberar objetos
                if (dtModulosAcciones != null)
                    dtModulosAcciones.Dispose();
            }
        }

        private bool ValidarCampos()
        {
            bool flg = true;

            if (TxtNombre.Text.Trim().Equals(string.Empty))
                flg = false;

            return flg;
        }

        private void LimpiarCampos()
        {
            HfIdPerfil.Value = TxtNombre.Text = TxtClave.Text = string.Empty;
            LimpiarPermisos();
            //Deselecciona las filas seleccionadas
            foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
                item.Selected = false;
        }

        private void LimpiarPermisos()
        {
            //Desmarca todos los permisos
            foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
            {
                ((CheckBox)(item["Todo"].Controls[1])).Checked = false;

                foreach (DataRow drAccion in dtAcciones.Rows)
                {
                    ((CheckBox)(item[drAccion["Accion"].ToString().Replace(" ", "")].Controls[0])).Checked = false;
                }
            }

            //foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
            //{
            //    ((CheckBox)(item["Todo"].Controls[1])).Checked =
            //        ((CheckBox)(item["Agregar"].Controls[1])).Checked =
            //        ((CheckBox)(item["Consultar"].Controls[1])).Checked =
            //        ((CheckBox)(item["Editar"].Controls[1])).Checked =
            //        ((CheckBox)(item["Eliminar"].Controls[1])).Checked =
            //        ((CheckBox)(item["Exportar"].Controls[1])).Checked = false;
            //}
        }

        private void CargarPermisos(DataTable dtPermisos)
        {
            bool marcarTodo = true;

            //Marca los permisos
            if (dtPermisos != null)
            {
                //HabiliarPermisos(true);
                foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
                {
                    string[] IdModulos = (from DataRow permisos in dtPermisos.Rows
                                          where permisos["IdModulo"].ToString().Equals(item["IdModulo"].Text)
                                          select permisos["IdModulo"].ToString()).Distinct().ToArray<string>();

                    if (IdModulos.Length == 0)
                        continue;

                    CheckBox chkTodo = ((CheckBox)(item["Todo"].Controls[1]));
                    bool ctlTotal = true;

                    foreach (DataRow drAccion in dtAcciones.Rows)
                    {
                        string Accion = drAccion["Accion"].ToString();

                        try
                        {
                            bool permiso = (from DataRow permisos in dtPermisos.Rows
                                            where permisos["IdModulo"].ToString().Equals(item["IdModulo"].Text)
                                            select Convert.ToBoolean(permisos[Accion])).First<bool>();

                            CheckBox cbAccion = ((CheckBox)(item[Accion.Replace(" ", "")].Controls[0]));
                            cbAccion.Checked = permiso;

                            if (cbAccion.Visible && !permiso)
                                ctlTotal = false;
                        }
                        catch (Exception)
                        {
                            //Cacha el error, si la accion no existe en la tabla de modulos acciones.
                        }
                    }
                    chkTodo.Checked = ctlTotal;
                }
            }

            //Valida checkbox Todo esta activo en todos los permisos
            foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
                if (marcarTodo)
                    marcarTodo = ((CheckBox)(item["Todo"].Controls[1])).Checked;
            CbxMarcarTodo.Checked = marcarTodo; //Asigna valor a checkbox Marcar Todo de la validación checkbox Todo en todos los permisos
            CbxMarcarTodo.Enabled = true;
            IcoMarcarTodo.Attributes["class"] = CbxMarcarTodo.Checked ? "glyphicon glyphicon-check" : "glyphicon glyphicon-unchecked";
            LblMarcarTodo.Attributes["class"] = "btn btn-primary btn-sm"; //Habilita checkbox Marcar Todo
        }

        private GridKnownFunction GetFilterFunction(string filter)
        {
            switch (filter)
            {
                case "Between":
                    return GridKnownFunction.Between;
                case "Contains":
                    return GridKnownFunction.Contains;
                case "DoesNotContain":
                    return GridKnownFunction.DoesNotContain;
                case "EndsWith":
                    return GridKnownFunction.EndsWith;
                case "EqualTo":
                    return GridKnownFunction.EqualTo;
                case "GreaterThan":
                    return GridKnownFunction.GreaterThan;
                case "GreaterThanOrEqualTo":
                    return GridKnownFunction.GreaterThanOrEqualTo;
                case "IsEmpty":
                    return GridKnownFunction.IsEmpty;
                case "IsNull":
                    return GridKnownFunction.IsNull;
                case "LessThan":
                    return GridKnownFunction.LessThan;
                case "LessThanOrEqualTo":
                    return GridKnownFunction.LessThanOrEqualTo;
                case "NotBetween":
                    return GridKnownFunction.NotBetween;
                case "NotEqualTo":
                    return GridKnownFunction.NotEqualTo;
                case "NotIsEmpty":
                    return GridKnownFunction.NotIsEmpty;
                case "NotIsNull":
                    return GridKnownFunction.NotIsNull;
                case "StartsWith":
                    return GridKnownFunction.StartsWith;
                default:
                    return GridKnownFunction.NoFilter;
            }
        }

        protected void Todos_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbTodos = (sender as CheckBox);
            GridDataItem RowNo = (sender as CheckBox).Parent.Parent as GridDataItem;

            foreach (DataRow drAccion in dtAcciones.Rows)
            {
                string Accion = drAccion["Accion"].ToString().Replace(" ", "");
                CheckBox cbAccion = ((CheckBox)(RowNo[Accion].Controls[0]));

                if (cbAccion.Visible)
                    cbAccion.Checked = cbTodos.Checked;
            }
        }

        protected void CbxMarcarTodo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbTodos = (sender as CheckBox);

            foreach (GridDataItem item in RadGrid2.MasterTableView.Items)
            {
                ((CheckBox)(item["Todo"].Controls[1])).Checked = cbTodos.Checked;

                foreach (DataRow drAccion in dtAcciones.Rows)
                {
                    string sAccion = drAccion["Accion"].ToString().Replace(" ", "");

                    CheckBox cbAccion = ((CheckBox)(item[sAccion].Controls[0]));
                    if (cbAccion.Visible)
                        cbAccion.Checked = cbTodos.Checked;
                }
            }
        }

        protected void CbxMarcarAccion_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbAccionMarcada = (sender as CheckBox);
            GridDataItem item = (sender as CheckBox).Parent.Parent as GridDataItem;
            bool todos = true;

            foreach (DataRow drAccion in dtAcciones.Rows)
            {
                string sAccion = drAccion["Accion"].ToString().Replace(" ", "");

                CheckBox cbAccion = ((CheckBox)(item[sAccion].Controls[0]));
                if (cbAccion.Visible && !cbAccion.Checked)
                    todos = false;
            }
            ((CheckBox)(item["Todo"].Controls[1])).Checked = todos;
        }

        private string GetSqlQuery(string filterFunction, string field, string value)
        {
            switch (filterFunction)
            {
                case "Contains":
                    return string.Format("([{0}] LIKE '%{1}%')", field, value);
                case "DoesNotContain":
                    return string.Format("([{0}] NOT LIKE '%{1}%')", field, value);
                case "StartsWith":
                    return string.Format("([{0}] LIKE '{1}%')", field, value);
                case "EndsWith":
                    return string.Format("([{0}] LIKE '%{1}')", field, value);
                case "EqualTo":
                    return string.Format("([{0}] = '{1}')", field, value);
                case "NotEqualTo":
                    return string.Format("([{0}] != '{1}')", field, value);
                case "GreaterThan":
                    return string.Format("([{0}] > '{1}')", field, value);
                case "LessThan":
                    return string.Format("([{0}] < '{1}')", field, value);
                case "GreaterThanOrEqualTo":
                    return string.Format("([{0}] >= '{1}')", field, value);
                case "LessThanOrEqualTo":
                    return string.Format("([{0}] <= '{1}')", field, value);
                //case "Between":
                //    return string.Format("([{0}] LIKE '%{1}%')", field, value);
                //case "NotBetween":
                //    return string.Format("([{0}] != '{1}')", field, value);
                case "IsEmpty":
                    return string.Format("([{0}] = '')", field);
                case "NotIsEmpty":
                    return string.Format("([{0}] != '')", field);
                case "IsNull":
                    return string.Format("([{0}] IS NULL)", field);
                case "NotIsNull":
                    return string.Format("([{0}] IS NOT NULL)", field);
                default:
                    return "";
            }
        }

    }
}