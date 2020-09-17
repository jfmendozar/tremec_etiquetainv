using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class ConexionCarga
    {
        Singleton singleton = Singleton.Global;
        ControlExcepciones excepcion = new ControlExcepciones();

        public DataTable Conectar(ref string mensaje)
        {
            string query = "spValidarLenguaje";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conexion = new SqlConnection(singleton.CadConexion))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                            else
                                dt = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
            return dt;

        }

        public DataTable ConsultarTabla(ref string mensaje)
        {
            string tabla = "NumerosParteRaw";
            DataTable dt = new DataTable();
            string query = "spConsultaInformacionTabla";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tabla", tabla);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                            else
                                dt = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
            return dt;
        }

        public void SubirDatos(string query, ref string mensaje)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = (excepcion.SerializarExMessage(ex));
            }
        }

        public void NumerosParteIntercambio(int pIdUsuario, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string query = "spNumerosParteIntercambio";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pIdUsuario", SqlDbType.Int).Value = pIdUsuario;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = (excepcion.SerializarExMessage(ex));
            }
        }
    }
}