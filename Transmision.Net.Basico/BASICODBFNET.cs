using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace BASICO.DBF.NET
{
    public class BASICO_TIPO
    {
        private BASICO_TIPO(string value) { Value = value; }

        public string Value { get; set; }

        public static BASICO_TIPO Caracter { get { return new BASICO_TIPO("Character"); } }
        public static BASICO_TIPO Fecha { get { return new BASICO_TIPO("Date"); } }
        public static BASICO_TIPO Numerico { get { return new BASICO_TIPO("Numeric"); } }
    }
    public class BASICO_DBFNET
    {

        private string _campos_dbf = "";
        private string _param_insert = "";
        private string _query_crear_campos = "";
        public string tabla { set; get; }

        private string _nombre { set; get; }

        private string _campo { set; get; }

        public string _zize { set; get; }

        public BASICO_DBFNET()
        {
        }
        public void addcol(string nombre, BASICO_TIPO campo, string zize = "")
        {
            _nombre = nombre;
            _campo = campo.Value;
            _zize = zize;

            if (_campos_dbf.Length == 0)
            {

                _campos_dbf = _nombre;
                _param_insert = "?";
            }
            else
            {
                _campos_dbf += "," + _nombre;
                _param_insert += "," + "?";
            }


            if (_query_crear_campos.Length == 0)
            {
                if (zize.Length > 0)
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
                    _query_crear_campos += " , " + nombre + " " + campo.Value + "(" + zize + ") ";
                }
                else
                {
                    _query_crear_campos += " , " + nombre + " " + campo.Value; //+ "(" + zize + ") ";
                }
            }

        }
        public string _path_envia = @"D:\Cons\Vales\in\temp";
        private string _conexion_envia
        {
            //get { return "Provider=VFPOLEDB.1;Data Source=" + _path_envia + ";Exclusive=No"; }
            get { return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path_envia + ";Extended Properties=dBASE IV;"; }
        }
        public void Insertar_tabla(DataTable dt)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string sqlquery = "INSERT INTO " + tabla + "(" + _campos_dbf + ")";
            sqlquery += "VALUES(" + _param_insert + ")";
            try
            {
                cn = new OleDbConnection(_conexion_envia);
                if (cn.State == 0) cn.Open();
                for (Int32 fila = 0; fila < dt.Rows.Count; ++fila)
                {
                    cmd = new OleDbCommand(sqlquery, cn);
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;
                    for (Int32 col = 0; col < dt.Columns.Count; ++col)
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
        public DataTable selectrow(string serie, string numero)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            string _query_select = "SELECT SERIE,NUMERO,DNI,NOMBRES,APEPAT,APEMAT,TELEFONO,EMAIL FROM " + tabla + " WHERE SERIE='" + serie + "' AND numero='" + numero + "'";
            try
            {
                cn = new OleDbConnection(_conexion_envia);
                cmd = new OleDbCommand(_query_select, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }
        public Boolean delete(string serie, string numero)
        {
            Boolean _valida = false;
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string _query_delete = "DELETE FROM " + tabla + " WHERE SERIE='" + serie + "' AND numero='" + numero + "'";
            try
            {
                cn = new OleDbConnection(_conexion_envia);
                if (cn.State == 0) cn.Open();
                cmd = new OleDbCommand(_query_delete, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                _valida = true;
            }
            catch
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                _valida = false;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
            return _valida;
        }
        public void creardbf()
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            string _querycrear = "CREATE TABLE " + tabla + "(" + _query_crear_campos + ")";
            try
            {
                cn = new OleDbConnection(_conexion_envia);
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
    }
}
