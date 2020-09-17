using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Excel;
using Telerik.Web.Spreadsheet;
using Telerik.Web.UI;
using TREMEC_EtiquetaInventario_WS.Negocios;

namespace TREMEC_EtiquetaInventario_WS.Presentacion
{
    public partial class CargaMasiva : PageEstandar
    {
        #region Constantes ViewState
        const string _xlsValido = "xlsValido";
        const string _path = "path";
        const string _temp = "temp";
        const string _carga = "carga";
        #endregion

        #region Definicion de objetos y variables
        ControlExcepciones excepcion = new ControlExcepciones();
        ConexionCarga carga = new ConexionCarga();
        static string nombreArchivo = string.Empty;
        static string tituloPagina = string.Empty;
        protected static string encFecha = string.Empty;
        protected static string encUbicacion = string.Empty;
        //protected static string tituloPanel = string.Empty;
        //static bool permisoConsultar = false;
        //static bool permisoExportar = false;
        //static bool busqueda = false;
        static List<int> listaItemsSel = new List<int>();
        static List<int> listaItemsSelDetalle = new List<int>();
        static List<int> listItemSelected = new List<int>();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logged"] == null) //No hay sesión
                Response.Redirect("Login.aspx?redirect=" + Request.Path.Substring(Request.Path.LastIndexOf("/") + 1));
            else if (!Page.IsPostBack)
                Session["Tab"] = "Inicio";

            if (!Page.IsPostBack)
            {
                TituloPanel(string.Empty);
                listItemSelected = new List<int>();
                ViewState[_xlsValido] = false;
                TituloPanel(string.Empty);
                Session["export"] = false;
                Session["dtXls"] = new DataTable();
                Session["nombre"] = "Español";

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
            }
            CambiarIdioma(Session["nombre"].ToString());
        }

        #region Eventos RadGrid1

        protected void RgXls_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dtConexion = new DataTable();
            dtConexion = ViewState["conexion"] as DataTable;
            string mensaje = "";

            if (!Page.IsPostBack)
            {
                dt.Columns.Add("Column");
                RadIdioma.Visible = false;
                RlIdioma.Visible = false;
                RbtIdentity.Visible = false;

                //btnLayout.Attributes.Add("disabled", "disabled");
                btnBuscarArchivo.Attributes.Add("disabled", "disabled");
                btnSubirDatos.Attributes.Add("disabled", "disabled");
                btnColumnas.Attributes.Add("disabled", "disabled");
                btnColumnas.Enabled = RauXls.Enabled = btnSubirDatos.Enabled = btnBuscarArchivo.Enabled = false;
            }
            else
                dt = Session["dtXls"] as DataTable;

            if (!mensaje.Equals(string.Empty))
                AlertError(mensaje);

