using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using TREMEC_EtiquetaInventario_WS.Negocios;
using TREMEC_EtiquetaInventario_WS.Entities;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class NumerosParte
    {
        Singleton singleton = Singleton.Global;
        ControlExcepciones excepcion = new ControlExcepciones();

        public DataTable NumerosParteConsultar(ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spNumerosParteConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

        public void NumerosParteEditar(string NumParte, int Lote, string NumSerie, int CantidadTotal, string FechaFabricacion, int UsuarioActualiza, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spNumerosParteEditar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@pNumParte", SqlDbType.VarChar).Value = NumParte;
                        cmd.Parameters.Add("@pLote", SqlDbType.Int).Value = Lote;
                        cmd.Parameters.Add("@pNumSerie", SqlDbType.VarChar).Value = NumSerie;
                        cmd.Parameters.Add("@pCantidadTotal", SqlDbType.Int).Value = CantidadTotal;
                        cmd.Parameters.Add("@pFechaFabricacion", SqlDbType.VarChar).Value = FechaFabricacion;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = UsuarioActualiza;
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
            excepcion.RegresarMensaje(dt, ref mensaje);
        }

        public void NumerosParteAgregar(string NumParte, int Lote, string NumSerie, int CantidadTotal, string FechaFabricacion, int UsuarioActualiza, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spNumerosParteAgregar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pNumParte", SqlDbType.VarChar).Value = NumParte;
                        cmd.Parameters.Add("@pLote", SqlDbType.Int).Value = Lote;
                        cmd.Parameters.Add("@pNumSerie", SqlDbType.VarChar).Value = NumSerie;
                        cmd.Parameters.Add("@pCantidadTotal", SqlDbType.Int).Value = CantidadTotal;
                        cmd.Parameters.Add("@pFechaFabricacion", SqlDbType.Int).Value = FechaFabricacion;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = UsuarioActualiza;

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
            //if (mensaje.Equals(""))
            //    mensaje = "OK";
            excepcion.RegresarMensaje(dt, ref mensaje);
        }
        //Duda
        public DataTable NumerosParteEliminar()
        {
            DataTable dt = new DataTable();
            string sp = "spNumerosParteEliminar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
                return null;
            }
            return dt;
        }
    }
}