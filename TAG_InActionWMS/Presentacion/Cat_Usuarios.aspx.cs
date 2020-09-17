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
    public partial class Cat_Usuarios : PageEstandar
    {
        #region Definicion de objetos y variables
        Inicio inicio = new Inicio();
        Usuarios usuarios = new Usuarios();
        Perfiles perfiles = new Perfiles();
        ControlExcepciones excepcion = new ControlExcepciones();
        static DataTable dtAcciones;
        static List<int> listItemSelected = new List<int>();
        static string nombreArchivo = string.Empty;
        static string tituloPagina = string.Empty;
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
            #endregion

            TxtContrasena.Attributes.Add("value", TxtContrasena.Text);
        }
        
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
            RadGrid1.Rebind();
            CargarCmbPerfil();
            CargarCmbTipo();
            CargarRadGrid2();
        }

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new DataTable();
            string mensaje = "";
            if (Page.IsPostBack)
                dt = usuarios.UsuarioConsultar(ref mensaje);

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
                    HfIdUsuario.Value = item["IdUsuario"].Text.Trim();
                    TxtUsuario.Text = item["Usuario"].Text.Trim();
                    TxtNombre.Text = item["Nombre"].Text.Trim();
                    CmbTipo.FindItemByValue(item["IdTipoUsuario"].Text.Trim()).Selected = true;
                    CmbPerfil.FindItemByValue(item["IdPerfil"].Text.Trim()).Selected = true;
                    //Se valida para saber si mostrar etiqueta de personalizado
                    if (item["Perfil"].Text.Contains("Personalizado"))
                        lblPersonalizado.Visible = true;
                    else
                        lblPersonalizado.Visible = false;

                    CargarPermisosUsuario();
                }
                MostrarModal();
            }
            else if (e.CommandName.Equals("Eliminar"))
            {
                foreach (GridDataItem item in RadGrid1.SelectedItems)
                    HfIdUsuario.Value = item["IdUsuario"].Text.Trim();

                usuarios.UsuarioEliminar(Convert.ToInt32(Session["IdUsuario"]), Convert.ToInt32(HfIdUsuario.Value), ref mensaje);
                if (mensaje.Equals(ControlExcepciones.Ok))
                {
                    (this.Master as Principal).AlertSuccess("El usuario se eliminó.");
                    RadGrid1.Rebind();
                }
                else
                    (this.Master as Principal).AlertError(mensaje);
            }
            else if (e.CommandName.Equals(RadGrid.ExportToExcelCommandName))
                ConfigureExport(RadGrid1);
            else if (e.CommandName == RadGrid.ExportToPdfCommandName)
            {
                RadGrid1.MasterTableView.GetColumn("Nombre").HeaderStyle.Width = 300;
                RadGrid1.MasterTableView.GetColumn("Usuario").HeaderStyle.Width =
                RadGrid1.MasterTableView.GetColumn("Perfil").HeaderStyle.Width =
                RadGrid1.MasterTableView.GetColumn("TipoUsuario").HeaderStyle.Width = 90;
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
                    item.Selected = listItemSelected.Contains(Convert.ToInt32(item["IdUsuario"].Text));
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
                    item["Usuario"].Width =
                    item["Perfil"].Width =
                    item["Nombre"].Width =
                    item["TipoUsuario"].Width = 120;
                }
        }

        protected void RadGrid1_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            e.RawHTML = e.RawHTML.Replace("border=\"1\"", " border=\"0\"");
            e.RawHTML = "<div style='text-align: center; font-family: Arial Unicode MS; font-size: 11pt; line-height: 16px;'>" + tituloPanel + "</div><br />" + e.RawHTML;
        }

        protected void CmbPerfil_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            DataTable dt = new DataTable();
            string mensaje = string.Empty;
            if (((RadComboBox)(sender)).SelectedValue.Equals(string.Empty))
                return;
            //Marca los permisos del perfil seleccionado
            int idPerfil = Convert.ToInt32(((RadComboBox)(sender)).SelectedValue);
            if (idPerfil > 0)
            {
                dt = perfiles.PerfilModuloAccionConsultar(idPerfil, ref mensaje);
                if (!mensaje.Equals(string.Empty))
                    (this.Master as Principal).AlertError(mensaje);
                else
                {
                    LimpiarPermisos();
                    CargarPermisos(dt);
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            string ModuloPermisos = string.Empty; //Permisos
            int IdUsuarioSession = Convert.ToInt32(Session["IdUsuario"]);

            if (ValidarCampos(ref mensaje))
            {
                int idUsuario = 0;
                mensaje = string.Empty;

                if (HfIdUsuario.Value.Length > 0)
                {
                    //Id del usuario que se Actualizará
                    idUsuario = Convert.ToInt32(HfIdUsuario.Value);
                }

                //Agregar permisos
                int i = 0;
                string permisos = "";
                string[] sDatos = new string[RadGrid2.MasterTableView.Items.Count];

                //Recuperar todos los permisos por modulo
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

                //AGREGAR USUARIO
                usuarios.UsuarioPermisoAgregar(idUsuario, TxtUsuario.Text.Trim(), TxtNombre.Text.Trim(), TxtContrasena.Text.Trim(),
                 Convert.ToInt32(CmbTipo.SelectedValue), Convert.ToInt32(CmbPerfil.SelectedValue), ModuloPermisos, IdUsuarioSession, ref mensaje);

                if (mensaje.Equals(ControlExcepciones.Ok))
                {
                    OcultarModal();
                    (this.Master as Principal).AlertSuccess("El usuario se " + (HfIdUsuario.Value.Length > 0 ? "actualizó" : "agregó") + ".");
                    RadGrid1.Rebind();
                    HfIdUsuario.Value = string.Empty;
                }
                else if (mensaje.Equals(ControlExcepciones.Existe))
                    (this.Master as Principal).AlertError("El usuario \"" + TxtUsuario.Text + "\" ya existe.");
                else
                    (this.Master as Principal).AlertError(mensaje);
            }
            else
                (this.Master as Principal).AlertError("Datos incompletos. " + mensaje);
        }

        private void CargarCmbPerfil()
        {
            DataTable dt = new DataTable();
            string mensaje = string.Empty;
            dt = perfiles.PerfilConsultar(ref mensaje);
            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            CmbPerfil.DataSource = dt == null ? new DataTable() : dt;
            CmbPerfil.DataBind();
        }

        private void CargarCmbTipo()
        {
            DataTable dt = new DataTable();
            string mensaje = string.Empty;
            dt = usuarios.TipoUsuarioConsultar(ref mensaje);
            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);

            CmbTipo.DataSource = dt == null ? new DataTable() : dt;
            CmbTipo.DataBind();
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
                            cbAccion.ID = sAccion;
                            cbAccion.ToolTip = sAccion;
                            cbAccion.Attributes.Add("runat", "server");
                            cbAccion.Attributes.Add("onclick", "permisosClick(this, event);");
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

        private void CargarPermisosUsuario()
        {
            //Marca los permisos del usuario seleccionado
            DataTable dt = new DataTable();
            string mensaje = string.Empty;
            int idUsuario = Convert.ToInt32(HfIdUsuario.Value);
            dt = usuarios.UsuarioObtenerPermisos(idUsuario, ref mensaje);
            if (!mensaje.Equals(string.Empty))
                (this.Master as Principal).AlertError(mensaje);
            else
                CargarPermisos(dt);
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
                listItemSelected.Add(Convert.ToInt32(item["IdUsuario"].Text));
        }

        private bool ValidarCampos(ref string mensaje)
        {
            string html = string.Empty;
            html = "<p align=\"left\">";
            if (TxtNombre.Text.Trim().Equals(string.Empty))
                mensaje += "<br> - Campo Nombre obligatorio.";

            if (TxtUsuario.Text.Trim().Length < 2)
                mensaje += "<br> - Campo Usuario mínimo 2 carácteres. ";

            if ((TxtContrasena.Text.Trim().Length > 0 && TxtContrasena.Text.Trim().Length < 3)
                || (TxtContrasena.Text.Trim().Length == 0 && HfIdUsuario.Value.Equals(string.Empty)))
                mensaje += "<br> - Campo Contraseña mínimo 3 carácteres. ";

            if (CmbTipo.SelectedIndex < 0)
                mensaje += "<br> - Seleccione un Tipo de usuario. ";

            if (CmbPerfil.SelectedIndex < 0)
                mensaje += "<br> - Seleccione un Perfil. ";

            if (mensaje.Length > 0)
            {
                mensaje = html + mensaje + "</p>";
                return false;
            }
            else
                return true;
        }

        private void CargarPermisos(DataTable dtPermisos)
        {
            try
            {
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
            }
            catch (Exception e) { string err = e.Message; }
        }

        private void LimpiarCampos()
        {
            HfIdUsuario.Value = TxtNombre.Text = TxtUsuario.Text = string.Empty;
            TxtContrasena.Attributes.Add("value", string.Empty);
            lblPersonalizado.Visible = false;
            CmbTipo.ClearSelection();
            CmbPerfil.ClearSelection();
            LimpiarPermisos();// false);
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
        }

        private void OcultarModal()
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(String), "OcultarModal", "<script> $('#Modal1').modal('hide');</script>", false);
        }

        private void MostrarModal()
        {
            if (HfIdUsuario.Value.Length > 0)
                ScriptManager.RegisterStartupScript(this.Page, typeof(String), "MostrarModal", "<script> document.getElementById('btnEditarH').click(); </script> ", false);
            else
                ScriptManager.RegisterStartupScript(this.Page, typeof(String), "MostrarModal", "<script> document.getElementById('btnNuevoH').click(); </script> ", false);

        }

    }
}