            RgXls.DataSource = dt == null ? new DataTable() : dt;
        }

        protected void RgColumnas_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (Session["dtColumnas"] != null)
            {
                RgColumnas.DataSource = Session["dtColumnas"];
            }
            else
            {
                RgColumnas.DataSource = new DataTable();
            }
        }

        protected void RgPlantilla_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new DataTable();
            dt = Session["dtPlantilla"] as DataTable;

            RgPlantilla.DataSource = dt == null ? new DataTable() : dt;
        }

        protected void RgPlantilla_ItemCommand(object sender, GridCommandEventArgs e)
        {
            Session["export"] = false;
            if (e.CommandName == RadGrid.ExportToExcelCommandName)
            {
                Session["export"] = true;
            }
        }

        #endregion

        #region Eventos Button

        protected void btnVista_Click(object sender, EventArgs e)
        {
            DataTable dtPlatilla = new DataTable();
            DataTable dtXls = new DataTable();
            bool bandera = false;
            //Limpiar
            RbtIdentity_CheckedChanged(null, null);
            dtPlatilla = Session["dtPlantilla"] as DataTable;
            int IdUsuario = Convert.ToInt32(Session["IdUsuario"]);

            //Carga el archivo xls en el grid para una vista previa (no lo carga al servidor, solo en temp, la lectura es desde memoria)
            //Una vez ejecutado este evento el RauXls limpia automaticamente los archivos subidos
            string mensaje = string.Empty;

            if (RauXls.UploadedFiles.Count > 0)
            {
                //Nombre del archivo
                string fileName = RauXls.UploadedFiles[0].FileName.ToString();

                //Lee el archivo en memoria y lo guarda en un datatable
                try
                {
                    using (Stream st = RauXls.UploadedFiles[0].InputStream)
                    {
                        using (IExcelDataReader excelReader = RauXls.UploadedFiles[0].GetExtension().Equals(".xls") ? ExcelReaderFactory.CreateBinaryReader(st) : ExcelReaderFactory.CreateOpenXmlReader(st))
                        {
                            excelReader.IsFirstRowAsColumnNames = true;
                            DataSet result = excelReader.AsDataSet(true);
                            dtXls = result.Tables[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    AlertError("Error al leer el archivo: " + fileName + ".</br>" + excepcion.SerializarExMessage(ex));
                    return;
                }

                foreach (DataColumn colReplace in dtXls.Columns)
                {
                    colReplace.ColumnName = RemplazarCaracter(colReplace.ColumnName);
                }

                if (dtXls.Rows.Count > 0)
                {
                    if (dtXls.Columns.Count <= dtPlatilla.Columns.Count)
                    {
                        try
                        {
                            foreach (DataRow row in dtXls.Rows)
                            {
                                DataRow newRow = dtPlatilla.NewRow();
                                bandera = true;

                                foreach (DataColumn col in dtXls.Columns)
                                {
                                    newRow[col.ColumnName.Trim()] = row[col.ColumnName.Trim()];

                                    if (dtPlatilla.Columns[col.ColumnName].MaxLength < 0 || dtPlatilla.Columns[col.ColumnName].MaxLength >= row[col.ColumnName].ToString().Length)
                                    {
                                        if (dtPlatilla.Columns[col.ColumnName].AllowDBNull ? true : row[col.ColumnName].ToString().Trim().Length > 0)
                                        {
                                            newRow[col.ColumnName] = row[col.ColumnName];
                                        }
                                        else
                                        {
                                            AlertError("La columna " + col.ColumnName + " no acepta campos vacíos.");
                                            dtPlatilla.Clear();
                                            bandera = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        AlertError("La columna " + col.ColumnName + " excede el tamaño permitido en la base de datos.");
                                        dtPlatilla.Clear();
                                        bandera = false;
                                        break;
                                    }
                                }
                                if (!bandera)
                                {
                                    break;
                                }
                                dtPlatilla.Rows.Add(newRow);
                            }
                            Session["dtXls"] = dtPlatilla;
                            RgXls.DataSource = dtPlatilla;
                            RgXls.Rebind();
                            AlertSuccess("Archivo Válido.");
                            btnSubirDatos.Attributes.Remove("disabled");
                            btnSubirDatos.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            string msg = "El archivo no cuenta con encabezado.";
                            AlertError("Favor de agregar el encabezado al archivo de carga.");
                            excepcion.RegistrarExcepcion(IdUsuario, "btnVista_Click", ex, ref msg);
                            dtPlatilla.Clear();
                        }
                    }
                    else
                    {
                        AlertError("El archivo seleccionado no conincide con la plantilla.");
                    }
                }
                else
                {
                    AlertError("El archivo " + fileName + " esta vacío.");
                }
            }
        }

        protected void ConstruirGrid_Click(object sender, EventArgs e)
        {
            DataTable dtColumnas = new DataTable();
            DataTable dtXlsNuevo = new DataTable();
            DataTable dtPlantilla = new DataTable();
            dtPlantilla = Session["dtPlantilla"] as DataTable;
            Session["dtColumnas"] = new DataTable();
            DataTable dtConexion = new DataTable();
            string mensaje = string.Empty;
            bool error = false;

            dtConexion = carga.Conectar(ref mensaje);

            if (mensaje.Length > 0)
            {
                error = true;
            }
            else
                if (dtConexion != null)
            {
                Session["nombre"] = dtConexion.Rows[0]["name"].ToString();
                Session["formato"] = dtConexion.Rows[0]["date_format"].ToString();
                RlIdioma.Text = Session["nombre"].ToString();
                CambiarIdioma(Session["nombre"].ToString());
                mensaje = "La conexión a la base de datos es válida";
                RadIdioma.Visible = true;
                RlIdioma.Visible = true;

                //btnLayout.Attributes.Remove("disabled");
                btnBuscarArchivo.Attributes.Remove("disabled");
                btnColumnas.Attributes.Remove("disabled");
                btnColumnas.Enabled = RauXls.Enabled = btnBuscarArchivo.Enabled = true;
            }
            else
            {
                mensaje = "La conexión a la base de datos no es válida, verifique la información";
                error = true;
            }
            //consulta la tabla Columnas y construye grid
            if (!error)
            {
                dtColumnas = carga.ConsultarTabla(ref mensaje);
                if (dtColumnas != null)
                {
                    Session["dtColumnas"] = dtColumnas;
                    ConstruirPlantilla(dtColumnas);
                    RgColumnas.Rebind();
                }
                else
                {
                    mensaje = "La Tabla especificada no existe.";
                    error = true;
                }
            }

            if (error)
                AlertError(mensaje);
            else
                AlertSuccess(mensaje);
        }

        protected void btnSubirDatos_Click(object sender, EventArgs e)
        {
            string tabla = "NumerosParteRaw";
            string mensaje = string.Empty;
            string query = string.Empty;
            int count = 0;
            bool esNum = false;
            DataTable dtXlsCarga = new DataTable();
            dtXlsCarga = Session["dtPlantilla"] as DataTable;
            int IdUsuarioSession = Convert.ToInt32(Session["IdUsuario"]);

            if (dtXlsCarga != null)
            {
                if (dtXlsCarga.Rows.Count < 10001)
                {
                    bool identity = Session["Identity"].ToString().Equals("True") ? RbtIdentity.Checked == false : true;
                    query = "BEGIN TRY BEGIN TRAN SET dateformat " + Session["formato"];

                    query += identity ? " SET IDENTITY_INSERT " + tabla + " ON " : "";

                    foreach (DataRow row in dtXlsCarga.Rows)
                    {
                        count = 0;
                        query += " INSERT INTO " + tabla + "(";

                        foreach (DataColumn col in dtXlsCarga.Columns)
                        {
                            query += (count > 0 ? "," : "") + col.ColumnName;
                            count++;
                        }
                        count = 0;
                        query += ") VALUES(";

                        foreach (DataColumn col in dtXlsCarga.Columns)
                        {
                            esNum = EsNumero(col.DataType.ToString());
                            query += (count > 0 ? "," : "") + (esNum ? "" : "'") + row[col].ToString() + (esNum ? "" : "'");
                            count++;
                        }
                        query += ")";
                    }

                    query += (identity ? " SET IDENTITY_INSERT " + tabla + " OFF" : "") + " COMMIT TRAN END TRY BEGIN CATCH IF @@TRANCOUNT > 0 ROLLBACK TRAN DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT SET @ErrMsg = ERROR_MESSAGE() SET @ErrSev = ERROR_SEVERITY() RAISERROR(@ErrMsg, @ErrSev, 1) END CATCH";

                    carga.SubirDatos(query, ref mensaje);

                    if (mensaje.Length > 0)
                    {
                        try { throw new Exception(mensaje); }
                        catch (Exception ex) { excepcion.RegistrarExcepcion(IdUsuarioSession, "btnSubirDatos_Click", ex, ref mensaje); }

                        AlertError("Error al guardar los registros. Verifique que el archivo no contenga código duplicados o algún registro con código vacio.");
                    }
                    else
                    {
                        carga.NumerosParteIntercambio(IdUsuarioSession, ref mensaje);
                        AlertSuccess("Datos cargados correctamente");
                    }

                }
                else
                {
                    AlertSuccess("Se ha excedido la cantidad máxima(10,000) de registros por evento, verifique.");
                }

                Session["dtXls"] = new DataTable();
                RbtIdentity_CheckedChanged(null, null);
            }
            else
            {
                AlertError("Seleccione un archivo válido");
            }
        }

        protected void btnLayout_Click(object sender, EventArgs e)
        {
            DataTable dtPlantilla = new DataTable();
            dtPlantilla = Session["dtPlantilla"] as DataTable;

            if (dtPlantilla != null)
            {
                RgPlantilla.Rebind();
                RgPlantilla.ExportSettings.ExportOnlyData = true;
                RgPlantilla.ExportSettings.OpenInNewWindow = true;
                RgPlantilla.ExportSettings.FileName = tituloPanel;
                //RgPlantilla.ExportSettings.Excel.FileExtension = "xlsx";
                RgPlantilla.MasterTableView.ExportToExcel();
            }
            else
            {
                AlertError("Valide datos de conexión.");
            }
        }

        protected void btnColumnas_Click(object sender, EventArgs e)
        {
            MostrarModal("btnColumnasH");
        }

        #endregion

        #region Métodos

        private void LimpiarCampos()
        {
            ViewState[_xlsValido] = false;
            //ViewState[_carga] = TxtServidor.Text = TxtBaseDatos.Text = TxtUsuario.Text = TxtContrasenia.Text = TxtTabla.Text = string.Empty;

            RgXls.Dispose();
            RgXls.Rebind();

            //EnableControl(TxtServidor, true);
            //EnableControl(TxtBaseDatos, true);
            //EnableControl(TxtUsuario, true);
            //EnableControl(TxtContrasenia, true);
            //EnableControl(TxtTabla, true);
        }

        private static Type ConvertirTipoDato(string TipoDato)
        {
            switch (TipoDato)
            {
                case "bigint":
                    return typeof(Int64);
                case "binary":
                    return typeof(Byte[]);
                case "bit":
                    return typeof(Boolean);
                case "char":
                    return typeof(String);
                case "date":
                    return typeof(DateTime);
                case "datetime":
                    return typeof(DateTime);
                case "datetimeoffset":
                    return typeof(DateTimeOffset);
                case "Decimal":
                    return typeof(Decimal);
                case "varbinary":
                    return typeof(Byte[]);
                case "float":
                    return typeof(Double);
                case "image":
                    return typeof(Byte[]);
                case "int":
                    return typeof(Int32);
                case "money":
                    return typeof(Decimal);
                case "nchar":
                    return typeof(String);
                case "ntext":
                    return typeof(String);
                case "numeric":
                    return typeof(Decimal);
                case "nvarchar":
                    return typeof(String);
                case "real":
                    return typeof(Single);
                case "rowversion":
                    return typeof(Byte[]);
                case "smalldatetime":
                    return typeof(DateTime);
                case "smallint":
                    return typeof(Int16);
                case "smallmoney":
                    return typeof(Decimal);
                case "text":
                    return typeof(String);
                case "time":
                    return typeof(TimeSpan);
                case "timestamp":
                    return typeof(Byte[]);
                case "tinyint":
                    return typeof(Byte);
                case "uniqueidentifier":
                    return typeof(Guid);
                case "varchar":
                    return typeof(String);
                case "xml":
                    return typeof(Xml);

                default: return typeof(string);
            }
        }

        private bool CampoNull(string nulo)
        {
            if (nulo.Equals("YES"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string RemplazarCaracter(string cadena)
        {
            //string caracter = string.Empty;
            Regex caracteres = new Regex(@"[ÑñÁáÉéÍíÓóÚúÜü]");

            while (caracteres.Match(cadena).Success)
            {
                if (cadena.Contains("Ñ"))
                    cadena = cadena.Replace("Ñ", "N");
                else if (cadena.Contains("ñ"))
                    cadena = cadena.Replace("ñ", "n");
                else if (cadena.Contains("Á"))
                    cadena = cadena.Replace("Á", "A");
                else if (cadena.Contains("á"))
                    cadena = cadena.Replace("á", "a");
                else if (cadena.Contains("É"))
                    cadena = cadena.Replace("É", "E");
                else if (cadena.Contains("é"))
                    cadena = cadena.Replace("é", "e");
                else if (cadena.Contains("Í"))
                    cadena = cadena.Replace("Í", "I");
                else if (cadena.Contains("í"))
                    cadena = cadena.Replace("í", "i");
                else if (cadena.Contains("Ó"))
                    cadena = cadena.Replace("Ó", "O");
                else if (cadena.Contains("ó"))
                    cadena = cadena.Replace("ó", "o");
                else if (cadena.Contains("Ú"))
                    cadena = cadena.Replace("Ú", "U");
                else if (cadena.Contains("ú"))
                    cadena = cadena.Replace("ú", "u");
                else if (cadena.Contains("Ü"))
                    cadena = cadena.Replace("Ü", "U");
                else if (cadena.Contains("ü"))
                    cadena = cadena.Replace("ü", "u");
            }
            return cadena;
        }

        private bool EsNumero(string numero)
        {
            switch (numero)
            {
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Float":
                case "System.Decimal":
                case "System.Double":
                    return true;
                    break;
                default:
                    return false;
                    break;
            }
        }

        private void CambiarIdioma(string idioma)
        {
            SqlCultureMapping obj = new SqlCultureMapping().getSqlMappings().Where(c => c.FullName == idioma).FirstOrDefault();
            Page.Culture = obj.specificulture;
        }

        private void ConstruirPlantilla(DataTable dtColumnas)
        {
            DataTable dtPlantilla = new DataTable();
            string mensaje = string.Empty;
            Session["dtPlantilla"] = new DataTable();
            Session["dtPlantillaOriginal"] = new DataTable();
            //Construye dtXlsNuevo
            if (dtColumnas != null)
            {
                Session["Identity"] = false;
                foreach (DataRow row in dtColumnas.Rows)
                {
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = row["COLUMN_NAME"].ToString();
                    dc.DataType = ConvertirTipoDato(row["DATA_TYPE"].ToString());
                    if (dc.DataType == typeof(string))
                    {
                        dc.MaxLength = Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
                    }
                    dc.AllowDBNull = CampoNull(row["IS_NULLABLE"].ToString());
                    dc.AutoIncrement = row["IS_IDENTITY"].ToString().Equals("1");
                    if (!Convert.ToBoolean(Session["Identity"].ToString()))
                    {
                        Session["Identity"] = row["IS_IDENTITY"].ToString().Equals("1");
                    }
                    Session["Identity"] = true;
                    bool addColumn = (row["IS_IDENTITY"].ToString().Equals("1")) ? RbtIdentity.Checked == false : true;
                    if (row["IS_IDENTITY"].ToString().Equals("1") || row["IS_IDENTITY"].ToString().Equals("0"))
                    {
                        RbtIdentity.Visible = true;
                    }
                    if (addColumn)
                        dtPlantilla.Columns.Add(dc);
                }
                Session["dtPlantilla"] = dtPlantilla;
                Session["dtPlantillaOriginal"] = dtPlantilla;
            }
        }

        protected void RbtIdentity_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dtColumnas = new DataTable();
            dtColumnas = Session["dtColumnas"] as DataTable;

            ConstruirPlantilla(dtColumnas);
            RgXls.DataSource = "";
            RgXls.DataBind();
            btnSubirDatos.Attributes.Add("disabled", "disabled");
            btnSubirDatos.Enabled = false;
        }

        #endregion
    }
}