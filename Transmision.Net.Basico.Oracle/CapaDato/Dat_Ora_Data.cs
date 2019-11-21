using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmision.Net.Basico.Oracle.CapaEntidad;

namespace Transmision.Net.Basico.Oracle.CapaDato
{
    public class Dat_Ora_Data
    {
        public DataTable get_documento_TRN_TRANS(DateTime fecha)
        {
            DataTable dtdoc = null;
            string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "'";
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());               

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                dtdoc = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
            }
            catch (Exception)
            {
               
                dtdoc = null;
            }
            return dtdoc;
        }
        public Boolean existe_tabla()
        {
            Boolean existe = false;
            string sqlquery = "select count(*) as existe from user_tables where table_name ='"+ Ent_Acceso_BD.nom_tabla + "'";
            DataTable dt_existe = null;
            try
            {
                Database db= new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);                                                                      
                dt_existe = db.ExecuteDataSet(dbCommandWrapper).Tables[0];                    
                if (dt_existe.Rows.Count>0)
                {
                    existe = Convert.ToInt32(dt_existe.Rows[0]["existe"]) == 1 ? true:false;
                }
            }
            catch 
            {
                                                
            }
            return existe;
        }
        public string crear_table()
        {
            string error = "";
            string sqlquery = "create table  " + Ent_Acceso_BD.nom_tabla + "(RTL_LOC_ID NUMBER(10,0),BUSINESS_DATE TIMESTAMP (6),WKSTN_ID NUMBER(19,0),TRANS_SEQ NUMBER(19,0),TOTAL NUMBER(17,6),FISCAL_NUMBER VARCHAR2(50 BYTE),SEND_SERVER VARCHAR2(10 BYTE),BARRA VARCHAR2(50 BYTE))";
            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                db.ExecuteNonQuery(dbCommandWrapper);

               
            }
            catch (Exception)
            {

                throw;
            }
            return error;
        }

        public Boolean inserta_tabla_temp(Ent_Bat_Tk_Return param)
        {
            Boolean insert = false;
            string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla + "(RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER)" +
                " VALUES(" + param.RTL_LOC_ID + ",'" + param.BUSINESS_DATE + "'," + param.WKSTN_ID + "," + param.TRANS_SEQ + "," + param.TOTAL + ",'" + param.FISCAL_NUMBER + "')";

            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);
                db.ExecuteNonQuery(dbCommandWrapper);
                insert = true;
            }
            catch (Exception exc)
            {

                
            }
            return insert;
        }

        public Boolean existe_tabla_temp(Ent_Bat_Tk_Return param)
        {
            Boolean existe = false;
            string sqlquery = "SELECT count(*) as existe FROM " + Ent_Acceso_BD.nom_tabla + " WHERE RTL_LOC_ID=" + param.RTL_LOC_ID + " and WKSTN_ID=" + param.WKSTN_ID + " AND TRANS_SEQ=" + param.TRANS_SEQ + " AND FISCAL_NUMBER='" + param.FISCAL_NUMBER + "'";
            DataTable dt_existe = null;
            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);
                dt_existe = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
                if (dt_existe.Rows.Count > 0)
                {
                    existe = Convert.ToInt32(dt_existe.Rows[0]["existe"]) >= 1 ? true : false;
                }
            }
            catch (Exception)
            {

                
            }
            return existe;
        }

        public void update_documento_TRN_TRANS(Ent_Bat_Tk_Return param)
        {            
            //string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "'";
            string sqlquery = "update trn_trans set RECORD_STATE='X' where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ + " AND fiscal_number='" + param.FISCAL_NUMBER + "'";
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                db.ExecuteNonQuery(dbCommandWrapper);
                
            }
            catch (Exception)
            {
                
            }            
        }

        public DataTable select_tmp_ora()
        {
            DataTable dt = null;
            string sqlquery = "SELECT RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER FROM " + Ent_Acceso_BD.nom_tabla + " WHERE SEND_SERVER IS NULL";
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                dtdoc = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
            }
            catch 
            {
                dt = null;                
            }
            return dt;
        }

    }
}
