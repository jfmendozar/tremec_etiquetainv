using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class ControlExcepciones
    {
        public const string Existe = "EXISTE";
        public const string IpExiste = "IPEXISTE";
        public const string Ok = "OK";
        public const string ExisteSKU = "EXISTE SKU";
        public const string Error = "ERROR";
        public const string RESTRICCIONFK = "RESTRICCIONFK";

        public void RegistrarExcepcion(int idUsuario, string funcion, Exception exepcion, ref string mensajeCritico)
        {
            IDataParameter[] para = { new SqlParameter("@pIdUsuarioActualiza", idUsuario),
                                        new SqlParameter("@pFuncionalidad", funcion),
                                        new SqlParameter("@pIncidente", exepcion.Message.Trim()),
                                        new SqlParameter("@pDetalleIncidente",  exepcion.StackTrace.Trim()),
                                        new SqlParameter("@pEsError", 1),
                                    };

            Singleton.Global.ExecSp("spLogMovimientoAgregar", ref mensajeCritico, para);
        }

        public string SerializarExMessage(Exception ex)
        {
            string mensaje = string.Empty;
            if (ex.Message.Contains("error: 26"))
                mensaje = "No se puede establecer la conexión al servidor SQL o la instancia es incorrecta.\\nContacte a un administrador. ";
            else if (ex.Message.Contains("error: 0"))
                mensaje = "Se agotó el tiempo de espera de respuesta del servidor. Intente nuevamente.";
            else if (ex.Message.Contains("timeout"))
                mensaje = "El tiempo de espera transcurrió antes de finalizar la operación o el servidor no responde. Intente nuevamente.";
            else if (ex.Message.Contains("login failed"))
                mensaje = "No se puede establecer la conexión a la base de datos o no tiene permisos para acceder a ella.\\nContacte a un administrador. ";

            if (mensaje.Equals(string.Empty))
                mensaje = new JavaScriptSerializer().Serialize(ex.Message.ToString());

            return mensaje;
        }

        public string SerializarExStackTrace(Exception ex)
        {
            return new JavaScriptSerializer().Serialize(ex.StackTrace.ToString());
        }

        public void RegresarMensaje(DataTable dt, ref string mensaje)
        {
            if (dt != null)
                if (mensaje.Equals(string.Empty))
                    mensaje = dt.Rows[0][0].ToString();
        }
    }
}