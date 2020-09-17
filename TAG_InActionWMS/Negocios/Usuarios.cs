using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using TREMEC_EtiquetaInventario_WS.Negocios;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class Usuarios
    {
        Singleton singleton = Singleton.Global;
        ControlExcepciones excepcion = new ControlExcepciones();

        public DataTable UsuarioConsultar(ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioConsultar";
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

        public DataTable TipoUsuarioConsultar(ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spTipoUsuarioConsultar";
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

        public void UsuarioAgregar(int idUsuarioActualiza, ref string idUsuario, string usuario, string nombre, string contrasena, int idPerfil, int idTipo, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioAgregar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pUsuario", SqlDbType.VarChar).Value = usuario;
                        cmd.Parameters.Add("@pNombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@pContrasena", SqlDbType.VarChar).Value = contrasena;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
                        cmd.Parameters.Add("@pIdTipoUsuario", SqlDbType.Int).Value = idTipo;
                        cmd.Parameters.Add("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.Int).Direction = ParameterDirection.Output;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                        idUsuario = cmd.Parameters["@pIdUsuario"].Value.ToString();
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

        public void UsuarioEditar(int idUsuarioActualiza, int idUsuario, string usuario, string nombre, string contrasena, int idPerfil, int idTipo, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioEditar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.Int).Value = idUsuario;
                        cmd.Parameters.Add("@pUsuario", SqlDbType.VarChar).Value = usuario;
                        cmd.Parameters.Add("@pNombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@pContrasena", SqlDbType.VarChar).Value = contrasena.Equals(string.Empty) ? Convert.DBNull : contrasena;
                        cmd.Parameters.Add("@pIdPerfil", SqlDbType.Int).Value = idPerfil;
                        cmd.Parameters.Add("@pIdTipoUsuario", SqlDbType.Int).Value = idTipo;
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

        public void UsuarioEliminar(int idUsuarioActualiza, int idUsuario, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioEliminar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.Int).Value = idUsuario;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public DataTable UsuarioObtenerPermisos(int idUsuario, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioModuloAccionConsultar";
            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pIdUsuario", SqlDbType.VarChar).Value = idUsuario;
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

        /// <summary>
        /// Permite agregar/actualizar los DATOS y PERMISOS de un usuario
        /// </summary>
        /// <param name="IdUsuario"> Agregar {IdUsuario = 0}, Actualiza {IdUsuario > 0} </param>
        /// <param name="Usuario"> Identificador de Usuario para iniciar sesión </param>
        /// <param name="Nombre"> Nombre(s) y Apellidos del Usuario </param>
        /// <param name="Contrasena"> Contraseña con la que iniciara sesión el usuario </param>
        /// <param name="IdTipo"> Identificador del tipo de usuario que se esta configurando </param>
        /// <param name="IdPerfil"> Identificar del Perfil asociado al Usuario </param>
        /// <param name="ModuloPermisos"> Lista de permisos por modulo en una cadena de caracteres </param>
        /// <param name="idUsuarioActualiza"> Identificador del Usuario que inicio sesión, usuario que realiza el movimiento </param>
        /// <param name="mensaje">  Mensaje de texto del resultado de la operación </param>
        public void UsuarioPermisoAgregar(int IdUsuario, string Usuario, string Nombre, string Contrasena, int IdTipo, int IdPerfil,
            string ModuloPermisos, int idUsuarioActualiza, ref string mensaje)
        {
            DataTable dt = new DataTable();
            string sp = "spUsuarioModuloAccionAgregar";

            try
            {
                using (SqlConnection con = new SqlConnection(singleton.CadConexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        // Obtener la lista de cabeceras de las Transferencias pendientes.
                        //Se obtienen los datos por medio del SP
                        cmd.CommandType = CommandType.StoredProcedure;
                        //Parametros de entrada
                        cmd.Parameters.AddWithValue("@pIdUsuario", SqlDbType.Int).Value = IdUsuario;
                        cmd.Parameters.AddWithValue("@pUsuario", SqlDbType.VarChar).Value = Usuario;
                        cmd.Parameters.AddWithValue("@pNombre", SqlDbType.VarChar).Value = Nombre;
                        cmd.Parameters.AddWithValue("@pContrasena", SqlDbType.VarChar).Value = Contrasena.Equals(string.Empty) ? Convert.DBNull : Contrasena;
                        cmd.Parameters.AddWithValue("@pIdTipoUsuario", SqlDbType.Int).Value = IdTipo;
                        cmd.Parameters.AddWithValue("@pIdPerfil", SqlDbType.Int).Value = IdPerfil;
                        cmd.Parameters.AddWithValue("@pModuloPermisos", SqlDbType.VarChar).Value = ModuloPermisos;
                        cmd.Parameters.AddWithValue("@pIdUsuarioActualiza", SqlDbType.Int).Value = idUsuarioActualiza;
                        //Set the SqlDataAdapter's SelectCommand.
                        cmd.Connection.Open();
                        //cmd.ExecuteNonQuery();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                dt = null;
                            else
                                dt.Load(reader);
                        }
                        cmd.Connection.Close();
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
    }
}