using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using TREMEC_EtiquetaInventario_WS.Negocios;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class Inicio
    {
        Singleton singleton = Singleton.Global;
        ControlExcepciones excepcion = new ControlExcepciones();

        public DataTable DatosSesion(string usuario, string contrasena, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spIniciarSesion";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pUsuario", SqlDbType.VarChar).Value = usuario;
                        cmd.Parameters.Add("@pPassword", SqlDbType.VarChar).Value = contrasena;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                excepcion.RegistrarExcepcion(0, sp, ex, ref mensaje);
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
            return dt;
        }

        public DataTable DatosPermisos(int idUsuario, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPermisoConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.VarChar).Value = idUsuario;
                        cmd.Parameters.Add("@pTipoModulo", SqlDbType.VarChar).Value = 'W';
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                excepcion.RegistrarExcepcion(0, sp, ex, ref mensaje);
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
            return dt;
        }

        public DataTable DatosPermisos(int idUsuario, string tipoModulo, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPermisoConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.VarChar).Value = idUsuario;
                        cmd.Parameters.Add("@pTipoModulo", SqlDbType.VarChar).Value = tipoModulo;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                excepcion.RegistrarExcepcion(0, sp, ex, ref mensaje);
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
            return dt;
        }
    }
}