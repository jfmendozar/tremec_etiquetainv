using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using TREMEC_EtiquetaInventario_WS.Negocios;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class Perfiles
    {
        Singleton singleton = Singleton.Global;
        ControlExcepciones excepcion = new ControlExcepciones();

        public DataTable PerfilConsultar(ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPerfilConsultar";
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

        public DataSet ModuloConsultar(ref string mensaje)
        {
            //DataTable dt = new DataTable();
            DataSet dsModulosAcciones = null;
            SqlDataAdapter adapter = null;
            string sp = "spModuloConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    using (SqlCommand vCommand = new SqlCommand(sp, con))
                    {
                        dsModulosAcciones = new DataSet();
                        //
                        vCommand.CommandType = CommandType.StoredProcedure;
                        //Set the SqlDataAdapter's SelectCommand.
                        adapter = new SqlDataAdapter();
                        adapter.SelectCommand = vCommand;
                        vCommand.Connection.Open();
                        adapter.Fill(dsModulosAcciones);
                        vCommand.Connection.Close();
                        //Valida que exista el usuario
                        if (dsModulosAcciones.Tables[0].Rows.Count <= 0)
                        {
                            mensaje = "No existen modulos definidos, verifique.";
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
            return dsModulosAcciones;
        }

        public DataTable PerfilModuloAccionConsultar(int idPerfil, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPerfilModuloAccionConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
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

        public DataTable ModuloAccionConsultar(ref string mensaje)
        {
            SqlDataAdapter adapter = null;
            DataTable dtModulosAcciones = null;
            string sp = "spModuloAccionConsultar";

            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    using (SqlCommand vCommand = new SqlCommand(sp, con))
                    {
                        dtModulosAcciones = new DataTable();
                        //
                        vCommand.CommandType = CommandType.StoredProcedure;
                        //Set the SqlDataAdapter's SelectCommand.
                        adapter = new SqlDataAdapter();
                        adapter.SelectCommand = vCommand;
                        vCommand.Connection.Open();
                        adapter.Fill(dtModulosAcciones);
                        vCommand.Connection.Close();
                        //Valida que exista el usuario
                        if (dtModulosAcciones.Rows.Count <= 0)
                        {
                            mensaje = "No existen acciones definidas, verifique.";
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
            finally
            {
                //Liberara objetos y memoria
                if (dtModulosAcciones != null)
                    dtModulosAcciones.Dispose();
                if (adapter != null)
                    adapter.Dispose();

            }
            return dtModulosAcciones;
        }

        /// <summary>
        /// Permite agregar/actualizar los PERMISOS que se asignaron a un Perfil.
        /// </summary>
        /// <param name="idUsuarioActualiza">Id del Usuario que inicio sesión.</param>
        /// <param name="idPerfil">Id del Perfil al que se agregaran o quitaran los permisos</param>
        /// <param name="mensaje">Mensaje de texto del resultado de la operación</param>
        public void PermisoAgregar(int idUsuarioActualiza, int idPerfil, ref string mensaje)
        {
            string sp = "spPermisoAgregar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add("@pIdUsuario", SqlDbType.Int).Value = idUsuario == null ? Convert.DBNull : idUsuario;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
                        //cmd.Parameters.Add("@pIdModulo", SqlDbType.Int).Value = idModulo;
                        //cmd.Parameters.Add("@pAgregar", SqlDbType.Int).Value = agregar;
                        //cmd.Parameters.Add("@pConsultar", SqlDbType.Int).Value = consultar;
                        //cmd.Parameters.Add("@pEditar", SqlDbType.Int).Value = editar;
                        //cmd.Parameters.Add("@pEliminar", SqlDbType.Int).Value = eliminar;
                        //cmd.Parameters.Add("@pExportar", SqlDbType.Int).Value = exportar;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                excepcion.RegistrarExcepcion(0, sp, ex, ref mensaje);
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
        }

        public void PerfilModuloAccionAgregar(int IdPerfil, string NombrePerfil, string Clave, string ModuloPermisos, int idUsuarioActualiza, ref string mensaje)
        {
            DataTable dt = new DataTable();

            string sp = "spPerfilModuloAccionAgregar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = IdPerfil;
                        cmd.Parameters.Add("@pNombre", SqlDbType.VarChar).Value = NombrePerfil;
                        cmd.Parameters.Add("@pClave", SqlDbType.VarChar).Value = Clave;
                        cmd.Parameters.Add("@pModuloPermisos", SqlDbType.VarChar).Value = ModuloPermisos;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
                        //cmd.ExecuteNonQuery();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                    }
                }
                excepcion.RegresarMensaje(dt, ref mensaje);
            }
            catch (Exception ex)
            {
                excepcion.RegistrarExcepcion(0, sp, ex, ref mensaje);
                if (mensaje.Length == 0)
                    mensaje = "Error: " + excepcion.SerializarExMessage(ex);
            }
        }

        public void PerfilAgregar(int idUsuarioActualiza, ref string idPerfil, string nombre, string clave, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPerfilAgregar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pNombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@pClave", SqlDbType.VarChar).Value = clave;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Direction = ParameterDirection.Output;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                        idPerfil = cmd.Parameters["@pIdPerfil"].Value.ToString();
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

        public void PerfilEditar(int idUsuarioActualiza, int idPerfil, string nombre, string clave, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPerfilEditar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
                        cmd.Parameters.Add("@pNombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@pClave", SqlDbType.VarChar).Value = clave;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
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

        public void PerfilEliminar(int idUsuarioActualiza, int idPerfil, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spPerfilEliminar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
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
    }
}