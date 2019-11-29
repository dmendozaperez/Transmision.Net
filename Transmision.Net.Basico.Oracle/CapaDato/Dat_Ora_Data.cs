using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmision.Net.Basico.Oracle.BataPos;
//using Transmision.Net.Basico.Oracle.BataTransaction;
using Transmision.Net.Basico.Oracle.CapaEntidad;

namespace Transmision.Net.Basico.Oracle.CapaDato
{
    public class Dat_Ora_Data
    {
        #region<METODO ESTATICOS PARA EL PROCESOS DE XSTORE>
        /// <summary>
        /// metodo para extraer los datos de la venta del xstore 
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns>data de venta</returns>
        public DataTable get_documento_TRN_TRANS(DateTime fecha, ref string error)
        {
            DataTable dtdoc = null;
            string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "' and fiscal_number is not null";
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());               

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                dtdoc = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>get_documento_TRN_TRANS" ;
                 //dtdoc = null;
                throw;
            }
            return dtdoc;
        }
        /// <summary>
        /// si existe la tabla que se va a crear en el xstore DTV
        /// </summary>
        /// <returns> TIPO FALSE O TRUE</returns>
        public Boolean existe_tabla(ref string error)
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
            catch (Exception exc)
            {
                error = exc.Message + " ==> existe_tabla" ;
                //throw;                               
            }
            return existe;
        }
        /// <summary>
        /// CREAR TABLA TEMPORAL PARA EL ORACLE
        /// </summary>
        /// <returns></returns>
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
            catch (Exception exc)
            {
                error = exc.Message + " ==>crear_table";
                //throw;
            }
            return error;
        }
        /// <summary>
        /// INSERTAR LA TABLA TEMPORAL DEL TRN_TRANS VENTA
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Boolean inserta_tabla_temp(Ent_Bat_Tk_Return param,ref string error)
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
                error = exc.Message + " ==>inserta_tabla_temp"; 
                
            }
            return insert;
        }
        /// <summary>
        /// SI EXISTE EN LA TABLA TEMPORAL LOS PARAMETROS DE VENTA
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Boolean existe_tabla_temp(Ent_Bat_Tk_Return param,ref string error)
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
            catch (Exception exc)
            {
                //throw;
                error = exc.Message + " ==>existe_tabla_temp";
            }
            return existe;
        }
        /// <summary>
        /// REALIZAR UN FLAG EN TRN_TRANS PARA YA NO ENVIAR AL TEMPORAL
        /// </summary>
        /// <param name="param"></param>
        public void update_documento_TRN_TRANS(Ent_Bat_Tk_Return param,ref string error)
        {            
            //string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "'";
            string sqlquery = "update trn_trans set RECORD_STATE='X' where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ + " AND fiscal_number='" + param.FISCAL_NUMBER + "'";
            try
            {
                //object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                db.ExecuteNonQuery(dbCommandWrapper);
                
            }
            catch (Exception exc)
            {
                error = exc.Message;
                //throw;
            }            
        }
        /// <summary>
        /// SELECCCIONAR EL TEMPORAL ORACLE
        /// </summary>
        /// <returns></returns>
        public DataTable select_tmp_ora()
        {
            DataTable dtdoc = null;
            string sqlquery = "SELECT RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER FROM " + Ent_Acceso_BD.nom_tabla + " WHERE SEND_SERVER IS NULL AND SUBSTR(FISCAL_NUMBER,0,1) IN ('B','F')";
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                dtdoc = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
            }
            catch 
            {
                dtdoc = null;                
            }
            return dtdoc;
        }
        public void update_tmp_ora(string BARRA, Int32 RTL_LOC_ID, Int32 WKSTN_ID, Int32 TRANS_SEQ,string FISCAL_NUMBER,ref string error)
        {
            //string sqlquery = "SELECT RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER FROM " + Ent_Acceso_BD.nom_tabla + " WHERE SEND_SERVER IS NULL AND SUBSTR(FISCAL_NUMBER,0,1) IN ('B','F')";
            string sqlquery = "UPDATE " + Ent_Acceso_BD.nom_tabla + " SET BARRA='" + BARRA + "',SEND_SERVER='X' WHERE SEND_SERVER IS NULL AND "  +
                " RTL_LOC_ID="+ RTL_LOC_ID + " AND WKSTN_ID=" + WKSTN_ID + " AND TRANS_SEQ=" + TRANS_SEQ + " AND FISCAL_NUMBER='" + FISCAL_NUMBER + "'";
            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                db.ExecuteNonQuery(dbCommandWrapper);
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>update_tmp_ora";
                
            }
        }

        /// <summary>
        /// ENVIO DE WEB SERVICE PARA VERIFICAR SI EXISTE CUPON PARA LA VENTA
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Ent_Tk_Return ws_envio_param(Ent_Tk_Set_Parametro param,ref string error)
        {
            Ent_Tk_Return re = null;
            try
            {
                re = new Ent_Tk_Return();
                ValidateAcceso header_user = new ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";
                Bata_TransactionSoapClient batatran = new Bata_TransactionSoapClient();
                re = batatran.ws_genera_cupon_return(header_user, param);

            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>ws_envio_param";
                re.estado_error = exc.Message;
            }
            return re;
        }

        public Ent_Conexion_Ora_Xstore ws_conexion_xstore(ref string error)
        {
            Ent_Conexion_Ora_Xstore con = null;
            try
            {
                con = new Ent_Conexion_Ora_Xstore();
                ValidateAcceso header_user = new ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";
                Bata_TransactionSoapClient batatran = new Bata_TransactionSoapClient();
                con = batatran.ws_get_conexion_xstore(header_user);

            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>ws_conexion_xstore"  ;
                //throw;
            }
            return con;
        }
        public List<Ent_Tk_Return> ws_get_reimprimir_tk_return(string cod_tda)
        {
            List<Ent_Tk_Return> list = null;
            try
            {
                list = new List<Ent_Tk_Return>();
                ValidateAcceso header_user = new ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";
                Bata_TransactionSoapClient batatran = new Bata_TransactionSoapClient();
                list = batatran.ws_get_tk_return_reimprimir(header_user, cod_tda).ToList();

            }
            catch
            {

            }
            return list;
        }
        public void ws_update_tk_return_reimprimir(string cod_tda, string barra)
        {           
            try
            {
         
                ValidateAcceso header_user = new ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";
                Bata_TransactionSoapClient batatran = new Bata_TransactionSoapClient();
                batatran.ws_update_tk_return_reimprimir(header_user, cod_tda,barra);
            }
            catch
            {

            }
         
        }

        #endregion

    }
}
