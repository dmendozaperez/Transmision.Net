using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
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
        public DataTable get_documento_TRN_TRANS(string fecha, ref string error,ref string query)
        {
            DataTable dtdoc = null;
            string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>=to_date('" + fecha + "') and fiscal_number is not null";
            try
            {
                query = sqlquery;
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

        public DataTable get_documento_TRN_INV_TRANS_POSLOG(String tienda,string fechaini,string fechafin, ref string error)
        {
            DataTable dtdoc = null;
            //string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "' and fiscal_number is not null";
            string sqlquery = "select RTL_LOC_ID,WKSTN_ID,TRANS_SEQ,BUSINESS_DATE,INVCTL_DOCUMENT_ID as numdoc,0 AS TOTAL,document_typcode "+
                              "from inv_invctl_trans where organization_id = 2000 and RTL_LOC_ID='" + tienda + "' and document_typcode = 'RECEIVING' and new_status_code = 'CLOSED' " + 
                              "AND(BUSINESS_DATE >=to_date('" + fechaini + "') AND BUSINESS_DATE <=to_date('" + fechafin + "')) AND RECORD_STATE is null " + 
                              "UNION ALL " + 
                              "SELECT a.rtl_loc_id,a.wkstn_id,b.trans_seq,b.BUSINESS_DATE,A.string_value as numdoc,0 AS TOTAL, b.document_typcode FROM trn_trans_p A " +
                              "inner join inv_invctl_trans B ON a.organization_id = b.organization_id AND a.rtl_loc_id = b.rtl_loc_id and b.new_status_code = 'CLOSED' " +
                              "and A.property_code = 'FOLIO_GD' AND a.wkstn_id = b.wkstn_id AND a.trans_seq = b.trans_seq AND B.organization_id = 2000 " +
                              "where A.organization_id = 2000 AND b.document_typcode = 'SHIPPING' AND a.rtl_loc_id='" + tienda + "' " +  
                              "AND(B.BUSINESS_DATE >=to_date('" + fechaini + "') AND B.BUSINESS_DATE <=to_date('" + fechafin + "'))  AND B.RECORD_STATE is null " +
                              "UNION ALL " +
                              "SELECT RTL_LOC_ID, WKSTN_ID, TRANS_SEQ, BUSINESS_DATE, FISCAL_NUMBER AS NUMDOC, TOTAL, TRANS_TYPCODE AS document_typcode " + 
                              "FROM TRN_TRANS WHERE organization_id = 2000 AND FISCAL_NUMBER IS NOT NULL AND RTL_LOC_ID ='" + tienda  +"' AND TRANS_TYPCODE = 'RETAIL_SALE' " +
                              "AND(BUSINESS_DATE >=to_date('" + fechaini + "') AND BUSINESS_DATE <=to_date('" + fechafin + "'))" +
                              "AND trans_statcode = 'COMPLETE' AND fiscal_number IS NOT NULL AND (RECORD_STATE = 'X' OR (RECORD_STATE is null and total<0) OR (RECORD_STATE is null and SUBSTR(FISCAL_NUMBER,0,1) NOT IN('B','F') ))";
                                        
            try
            {
                object results = new object[1];
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                dtdoc = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>get_documento_TRN_TRANS";
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

        public Boolean existe_tabla_poslog(ref string error)
        {
            Boolean existe = false;
            string sqlquery = "select count(*) as existe from user_tables where table_name ='" + Ent_Acceso_BD.nom_tabla_poslog + "'";
            DataTable dt_existe = null;
            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);
                dt_existe = db.ExecuteDataSet(dbCommandWrapper).Tables[0];
                if (dt_existe.Rows.Count > 0)
                {
                    existe = Convert.ToInt32(dt_existe.Rows[0]["existe"]) == 1 ? true : false;
                }

            }
            catch (Exception exc)
            {
                error = exc.Message + " ==> existe_tabla";
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

            decimal _n = Convert.ToDecimal(param.TOTAL, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            //string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla + "(RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER)" +
            //    " VALUES(" + param.RTL_LOC_ID + ",'" + param.BUSINESS_DATE + "'," + param.WKSTN_ID + "," + param.TRANS_SEQ + "," + Convert.ToDecimal(param.TOTAL, new NumberFormatInfo() { NumberDecimalSeparator = "." }) + ",'" + param.FISCAL_NUMBER + "')";
            string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla + "(RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER) VALUES(:RTL_LOC_ID,:BUSINESS_DATE,:WKSTN_ID,:TRANS_SEQ,:TOTAL,:FISCAL_NUMBER)";

            try
            {


                Database db = new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);
                db.AddInParameter(dbCommandWrapper, "RTL_LOC_ID", DbType.Decimal,param.RTL_LOC_ID);
                db.AddInParameter(dbCommandWrapper, "BUSINESS_DATE", DbType.DateTime, param.BUSINESS_DATE);
                db.AddInParameter(dbCommandWrapper, "WKSTN_ID", DbType.Decimal, param.WKSTN_ID);
                db.AddInParameter(dbCommandWrapper, "TRANS_SEQ", DbType.Decimal, param.TRANS_SEQ);
                db.AddInParameter(dbCommandWrapper, "TOTAL", DbType.Decimal, param.TOTAL);
                db.AddInParameter(dbCommandWrapper, "FISCAL_NUMBER", DbType.String, param.FISCAL_NUMBER);


                //dbCommandWrapper.Parameters.Add("RTL_LOC_ID",DbType.)
                //dbCommandWrapper.Parameters.Add(new OracleParameter(":FName", TextBox1.Text));


                db.ExecuteNonQuery(dbCommandWrapper);
                               
                               
                insert = true;
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>inserta_tabla_temp ==QUERY==> " + sqlquery; 
                
            }
            return insert;
        }

        public Boolean inserta_tabla_temp_poslog(Ent_Ora_Transac param, ref string error)
        {
            Boolean insert = false;

            decimal _n = Convert.ToDecimal(param.TOTAL, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            //string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla + "(RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER)" +
            //    " VALUES(" + param.RTL_LOC_ID + ",'" + param.BUSINESS_DATE + "'," + param.WKSTN_ID + "," + param.TRANS_SEQ + "," + Convert.ToDecimal(param.TOTAL, new NumberFormatInfo() { NumberDecimalSeparator = "." }) + ",'" + param.FISCAL_NUMBER + "')";
            //string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla_poslog + "(RTL_LOC_ID,WKSTN_ID,TRANS_SEQ,BUSINESS_DATE,NUMDOC,TOTAL,DOCUMENT_TYPCODE) VALUES(:RTL_LOC_ID,:WKSTN_ID,:TRANS_SEQ,:BUSINESS_DATE,:NUMDOC,:TOTAL,:DOCUMENT_TYPCOE)";
            string sqlquery = "INSERT INTO  " + Ent_Acceso_BD.nom_tabla_poslog + "(RTL_LOC_ID,WKSTN_ID,TRANS_SEQ,BUSINESS_DATE,NUMDOC,TOTAL,DOCUMENT_TYPCODE) VALUES(:RTL_LOC_ID,:WKSTN_ID,:TRANS_SEQ,:BUSINESS_DATE,:NUMDOC,:TOTAL,:DOCUMENT_TYPCODE)";

            try
            {


                Database db = new OracleDatabase(Ent_Acceso_BD.conn());
                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);
                db.AddInParameter(dbCommandWrapper, "RTL_LOC_ID", DbType.Decimal, param.RTL_LOC_ID);
                db.AddInParameter(dbCommandWrapper, "WKSTN_ID", DbType.Decimal, param.WKSTN_ID);
                db.AddInParameter(dbCommandWrapper, "TRANS_SEQ", DbType.Decimal, param.TRANS_SEQ);
                db.AddInParameter(dbCommandWrapper, "BUSINESS_DATE", DbType.DateTime, param.BUSINESS_DATE);
                db.AddInParameter(dbCommandWrapper, "NUMDOC", DbType.String, param.NUMDOC);
                db.AddInParameter(dbCommandWrapper, "TOTAL", DbType.Decimal, param.TOTAL);
                db.AddInParameter(dbCommandWrapper, "DOCUMENT_TYPCODE", DbType.String, param.DOCUMENT_TYPCODE);
                //dbCommandWrapper.Parameters.Add("RTL_LOC_ID",DbType.)
                //dbCommandWrapper.Parameters.Add(new OracleParameter(":FName", TextBox1.Text));

                db.ExecuteNonQuery(dbCommandWrapper);


                insert = true;
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>inserta_tabla_temp ==QUERY==> " + sqlquery;

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

        public Boolean existe_data_temp_transac(Ent_Ora_Transac param, ref string error)
        {
            Boolean existe = false;
            string sqlquery = "SELECT count(*) as existe FROM " + Ent_Acceso_BD.nom_tabla_poslog + " WHERE RTL_LOC_ID=" + param.RTL_LOC_ID + " and WKSTN_ID=" + param.WKSTN_ID + " AND TRANS_SEQ=" + param.TRANS_SEQ + " AND NUMDOC='" + param.NUMDOC + "'";
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

        public void update_documento_Transac_Mov(Ent_Ora_Transac param, ref string error)
        {
            //string sqlquery = "select rtl_loc_id,business_date,wkstn_id,trans_seq,TOTAL,fiscal_number from trn_trans where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL and business_date>='" + fecha + "'";
            //string sqlquery = "update trn_trans set RECORD_STATE='X' where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ + " AND fiscal_number='" + param.FISCAL_NUMBER + "'";
            string sqlquery = "";
            try
            {
                switch(param.DOCUMENT_TYPCODE)
                {
                    /*venta*/
                    case "RETAIL_SALE":
                        /*X ES PARA EL TEMA DE LAS TICKET REGALO Y A ES PARA ENVIO DE POSLOG*/
                        sqlquery = "update trn_trans set RECORD_STATE='A' where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' and (RECORD_STATE='X' or (RECORD_STATE is null and total<0) OR (RECORD_STATE is null and SUBSTR(FISCAL_NUMBER,0,1) NOT IN('B','F') )) AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ + " AND fiscal_number='" + param.NUMDOC + "'";
                        break;
                    /*envio de guia*/
                    /*recibo de guias*/
                    case "SHIPPING":
                    case "RECEIVING":
                        sqlquery = "update inv_invctl_trans set RECORD_STATE='X' where document_typcode='" + param.DOCUMENT_TYPCODE  +"' AND  RECORD_STATE IS NULL AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ ;
                        break;
                    
                    //case "RECEIVING":
                    //    sqlquery = "update trn_trans set RECORD_STATE='X' where trans_typcode='RETAIL_SALE' AND trans_statcode='COMPLETE' AND TOTAL>0 AND RECORD_STATE IS NULL AND rtl_loc_id=" + param.RTL_LOC_ID + " AND wkstn_id=" + param.WKSTN_ID + " AND trans_seq=" + param.TRANS_SEQ + " AND fiscal_number='" + param.FISCAL_NUMBER + "'";
                    //    break;
                }
               

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

        public DataTable select_tmp_poslog_ora()
        {
            DataTable dtdoc = null;
            //string sqlquery = "select RTL_LOC_ID,WKSTN_ID,TRANS_SEQ,BUSINESS_DATE,NUMDOC,TOTAL,DOCUMENT_TYPCODE," + 
            //                  "(SELECT POSLOG_DATA FROM TRN_POSLOG_DATA B WHERE B.ORGANIZATION_ID = 2000 " + 
            //                  "AND B.RTL_LOC_ID = A.RTL_LOC_ID AND B.WKSTN_ID = A.WKSTN_ID AND B.TRANS_SEQ = A.TRANS_SEQ " +
            //                  "AND ROWNUM = 1) AS POSLOG_DATA " +
            //                  "from "  + Ent_Acceso_BD.nom_tabla_poslog + " A WHERE (SEND_SERVER IS NULL OR SEND_SERVER='X') AND ROWNUM<=50";
            string sqlquery = "select A.RTL_LOC_ID,A.WKSTN_ID,A.TRANS_SEQ,A.BUSINESS_DATE,A.NUMDOC,A.TOTAL,A.DOCUMENT_TYPCODE,B.POSLOG_DATA " + 
                              "from " + Ent_Acceso_BD.nom_tabla_poslog  + " A " + 
                              "INNER JOIN TRN_POSLOG_DATA B ON B.ORGANIZATION_ID = 2000 AND B.RTL_LOC_ID = A.RTL_LOC_ID " + 
                              "AND B.WKSTN_ID = A.WKSTN_ID AND B.TRANS_SEQ = A.TRANS_SEQ " + 
                              "WHERE(SEND_SERVER IS NULL OR SEND_SERVER = 'X') AND ROWNUM<= 50";

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


        public void update_tmp_bata_transac_ora(Ent_PosLog_Tda param, ref string error)
        {
            //string sqlquery = "SELECT RTL_LOC_ID,BUSINESS_DATE,WKSTN_ID,TRANS_SEQ,TOTAL,FISCAL_NUMBER FROM " + Ent_Acceso_BD.nom_tabla + " WHERE SEND_SERVER IS NULL AND SUBSTR(FISCAL_NUMBER,0,1) IN ('B','F')";
            string sqlquery = "UPDATE " + Ent_Acceso_BD.nom_tabla_poslog + " SET SEND_SERVER='A' WHERE (SEND_SERVER IS NULL OR SEND_SERVER='X') AND " +
                " RTL_LOC_ID=" + param.rtl_loc_id + " AND WKSTN_ID=" + param.wkstn_id + " AND TRANS_SEQ=" + param.trans_seq + " AND DOCUMENT_TYPCODE='" + param.document_typcode + "' AND NUMDOC='" + param.numdoc + "'";
            try
            {
                Database db = new OracleDatabase(Ent_Acceso_BD.conn());

                DbCommand dbCommandWrapper = db.GetSqlStringCommand(sqlquery);

                db.ExecuteNonQuery(dbCommandWrapper);
            }
            catch (Exception exc)
            {
                error = exc.Message + " ==>update_tmp_bata_transac_ora";

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

        public string ws_envio_poslog_tda(Ent_PosLog_Tda param)
        {
            string error = "";
            try
            {
                
                ValidateAcceso header_user = new ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";
                Bata_TransactionSoapClient batatran = new Bata_TransactionSoapClient();
                error = batatran.ws_envio_poslog_xstore_tda(header_user, param);

            }
            catch (Exception exc)
            {
                error = exc.Message;
               
            }
            return error;
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
        #region<PROCESOS PARA EL POSLOG>
        public string crear_table_poslog()
        {
            string error = "";
            string sqlquery = "create table  " + Ent_Acceso_BD.nom_tabla_poslog + "(RTL_LOC_ID NUMBER(10,0),WKSTN_ID NUMBER(19,0),TRANS_SEQ NUMBER(19,0),BUSINESS_DATE TIMESTAMP (6),NUMDOC VARCHAR2(50 BYTE),TOTAL NUMBER(17,6),DOCUMENT_TYPCODE VARCHAR2(50 BYTE),SEND_SERVER VARCHAR2(10 BYTE))";
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

        #endregion

    }
}
