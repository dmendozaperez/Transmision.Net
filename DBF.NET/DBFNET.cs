using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using GlobalSolucion;
using System.Data;

namespace DBF.NET
{
    public class Tipo
    {
        private Tipo(string value) { Value = value; }

        public string Value { get; set; }

        public static Tipo Caracter { get { return new Tipo("Character"); } }
        public static Tipo Fecha { get { return new Tipo("Date"); } }
        public static Tipo Numerico { get { return new Tipo("Numeric"); } }       
    }
    public class DBFNET
    {

        private string _campos_dbf = "";
        private string _param_insert = "";
        private string _query_crear_campos = "";
        public string tabla { set; get; }

        private string _nombre { set; get; }
        
        private string _campo { set; get; }

        public string _zize { set; get; }

        public DBFNET()
        {
        }
        public void addcol(string nombre, Tipo campo, string zize="")
        {
            _nombre = nombre;
            _campo = campo.Value;
            _zize = zize;
             
            if (_campos_dbf.Length==0)
            {

                _campos_dbf = _nombre;
                _param_insert = "?";
            }
            else
            {
                _campos_dbf +="," + _nombre;
                _param_insert +="," +  "?";
            }


            if (_query_crear_campos.Length==0)
            { 
                if (zize.Length>0)
                { 
                 _query_crear_campos += nombre + " " + campo.Value + "(" + zize + ") ";
                }
                else
                {
                    _query_crear_campos += nombre + " " + campo.Value;// + "(" + zize + ") ";
                }
            }
            else
            {
                if (zize.Length > 0)
                { 
                    _query_crear_campos +=" , " +  nombre + " " + campo.Value + "(" + zize + ") ";
                }
                else
                {
                    _query_crear_campos += " , " + nombre + " " + campo.Value; //+ "(" + zize + ") ";
                }
            }
           
        }
        
        public void Insertar_tabla(DataTable dt)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string sqlquery = "INSERT INTO " + tabla + "(" + _campos_dbf + ")";
            sqlquery +="VALUES(" + _param_insert + ")";
            try
            {
                cn = new OleDbConnection(Variables._conexion_envia);                
                if (cn.State == 0) cn.Open();
                for (Int32 fila=0; fila < dt.Rows.Count;++fila)
                {
                    cmd = new OleDbCommand(sqlquery, cn);
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;
                    for (Int32 col=0;col<dt.Columns.Count;++col)
                    {
                        cmd.Parameters.AddWithValue(dt.Columns[col].ColumnName.ToString(), dt.Rows[fila][col]);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                throw;
            }
            if (cn != null)
            if (cn.State == ConnectionState.Open) cn.Close();
        }
        public void creardbf()
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string _querycrear = "CREATE TABLE " + tabla + "(" + _query_crear_campos + ")";
            try
            {
                cn = new OleDbConnection(Variables._conexion_envia);
                if (cn.State == 0) cn.Open();
                cmd = new OleDbCommand(_querycrear, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();

            }
            catch
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                throw;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
        }
        /*sostic 05-2019*/
        public void agregarColumnaDBF(string columna, string dbf)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string _query = "alter table " + dbf + " add " + columna + " numeric(1)";  // "if exists ( select * from INFORMATION_SCHEMA.columns where COLUMN_NAME = 'xstore' AND TABLE_NAME = 'FPALMA02' ) begin select 1 end else begin select 0 end";
                                                                                       //string _querycrear = "update fpalma02 set xstore = 'x' "; // "alter table FPALMA02 add xstore char(1)";
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                if (cn.State == 0) cn.Open();
                cmd = new OleDbCommand(_query, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery(); //Convert.ToInt32( cmd.ExecuteScalar()); 
            }
            catch (Exception ex)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                throw;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
        }
        /*sostic 06/2019*/
        public void updateColumnaDBF(string query)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string _query = query;  // "if exists ( select * from INFORMATION_SCHEMA.columns where COLUMN_NAME = 'xstore' AND TABLE_NAME = 'FPALMA02' ) begin select 1 end else begin select 0 end";
                                    //string _querycrear = "update fpalma02 set xstore = 'x' "; // "alter table FPALMA02 add xstore char(1)";
            try
            {
                cn = new OleDbConnection(Variables._conexion_vfpoledb);
                if (cn.State == 0) cn.Open();
                cmd = new OleDbCommand(_query, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery(); //Convert.ToInt32( cmd.ExecuteScalar()); 
            }
            catch (Exception ex)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                throw;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
        }
        /*sostic 06/2019*/
        public bool verificarColumnaDBF(string columna, string dbf)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            bool _rt = false;
            string _queryVerificar = "select " + columna + " from " + dbf + "";
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                if (cn.State == 0) cn.Open();
                cmd = new OleDbCommand(_queryVerificar, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery(); //Convert.ToInt32( cmd.ExecuteScalar());  
                _rt = true;
            }
            catch (Exception ex)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                _rt = false;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
            return _rt;
        }
        /*sostic 05-2019*/
    }
}
