using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TREMEC_EtiquetaInventario_WS.Negocios
{
    public class Singleton
    {
        ControlExcepciones excepcion = new ControlExcepciones();
        #region CONSTRUCTOR

        private Singleton() { }

        private Singleton(string cadConexion)
        {
            this.CadConexion = cadConexion;
        }

        #endregion

        private static Singleton _global;
        public static Singleton Global
        {
            get
            {
                if (_global == null)
                {
                    if (ConfigurationManager.ConnectionStrings["BD"] != null)
                        _global = new Singleton(ConfigurationManager.ConnectionStrings["BD"].ConnectionString);
                    else
                        _global = new Singleton();
                }
                return _global;
            }
        }

        private string _cadConexion;
        public string CadConexion
        {
            get { return _cadConexion; }
            private set { _cadConexion = value; }
        }

        #region Llamadas a la BD

        ///<summary>Ejecuta en la base de datos un Stored Procedure y regresa un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar.</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de dato.s</param> 
        public void ExecSp(string sp, ref string mensaje)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
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
                return;
            }
            if (dt != null)
                excepcion.RegresarMensaje(dt, ref mensaje);
        }
        ///<summary>Ejecuta en la base de datos un Stored Procedure con parámetros y regresa un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar.</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de datos.</param> 
        ///<param name="sqlParam">Es el array con los parámetros que requiere el Stored Procedure.</param> 
        public void ExecSp(string sp, ref string mensaje, params IDataParameter[] sqlParam)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (IDataParameter para in sqlParam)
                            cmd.Parameters.Add(para);
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
            if (dt != null)
                excepcion.RegresarMensaje(dt, ref mensaje);
        }
        ///<summary>Ejecuta en la base de datos un Stored Procedure de consulta, regresa el resultado de la consulta en un DataTable y un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar.</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de datos.</param> 
        public DataTable ExecSpDataTable(string sp, ref string mensaje)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
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
        ///<summary>Ejecuta en la base de datos un Stored Procedure de consulta con parámetros, regresa el resultado de la consulta en un DataTable y un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de datos.</param> 
        ///<param name="sqlParam">Es el array con los parámetros que requiere el Stored Procedure.</param> 
        public DataTable ExecSpDataTable(string sp, ref string mensaje, params IDataParameter[] sqlParam)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (IDataParameter para in sqlParam)
                            cmd.Parameters.Add(para);
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
        ///<summary>Ejecuta en la base de datos un Stored Procedure de consulta, regresa el resultado de la consulta en un DataSet y un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar.</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de datos.</param> 
        public DataSet ExecSpDataSet(string sp, ref string mensaje)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(ds);
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
            return ds;
        }
        ///<summary>Ejecuta en la base de datos un Stored Procedure de consulta con parámetros, regresa el resultado de la consulta en un DataSet y un mensaje en una cadena referenciada.</summary>
        ///<param name="sp">Es la cadena con el Stored Procedure a ejecutar.</param> 
        ///<param name="mensaje">Es la cadena referenciada donde se guarda el mensaje que regresa la base de datos.</param> 
        ///<param name="sqlParam">Es el array con los parámetros que requiere el Stored Procedure.</param> 
        public DataSet ExecSpDataSet(string sp, ref string mensaje, params IDataParameter[] sqlParam)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(CadConexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sp, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParam.Count() > 0)
                            foreach (IDataParameter para in sqlParam)
                                cmd.Parameters.Add(para);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(ds);
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
            return ds;
        }

        #endregion
    }
}