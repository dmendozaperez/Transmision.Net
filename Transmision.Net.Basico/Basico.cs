using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using GlobalSolucion;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Timers;
using System.ServiceModel.Configuration;
using System.ServiceProcess;

namespace Transmision.Net.Basico
{
    public class Basico
    {


        #region<REGIONES DE VARIABLES>
        private static string _tienda { set; get; }
       
        private static DateTime _fecha_tda { set; get; }

       
        #endregion
        #region<REGIONES DE PROPIEDADES ESTATICOS>
        private static void _crearfolder_limpiar()
        {
            string _folder_tmp = Variables._path_envia;
            try
            {
                if  (!Directory.Exists(@_folder_tmp))
                {
                    Directory.CreateDirectory(@_folder_tmp);
                }
                string[] _file = Directory.GetFiles(@_folder_tmp, "*.*");

                foreach (string f in _file)
                {
                    // Remove path from the file name.                  
                    File.Delete(f.ToString());
                }
            }
            catch
            {
                throw;
            }
        }

        private static Boolean _valida_archivo_proceso(DateTime _fecha_proceso)
        {
            Boolean _valida = false;
            string _archivo = "";
            try
            {
                
                //*********************************************************

                string _comprimir = Variables._path_envia + "\\Comp";

                if (!Directory.Exists(@_comprimir))
                    Directory.CreateDirectory(@_comprimir);

                string _ano = _fecha_proceso.ToString("yy");
                string _mes = _fecha_proceso.Month.ToString("D2");
                string _dia = _fecha_proceso.Day.ToString("D2");
                string _fecha = _ano + _mes + _dia;
                String[] filenames = Directory.GetFiles(Variables._path_envia, "*.*");
                _archivo = "TD" + _fecha + "." + Right(_tienda, 3);

                BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                string _error = trans.ws_existe_archivo_SQL(conexion, _archivo);
                if (_error.Length>0)
                {
                    switch (_error)
                    {
                        case "0":
                            return false;
                        case "1":
                            return true;
                        default:
                            return true;
                    }
                }

            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }

        private static DataTable _dt_fecha_venta()
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;            
            string sqlquery = "";
            DataTable _dt_fecha =null;
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                //tienda
                sqlquery = "select DISTINCT fc_ffac from FFACTC02 WHERE FC_FFAC>=? AND (FC_FTX IS NULL OR LEN(FC_FTX)=0)";                
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_tda;                
                da = new OleDbDataAdapter(cmd);
                _dt_fecha = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(_dt_fecha);
                }
            }
            catch
            {
                _dt_fecha = null;
                throw;
            }
            return _dt_fecha;
        }
        private static void _update_version_winupdateexe()
        {
            try
            {
                string _assembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string _exe_servicio = "Transmision.NetWin.Update.exe";
                string _path_service = _assembly + "\\" + _exe_servicio;
                var fvi = FileVersionInfo.GetVersionInfo(_path_service);
                var version = fvi.FileVersion;
                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                Boolean _valida_version = updateversion.ws_existe_exewinupdate_version(conexion, version);
                if (_valida_version)
                {
                    //si la version es diferente entonces vasmoa  copiar la version
                    copiar_archivo_service(_exe_servicio);
                }

            }
            catch
            {

            }
        }
        private static void copiar_archivo_service(string _name)
        {
            try
            {
                //string _ruta_local_service = @"D:\POS\Transmision.net\" + _name;

                string _ruta_local_service= System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + _name;

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                byte[] _archivo = updateversion.ws_dll_service_tda(conexion, _name);
                System.IO.File.WriteAllBytes(_ruta_local_service, _archivo);

                //if (_name == "Transmision.Net.Basico.dll")
                //{
                //    _dbftienda();
                FileInfo infofile_pro = new FileInfo(@_ruta_local_service);
                var fvi_pro = FileVersionInfo.GetVersionInfo(@_ruta_local_service);
                var version_pro = fvi_pro.FileVersion;

                string _act = updateversion.ws_update_versiondll_net(conexion, _tienda, infofile_pro.Name, version_pro);
                //}


            }
            catch
            {
                                
            }
        }

        private static void _update_version_serviowin()
        {
            try
            {
                //TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                //tw.WriteLine("ANTES DE INGRESAR AL SERVICE");
                //tw.Flush();
                //tw.Close();
                //tw.Dispose();
                //vamos a verificar y vemos si hay alguna actualizacion del servicio windows
                //esta respuesta tiene que ser de la web service de archivos de actualizacion
                //var _assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;

                string _assembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string _exe_servicio = "Transmision.Net.Basico.dll";
                string _path_service = _assembly + "\\" + _exe_servicio;
                var fvi = FileVersionInfo.GetVersionInfo(_path_service);
                var version = fvi.FileVersion;

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                Boolean _valida_version = updateversion.ws_existe_serviciowin_version(conexion, version);

                if (_valida_version)
                {
                    //TextWriter tw1 = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                    //tw1.WriteLine("reintentando");
                    //tw1.Flush();
                    //tw1.Close();
                    //tw1.Dispose();

                    string _exe_name = "Transmision.NetWin.Update.exe";
                    string _path = _assembly + "\\" + _exe_name;
                    System.Diagnostics.Process.Start(@_path);
                }
                //else
                //{
                //    TextWriter tw2 = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                //    tw2.WriteLine("no para esta dll");
                //    tw2.Flush();
                //    tw2.Close();
                //    tw2.Dispose();
                //}


            }
            catch
            {
                //throw;
            }
        }
        private static void _update_version_vfpoledbdll(Boolean register=false)
        {
            try
            {
                string _wind32_vfpoledb = @"C:\\Windows\\System32";                
                string _dll_vfpoledb = "vfpoledb.dll";
                Boolean valida = false;
                //si no existe archivo
               if  (!File.Exists(_wind32_vfpoledb + "\\" + _dll_vfpoledb))
               {
                    valida = true;
                    copiar_archivo_dll_com(_wind32_vfpoledb, _dll_vfpoledb);
               }
               
               if (!valida)
                {               
                   if (register)
                    {
                        string _ruta_local_vfpoledb = @_wind32_vfpoledb + "\\" + _dll_vfpoledb;
                        string fileinfo = "/s" + " " + "\"" + _ruta_local_vfpoledb + "\"";

                        Process reg = new Process();
                        reg.StartInfo.FileName = "regsvr32.exe";
                        reg.StartInfo.Arguments = fileinfo;
                        reg.StartInfo.UseShellExecute = false;
                        reg.StartInfo.CreateNoWindow = true;
                        reg.StartInfo.RedirectStandardOutput = true;
                        reg.Start();
                        reg.WaitForExit();
                        reg.Close();
                        //copiar_archivo_dll_com(_wind32_vfpoledb, _dll_vfpoledb);
                    }
                }

                //else
                // {
                //     File.Delete(_wind32_vfpoledb + "\\" + _dll_vfpoledb);
                // }
            }
            catch
            {

            }
        }
        private static void copiar_archivo_dll_com(string _ruta_win_system,string _name)
        {
            try
            {
                string _ruta_local_vfpoledb = @_ruta_win_system + "\\" + _name;

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                byte[] _archivo = updateversion.ws_dll_service_tda(conexion, _name);
                System.IO.File.WriteAllBytes(_ruta_local_vfpoledb, _archivo);

                string fileinfo= "/s" + " " + "\"" + _ruta_local_vfpoledb + "\"";

                Process reg = new Process();
                reg.StartInfo.FileName = "regsvr32.exe";
                reg.StartInfo.Arguments = fileinfo;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.Start();
                reg.WaitForExit();
                reg.Close();

                string _act = updateversion.ws_update_dll_com(conexion, _tienda, _name);


            }
            catch(Exception exc)
            {
                
            }
        }

        private static Boolean _ejecute_reindex_dbf()
        {
            Boolean _valida = false;
            try
            {
                string _dbf_index = Variables._path_default + "\\tBLOCK.DBF";
                if (File.Exists(_dbf_index))
                {
                    _valida = true;
                }

            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }


        private static void _enviar_movimiento(ref String _error, string _fecha_plan="",Int32 _envio_inv=0)
        {
            string _FMSUCU02 = Variables._path_default + "\\FMSUCU02.DBF";
            string sqlquery = "select s_fpro from FMSUCU02 where s_csuc='" + Right(_tienda,3) + "'";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            DataTable dt_fmc = null;
            DataTable dt_fmd = null;

            //clonar tablas
            DataTable dt_fmc_clone = null;
            DataTable dt_fmd_clone = null;

            DateTime _fecha_proceso_mov ;
            string _FMC = "FMC";
            string _FMD = "FMD";              
            try
            {
                cn = new OleDbConnection(Variables._conexion_vfpoledb);
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                if(!_ejecute_reindex_dbf())
                {
                    da.Fill(dt);

                    if (dt!=null)
                    {
                        if (dt.Rows.Count>0)
                        {
                            //EN ESTA LINEA VAMOS A CAPTURAR EL NOMBRE DEL ARCHIVO DEPENDE DE LA FECHA DE PROCESO
                            if (_fecha_plan.Length==0)
                            { 
                                _fecha_proceso_mov = Convert.ToDateTime(dt.Rows[0]["s_fpro"]);
                            }
                            else
                            {
                                _fecha_proceso_mov = Convert.ToDateTime(_fecha_plan);
                            }
                            

                            string _ano = Right(_fecha_proceso_mov.ToString("yy"),1);
                             string _mes = _fecha_proceso_mov.Month.ToString("D2");
                            _FMC = _FMC +  _mes + _ano + Variables._codigo_empresa_tda;
                            _FMD = _FMD + _mes + _ano + Variables._codigo_empresa_tda;

                            //SI LA TABLA NO EXISTE ENTONCES LE HACEMOS UN RETURN
                            if (!File.Exists(Variables._path_default + "\\" + _FMC +".dbf" ))
                                return;                            

                            //*******************************************************UNA VES CAPTURADO
                            //VAMOS A VER    
                            //DateTime _ano_null =Convert.ToDateTime("30/12/1899");
                            if (_envio_inv==1)
                            { 
                                sqlquery = "SELECT v_tfor,v_proc,v_cfor,v_sfor,v_nfor,v_ffor,v_mone,v_tasa,v_almo," +
	                                       "v_almd,v_tane,v_anex,v_tdoc,v_suna,v_sdoc,v_ndoc,v_fdoc,v_tref,v_sref,v_nref,v_tipo," +
	                                       "v_arti,v_regl,v_colo,v_cant,v_pres,v_pred,v_vvts,v_vvtd,v_auto,v_ptot,v_impr,v_cuse," +
	                                       "v_muse,v_fcre,v_fmod,v_ftrx,v_ctra,v_memo,v_motr,v_par1,v_par2,v_par3,v_lle1,v_lle2," +
                                           "v_lle3,v_tipe,v_ruc2,v_rzo2,'' as v_hstd FROM " + _FMC + " WHERE EMPTY(v_tane) and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') " + ((_fecha_plan.Length==0)?"": " and DTOS(V_ffor)>'" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') "); //and (v_cfor='32' or v_cfor='31' )";
                            }
                            else
                            {
                                sqlquery = "SELECT v_tfor,v_proc,v_cfor,v_sfor,v_nfor,v_ffor,v_mone,v_tasa,v_almo," +
                                           "v_almd,v_tane,v_anex,v_tdoc,v_suna,v_sdoc,v_ndoc,v_fdoc,v_tref,v_sref,v_nref,v_tipo," +
                                           "v_arti,v_regl,v_colo,v_cant,v_pres,v_pred,v_vvts,v_vvtd,v_auto,v_ptot,v_impr,v_cuse," +
                                           "v_muse,v_fcre,v_fmod,v_ftrx,v_ctra,v_memo,v_motr,v_par1,v_par2,v_par3,v_lle1,v_lle2," +
                                           "v_lle3,v_tipe,v_ruc2,v_rzo2,'' as v_hstd FROM " + _FMC + " WHERE EMPTY(v_tane) and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') " + ((_fecha_plan.Length==0)?"": " and DTOS(V_ffor)>='" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') "); //and (v_cfor='32' or v_cfor='31' )";
                            }
                            cmd = new OleDbCommand(sqlquery, cn);
                            cmd.CommandTimeout = 0;                            
                            da = new OleDbDataAdapter(cmd);
                            dt_fmc = new DataTable();
                            if (!_ejecute_reindex_dbf())
                            {   
                                //insertar cabecera del movimiento                             
                                da.Fill(dt_fmc);
                                //ahora vamo a verificar el detalle

                                if (_envio_inv == 1)
                                { 
                                        sqlquery = "SELECT * FROM " + _FMC + " INNER JOIN " + _FMD +
                                           " ON v_tfor=i_tfor AND v_cfor=i_cfor  AND v_sfor=i_sfor  AND v_nfor=i_nfor  WHERE EMPTY(v_tane)" + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>'" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI')"); //and (v_cfor='32' or v_cfor='31' )";
                                }
                                else
                                {
                                    sqlquery = "SELECT * FROM " + _FMC + " INNER JOIN " + _FMD +
                                       " ON v_tfor=i_tfor AND v_cfor=i_cfor  AND v_sfor=i_sfor  AND v_nfor=i_nfor  WHERE EMPTY(v_tane)" + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>='" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI')"); //and (v_cfor='32' or v_cfor='31' )";
                                }

                                cmd = new OleDbCommand(sqlquery, cn);
                                cmd.CommandTimeout = 0;
                                da = new OleDbDataAdapter(cmd);
                                dt_fmd = new DataTable();
                                if (!_ejecute_reindex_dbf())
                                {
                                    da.Fill(dt_fmd);
                                }

                                if (dt_fmc!=null && dt_fmd!=null)
                                {
                                    dt_fmc_clone = dt_fmc.Clone();
                                    dt_fmd_clone = dt_fmd.Clone();

                                        foreach (DataRow _fila_cab in dt_fmc.Rows)
                                        {
                                            string _v_tfor = _fila_cab["v_tfor"].ToString();
                                            string _v_cfor = _fila_cab["v_cfor"].ToString();
                                            string _v_sfor = _fila_cab["v_sfor"].ToString();
                                            string _v_nfor = _fila_cab["v_nfor"].ToString();


                                            DataRow[] _existe_vdetalle = dt_fmd.Select("v_tfor='" + _v_tfor + "' and v_cfor='" + _v_cfor + "' and v_sfor='" + _v_sfor + "' and v_nfor='" + _v_nfor + "'");

                                            if (_existe_vdetalle.Length > 0)
                                            {
                                                dt_fmc_clone.ImportRow(_fila_cab);
                                                for (Int32 i = 0; i < _existe_vdetalle.Length; ++i)
                                                {
                                                   dt_fmd_clone.ImportRow(_existe_vdetalle[i]);
                                                }
                                            }
                                        }
                                        //EN ESTE CASO VAMOS A QUITAR COLUMNAS SIEMPRE CUANO FILA SEA MAYOR A CERO
                                        if (dt_fmd.Rows.Count>0)
                                        { 
                                            #region<BORRAMOS COLUMNAS CON INICIO DE CAMPO V PORQUE EN INNER SELECT FMD_CAB Y FMD_DET>                    
                                            List<DataColumn> columnsToDelete = new List<DataColumn>();
                                            foreach (DataColumn col in dt_fmd_clone.Columns)
                                            {
                                                if (Left(col.ColumnName, 1).ToUpper() == "V".ToUpper())
                                                    columnsToDelete.Add(col);
                                            }

                                            foreach (DataColumn col in columnsToDelete)
                                                dt_fmd_clone.Columns.Remove(col);
                                            #endregion
                                        }


                                    #region<AHORA CAPTURAMOS EL DATATABLE DE MOV CAB Y DET>
                                    dt_fmc_clone.TableName = "FMC";
                                    dt_fmd_clone.TableName = "FMD";
                                    DataSet ds = new DataSet();
                                    ds.Tables.Add(dt_fmc_clone);
                                    ds.Tables.Add(dt_fmd_clone);
                                    //EN ESTE VERIFICAMOS QUE EXISTA DATOS PARA ENVIAR POR LA WEB SERVICE
                                    //TANTO VALIDA NULL Y FILAS >0
                                    if (dt_fmc_clone!=null && dt_fmd_clone!=null)
                                    {
                                        if (dt_fmc_clone.Rows.Count>0 && dt_fmd_clone.Rows.Count>0)
                                        {
                                            BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                                            BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                                            conexion.user_name = "emcomer";
                                            conexion.user_password = "Bata2013";

                                           // if (_tienda == "50140")
                                           // { 
                                                trans.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
                                           // }


                                            //verificar el numero de columna de 26 en el detalle

                                            if (dt_fmd_clone.Columns.Count==31)
                                            {
                                                dt_fmd_clone.Columns.RemoveAt(30);
                                                dt_fmd_clone.Columns.RemoveAt(29);
                                                dt_fmd_clone.Columns.RemoveAt(28);
                                                dt_fmd_clone.Columns.RemoveAt(27);
                                                dt_fmd_clone.Columns.RemoveAt(26);

                                            }

                                            

                                            String _valida = trans.ws_genera_mov(conexion,"T", _tienda,dt_fmc_clone,dt_fmd_clone);

                                            //***ESTE CODIGO ES PARA VALIDAR EN EL DBF Y HACER UN UPDATE A LA TABLA DE FMC
                                            if (_valida.Length==0)
                                            { 
                                                DateTime _fecha_act = DateTime.Today;
                                                for(Int32 i=0;i< dt_fmc_clone.Rows.Count;++i)
                                                {
                                                    string v_tfor = dt_fmc_clone.Rows[i]["v_tfor"].ToString();
                                                    string v_cfor = dt_fmc_clone.Rows[i]["v_cfor"].ToString();
                                                    string v_sfor = dt_fmc_clone.Rows[i]["v_sfor"].ToString();
                                                    string v_nfor = dt_fmc_clone.Rows[i]["v_nfor"].ToString();
                                                    sqlquery = "update " + _FMC + " set v_tane=? where v_tfor='" + v_tfor + "' and v_cfor='" + v_cfor + "' " +
                                                               " and v_sfor='" + v_sfor + "' and v_nfor='" + v_nfor + "'";

                                                    cmd = new OleDbCommand(sqlquery, cn);
                                                    cmd.CommandTimeout = 0;
                                                    cmd.Parameters.Add("@v_tane", OleDbType.Char).Value = "X";
                                                    if (cn.State == 0) cn.Open();
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                            else
                                            {
                                                _error = _valida;
                                            }
                                        }
                                        else
                                        {
                                           // _error = "NO HAY DATOS";
                                        }
                                    }


                                    #endregion
                                }
                            }

                        }
                    }
                }

            }
            catch(Exception exc)
            {
                _error = exc.Message;
                if (cn!=null)
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            if (cn.State == ConnectionState.Open) cn.Close();
        }
        private static void _recepcion_movimiento()
        {
            DataSet ds = null;
            string sqlquery = "";
            string _FFCGUD02 = "FFCGUD02";
            string _FFDGUD02 = "FFDGUD02";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            try
            {
                if (!_ejecute_reindex_dbf())
                {
                
                    BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                    BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                    conexion.user_name = "emcomer";
                    conexion.user_password = "Bata2013";

                    ds = trans.ws_recepcion_transac(conexion, _tienda);
                    //****verificar que el dataset te devuelva datos o este eliminada el dataset
                    if (ds!=null)
                    {
                        //***verificar si hay tablas en el retorno de la web service de recepcion para crear sus tables cab y det
                        if (ds.Tables.Count>0)
                        {
                            DataTable dt_cab = ds.Tables[0];
                            dt_cab.TableName = "FFCGUD02";
                            DataTable dt_det = ds.Tables[1];
                            dt_det.TableName = "FFDGUD02";

                            //
                            if (dt_cab.Rows.Count>0)
                            {
                                //primero pasamos traspasos de tiendas concepto 31
                                DataRow[] _fila_tras_cab = dt_cab.Select("gtc_tipo='31'");
                                //si hay despachos de tiendas por recibir entonces
                                //verificamos
                                if (_fila_tras_cab.Length>0)
                                {
                                    for (Int32 i=0;i<_fila_tras_cab.Length;++i)
                                    {
                                        string gtc_nume = _fila_tras_cab[i]["gtc_nume"].ToString();
                                        decimal _nro_transac=Convert.ToDecimal(_fila_tras_cab[i]["nro_transa"]);
                                        //verificamos que la guia exista, si existe entonces no hacemos el insert
                                        sqlquery = "SELECT * FROM " + _FFCGUD02 + " WHERE GTC_NUME='" + gtc_nume + "' and gtc_tipo='31'";
                                        cn = new OleDbConnection(Variables._conexion);
                                        cmd = new OleDbCommand(sqlquery, cn);
                                        cmd.CommandTimeout = 0;
                                        da = new OleDbDataAdapter(cmd);
                                        DataTable dt = new DataTable();
                                        if (!_ejecute_reindex_dbf())
                                        {
                                            da.Fill(dt);
                                            //si el datatable no devuelve fila quiere decir que la guia aun no se actualizado en la tda
                                            if (dt!=null)
                                            {
                                                if (dt.Rows.Count==0)
                                                {

                                                    string _gtc_tipo = _fila_tras_cab[i]["gtc_tipo"].ToString();string _gtc_alm = _fila_tras_cab[i]["gtc_alm"].ToString();
                                                    string _gtc_nume = _fila_tras_cab[i]["gtc_nume"].ToString(); DateTime _gtc_femi =Convert.ToDateTime(_fila_tras_cab[i]["gtc_femi"]);
                                                    string _gtc_semi = _fila_tras_cab[i]["gtc_semi"].ToString(); string _gtc_gudis = _fila_tras_cab[i]["gtc_gudis"].ToString();
                                                    string _gtc_tndcl = _fila_tras_cab[i]["gtc_tndcl"].ToString(); string _gtc_estad = _fila_tras_cab[i]["gtc_estad"].ToString();
                                                    string _gtc_frect= "{ / / }" /*Convert.ToDateTime(_fila_tras_cab[i]["gtc_frect"])*/; Decimal _gtc_cal =Convert.ToDecimal(_fila_tras_cab[i]["gtc_cal"]);
                                                    Decimal _gtc_calm =Convert.ToDecimal(_fila_tras_cab[i]["gtc_calm"]); Decimal _gtc_acc =Convert.ToDecimal(_fila_tras_cab[i]["gtc_acc"]);
                                                    Decimal _gtc_accm =Convert.ToDecimal(_fila_tras_cab[i]["gtc_accm"]); Decimal _gtc_caj =Convert.ToDecimal(_fila_tras_cab[i]["gtc_caj"]);
                                                    Decimal _gtc_cajm = Convert.ToDecimal(_fila_tras_cab[i]["gtc_cajm"]); string _gtc_glosa = _fila_tras_cab[i]["gtc_glosa"].ToString();
                                                    //insertamos la guia
                                                    sqlquery = "insert into " + _FFCGUD02 + "(gtc_tipo,gtc_alm,gtc_nume,gtc_femi,gtc_semi,gtc_gudis," +
                                                                                            "gtc_tndcl,gtc_estad,gtc_frect,gtc_cal,gtc_calm,gtc_acc," +
                                                                                            "gtc_accm,gtc_caj,gtc_cajm,gtc_glosa) " + 
                                                                                            "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                                                    cmd = new OleDbCommand(sqlquery,cn);
                                                    cmd.CommandTimeout = 0;
                                                    cmd.CommandType = CommandType.Text;
                                                    cmd.Parameters.Add("@gtc_tipo", OleDbType.Char).Value = _gtc_tipo;
                                                    cmd.Parameters.Add("@gtc_alm",OleDbType.Char).Value=_gtc_alm;
                                                    cmd.Parameters.Add("@gtc_nume",OleDbType.Char).Value=_gtc_nume;
                                                    cmd.Parameters.Add("@gtc_femi",OleDbType.Date).Value=_gtc_femi;
                                                    cmd.Parameters.Add("@gtc_semi", OleDbType.Char).Value = _gtc_semi;
                                                    cmd.Parameters.Add("@gtc_gudis",OleDbType.Char).Value=_gtc_gudis;
                                                    cmd.Parameters.Add("@gtc_tndcl",OleDbType.Char).Value=_gtc_tndcl;
                                                    cmd.Parameters.Add("@gtc_estad",OleDbType.Char).Value=_gtc_estad;
                                                    cmd.Parameters.Add("@gtc_frect", OleDbType.Date).Value = DBNull.Value;// _gtc_frect;
                                                    cmd.Parameters.Add("@gtc_cal",OleDbType.Decimal).Value=_gtc_cal;
                                                    cmd.Parameters.Add("@gtc_calm", OleDbType.Decimal).Value =_gtc_calm;
                                                    cmd.Parameters.Add("@gtc_acc", OleDbType.Decimal).Value = _gtc_acc;
                                                    cmd.Parameters.Add("@gtc_accm", OleDbType.Decimal).Value = _gtc_accm;
                                                    cmd.Parameters.Add("@gtc_caj", OleDbType.Decimal).Value = _gtc_caj;
                                                    cmd.Parameters.Add("@gtc_cajm", OleDbType.Decimal).Value = _gtc_cajm;
                                                    cmd.Parameters.Add("@gtc_glosa", OleDbType.Char).Value = _gtc_glosa;
                                                    if (cn.State == 0) cn.Open();
                                                    if (!_ejecute_reindex_dbf())
                                                    {
                                                        //ejecutasmo la inseracion al dbf cab
                                                        cmd.ExecuteNonQuery();
                                                        //ahora insertar detalle
                                                        DataRow[] _fila_tras_det = dt_det.Select("nro_transa=" + _nro_transac);
                                                        if (_fila_tras_det.Length>0)
                                                        {
                                                            for (Int32 a=0;a<_fila_tras_det.Length;++a)
                                                            {
                                                                string _gtd_tipo=_fila_tras_det[a]["gtd_tipo"].ToString();string _gtd_nume= _fila_tras_det[a]["gtd_nume"].ToString();
                                                                string _gtd_gudis= _fila_tras_det[a]["gtd_gudis"].ToString(); string _gtd_artic= _fila_tras_det[a]["gtd_artic"].ToString();
                                                                string _gtd_categ = _fila_tras_det[a]["gtd_categ"].ToString();string _gtd_subca= _fila_tras_det[a]["gtd_subca"].ToString();
                                                                string _gtd_cndme= _fila_tras_det[a]["gtd_cndme"].ToString(); Decimal _gtd_prvta=Convert.ToDecimal(_fila_tras_det[a]["gtd_prvta"]);
                                                                Decimal _gtd_impor= Convert.ToDecimal(_fila_tras_det[a]["gtd_impor"]); Decimal _gtd_med00= Convert.ToDecimal(_fila_tras_det[a]["gtd_med00"]);
                                                                Decimal _gtd_med01= Convert.ToDecimal(_fila_tras_det[a]["gtd_med01"]); Decimal _gtd_med02= Convert.ToDecimal(_fila_tras_det[a]["gtd_med02"]);
                                                                Decimal _gtd_med03= Convert.ToDecimal(_fila_tras_det[a]["gtd_med03"]); Decimal _gtd_med04= Convert.ToDecimal(_fila_tras_det[a]["gtd_med04"]);
                                                                Decimal _gtd_med05= Convert.ToDecimal(_fila_tras_det[a]["gtd_med05"]); Decimal _gtd_med06= Convert.ToDecimal(_fila_tras_det[a]["gtd_med06"]);
                                                                Decimal _gtd_med07= Convert.ToDecimal(_fila_tras_det[a]["gtd_med07"]); Decimal _gtd_med08= Convert.ToDecimal(_fila_tras_det[a]["gtd_med08"]);
                                                                Decimal _gtd_med09= Convert.ToDecimal(_fila_tras_det[a]["gtd_med09"]); Decimal _gtd_med10= Convert.ToDecimal(_fila_tras_det[a]["gtd_med10"]);
                                                                Decimal _gtd_med11= Convert.ToDecimal(_fila_tras_det[a]["gtd_med11"]); Decimal _gtd_total= Convert.ToDecimal(_fila_tras_det[a]["gtd_total"]);
                                                                string _gtd_conf= _fila_tras_det[a]["gtd_conf"].ToString();

                                                                sqlquery = "insert into " + _FFDGUD02 + "(gtd_tipo,gtd_nume,gtd_gudis,gtd_artic,gtd_categ,gtd_subca,gtd_cndme,gtd_prvta," +
                                                                                                        "gtd_impor,gtd_med00,gtd_med01,gtd_med02,gtd_med03,gtd_med04,gtd_med05,gtd_med06," +
                                                                                                        "gtd_med07,gtd_med08,gtd_med09,gtd_med10,gtd_med11,gtd_total,gtd_conf)" +
                                                                                                        "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                                                                cmd = new OleDbCommand(sqlquery, cn);
                                                                cmd.CommandTimeout = 0;
                                                                cmd.CommandType = CommandType.Text;
                                                                cmd.Parameters.Add("@gtd_tipo", OleDbType.Char).Value = _gtd_tipo;
                                                                cmd.Parameters.Add("@gtd_nume", OleDbType.Char).Value = _gtd_nume;
                                                                cmd.Parameters.Add("@gtd_gudis", OleDbType.Char).Value = _gtd_gudis;
                                                                cmd.Parameters.Add("@gtd_artic", OleDbType.Char).Value = _gtd_artic;
                                                                cmd.Parameters.Add("@gtd_categ", OleDbType.Char).Value = _gtd_categ;
                                                                cmd.Parameters.Add("@gtd_subca", OleDbType.Char).Value = _gtd_subca;
                                                                cmd.Parameters.Add("@gtd_cndme", OleDbType.Char).Value = _gtd_cndme;
                                                                cmd.Parameters.Add("@gtd_prvta", OleDbType.Numeric).Value = _gtd_prvta;
                                                                cmd.Parameters.Add("@gtd_impor", OleDbType.Numeric).Value = _gtd_impor;
                                                                cmd.Parameters.Add("@gtd_med00", OleDbType.Numeric).Value = _gtd_med00;
                                                                cmd.Parameters.Add("@gtd_med01", OleDbType.Numeric).Value = _gtd_med01;
                                                                cmd.Parameters.Add("@gtd_med02", OleDbType.Numeric).Value = _gtd_med02;
                                                                cmd.Parameters.Add("@gtd_med03", OleDbType.Numeric).Value = _gtd_med03;
                                                                cmd.Parameters.Add("@gtd_med04", OleDbType.Numeric).Value = _gtd_med04;
                                                                cmd.Parameters.Add("@gtd_med05", OleDbType.Numeric).Value = _gtd_med05;
                                                                cmd.Parameters.Add("@gtd_med06", OleDbType.Numeric).Value = _gtd_med06;
                                                                cmd.Parameters.Add("@gtd_med07", OleDbType.Numeric).Value = _gtd_med07;
                                                                cmd.Parameters.Add("@gtd_med08", OleDbType.Numeric).Value = _gtd_med08;
                                                                cmd.Parameters.Add("@gtd_med09", OleDbType.Numeric).Value = _gtd_med09;
                                                                cmd.Parameters.Add("@gtd_med10", OleDbType.Numeric).Value = _gtd_med10;
                                                                cmd.Parameters.Add("@gtd_med11", OleDbType.Numeric).Value = _gtd_med11;
                                                                cmd.Parameters.Add("@gtd_total", OleDbType.Numeric).Value = _gtd_total;
                                                                cmd.Parameters.Add("@gtd_conf", OleDbType.Char).Value = _gtd_conf;
                                                                if (cn.State == 0) cn.Open();
                                                                cmd.ExecuteNonQuery();
                                                            }
                                                            //en este caso vamos actualizar el flag de recepcion
                                                            string _update_flag = trans.ws_transaccion_flag(conexion, _nro_transac);

                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    //verificar si no tiene detalle tiene detalle 
                                                    sqlquery = "SELECT * FROM " + _FFDGUD02 + " WHERE GTD_NUME='" + gtc_nume + "' and gtd_tipo='31'";
                                                    cn = new OleDbConnection(Variables._conexion);
                                                    cmd = new OleDbCommand(sqlquery, cn);
                                                    cmd.CommandTimeout = 0;
                                                    da = new OleDbDataAdapter(cmd);
                                                    DataTable dt_det_v = new DataTable();
                                                    if (!_ejecute_reindex_dbf())
                                                    {
                                                        da.Fill(dt_det_v);
                                                        //***verificar que no tiene detalle
                                                        if (dt_det_v != null)
                                                        {
                                                            if (dt_det_v.Rows.Count==0)
                                                            {
                                                                DataRow[] _fila_tras_det = dt_det.Select("nro_transa=" + _nro_transac);
                                                                if (_fila_tras_det.Length > 0)
                                                                {
                                                                    for (Int32 a = 0; a < _fila_tras_det.Length; ++a)
                                                                    {
                                                                        string _gtd_tipo = _fila_tras_det[a]["gtd_tipo"].ToString(); string _gtd_nume = _fila_tras_det[a]["gtd_nume"].ToString();
                                                                        string _gtd_gudis = _fila_tras_det[a]["gtd_gudis"].ToString(); string _gtd_artic = _fila_tras_det[a]["gtd_artic"].ToString();
                                                                        string _gtd_categ = _fila_tras_det[a]["gtd_categ"].ToString(); string _gtd_subca = _fila_tras_det[a]["gtd_subca"].ToString();
                                                                        string _gtd_cndme = _fila_tras_det[a]["gtd_cndme"].ToString(); Decimal _gtd_prvta = Convert.ToDecimal(_fila_tras_det[a]["gtd_prvta"]);
                                                                        Decimal _gtd_impor = Convert.ToDecimal(_fila_tras_det[a]["gtd_impor"]); Decimal _gtd_med00 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med00"]);
                                                                        Decimal _gtd_med01 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med01"]); Decimal _gtd_med02 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med02"]);
                                                                        Decimal _gtd_med03 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med03"]); Decimal _gtd_med04 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med04"]);
                                                                        Decimal _gtd_med05 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med05"]); Decimal _gtd_med06 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med06"]);
                                                                        Decimal _gtd_med07 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med07"]); Decimal _gtd_med08 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med08"]);
                                                                        Decimal _gtd_med09 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med09"]); Decimal _gtd_med10 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med10"]);
                                                                        Decimal _gtd_med11 = Convert.ToDecimal(_fila_tras_det[a]["gtd_med11"]); Decimal _gtd_total = Convert.ToDecimal(_fila_tras_det[a]["gtd_total"]);
                                                                        string _gtd_conf = _fila_tras_det[a]["gtd_conf"].ToString();

                                                                        sqlquery = "insert into " + _FFDGUD02 + "(gtd_tipo,gtd_nume,gtd_gudis,gtd_artic,gtd_categ,gtd_subca,gtd_cndme,gtd_prvta," +
                                                                                                                "gtd_impor,gtd_med00,gtd_med01,gtd_med02,gtd_med03,gtd_med04,gtd_med05,gtd_med06," +
                                                                                                                "gtd_med07,gtd_med08,gtd_med09,gtd_med10,gtd_med11,gtd_total,gtd_conf)" +
                                                                                                                "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                                                                        cmd = new OleDbCommand(sqlquery, cn);
                                                                        cmd.CommandTimeout = 0;
                                                                        cmd.CommandType = CommandType.Text;
                                                                        cmd.Parameters.Add("@gtd_tipo", OleDbType.Char).Value = _gtd_tipo;
                                                                        cmd.Parameters.Add("@gtd_nume", OleDbType.Char).Value = _gtd_nume;
                                                                        cmd.Parameters.Add("@gtd_gudis", OleDbType.Char).Value = _gtd_gudis;
                                                                        cmd.Parameters.Add("@gtd_artic", OleDbType.Char).Value = _gtd_artic;
                                                                        cmd.Parameters.Add("@gtd_categ", OleDbType.Char).Value = _gtd_categ;
                                                                        cmd.Parameters.Add("@gtd_subca", OleDbType.Char).Value = _gtd_subca;
                                                                        cmd.Parameters.Add("@gtd_cndme", OleDbType.Char).Value = _gtd_cndme;
                                                                        cmd.Parameters.Add("@gtd_prvta", OleDbType.Numeric).Value = _gtd_prvta;
                                                                        cmd.Parameters.Add("@gtd_impor", OleDbType.Numeric).Value = _gtd_impor;
                                                                        cmd.Parameters.Add("@gtd_med00", OleDbType.Numeric).Value = _gtd_med00;
                                                                        cmd.Parameters.Add("@gtd_med01", OleDbType.Numeric).Value = _gtd_med01;
                                                                        cmd.Parameters.Add("@gtd_med02", OleDbType.Numeric).Value = _gtd_med02;
                                                                        cmd.Parameters.Add("@gtd_med03", OleDbType.Numeric).Value = _gtd_med03;
                                                                        cmd.Parameters.Add("@gtd_med04", OleDbType.Numeric).Value = _gtd_med04;
                                                                        cmd.Parameters.Add("@gtd_med05", OleDbType.Numeric).Value = _gtd_med05;
                                                                        cmd.Parameters.Add("@gtd_med06", OleDbType.Numeric).Value = _gtd_med06;
                                                                        cmd.Parameters.Add("@gtd_med07", OleDbType.Numeric).Value = _gtd_med07;
                                                                        cmd.Parameters.Add("@gtd_med08", OleDbType.Numeric).Value = _gtd_med08;
                                                                        cmd.Parameters.Add("@gtd_med09", OleDbType.Numeric).Value = _gtd_med09;
                                                                        cmd.Parameters.Add("@gtd_med10", OleDbType.Numeric).Value = _gtd_med10;
                                                                        cmd.Parameters.Add("@gtd_med11", OleDbType.Numeric).Value = _gtd_med11;
                                                                        cmd.Parameters.Add("@gtd_total", OleDbType.Numeric).Value = _gtd_total;
                                                                        cmd.Parameters.Add("@gtd_conf", OleDbType.Char).Value = _gtd_conf;
                                                                        if (cn.State == 0) cn.Open();
                                                                        cmd.ExecuteNonQuery();
                                                                    }
                                                                    //en este caso vamos actualizar el flag de recepcion
                                                                    string _update_flag = trans.ws_transaccion_flag(conexion, _nro_transac);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }                                            
                                            }

                                        }


                                   }
                                }

                            }


                        }
                    }
                }
            }
            catch(Exception exc)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
        }
        

        private static void _espera_ejecuta(Int32 _segundos)
        {
            try
            {
                _segundos = _segundos * 1000;
                System.Threading.Thread.Sleep(_segundos);
            }
            catch
            {

            }
        }

        #region<ACTIVAR SERVICIO DE LA FACTURACION ELECTRONICA>
        private static void habilitando_servicio_FE()
        {
            try
            {
                ServiceController[] service;
                service = (ServiceController[])ServiceController.GetServices();
                for (Int32 s = 0; s < service.Length; ++s)
                {
                    string nameservicio = service[s].ServiceName;
                    if (nameservicio == "Service Hash (Bata)")
                    {
                        //en este caso vamos activar el firewall para la tranferencia de ftp al server
                        //agregarfirewall(2);

                        string status = service[s].Status.ToString();
                        string DisplayName = service[s].DisplayName.ToString();
                        string ServiceType = service[s].ServiceType.ToString();
                        string MachineName = service[s].MachineName.ToString();

                        ServiceController servicio;
                        ServiceControllerStatus servStatus;
                        servicio = (ServiceController)service[s];
                        servicio.Refresh();
                        servStatus = servicio.Status;
                        if (Left(servStatus.ToString(), 1) != "R")
                        {
                            servicio.Start();
                            servicio.Refresh();
                            return;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private static Boolean deshabilitando_servicio_FE()
        {
            Boolean _valida = false;
            try
            {
                ServiceController[] service;
                service = (ServiceController[])ServiceController.GetServices();
                for (Int32 s = 0; s < service.Length; ++s)
                {
                    string nameservicio = service[s].ServiceName;
                    if (nameservicio == "Service Hash (Bata)")
                    {
                        //en este caso vamos activar el firewall para la tranferencia de ftp al server
                        //agregarfirewall(2);

                        string status = service[s].Status.ToString();
                        string DisplayName = service[s].DisplayName.ToString();
                        string ServiceType = service[s].ServiceType.ToString();
                        string MachineName = service[s].MachineName.ToString();

                        ServiceController servicio;
                        ServiceControllerStatus servStatus;
                        servicio = (ServiceController)service[s];
                        servicio.Refresh();
                        servStatus = servicio.Status;
                        if (Left(servStatus.ToString(), 1) == "R")
                        {
                            servicio.Stop();
                            servicio.Refresh();
                            _valida = true;
                            break;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                _valida = false;
            }
            return _valida;
        }
        #endregion
        private static void _update_ws_facturacion_electronica()
        {
            string _ruta_config_fe = @"D:\INTERFA\CARVAJAL\bata_proceso\Genera_Hash_Xml.exe.config";
            Int32 _valida_service = 0;
            try
            {
                if (File.Exists(_ruta_config_fe))
                {
                    System.Configuration.Configuration wConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = _ruta_config_fe }, System.Configuration.ConfigurationUserLevel.None);
                    ServiceModelSectionGroup wServiceSection = ServiceModelSectionGroup.GetSectionGroup(wConfig);
                    ClientSection wClientSection = wServiceSection.Client;
                    string ws_url_felectronica = wClientSection.Endpoints[0].Address.ToString();

                    BataTransmision.bata_transaccionSoapClient updatews_fe = new BataTransmision.bata_transaccionSoapClient();
                    BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                    conexion.user_name = "emcomer";
                    conexion.user_password = "Bata2013";

                    string[] _updat_ws_tda = updatews_fe.ws_update_url_fe(conexion, "FE", ws_url_felectronica);
                    if (_updat_ws_tda[0].ToString() == "1")
                    {
                        _dbftienda();
                        deshabilitando_servicio_FE();
                        _valida_service = 1;
                        //**si el archivo existe entonces vamos actualizar 
                        string _url_new_ws = _updat_ws_tda[1].ToString();
                        wClientSection.Endpoints[0].Address = new Uri(_url_new_ws);
                        wConfig.Save();
                        habilitando_servicio_FE();
                        string _valida_update = updatews_fe.ws_tdaupdate_wsurl(conexion, _tienda, _url_new_ws);
                    }
                    else
                    {
                        _dbftienda();
                        string[] _existe_ws_urldata = updatews_fe.ws_existe_url_WS(conexion, _tienda, ws_url_felectronica);
                        if (_existe_ws_urldata[0].ToString() == "0")
                        {
                            string _valida_update = updatews_fe.ws_tdaupdate_wsurl(conexion, _tienda, ws_url_felectronica);
                        }
                    }
                }
            }
            catch
            {
                habilitando_servicio_FE();
            }
            if (_valida_service==1) habilitando_servicio_FE();
        }

        private static Boolean _valida_tda_ecu()
        {
            Boolean valida = false;
            BataTransmision.Resultado_Tda_Ecu result = null;
            try
            {
                result = new BataTransmision.Resultado_Tda_Ecu();
                BataTransmision.bata_transaccionSoapClient valida_ecu = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                result = valida_ecu.ws_valida_tda_ecu(conexion, _tienda);    
            }
            catch (Exception exc)
            {
                result.existe = true;
                result.descripcion = exc.Message;
            }

            valida = result.existe;

            return valida;
        }

        private static void _update_ws_transmision()
        {
            string _ruta_config_fe = @"D:\POS\Transmision.net\Genera_Transmision.exe.config";
            //Int32 _valida_service = 0;
            try
            {
                if (File.Exists(_ruta_config_fe))
                {
                    System.Configuration.Configuration wConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = _ruta_config_fe }, System.Configuration.ConfigurationUserLevel.None);
                    ServiceModelSectionGroup wServiceSection = ServiceModelSectionGroup.GetSectionGroup(wConfig);
                    ClientSection wClientSection = wServiceSection.Client;
                    string ws_url_felectronica = wClientSection.Endpoints[0].Address.ToString();

                    BataTransmision.bata_transaccionSoapClient updatews_fe = new BataTransmision.bata_transaccionSoapClient();
                    BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                    conexion.user_name = "emcomer";
                    conexion.user_password = "Bata2013";

                    string[] _updat_ws_tda = updatews_fe.ws_update_url_fe(conexion, "TR", ws_url_felectronica);
                    if (_updat_ws_tda[0].ToString() == "1")
                    {
                        //deshabilitando_servicio_FE();
                       // _valida_service = 1;
                        //**si el archivo existe entonces vamos actualizar 
                        string _url_new_ws = _updat_ws_tda[1].ToString();
                        wClientSection.Endpoints[0].Address = new Uri(_url_new_ws);
                        wConfig.Save();
                        //habilitando_servicio_FE();
                        //_dbftienda();
                        //string _valida_update = updatews_fe.ws_tdaupdate_wsurl(conexion, _tienda, _url_new_ws);
                    }
                }
            }
            catch
            {
                //habilitando_servicio_FE();
            }
            //if (_valida_service == 1) habilitando_servicio_FE();
        }
        private static void update_archivo_certificado()
        {
            try
            {
                
                string _assembly = @"D:\INTERFA\CARVAJAL\bata_proceso\Certificado";

                string _exe_servicio = "CDBATA.pfx";
                string _path_service = _assembly + "\\" + _exe_servicio;
                if (!File.Exists(@_path_service)) return;
                FileInfo info = new FileInfo(_path_service);
                decimal tamaño = info.Length;
                //var fvi = File.in .GetAttributes(_path_service);
                //var version = fvi.FileVersion;

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                Boolean _valida_version = updateversion.ws_existe_certificado(conexion, tamaño);

                if (_valida_version)
                {

                    _dbftienda();
                    byte[] _archivo = updateversion.ws_dll_service_tda(conexion, _exe_servicio);
                    System.IO.File.WriteAllBytes(_path_service, _archivo);

                    info = new FileInfo(_path_service);
                    tamaño = info.Length;

                    string _act = updateversion.ws_update_certificado_pfx(conexion, _tienda, _exe_servicio, tamaño);
                    //System.IO.File.WriteAllBytes(_ruta_local_service, _archivo);

                    //    string _exe_name = "Transmision.NetWin.Update.exe";
                    //    string _path = _assembly + "\\" + _exe_name;
                    //    System.Diagnostics.Process.Start(@_path);
                }
                else
                {
                    _dbftienda();
                    string[] _existe_ws_urldata = updateversion.ws_existe_certificado_WS(conexion, _tienda,_exe_servicio,tamaño);
                    if (_existe_ws_urldata[0].ToString() == "0")
                    {
                        string _act = updateversion.ws_update_certificado_pfx(conexion, _tienda, _exe_servicio, tamaño);
                    }
                }

            }
            catch
            {
             
            }
        }
        private static void _update_ws_win_actualiza()
        {
            string _ruta_config_fe = @"D:\POS\Transmision.net\Transmision.NetWin.Update.exe.config";
            //Int32 _valida_service = 0;
            try
            {
                if (File.Exists(_ruta_config_fe))
                {
                    System.Configuration.Configuration wConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = _ruta_config_fe }, System.Configuration.ConfigurationUserLevel.None);
                    ServiceModelSectionGroup wServiceSection = ServiceModelSectionGroup.GetSectionGroup(wConfig);
                    ClientSection wClientSection = wServiceSection.Client;
                    string ws_url_felectronica = wClientSection.Endpoints[0].Address.ToString();

                    BataTransmision.bata_transaccionSoapClient updatews_fe = new BataTransmision.bata_transaccionSoapClient();
                    BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                    conexion.user_name = "emcomer";
                    conexion.user_password = "Bata2013";

                    string[] _updat_ws_tda = updatews_fe.ws_update_url_fe(conexion, "TR", ws_url_felectronica);
                    if (_updat_ws_tda[0].ToString() == "1")
                    {
                        _dbftienda();
                        //deshabilitando_servicio_FE();
                        //_valida_service = 1;
                        //**si el archivo existe entonces vamos actualizar 
                        string _url_new_ws = _updat_ws_tda[1].ToString();
                        wClientSection.Endpoints[0].Address = new Uri(_url_new_ws);
                        wConfig.Save();
                        //habilitando_servicio_FE();
                        string _valida_update = updatews_fe.ws_tdaupdate_wsurl_win(conexion, _tienda, _url_new_ws);
                    }
                    else
                    {
                        _dbftienda();
                        string[] _existe_ws_urldata = updatews_fe.ws_existe_url_WS_Win(conexion, _tienda, ws_url_felectronica);
                        if (_existe_ws_urldata[0].ToString() == "0")
                        {
                            string _valida_update = updatews_fe.ws_tdaupdate_wsurl_win(conexion, _tienda, ws_url_felectronica);
                        }
                    }
                }
            }
            catch
            {
                //habilitando_servicio_FE();
            }
            //if (_valida_service == 1) habilitando_servicio_FE();
        }

        private static void update_archivo_carvajal()
        {
            Int32 _valida_servicio_fe = 0;
            try
            {
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                var _lista_file = trans.ws_get_file_upload(conexion);
                if (_lista_file != null)
                {
                    foreach (var itemcab in _lista_file)
                    {
                        string _carpeta_local = itemcab.tda_act_carpetalocal;
                        string _carpeta_server = itemcab.tda_act_rutaws + "\\" + itemcab.tda_act_carpetanom;
                        foreach (var itemdet in itemcab.tda_act_file)
                        {
                            string _fecha_file_server = itemdet.fecha_file_server;
                            string _nombre_filer_server = itemdet.name_file_server;
                            decimal _longitud_file_server = itemdet.longitud_file_server;

                            string _ruta_local_file = _carpeta_local + "\\" + _nombre_filer_server;
                            /*si el archivo existe entonces verificamos que este con la ultima version por la fecha de modificacion*/
                            if (File.Exists(@_ruta_local_file))
                            {

                                FileInfo info = new FileInfo(@_ruta_local_file);
                                string _fecha_file_local = info.LastWriteTime.ToString("dd/MM/yyyy H:mm:ss");
                                decimal _longitud_file_local = info.Length;

                                /*si la fecha es diferente entonces modificamos*/
                                if (_longitud_file_server != _longitud_file_local)
                                {
                                    string file_ruta_server = _carpeta_server + "\\" + _nombre_filer_server;

                                    byte[] file_upload = trans.ws_bytes_file_server(conexion, file_ruta_server);

                                    if (file_upload != null)
                                    {
                                        if (_valida_servicio_fe==0)
                                        { 
                                            _valida_servicio_fe = 1;
                                            deshabilitando_servicio_FE();
                                            _espera_ejecuta(5);
                                        }
                                        File.WriteAllBytes(@_ruta_local_file, file_upload);



                                        string[] _existe_ws_urldata = trans.ws_existe_fepe_dll_data(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                        if (_existe_ws_urldata[0].ToString() == "0")
                                        {
                                            string _act = trans.ws_update_fepe_dll(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                        }

                                    }
                                }
                                else
                                {
                                    _dbftienda();
                                    string[] _existe_ws_urldata = trans.ws_existe_fepe_dll_data(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                    if (_existe_ws_urldata[0].ToString() == "0")
                                    {
                                        string _act = trans.ws_update_fepe_dll(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                    }
                                }

                            }
                            else
                            {
                                /*SOLO PARA PROGRAMA DE TDA DBF Y OTROS TRANSMISION*/
                                if (itemcab.tda_act_carpetanom=="PROG")
                                {

                                    string file_ruta_server = _carpeta_server + "\\" + _nombre_filer_server;

                                    byte[] file_upload = trans.ws_bytes_file_server(conexion, file_ruta_server);

                                    if (file_upload != null)
                                    {
                                        //if (_valida_servicio_fe == 0)
                                        //{
                                        //    _valida_servicio_fe = 1;
                                        //    deshabilitando_servicio_FE();
                                        //    _espera_ejecuta(5);
                                        //}
                                        File.WriteAllBytes(@_ruta_local_file, file_upload);



                                        //string[] _existe_ws_urldata = trans.ws_existe_fepe_dll_data(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                        //if (_existe_ws_urldata[0].ToString() == "0")
                                        //{
                                            string _act = trans.ws_update_fepe_dll(conexion, _tienda, _nombre_filer_server, _longitud_file_server);
                                        //}

                                    }
                                }
                            }

                        }
                    }
                }
                if (_valida_servicio_fe == 1)
                {
                    _valida_servicio_fe = 1;
                    habilitando_servicio_FE();
                }
            }
            catch(Exception exc)
            {
                _valida_servicio_fe = 0;
                habilitando_servicio_FE();
            }
        }
        private static void update_archivo_fepe_dll()
        {
            try
            {

                string _assembly = @"D:\INTERFA\CARVAJAL\bata_proceso";

                string _exe_servicio = "Carvajal.FEPE.PreSC.dll";
                string _path_service = _assembly + "\\" + _exe_servicio;
                if (!File.Exists(@_path_service)) return;
                FileInfo info = new FileInfo(_path_service);
                decimal tamaño = info.Length;
               

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                Boolean _valida_version = updateversion.ws_existe_fepe_dll(conexion, tamaño);

                if (_valida_version)
                {

                    _dbftienda();
                    byte[] _archivo = updateversion.ws_dll_service_tda(conexion, _exe_servicio);
                    System.IO.File.WriteAllBytes(_path_service, _archivo);


                    info = new FileInfo(_path_service);
                    tamaño = info.Length;

                    string[] _existe_ws_urldata = updateversion.ws_existe_fepe_dll_data(conexion, _tienda, _exe_servicio, tamaño);
                    if (_existe_ws_urldata[0].ToString() == "0") {
                        string _act = updateversion.ws_update_fepe_dll(conexion, _tienda, _exe_servicio, tamaño);
                     }
                    
                }
                else
                {
                    _dbftienda();
                    string[] _existe_ws_urldata = updateversion.ws_existe_fepe_dll_data(conexion, _tienda, _exe_servicio, tamaño);
                    if (_existe_ws_urldata[0].ToString() == "0")
                    {
                        string _act = updateversion.ws_update_fepe_dll(conexion, _tienda, _exe_servicio, tamaño);
                    }
                }

            }
            catch
            {

            }
        }

        private static void update_get_version_so()
        {
            try
            {
                System.OperatingSystem osInfo = System.Environment.OSVersion;
                string str;
            }
            catch
            {

            }
        }

        private static void set_get_version_windows()
        {
            try
            {
                string _version = Environment.OSVersion.ToString();
            }
            catch
            {

            }
        }

        #region<LIMPIAR DBF>
        private static void _envia_inventario_planilla_clear(ref string _fecha_planilla, ref string _ingreso_inv)
        {
            string _FMSUCU02 = Variables._path_default + "\\fplani02.DBF";
            //_tienda;
            string sqlquery = "select max(i_fech) as i_fech from FPLANI02 where i_tane='X' and I_indi='2'";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0 && (!String.IsNullOrEmpty(dt.Rows[0]["i_fech"].ToString())))
                        {
                            //CONSULTAMOS LA PLANILA DE STOCK PARA VER SI ESTA CERRADA
                            DateTime _fecha_inv = Convert.ToDateTime(dt.Rows[0]["i_fech"]);
                            sqlquery = "select * from FPLANI02 where i_tane='X' and I_indi='2' and i_fech=?";
                            cmd = new OleDbCommand(sqlquery, cn);
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_inv;
                            da = new OleDbDataAdapter(cmd);
                            DataTable DT_FPLANI = new DataTable();
                            da.Fill(DT_FPLANI);

                            if (DT_FPLANI != null)
                            {
                                if (DT_FPLANI.Rows.Count > 0)
                                {
                                    DataTable dt_inventario = get_dt_inventario(DT_FPLANI);
                                    dt_inventario.TableName = "Inventario";

                                    //BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                                    //BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                                    //conexion.user_name = "emcomer";
                                    //conexion.user_password = "Bata2013";

                                    String _valida = "";// trans.ws_envia_stock_inv(conexion, _tienda, dt_inventario);

                                    //ahora validamos el dbf si grabo la planilla correcatmente
                                    if (_valida.Length == 0)
                                    {
                                        _fecha_planilla = _fecha_inv.ToString("dd/MM/yyyy");
                                        _ingreso_inv = "1";                                                                       
                                    }

                                }
                            }


                        }
                    }


                }
            }
            catch (Exception exc)
            {

            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();

        }
        private static void _enviar_movimiento_clear(ref String _error, string _fecha_plan = "", Int32 _envio_inv = 0)
        {
            string _FMSUCU02 = Variables._path_default + "\\FMSUCU02.DBF";
            string sqlquery = "select s_fpro from FMSUCU02 where s_csuc='" + Right(_tienda, 3) + "'";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            DataTable dt_fmc = null;
            DataTable dt_fmd = null;

            //clonar tablas
            DataTable dt_fmc_clone = null;
            DataTable dt_fmd_clone = null;

            DateTime _fecha_proceso_mov;
            string _FMC = "FMC";
            string _FMD = "FMD";
            try
            {
                cn = new OleDbConnection(Variables._conexion_vfpoledb);
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt);

                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            //EN ESTA LINEA VAMOS A CAPTURAR EL NOMBRE DEL ARCHIVO DEPENDE DE LA FECHA DE PROCESO
                            if (_fecha_plan.Length == 0)
                            {
                                _fecha_proceso_mov = Convert.ToDateTime(dt.Rows[0]["s_fpro"]);
                            }
                            else
                            {
                                _fecha_proceso_mov = Convert.ToDateTime(_fecha_plan);
                            }


                            string _ano = Right(_fecha_proceso_mov.ToString("yy"), 1);
                            string _mes = _fecha_proceso_mov.Month.ToString("D2");
                            _FMC = _FMC + _mes + _ano + Variables._codigo_empresa_tda;
                            _FMD = _FMD + _mes + _ano + Variables._codigo_empresa_tda;

                            //SI LA TABLA NO EXISTE ENTONCES LE HACEMOS UN RETURN
                            if (!File.Exists(Variables._path_default + "\\" + _FMC + ".dbf"))
                                return;

                            //*******************************************************UNA VES CAPTURADO
                            //VAMOS A VER    
                            //DateTime _ano_null =Convert.ToDateTime("30/12/1899");
                            if (_envio_inv == 1)
                            {
                                sqlquery = "SELECT v_tfor,v_proc,v_cfor,v_sfor,v_nfor,v_ffor,v_mone,v_tasa,v_almo," +
                                           "v_almd,v_tane,v_anex,v_tdoc,v_suna,v_sdoc,v_ndoc,v_fdoc,v_tref,v_sref,v_nref,v_tipo," +
                                           "v_arti,v_regl,v_colo,v_cant,v_pres,v_pred,v_vvts,v_vvtd,v_auto,v_ptot,v_impr,v_cuse," +
                                           "v_muse,v_fcre,v_fmod,v_ftrx,v_ctra,v_memo,v_motr,v_par1,v_par2,v_par3,v_lle1,v_lle2," +
                                           "v_lle3,v_tipe,v_ruc2,v_rzo2,'' as v_hstd FROM " + _FMC + " WHERE v_tane='X' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') " + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>'" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') "); //and (v_cfor='32' or v_cfor='31' )";
                            }
                            else
                            {
                                sqlquery = "SELECT v_tfor,v_proc,v_cfor,v_sfor,v_nfor,v_ffor,v_mone,v_tasa,v_almo," +
                                           "v_almd,v_tane,v_anex,v_tdoc,v_suna,v_sdoc,v_ndoc,v_fdoc,v_tref,v_sref,v_nref,v_tipo," +
                                           "v_arti,v_regl,v_colo,v_cant,v_pres,v_pred,v_vvts,v_vvtd,v_auto,v_ptot,v_impr,v_cuse," +
                                           "v_muse,v_fcre,v_fmod,v_ftrx,v_ctra,v_memo,v_motr,v_par1,v_par2,v_par3,v_lle1,v_lle2," +
                                           "v_lle3,v_tipe,v_ruc2,v_rzo2,'' as v_hstd FROM " + _FMC + " WHERE v_tane='X' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') " + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>='" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI') "); //and (v_cfor='32' or v_cfor='31' )";
                            }
                            cmd = new OleDbCommand(sqlquery, cn);
                            cmd.CommandTimeout = 0;
                            da = new OleDbDataAdapter(cmd);
                            dt_fmc = new DataTable();
                            if (!_ejecute_reindex_dbf())
                            {
                                //insertar cabecera del movimiento                             
                                da.Fill(dt_fmc);
                                //ahora vamo a verificar el detalle

                                if (_envio_inv == 1)
                                {
                                    sqlquery = "SELECT * FROM " + _FMC + " INNER JOIN " + _FMD +
                                       " ON v_tfor=i_tfor AND v_cfor=i_cfor  AND v_sfor=i_sfor  AND v_nfor=i_nfor  WHERE v_tane='X'" + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>'" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI')"); //and (v_cfor='32' or v_cfor='31' )";
                                }
                                else
                                {
                                    sqlquery = "SELECT * FROM " + _FMC + " INNER JOIN " + _FMD +
                                       " ON v_tfor=i_tfor AND v_cfor=i_cfor  AND v_sfor=i_sfor  AND v_nfor=i_nfor  WHERE v_tane='X'" + ((_fecha_plan.Length == 0) ? "" : " and DTOS(V_ffor)>='" + _fecha_proceso_mov.ToString("yyyyMMdd") + "' and (v_cfor!='01' and v_cfor!='03' and v_cfor!='NB' and v_cfor!='NC' and v_cfor!='ND' and v_cfor!='NF' and v_cfor!='TI')"); //and (v_cfor='32' or v_cfor='31' )";
                                }

                                cmd = new OleDbCommand(sqlquery, cn);
                                cmd.CommandTimeout = 0;
                                da = new OleDbDataAdapter(cmd);
                                dt_fmd = new DataTable();
                                if (!_ejecute_reindex_dbf())
                                {
                                    da.Fill(dt_fmd);
                                }

                                if (dt_fmc != null && dt_fmd != null)
                                {
                                    dt_fmc_clone = dt_fmc.Clone();
                                    dt_fmd_clone = dt_fmd.Clone();

                                    foreach (DataRow _fila_cab in dt_fmc.Rows)
                                    {
                                        string _v_tfor = _fila_cab["v_tfor"].ToString();
                                        string _v_cfor = _fila_cab["v_cfor"].ToString();
                                        string _v_sfor = _fila_cab["v_sfor"].ToString();
                                        string _v_nfor = _fila_cab["v_nfor"].ToString();


                                        DataRow[] _existe_vdetalle = dt_fmd.Select("v_tfor='" + _v_tfor + "' and v_cfor='" + _v_cfor + "' and v_sfor='" + _v_sfor + "' and v_nfor='" + _v_nfor + "'");

                                        if (_existe_vdetalle.Length > 0)
                                        {
                                            dt_fmc_clone.ImportRow(_fila_cab);
                                            for (Int32 i = 0; i < _existe_vdetalle.Length; ++i)
                                            {
                                                dt_fmd_clone.ImportRow(_existe_vdetalle[i]);
                                            }
                                        }
                                    }
                                    //EN ESTE CASO VAMOS A QUITAR COLUMNAS SIEMPRE CUANO FILA SEA MAYOR A CERO
                                    if (dt_fmd.Rows.Count > 0)
                                    {
                                        #region<BORRAMOS COLUMNAS CON INICIO DE CAMPO V PORQUE EN INNER SELECT FMD_CAB Y FMD_DET>                    
                                        List<DataColumn> columnsToDelete = new List<DataColumn>();
                                        foreach (DataColumn col in dt_fmd_clone.Columns)
                                        {
                                            if (Left(col.ColumnName, 1).ToUpper() == "V".ToUpper())
                                                columnsToDelete.Add(col);
                                        }

                                        foreach (DataColumn col in columnsToDelete)
                                            dt_fmd_clone.Columns.Remove(col);
                                        #endregion
                                    }


                                    #region<AHORA CAPTURAMOS EL DATATABLE DE MOV CAB Y DET>
                                    dt_fmc_clone.TableName = "FMC";
                                    dt_fmd_clone.TableName = "FMD";
                                    DataSet ds = new DataSet();
                                    ds.Tables.Add(dt_fmc_clone);
                                    ds.Tables.Add(dt_fmd_clone);
                                    //EN ESTE VERIFICAMOS QUE EXISTA DATOS PARA ENVIAR POR LA WEB SERVICE
                                    //TANTO VALIDA NULL Y FILAS >0
                                    if (dt_fmc_clone != null && dt_fmd_clone != null)
                                    {
                                        if (dt_fmc_clone.Rows.Count > 0 && dt_fmd_clone.Rows.Count > 0)
                                        {
                                            BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                                            BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                                            conexion.user_name = "emcomer";
                                            conexion.user_password = "Bata2013";

                                            // if (_tienda == "50140")
                                            // { 
                                            trans.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
                                            // }


                                            //verificar el numero de columna de 26 en el detalle

                                            if (dt_fmd_clone.Columns.Count == 31)
                                            {
                                                dt_fmd_clone.Columns.RemoveAt(30);
                                                dt_fmd_clone.Columns.RemoveAt(29);
                                                dt_fmd_clone.Columns.RemoveAt(28);
                                                dt_fmd_clone.Columns.RemoveAt(27);
                                                dt_fmd_clone.Columns.RemoveAt(26);

                                            }



                                            String _valida = ""; //trans.ws_genera_mov(conexion, "T", _tienda, dt_fmc_clone, dt_fmd_clone);

                                            //***ESTE CODIGO ES PARA VALIDAR EN EL DBF Y HACER UN UPDATE A LA TABLA DE FMC
                                            if (_valida.Length == 0)
                                            {
                                                DateTime _fecha_act = DateTime.Today;
                                                for (Int32 i = 0; i < dt_fmc_clone.Rows.Count; ++i)
                                                {
                                                    string v_tfor = dt_fmc_clone.Rows[i]["v_tfor"].ToString();
                                                    string v_cfor = dt_fmc_clone.Rows[i]["v_cfor"].ToString();
                                                    string v_sfor = dt_fmc_clone.Rows[i]["v_sfor"].ToString();
                                                    string v_nfor = dt_fmc_clone.Rows[i]["v_nfor"].ToString();
                                                    sqlquery = "update " + _FMC + " set v_tane=? where v_tfor='" + v_tfor + "' and v_cfor='" + v_cfor + "' " +
                                                               " and v_sfor='" + v_sfor + "' and v_nfor='" + v_nfor + "'";

                                                    cmd = new OleDbCommand(sqlquery, cn);
                                                    cmd.CommandTimeout = 0;
                                                    cmd.Parameters.Add("@v_tane", OleDbType.Char).Value = "";
                                                    if (cn.State == 0) cn.Open();
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                            else
                                            {
                                                _error = _valida;
                                            }
                                        }
                                        else
                                        {
                                            // _error = "NO HAY DATOS";
                                        }
                                    }


                                    #endregion
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception exc)
            {
                _error = exc.Message;
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
            }
            if (cn.State == ConnectionState.Open) cn.Close();
        }
        #endregion
        private static void _envia_inventario_planilla(ref string _fecha_planilla,ref string _ingreso_inv,ref string _error_planilla)
        {
            string _FMSUCU02 = Variables._path_default + "\\fplani02.DBF";
            //_tienda;
            string sqlquery = "select max(i_fech) as i_fech from FPLANI02 where i_tane IS NULL and I_indi='2'";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt);
                    if (dt!=null)
                    {
                        if (dt.Rows.Count>0 &&   (!String.IsNullOrEmpty(dt.Rows[0]["i_fech"].ToString())))
                        {
                            //CONSULTAMOS LA PLANILA DE STOCK PARA VER SI ESTA CERRADA
                            DateTime _fecha_inv = Convert.ToDateTime(dt.Rows[0]["i_fech"]);
                            sqlquery = "select * from FPLANI02 where i_tane IS NULL and I_indi='2' and i_fech=?";                            
                            cmd = new OleDbCommand(sqlquery, cn);
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_inv;
                            da = new OleDbDataAdapter(cmd);
                            DataTable DT_FPLANI  = new DataTable();                            
                            da.Fill(DT_FPLANI);                            

                            if (DT_FPLANI!=null)
                            {
                                if (DT_FPLANI.Rows.Count>0)
                                {
                                    DataTable dt_inventario = get_dt_inventario(DT_FPLANI);
                                    dt_inventario.TableName = "Inventario";

                                    BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                                    BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                                    conexion.user_name = "emcomer";
                                    conexion.user_password = "Bata2013";
                                    trans.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);

                                    String _valida = trans.ws_envia_stock_inv(conexion, _tienda,dt_inventario);

                                    //ahora validamos el dbf si grabo la planilla correcatmente
                                    if (_valida.Length==0)
                                    {
                                        _fecha_planilla = _fecha_inv.ToString("dd/MM/yyyy");
                                        _ingreso_inv = "1";
                                        ////editamos la tabla de stock de planilla
                                        //sqlquery = "UPDATE FPLANI02 SET i_tane='X' where i_tane IS NULL and I_indi='2'";                                        
                                        //cmd = new OleDbCommand(sqlquery, cn);
                                        //if (cn.State == 0) cn.Open();
                                        //cmd.CommandTimeout = 0;
                                        //cmd.CommandType = CommandType.Text;                                        
                                        //cmd.ExecuteNonQuery();                                        
                                    }

                                }
                            }


                        }
                    }


                }
            }
            catch(Exception exc)
            {
                _error_planilla ="problema con planilla" + exc.Message;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();

        }

        private static DataTable get_dt_inventario(DataTable dt)
        {
            DataTable dtplan = null;
            try
            {
                dtplan = new DataTable();
                dtplan.Columns.Add("fec_inv", typeof(DateTime));
                dtplan.Columns.Add("cod_alm", typeof(string));
                dtplan.Columns.Add("cod_producto", typeof(string));
                dtplan.Columns.Add("cod_calidad", typeof(string));
                dtplan.Columns.Add("cod_talla", typeof(string));
                dtplan.Columns.Add("can_inv", typeof(decimal));

                for(Int32 i=0;i<dt.Rows.Count;++i)
                {
                    DateTime _fec_inv=Convert.ToDateTime(dt.Rows[i]["i_fech"]);                    
                    string _cod_producto=Basico.Left(dt.Rows[i]["i_arti"].ToString().Trim(),7);
                    string _cod_calidad= Basico.Right(dt.Rows[i]["i_arti"].ToString().Trim(), 1);
                    string _cod_talla= dt.Rows[i]["i_regl"].ToString().Trim();
                    decimal _can_inv=Convert.ToDecimal(dt.Rows[i]["i_cant"]);

                    dtplan.Rows.Add(_fec_inv, _tienda, _cod_producto, _cod_calidad, _cod_talla, _can_inv);
                }

            }
            catch
            {
                dtplan = null;
            }
            return dtplan;
        }

        //diferencia de meses 
        private static int CalcularMesesDeDiferencia(DateTime fechaDesde, DateTime fechaHasta)
        {
            return Math.Abs((fechaDesde.Month - fechaHasta.Month) + 12 * (fechaDesde.Year - fechaHasta.Year));
        }
        private static void _update_version_gerena_hash()
        {
            try
            {

                /*EN ESTE CASO SOLO ENCOMER SE ACTUALIZA EL HASH*/
                /*VERIFICA EL CERTIFICADO*/

                if (Environment.GetEnvironmentVariable("codtda") != null)
                    _tienda = "50" + Environment.GetEnvironmentVariable("codtda").ToString();
                if (_tienda == null || _tienda.Length == 0) return;

                string _certificado_ruta = @"D:\INTERFA\CARVAJAL\bata_proceso\Certificado\CDBATA.pfx";

                if (!File.Exists(_certificado_ruta)) return;                

                string _assembly = "D:\\INTERFA\\CARVAJAL\\bata_proceso";//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string _modulo_Hash_ser = "Modulo_Hash.dll";
                string _genera_Hash_ser = "Genera_Hash_Xml.exe";
                string _genera_Hash_exe_ser = "Genera_Hash_Xml.exe.config";

                string _Gma_QrCodeNet_Encoding = "Gma.QrCodeNet.Encoding.dll";

                string _path_ser_mod_hash = _assembly + "\\" + _modulo_Hash_ser;
                string _path_ser_gen_hash = _assembly + "\\" + _genera_Hash_ser;
                string _path_ser_gen_hash_exe = _assembly + "\\" + _genera_Hash_exe_ser;

                string _path_Gma_QrCodeNet_Encoding = _assembly + "\\" + _Gma_QrCodeNet_Encoding;



                string _valida_dll = _assembly + "//validadll_mod.txt";

                if (File.Exists(_path_ser_mod_hash))
                { 

                        var fvi = FileVersionInfo.GetVersionInfo(_path_ser_mod_hash);
                        var version = fvi.FileVersion;

                        BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                        BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                        conexion.user_name = "emcomer";
                        conexion.user_password = "Bata2013";

                        Boolean _valida_version = updateversion.ws_existe_genera_hash_version(conexion, version);

                        Byte[] _Genera_Hash_Xml =null;
                        Byte[] _Genera_Hash_Xml_exe =null;
                        Byte[] _Modulo_Hash_dll = null;
                        Byte[] _QrCodeNet_Encoding_dll = null;

                    if (_valida_version)
                        {

                            _Genera_Hash_Xml = updateversion.ws_dll_service_tda(conexion, _genera_Hash_ser);
                            _Genera_Hash_Xml_exe = updateversion.ws_dll_service_tda(conexion, _genera_Hash_exe_ser);
                            _Modulo_Hash_dll = updateversion.ws_dll_service_tda(conexion, _modulo_Hash_ser);
                            _QrCodeNet_Encoding_dll= updateversion.ws_dll_service_tda(conexion, _Gma_QrCodeNet_Encoding);
                        /*extrae el archivo para modificar*/
                        if (_Genera_Hash_Xml!=null && _Genera_Hash_Xml_exe!=null && _Modulo_Hash_dll!=null && _QrCodeNet_Encoding_dll!=null)
                            {
                                if (!File.Exists(_valida_dll))
                                {
                                    TextWriter tw = new StreamWriter(@_valida_dll, true);
                                    //tw.WriteLine(_error);
                                    tw.Flush();
                                    tw.Close();
                                    tw.Dispose();

                                    //File.Create(_valida_dll);
                                }
                                
                                deshabilitando_servicio_FE();
                                System.IO.File.WriteAllBytes(_path_ser_gen_hash, _Genera_Hash_Xml);
                                System.IO.File.WriteAllBytes(_path_ser_gen_hash_exe, _Genera_Hash_Xml_exe);
                                System.IO.File.WriteAllBytes(_path_ser_mod_hash, _Modulo_Hash_dll);
                                System.IO.File.WriteAllBytes(_path_Gma_QrCodeNet_Encoding, _QrCodeNet_Encoding_dll);

                            habilitando_servicio_FE();
                                FileInfo infofile_pro = new FileInfo(@_path_ser_mod_hash);
                                var fvi_pro = FileVersionInfo.GetVersionInfo(@_path_ser_mod_hash);
                                var version_pro = fvi_pro.FileVersion;
                                string _act = updateversion.ws_update_versiondll_net(conexion, _tienda, infofile_pro.Name, version_pro);

                                if (File.Exists(_valida_dll))
                                {
                                    File.Delete(_valida_dll);
                                }

                            
                            }
                                                    
                        }
                        else
                        {
                            /*si este archivo existe entonces quiere decir que paso un error en el proceso de copiar*/
                            if (File.Exists(_valida_dll))
                            {
                                _Genera_Hash_Xml = updateversion.ws_dll_service_tda(conexion, _genera_Hash_ser);
                                _Genera_Hash_Xml_exe = updateversion.ws_dll_service_tda(conexion, _genera_Hash_exe_ser);
                                _Modulo_Hash_dll = updateversion.ws_dll_service_tda(conexion, _modulo_Hash_ser);

                                _QrCodeNet_Encoding_dll = updateversion.ws_dll_service_tda(conexion, _Gma_QrCodeNet_Encoding);

                            if (_Genera_Hash_Xml != null && _Genera_Hash_Xml_exe != null && _Modulo_Hash_dll != null && _QrCodeNet_Encoding_dll!=null)
                                {
                                    deshabilitando_servicio_FE();
                                    System.IO.File.WriteAllBytes(_path_ser_gen_hash, _Genera_Hash_Xml);
                                    System.IO.File.WriteAllBytes(_path_ser_gen_hash_exe, _Genera_Hash_Xml_exe);
                                    System.IO.File.WriteAllBytes(_path_ser_mod_hash, _Modulo_Hash_dll);
                                    System.IO.File.WriteAllBytes(_path_Gma_QrCodeNet_Encoding, _QrCodeNet_Encoding_dll);

                                habilitando_servicio_FE();
                                    FileInfo infofile_pro = new FileInfo(@_path_ser_mod_hash);
                                    var fvi_pro = FileVersionInfo.GetVersionInfo(@_path_ser_mod_hash);
                                    var version_pro = fvi_pro.FileVersion;
                                    string _act = updateversion.ws_update_versiondll_net(conexion, _tienda, infofile_pro.Name, version_pro);

                                    if (File.Exists(_valida_dll))
                                    {
                                        File.Delete(_valida_dll);
                                    }
                                }
                            }   
                        }
                }

            }
            catch(Exception exc)
            {
                habilitando_servicio_FE();
            }
        }

        private static string _update_venta_empl(string _Tip_Id_Ven, string _Nro_Dni_Ven, string _Cod_Tda_Ven,
                                                  string _Nro_Doc_Ven, string _Tip_Doc_Ven, string _Ser_Doc_Ven,
                                                  string _Num_Doc_Ven, string _Fec_Doc_Ven, string _Est_Doc_Ven,
                                                  string _Fc_Nin_Ven)
        {
            string _error = "";
            try
            {
                BataTransmision.bata_transaccionSoapClient ws_update = new BataTransmision.bata_transaccionSoapClient();
                _error = ws_update.ws_update_venta_empl(_Tip_Id_Ven, _Nro_Dni_Ven, _Cod_Tda_Ven,
                                                      _Nro_Doc_Ven, _Tip_Doc_Ven, _Ser_Doc_Ven,
                                                      _Num_Doc_Ven, _Fec_Doc_Ven, _Est_Doc_Ven,
                                                      _Fc_Nin_Ven);
            }
            catch (Exception exc)
            {
                _error = exc.Message;
            }
            return _error;
        }

        private static string _update_vales(string _serie, string _correlativo, string _cod_tda_venta, string _dni_venta,
           string _nombres_venta, string _fecha_doc, string _tipo_doc, string _serie_doc, string _numero_doc,
           string _estado_doc, string _fc_nint, string _email_venta, string _telefono_venta)
        {
            string _error = "";
            try
            {
                BataTransmision.bata_transaccionSoapClient ws_vales = new BataTransmision.bata_transaccionSoapClient();
                _error = ws_vales.ws_update_vales(_serie, _correlativo, _cod_tda_venta, _dni_venta,
                                                  _nombres_venta, _fecha_doc, _tipo_doc, _serie_doc, _numero_doc,
                                                  _estado_doc, _fc_nint, _email_venta, _telefono_venta);
            }
            catch (Exception exc)
            {
                _error = exc.Message;
            }
            return _error;
        }
        private static Boolean get_tel_email_in(String file_in, ref string _dni, ref string _nombres, ref string _telefono, ref string _email)
        {
            Boolean _valida = false;
            try
            {
                if (File.Exists(@file_in))
                {
                    string line;
                    using (StreamReader sr = new StreamReader(@file_in))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {

                            string[] split = line.Split(new Char[] { ',' });
                            _dni = split[4].ToString().Trim();
                            _nombres = split[6].ToString().Trim() + ", " + split[5].ToString().Trim();
                            _telefono = split[9].ToString().Trim();
                            _email = split[10].ToString().Trim();
                            if (_dni.Length>0)
                            { 
                                _valida = true;
                                return _valida;
                            }
                        }
                    }

                }
            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }
        private static void telefono_email_clienteV(string _fc_nint, ref string _emai, ref string telefono)
        {
            string sqlquery = "select FC_LCON,FC_RUC from FFACTC02 where fc_nint='" + _fc_nint + "'";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt = null;
            string _path_default = @"D:\POS";
            //conexion
            string _conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _path_default + ";Extended Properties=dBASE IV;";
            try
            {
                cn = new OleDbConnection(_conexion);
                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        telefono = dt.Rows[0]["FC_LCON"].ToString();
                        String _dni = dt.Rows[0]["FC_RUC"].ToString();
                        sqlquery = "select fc_mail from FMACLI02 where fc_ruc='" + _dni + "'";
                        cmd = new OleDbCommand(sqlquery, cn);
                        cmd.CommandTimeout = 0;
                        da = new OleDbDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                _emai = dt.Rows[0]["fc_mail"].ToString();
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {

            }
        }

        private static Boolean captura_data_dbf_in(string _serie, string _numero, ref string _dni, ref string _nombres,
                                            ref string _telefono, ref string _email)
        {
            Boolean _valida = false;
            BASICO.DBF.NET.BASICO_DBFNET select_table = null;
            try
            {
                select_table = new BASICO.DBF.NET.BASICO_DBFNET();
                select_table.tabla = "TEMPIN";
                DataTable dt = select_table.selectrow(_serie, _numero);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        _dni = dt.Rows[0]["DNI"].ToString();
                        _nombres = dt.Rows[0]["APEPAT"].ToString() + " " + dt.Rows[0]["APEMAT"].ToString() + ", " + dt.Rows[0]["NOMBRES"].ToString();
                        _telefono = dt.Rows[0]["TELEFONO"].ToString();
                        _email = dt.Rows[0]["EMAIL"].ToString();
                        _valida = true;
                    }
                }

            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }

        private static void actualizar_vale()
        {
            try
            {
                string _ruta_txt_out = @"D:\Cons\Vales\out";
                string _ruta_txt_in = @"D:\Cons\Vales\in";
                if (Directory.Exists(@_ruta_txt_out))
                {
                    string[] _archivos_txt = Directory.GetFiles(@_ruta_txt_out, "*.txt");
                    if (_archivos_txt.Length > 0)
                    {
                        for (Int32 a = 0; a < _archivos_txt.Length; ++a)
                        {
                            string _archivo = _archivos_txt[a].ToString();
                            string _nombrearchivo_txt = System.IO.Path.GetFileNameWithoutExtension(@_archivo);
                            string _rutaarchivo_in = @_ruta_txt_in + "\\" + _nombrearchivo_txt + ".txt";

                            //string _ven_dni = "";
                            //string _ven_nombre = "";
                            //string _ven_email = "";
                            //string _ven_telefono = "";
                            //get_tel_email_in(_rutaarchivo_in, ref _ven_dni, ref _ven_nombre, ref _ven_email, ref _ven_telefono);

                            int counter = 0;
                            string line;

                            string _error = ""; Int32 _ingreso_data = 0;
                            // Read the file and display it line by line.
                            using (StreamReader sr = new StreamReader(@_archivo))
                            {

                                try
                                {

                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        string _cadena = "";
                                        Int32 _fecha_int = 0;
                                        if (line.Length > 0)
                                        {
                                            Int32 _abierto = 0;
                                            //quitar las , dentro de un cadena
                                            for (Int32 i = 0; i < line.Length; ++i)
                                            {
                                                if (_abierto == 0 && line.Substring(i, 1) == "," && _fecha_int != 2)
                                                {
                                                    _cadena += line.Substring(i, 1);
                                                }

                                                if (line.Substring(i, 1) == "\"")
                                                {
                                                    _abierto += 1;
                                                }

                                                if (line.Substring(i, 1) != "," && _abierto >= 1)
                                                {
                                                    _cadena += line.Substring(i, 1);
                                                }

                                                if (_fecha_int == 2)
                                                {
                                                    _cadena += line.Substring(i, 1);
                                                }

                                                if (line.Substring(i, 1) == ",")
                                                {
                                                    _fecha_int += 1;

                                                }

                                                if (_abierto == 2) _abierto = 0;

                                            }
                                        }

                                        _cadena = _cadena.Replace("\"", "");

                                        if (_nombrearchivo_txt == "000051" || _nombrearchivo_txt == "000055")
                                            if (_cadena.Trim().Length == 0) break;

                                        string[] split = _cadena.Split(new Char[] { ',' });
                                        string _serie = "";
                                        string _numero = "";
                                        _serie = split[0].ToString(); _numero = split[1].ToString();


                                        string _dni_venta, _nombres_venta, _tipo_doc, _serie_doc, _numero_doc, _estado_doc, _fecha_doc;

                                        if (_serie.Trim().Length > 0)
                                        {
                                            _ingreso_data = 1;
                                            _fecha_doc = split[2].ToString();
                                            _tipo_doc = split[3].ToString().Trim();
                                            _serie_doc = split[4].ToString().Trim();
                                            _numero_doc = split[5].ToString().Trim();
                                            _dni_venta = split[6].ToString().Trim();
                                            _nombres_venta = split[7].ToString().Trim();
                                            _estado_doc = split[8].ToString().Trim();
                                            _tienda = "50" + split[9].ToString().Trim();

                                            string _fc_nint = "";// (_estado_doc == "A") ? "" : split[10].ToString().Trim();
                                            if (split.Count() > 10)
                                            {
                                                _fc_nint = (_estado_doc == "A") ? "" : split[10].ToString().Trim();
                                            }


                                            //string _fc_nint=(_estado_doc=="A")?"": split[10].ToString().Trim();
                                            string _emai_venta = "";
                                            string _telefono_venta = "";

                                            if (_serie == "302001" || _serie == "302003" || _serie == "000051" || _serie == "000055")
                                            {
                                                /*en este proceso vamos a capturar el archivo dbf cuando se genero en el in*/
                                                if (!captura_data_dbf_in(_serie, _numero, ref _dni_venta, ref _nombres_venta, ref _telefono_venta, ref _emai_venta))
                                                {
                                                    if (!get_tel_email_in(_rutaarchivo_in, ref _dni_venta, ref _nombres_venta, ref _telefono_venta, ref _emai_venta))
                                                    {
                                                        telefono_email_clienteV(_fc_nint, ref _emai_venta, ref _telefono_venta);
                                                    }
                                                }
                                                _error = _update_vales(_serie.Trim().ToString(), (_serie != "000051" && _serie != "000055") ? _nombrearchivo_txt : _numero, _tienda, _dni_venta, _nombres_venta, _fecha_doc, _tipo_doc, _serie_doc, _numero_doc, _estado_doc, _fc_nint, _emai_venta, _telefono_venta);
                                            }
                                            else
                                            {
                                                //if (!get_tel_email_in(_rutaarchivo_in, ref _dni_venta, ref _nombres_venta, ref _telefono_venta, ref _emai_venta))
                                                //{
                                                //    telefono_email_clienteV(_fc_nint, ref _emai_venta, ref _telefono_venta);
                                                //}
                                                //_dni_venta = _nombrearchivo_txt;
                                                _error = _update_venta_empl(_serie, _dni_venta, _tienda, _serie_doc + _numero_doc, _tipo_doc, _serie_doc, _numero_doc, _fecha_doc, _estado_doc, _fc_nint);
                                            }

                                        }

                                        if (_serie != "000051" && _serie != "000055")
                                            if (_error.Length == 0) break;

                                        //System.Console.WriteLine(line);
                                        counter++;
                                    }
                                }
                                catch (Exception)
                                {


                                }

                            }

                            //System.IO.StreamReader file =
                            //    new System.IO.StreamReader(@_archivo);


                            if (_error.Length == 0 && _ingreso_data == 1)
                            {
                                File.Delete(@_archivo);

                                if (File.Exists(@_rutaarchivo_in))
                                {
                                    File.Delete(@_rutaarchivo_in);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                //string _RUTA = @"D\ERROR.TXT";
                //TextWriter tw = new StreamWriter(_RUTA, true);
                //tw.WriteLine(exc.Message);
                //tw.Flush();
                //tw.Close();
                //tw.Dispose();
            }
        }

        private static void elimina_tblock()
        {
            try
            {
                string _dbf_block = Variables._path_default + "\\tBLOCK.DBF";

                if (File.Exists(_dbf_block))
                {
                    DateTime fec_creacion= File.GetCreationTime(@_dbf_block);

                    DateTime fec_actual = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

                    TimeSpan dif = fec_actual - fec_creacion;

                    Int32 mindif =Convert.ToInt32(dif.TotalMinutes);

                    if (mindif>=10)
                    {
                        File.Delete(@_dbf_block);
                    }

                }
            }
            catch (Exception)
            {
                                
            }
        }

        public static void _genera_transmision(ref string _error)
        {
            try
            {
                /*elimina si diferente a 10 minutos*/
                elimina_tblock();
                /********************************/
                _register_vfpoledb();

                //return;

                //recepcion_guias_alma();
                //modificar url de windows actualizable
                //_espera_ejecuta(60);
                _update_ws_win_actualiza();

                //ACTUALIZAR EXE DE WIN UPDATE
                _update_version_winupdateexe();

                /**/
                /*enviar stock*/
                envio_stock();
                /**/
                #region<INVOCAR WEB SERVICE NUBE POS>
                conexion_service_nube();
                #endregion
                //***actualizar si ha alguna version nueva de la ddl basico
                _update_version_serviowin();
                //*****************************************
                actualizar_vale();
                //****UPDATE DE GENERA HASH
                _update_version_gerena_hash();
                /**/
                //**version del windows 
                set_get_version_windows();
                //***actualizar dll vfpoledb.dll
                //version sistema operativo
                update_get_version_so();

                /*ACTUALIZAR FEPE DLL*/
                //update_archivo_fepe_dll();
                
                //if (_tienda!="50143")
                //{ 
                if (!_valida_tda_ecu())
                    update_archivo_carvajal();
                //}

                #region<RECEPCIONDE GUIAS DE ALMACEN HACIA TIENDA>

                recepcion_guias_alma("");

                #endregion

                //CERTIFICADO ELECTRONICO
                update_archivo_certificado();

                //*VAMOS ACTUALIZAR LA WEB SERVICE DE LA FACTURACION ELECTRONICA
                _update_ws_facturacion_electronica();
                //****
                //**ACTUALIZAR URL WEB SERVICE TRANSMISION NET
                _update_ws_transmision();

                //*ejecutar cada 60 segundos el rpceso
                
                //**VAMO

                //*****************************************************************************
                //VERIFICAR SI SE ESTA REINDEXANDO LOS DBF
                if (!_ejecute_reindex_dbf())
                {
                    _dbftienda();

                    /*ACTUAIZAR LA DLL DEL FOX*/
                    _update_version_vfpoledbdll();

                    /*ACTUALIZAR CUPONES DE TDAS*/
                    //actualizar_vale();

                    #region<ENVIO DE STOCK DE PLANILLLA Y MOVIMIENTO>
                    //_clear_mov_dbf();
                    // _envia_transaccion_mov();
                    #endregion
                    /*ENVIA PAQ DE TRASMISION EN TX*/
                    envia_paq_tx();
                    /******************************/
                    //en esta opcion vamos a enviar el archivo de movimiento

                    ////****************
                    ///**RECEPCIONA TRANSITO O DESPACHO DE OTRA TIENDA-ALMACEN
                    ///
                    //_recepcion_movimiento();

                    _fecha_tda = DateTime.Today.AddDays(-7);//.AddDays(-);//.AddDays(1);//.AddDays(-5);

                    //verificar fecha de procesos
                    if (!_ejecute_reindex_dbf())
                    {
                        DataTable dt = _dt_fecha_venta();

                                if (dt != null)
                                {
                                        if (dt.Rows.Count > 0)
                                        {
                                            if (!_ejecute_reindex_dbf())
                                            { 
                                                   
                                                    for (Int32 i = 0; i < dt.Rows.Count; ++i)
                                                    {
                                                        DateTime _fecha_proceso = Convert.ToDateTime(dt.Rows[i]["fc_ffac"]);
                                                        if (!_valida_archivo_proceso(_fecha_proceso))
                                                        {
                                                            DataTable dtventa_envio = null;
                                                            _crearfolder_limpiar();

                                                            string _error_venta = "";
                                                            
                                                            _genera_ventas(_fecha_proceso, ref dtventa_envio,ref _error_venta);
                                                            //genera archivos y lo convierte en BYTE[]
                                                            #region<PROCDIMIENTO PARA CONVERTIR EN BYTES Y ENVIAR WEB SERVICE> 

                                                            //VERIFICAR SI HAY DATOS PARA ENVIAR AL SERVIDOR
                                                            string[] _file = Directory.GetFiles(@Variables._path_envia, "*.*");
                                                            if (_file.Length > 0)
                                                            {
                                                                string _name_archivo = "";
                                                                byte[] _archivo = _comprimir_archivo(_fecha_proceso, ref _name_archivo);
                                                                BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                                                                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                                                                conexion.user_name = "emcomer";
                                                                conexion.user_password = "Bata2013";

                                                                String[] _mensaje = trans.ws_transmision_ingreso_SQL(conexion, _archivo, _name_archivo);

                                                                //si la transmision es exitosa entonces vamos a modificar al campo FC_FTX de la tabla FFACTC02
                                                                if (_mensaje[0].ToString() == "1" && _error_venta.Length==0)
                                                                {
                                                                    //vamos a modificar el estado
                                                                    _editestadoventas(dtventa_envio);
                                                                }
                                                            }
                                                          }
                                                    #endregion
                                                    }
                                                 }
                                                }
                                }
                     }
                }
                ////ACTUALIZAR EXE DE WIN UPDATE
                //_update_version_winupdateexe();

            }
            catch (Exception exc)
            {
                _error = exc.Message;
                //sis necesitas poner un mensaje debes de poner throw para que te capture
            }
        }

        #region<ENVIO DE TRANSACCION DE MOVIMIENTOS>
        private static Boolean _tda_inv_activo()
        {
            Boolean _activo = false;
            try
            {                
                BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                _activo =trans.ws_tienda_inv(conexion, _tienda);
            }
            catch
            {
                _activo = false;
            }
            return _activo;
        }

        private static void _clear_mov_dbf()
        {
            try
            {
                if (_tda_inv_activo())
                //if (_tienda == "50140" || _tienda == "50143" || _tienda == "50147")
                {

                    //verificar si la planillla fue enviado
                    string _fecha_planilla = "";
                    string _ingreso_inv = "";
                    string _error = "";
                    //ACA VAMOS A ENVIAR E INVENTARIO DE PLANILLA
                    _envia_inventario_planilla_clear(ref _fecha_planilla, ref _ingreso_inv);

                    if (_fecha_planilla.Length > 0)
                    {

                        DateTime _fecha_plan = Convert.ToDateTime(_fecha_planilla);
                        int _mese_dif = CalcularMesesDeDiferencia(_fecha_plan, DateTime.Today);
                        for (Int32 i = 0; i <= _mese_dif; ++i)
                        {
                            int _valor_inv = ((i == 0) ? 1 : 0);

                            _enviar_movimiento_clear(ref _error, _fecha_plan.ToString("dd/MM/yyyy"), _valor_inv);

                            _fecha_plan = new DateTime(_fecha_plan.Year, _fecha_plan.Month, DateTime.DaysInMonth(_fecha_plan.Year, _fecha_plan.Month));
                            _fecha_plan = _fecha_plan.AddDays(1);
                        }
                        //editamos la tabla de stock de planilla
                        if (_ingreso_inv == "1" && _error.Length == 0)
                        {
                            OleDbConnection cn = null;
                            OleDbCommand cmd = null;
                            String sqlquery = "UPDATE FPLANI02 SET i_tane='' where  I_indi='2'";
                            cn = new OleDbConnection(Variables._conexion);
                            cmd = new OleDbCommand(sqlquery, cn);
                            if (cn.State == 0) cn.Open();
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            if (cn.State == ConnectionState.Open) cn.Close();
                        }
                    }
                    else
                    {
                        _enviar_movimiento_clear(ref _error);
                    }

                    if (_error.Length == 0)
                    {
                        String _mensaje = "perfecto tda ==>" + _tienda;
                        BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                        BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                        conexion.user_name = "emcomer";
                        conexion.user_password = "Bata2013";

                        string _envia = trans.ws_error_mov_transac(conexion, _tienda, _mensaje);

                    }
                }

            }
            catch
            {

            }
        }

        private static void _envia_transaccion_mov()
        {
            try
            {
                if (_tda_inv_activo())
                //if (_tienda == "50140" || _tienda == "50143" || _tienda == "50147")
                {

                    //verificar si la planillla fue enviado
                    string _fecha_planilla = "";
                    string _ingreso_inv = "";
                    string _error = "";
                    //ACA VAMOS A ENVIAR E INVENTARIO DE PLANILLA
                    _envia_inventario_planilla(ref _fecha_planilla,ref _ingreso_inv,ref _error);

                    if (_fecha_planilla.Length > 0)
                    {
                        
                        DateTime _fecha_plan = Convert.ToDateTime(_fecha_planilla);
                        int _mese_dif = CalcularMesesDeDiferencia(_fecha_plan, DateTime.Today);
                        for (Int32 i = 0; i <= _mese_dif; ++i)
                        {
                            int _valor_inv = ((i == 0) ? 1 : 0);

                            _enviar_movimiento(ref _error,_fecha_plan.ToString("dd/MM/yyyy"), _valor_inv);

                            _fecha_plan = new DateTime(_fecha_plan.Year, _fecha_plan.Month, DateTime.DaysInMonth(_fecha_plan.Year, _fecha_plan.Month));
                            _fecha_plan = _fecha_plan.AddDays(1);
                        }
                        //editamos la tabla de stock de planilla
                        if (_ingreso_inv=="1" && _error.Length==0)
                        {
                            OleDbConnection cn = null;
                            OleDbCommand cmd = null;
                            String sqlquery = "UPDATE FPLANI02 SET i_tane='X' where i_tane IS NULL and I_indi='2'";
                            cn = new OleDbConnection(Variables._conexion);
                            cmd = new OleDbCommand(sqlquery, cn);
                            if (cn.State == 0) cn.Open();
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            if (cn.State ==ConnectionState.Open) cn.Close();
                        }
                    }
                    else
                    {
                        _enviar_movimiento(ref _error);
                    }

                    if (_error.Length>0)
                    {

                        BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();
                        BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                        conexion.user_name = "emcomer";
                        conexion.user_password = "Bata2013";

                        string _envia = trans.ws_error_mov_transac(conexion, _tienda, _error);

                    }
                }

            }
            catch
            {

            }
        }
        #endregion
        private static void _editestadoventas(DataTable dt)
        {
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            
            try
            {
                if (dt!=null)
                {
                    if (dt.Rows.Count>0)
                    {
                        cn = new OleDbConnection(Variables._conexion);
                        if (cn.State == 0) cn.Open();
                        for (Int32 i=0;i<dt.Rows.Count;++i)
                        {
                            string _codigo = dt.Rows[i]["FC_NINT"].ToString();
                            string _estado= dt.Rows[i]["FC_ESTA"].ToString();
                            string sqlquery = "";
                            if (_estado.Length==0)
                            {
                                sqlquery = "UPDATE FFACTC02 SET FC_FTX='X' WHERE FC_NINT='" + _codigo + "'  AND (FC_FTX IS NULL OR LEN(FC_FTX) = 0)";
                            }
                            else
                            {
                                sqlquery = "UPDATE FFACTC02 SET FC_FTX='X' WHERE FC_NINT='" + _codigo + "' AND FC_ESTA='" + _estado + "'";
                            }
                            //string sqlquery = "UPDATE FFACTC02 SET FC_FTX='X' WHERE FC_NINT='" + _codigo + "' AND FC_ESTA='" + _estado + "'";

                            //(FC_FTX IS NULL OR LEN(FC_FTX) = 0)

                            cmd = new OleDbCommand(sqlquery, cn);
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.Text;
                            if (!_ejecute_reindex_dbf())
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
        }
        public static byte[] _comprimir_archivo(DateTime _fecha_proceso, ref string _archivo)
        {
            byte[] _archivo_comp = null;
            ZipOutputStream zipOut = null;
            try
            {
                string _comprimir = Variables._path_envia + "\\Comp";

                if (!Directory.Exists(@_comprimir))
                    Directory.CreateDirectory(@_comprimir);
                        
                string _ano = _fecha_proceso.ToString("yy");
                string _mes = _fecha_proceso.Month.ToString("D2");
                string _dia = _fecha_proceso.Day.ToString("D2");
                string _fecha = _ano + _mes + _dia;
                String[] filenames =Directory.GetFiles(Variables._path_envia, "*.*");
                _archivo = "TD" + _fecha + "."  + Right(_tienda, 3);
                string ruta_zip = @_comprimir + "\\TD" + _fecha + "."  + Right(_tienda, 3);

                if (File.Exists(ruta_zip))
                {
                    File.Delete(ruta_zip);
                }

                //crear archivo zip
                zipOut = new ZipOutputStream(File.Create(@ruta_zip));

                //*********************               

                for (Int32 i = 0; i < filenames.Length; ++i)
                {
                    string _archivo_xml = filenames[i].ToString();
                    FileInfo fi = new FileInfo(_archivo_xml);
                    ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(fi.Name);
                    FileStream sReader = File.OpenRead(_archivo_xml);
                    byte[] buff = new byte[Convert.ToInt32(sReader.Length)];
                    sReader.Read(buff, 0, (int)sReader.Length);
                    entry.DateTime = fi.LastWriteTime;
                    entry.Size = sReader.Length;
                    sReader.Close();
                    zipOut.PutNextEntry(entry);
                    zipOut.Write(buff, 0, buff.Length);
                }

                zipOut.Finish();
                zipOut.Close();
                _crearfolder_limpiar();
                byte[] file = File.ReadAllBytes(ruta_zip);

                _archivo_comp = file;

                //si el archivo existe entonces lo eliminamos porque ya se transformo en bytes
                if (File.Exists(ruta_zip))
                {
                    File.Delete(ruta_zip);
                }

            }
            catch
            {
                if  (zipOut!=null)
                { 
                    zipOut.Finish();
                    zipOut.Close();
                }
                throw;
            }
            return _archivo_comp;
        }
        //
        private static void _genera_ventas(DateTime _fecha_proceso,ref DataTable dtventa_envio,ref string error_venta)
        {
                      
            string sqlquery = "";
            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt_cab = null;
            DataTable dt_det = null;
            DataTable dt_pago = null;

            //tablas clonadoras
            DataTable dt_cab_clone = null;
            DataTable dt_det_clone = null;
            DataTable dt_pago_clone = null;
            try
            {
                //limpiar carpeta temporal
                
                //

                //CONEXIONES OLEDB 
                cn = new OleDbConnection(Variables._conexion);
                //CABEZERA DE VENTAS SELECT DEL DIA ACTUAL
                sqlquery = "select * from " + Variables._venta_cab + " WHERE FC_FFAC=? AND (FC_FTX IS NULL OR LEN(FC_FTX)=0)";

                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_proceso;
                da = new OleDbDataAdapter(cmd);
                dt_cab = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt_cab);
                }
                //verificar si hay datos en la cabezera d la ventas
                if (dt_cab != null)
                {
                    if (dt_cab.Rows.Count > 0)
                    {
                        //poner llave en el table 
                        dt_cab.PrimaryKey = new DataColumn[] { dt_cab.Columns["fc_nint"] };
                        DataColumn[] columns = new DataColumn[1];
                        columns[0] = dt_cab.Columns["fc_nint"];
                        dt_cab.PrimaryKey = columns;


                        //**************************************************
                        //****DETALLE DE LE VENTA CAPTURAR
                        sqlquery = "select * from " + Variables._venta_cab + " inner join  " + Variables._venta_det + " on " + Variables._venta_cab + ".FC_NINT =" + Variables._venta_det + ".FD_NINT  WHERE " + Variables._venta_cab + ".FC_FFAC =? AND (FC_FTX IS NULL OR LEN(FC_FTX)=0)";
                        cmd = new OleDbCommand(sqlquery, cn);
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_proceso;
                        da = new OleDbDataAdapter(cmd);
                        dt_det = new DataTable();

                        if (!_ejecute_reindex_dbf())
                        {
                            da.Fill(dt_det);
                        }
                        //***FORMA DE PAGO DE LA VENTA
                        sqlquery = "select * from " + Variables._venta_cab + " inner join  " + Variables._venta_pago + " on " + Variables._venta_cab + ".FC_NNOT =" + Variables._venta_pago + ".NA_NOTA  WHERE " + Variables._venta_cab + ".FC_FFAC =? AND (FC_FTX IS NULL OR LEN(FC_FTX)=0)";
                        cmd = new OleDbCommand(sqlquery, cn);
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add("DATE", OleDbType.Date).Value = _fecha_proceso;
                        da = new OleDbDataAdapter(cmd);
                        dt_pago = new DataTable();
                        if (!_ejecute_reindex_dbf())
                        {
                            da.Fill(dt_pago);
                        }
                        //validacion que las ventas contengan sus detalles
                        //vamos a clonar sus estruturas la tabla dt_cab
                        dt_cab_clone = dt_cab.Clone();
                        dt_det_clone = dt_det.Clone();
                        dt_pago_clone = dt_pago.Clone();
                        foreach (DataRow _fila_cab in dt_cab.Rows)
                        {
                            string _fcnint = _fila_cab["FC_NINT"].ToString();
                            string _fcnnot= _fila_cab["FC_NNOT"].ToString();
                            DataRow[] _existe_vdetalle = dt_det.Select("FC_NINT='" + _fcnint + "'");
                            //si existe en el detalle copio tanto de cabezera y detalle
                            if (_existe_vdetalle.Length > 0)
                            {
                                dt_cab_clone.ImportRow(_fila_cab);
                                for (Int32 i = 0; i < _existe_vdetalle.Length; ++i)
                                {
                                    dt_det_clone.ImportRow(_existe_vdetalle[i]);
                                }

                                DataRow[] _existe_vpago = dt_pago.Select("FC_NNOT='" + _fcnnot + "'");
                                //vamos a ingresar la forma de pago
                                if (_existe_vpago.Length>0)
                                {
                                    for (Int32 i = 0; i < _existe_vpago.Length; ++i)
                                    {
                                       dt_pago_clone.ImportRow(_existe_vpago[i]);
                                    }
                                }
                            }


                        }

                                

                    }
                }
                //VAMOS A VERIFICAR SI ES LAS TABLA DE VENTAS CLONADORAS ESTAN CON DATA
                if (dt_cab_clone != null)
                {
                    if (dt_cab_clone.Rows.Count > 0)
                    {

                        tabla_FFACTC(dt_cab_clone);
                        dtventa_envio = dt_cab_clone;
                        #region<BORRAMOS COLUMNAS CON INICIO DE CAMPO FC PORQUE EN INNER SELECT CAB Y DET>                    
                        List<DataColumn> columnsToDelete = new List<DataColumn>();
                        foreach (DataColumn col in dt_det_clone.Columns)
                        {                            
                            if (Left(col.ColumnName,2).ToUpper()=="fc".ToUpper())
                                columnsToDelete.Add(col);
                        }

                        foreach (DataColumn col in columnsToDelete)
                            dt_det_clone.Columns.Remove(col);
                        #endregion

                        tabla_FFACTD(dt_det_clone);

                        //****AHORA VAMOS A VER LA FORMA DE PAGO
                        //**VERIFICAMOS SI HAY DATA EN LA FORMA DE PAGO
                        if (dt_pago_clone!=null)
                        { 
                            if (dt_pago_clone.Rows.Count>0)
                            {
                                columnsToDelete = new List<DataColumn>();
                                foreach (DataColumn col in dt_pago_clone.Columns)
                                {
                                    if (Left(col.ColumnName, 2).ToUpper() == "fc".ToUpper())
                                        columnsToDelete.Add(col);
                                }

                                foreach (DataColumn col in columnsToDelete)
                                    dt_pago_clone.Columns.Remove(col);

                                tabla_FNOTAA(dt_pago_clone);
                            }
                            else
                            {
                                columnsToDelete = new List<DataColumn>();
                                foreach (DataColumn col in dt_pago_clone.Columns)
                                {
                                    if (Left(col.ColumnName, 2).ToUpper() == "fc".ToUpper())
                                        columnsToDelete.Add(col);
                                }

                                foreach (DataColumn col in columnsToDelete)
                                    dt_pago_clone.Columns.Remove(col);
                            }
                        }

                        //DataSet ds_venta = new DataSet();
                        //ds_venta.Tables.Add(dt_cab_clone);
                        //ds_venta.Tables.Add(dt_det_clone);
                        //ds_venta.Tables.Add(dt_pago_clone);


                        #region <LISTA DE VENTA>                       
                        if (dt_cab_clone.Rows.Count>0)
                        {
                            #region<insertar ffactc>   
                            List<BataPos.Ent_Ffactc> result_ffactc = new List<BataPos.Ent_Ffactc>();
                            for (Int32 i = 0; i < dt_cab_clone.Rows.Count; ++i)
                            {
                                DateTime fec_cre = Convert.ToDateTime(dt_cab_clone.Rows[i]["fc_fcre"]);
                                DateTime fec_mod;
                                if (dt_cab_clone.Rows[i]["fc_fmod"] == DBNull.Value)
                                {
                                    fec_mod = fec_cre;
                                }
                                else
                                {
                                    fec_mod = Convert.ToDateTime(dt_cab_clone.Rows[i]["fc_fmod"]);
                                }

                               
                                    BataPos.Ent_Ffactc ffactc = new BataPos.Ent_Ffactc()
                                    {

                                        fc_nint = dt_cab_clone.Rows[i]["fc_nint"].ToString(),
                                        fc_nnot = dt_cab_clone.Rows[i]["fc_nnot"].ToString(),
                                        fc_codi = dt_cab_clone.Rows[i]["fc_codi"].ToString(),
                                        fc_suna = dt_cab_clone.Rows[i]["fc_suna"].ToString(),
                                        fc_sfac = dt_cab_clone.Rows[i]["fc_sfac"].ToString(),
                                        fc_nfac = dt_cab_clone.Rows[i]["fc_nfac"].ToString(),
                                        fc_ffac = Convert.ToDateTime(dt_cab_clone.Rows[i]["fc_ffac"].ToString()),
                                        fc_nord = dt_cab_clone.Rows[i]["fc_nord"].ToString(),
                                        fc_cref = dt_cab_clone.Rows[i]["fc_cref"].ToString(),
                                        fc_sref = dt_cab_clone.Rows[i]["fc_sref"].ToString(),
                                        fc_pvta = dt_cab_clone.Rows[i]["fc_pvta"].ToString(),
                                        fc_csuc = dt_cab_clone.Rows[i]["fc_csuc"].ToString(),
                                        fc_gvta = dt_cab_clone.Rows[i]["fc_gvta"].ToString(),
                                        fc_zona = dt_cab_clone.Rows[i]["fc_zona"].ToString(),
                                        fc_clie = dt_cab_clone.Rows[i]["fc_clie"].ToString(),
                                        fc_ncli = dt_cab_clone.Rows[i]["fc_ncli"].ToString(),
                                        fc_nomb = dt_cab_clone.Rows[i]["fc_nomb"].ToString(),
                                        fc_apep = dt_cab_clone.Rows[i]["fc_apep"].ToString(),
                                        fc_apem = dt_cab_clone.Rows[i]["fc_apem"].ToString(),
                                        fc_dcli = dt_cab_clone.Rows[i]["fc_dcli"].ToString(),
                                        fc_cubi = dt_cab_clone.Rows[i]["fc_cubi"].ToString(),
                                        fc_ruc = dt_cab_clone.Rows[i]["fc_ruc"].ToString(),
                                        fc_vuse = dt_cab_clone.Rows[i]["fc_vuse"].ToString(),
                                        fc_vend = dt_cab_clone.Rows[i]["fc_vend"].ToString(),
                                        fc_ipre = dt_cab_clone.Rows[i]["fc_ipre"].ToString(),
                                        fc_tint = dt_cab_clone.Rows[i]["fc_tint"].ToString(),
                                        fc_pint = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_pint"]),
                                        fc_lcsg = dt_cab_clone.Rows[i]["fc_lcsg"].ToString(),
                                        fc_ncon = dt_cab_clone.Rows[i]["fc_ncon"].ToString(),
                                        fc_dcon = dt_cab_clone.Rows[i]["fc_dcon"].ToString(),
                                        fc_lcon = dt_cab_clone.Rows[i]["fc_lcon"].ToString(),
                                        fc_lruc = dt_cab_clone.Rows[i]["fc_lruc"].ToString(),
                                        fc_agen = dt_cab_clone.Rows[i]["fc_agen"].ToString(),
                                        fc_mone = dt_cab_clone.Rows[i]["fc_mone"].ToString(),
                                        fc_tasa = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_tasa"]),
                                        fc_fpag = dt_cab_clone.Rows[i]["fc_fpag"].ToString(),
                                        fc_nlet = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_nlet"]),
                                        fc_qtot = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_qtot"]),
                                        fc_pref = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_pref"]),
                                        fc_dref = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_dref"]),
                                        fc_brut = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_brut"]),
                                        fc_vimp1 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vimp1"]),
                                        fc_vimp2 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vimp2"]),
                                        fc_vdct1 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vdct1"]),
                                        fc_vdct4 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vdct4"]),
                                        fc_pdc2 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_pdc2"]),
                                        fc_pdc3 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_pdc3"]),
                                        fc_vdc23 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vdc23"]),
                                        fc_vvta = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vvta"]),
                                        fc_vimp3 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vimp3"]),
                                        fc_pimp4 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_pimp4"]),
                                        fc_vimp4 = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_vimp4"]),
                                        fc_total = Convert.ToDecimal(dt_cab_clone.Rows[i]["fc_total"]),
                                        fc_esta = dt_cab_clone.Rows[i]["fc_esta"].ToString(),
                                        fc_tdoc = dt_cab_clone.Rows[i]["fc_tdoc"].ToString(),
                                        fc_cuse = dt_cab_clone.Rows[i]["fc_cuse"].ToString(),
                                        fc_muse = dt_cab_clone.Rows[i]["fc_muse"].ToString(),
                                        fc_fcre = fec_cre,//Convert.ToDateTime(dt_cab.Rows[i]["fc_fcre"]),
                                        fc_fmod = fec_mod,//Convert.ToDateTime(dt_cab.Rows[i]["fc_fmod"]),
                                        fc_hora = dt_cab_clone.Rows[i]["fc_hora"].ToString(),
                                        fc_auto = dt_cab_clone.Rows[i]["fc_auto"].ToString(),
                                        fc_ftx = dt_cab_clone.Rows[i]["fc_ftx"].ToString(),
                                        fc_estc = dt_cab_clone.Rows[i]["fc_estc"].ToString(),
                                        fc_sexo = dt_cab_clone.Rows[i]["fc_sexo"].ToString(),
                                        fc_mpub = dt_cab_clone.Rows[i]["fc_mpub"].ToString(),
                                        fc_edad = dt_cab_clone.Rows[i]["fc_edad"].ToString(),
                                        fc_regv = dt_cab_clone.Rows[i]["fc_regv"].ToString(),
                                    };
                                    result_ffactc.Add(ffactc);
                                                                                               
                            }
                            #endregion
                            #region<insertar ffactd>   
                            List<BataPos.Ent_Ffactd> result_ffactd = new List<BataPos.Ent_Ffactd>();
                            for (Int32 i = 0; i < dt_det_clone.Rows.Count; ++i)
                            {
                                BataPos.Ent_Ffactd ffactd = new BataPos.Ent_Ffactd()
                                {
                                    fd_nint = dt_det_clone.Rows[i]["fd_nint"].ToString(),
                                    fd_tipo = dt_det_clone.Rows[i]["fd_tipo"].ToString(),
                                    fd_arti = dt_det_clone.Rows[i]["fd_arti"].ToString(),
                                    fd_regl = dt_det_clone.Rows[i]["fd_regl"].ToString(),
                                    fd_colo = dt_det_clone.Rows[i]["fd_colo"].ToString(),
                                    fd_item = dt_det_clone.Rows[i]["fd_item"].ToString(),
                                    fd_icmb = dt_det_clone.Rows[i]["fd_icmb"].ToString(),
                                    fd_qfac =Convert.ToDecimal(dt_det_clone.Rows[i]["fd_qfac"]),
                                    fd_lpre = dt_det_clone.Rows[i]["fd_lpre"].ToString(),
                                    fd_calm = dt_det_clone.Rows[i]["fd_calm"].ToString(),
                                    fd_pref =Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pref"]),
                                    fd_dref = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_dref"]),
                                    fd_prec = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_prec"]),
                                    fd_brut = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_brut"]),
                                    fd_pimp1 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pimp1"]),
                                    fd_vimp1 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vimp1"]),
                                    fd_subt1 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_subt1"]),
                                    fd_pimp2 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pimp2"]),
                                    fd_vimp2 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vimp2"]),
                                    fd_subt2 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_subt2"]),
                                    fd_pdct1 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pdct1"]),
                                    fd_vdct1 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vdct1"]),
                                    fd_subt3 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_subt3"]),
                                    fd_vdct4 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vdct4"]),
                                    fd_vdc23 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vdc23"]),
                                    fd_vvta = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vvta"]),
                                    fd_pimp3 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pimp3"]),
                                    fd_vimp3 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vimp3"]),
                                    fd_pimp4 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_pimp4"]),
                                    fd_vimp4 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_vimp4"]),
                                    fd_total = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_total"]),
                                    fd_cuse = dt_det_clone.Rows[i]["fd_cuse"].ToString(),
                                    fd_muse = dt_det_clone.Rows[i]["fd_muse"].ToString(),
                                    fd_fcre = Convert.ToDateTime(dt_det_clone.Rows[i]["fd_fcre"]),
                                    fd_fmod = Convert.ToDateTime(dt_det_clone.Rows[i]["fd_fmod"]),
                                    fd_auto = dt_det_clone.Rows[i]["fd_auto"].ToString(),
                                    fd_dre2 = Convert.ToDecimal(dt_det_clone.Rows[i]["fd_dre2"]),
                                    fd_asoc = dt_det_clone.Rows[i]["fd_asoc"].ToString(),             
                                };

                                result_ffactd.Add(ffactd);

                            }
                            #endregion
                            #region<insertar fnotaa>   
                            List<BataPos.Ent_Fnotaa> result_fnotaa = new List<BataPos.Ent_Fnotaa>();
                            for (Int32 i = 0; i < dt_pago_clone.Rows.Count; ++i)
                            {
                                BataPos.Ent_Fnotaa fnotaa = new BataPos.Ent_Fnotaa()
                                {
                                    na_nota = dt_pago_clone.Rows[i]["na_nota"].ToString(),
                                    na_item = dt_pago_clone.Rows[i]["na_item"].ToString(),
                                    na_mone = dt_pago_clone.Rows[i]["na_mone"].ToString(),
                                    na_tpag = dt_pago_clone.Rows[i]["na_tpag"].ToString(),
                                    na_tasa =Convert.ToDecimal(dt_pago_clone.Rows[i]["na_tasa"]),
                                    na_cref = dt_pago_clone.Rows[i]["na_cref"].ToString(),
                                    na_sref = dt_pago_clone.Rows[i]["na_sref"].ToString(),
                                    na_nref = dt_pago_clone.Rows[i]["na_nref"].ToString(),
                                    na_vref =Convert.ToDecimal(dt_pago_clone.Rows[i]["na_vref"]),
                                    na_vpag = Convert.ToDecimal(dt_pago_clone.Rows[i]["na_vpag"]),
                                    na_esta = dt_pago_clone.Rows[i]["na_esta"].ToString(),
                                    na_cier = dt_pago_clone.Rows[i]["na_cier"].ToString(),
                                    na_fcre = dt_pago_clone.Rows[i]["na_fcre"].ToString(),
                                    na_fmod = dt_pago_clone.Rows[i]["na_fcre"].ToString(),
                                };

                                result_fnotaa.Add(fnotaa);

                            }
                            #endregion

                            var array_ffactc = new BataPos.Ent_List_Ffactc();
                            array_ffactc.lista_ffactc = result_ffactc.ToArray();

                            var array_ffactd = new BataPos.Ent_List_Ffactd();
                            array_ffactd.lista_ffactd = result_ffactd.ToArray();

                            var array_fnotaa = new BataPos.Ent_List_Fnotaa();
                            array_fnotaa.lista_fnotaa = result_fnotaa.ToArray();

                            error_venta = envia_transac_ventas_lista(array_ffactc, array_ffactd, array_fnotaa);
                        }

                        //envia_transac_ventas_lista()
                        #endregion

                       


                    }
                }

            }
            catch (Exception exc)
            {
                dt_cab = null;
                dt_det = null;
                string _FFACTC = Variables._path_envia + "\\FFACTC.DBF";
                string _FFACTD = Variables._path_envia + "\\FFACTD.DBF";

                if (File.Exists(@_FFACTC))
                    File.Delete(@_FFACTC);
                if (File.Exists(_FFACTD))
                    File.Delete(_FFACTD);
            }
            if (cn!=null)            
            if (cn.State == ConnectionState.Open) cn.Close();            
        }

        private static Boolean _verifica_venta_existe()
        {

            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt_tda = null;
            string sqlquery = "";
            Boolean _existe_venta = false;
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                //tienda
                sqlquery = "select C_sucu from FPCTRL02";

                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt_tda = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt_tda);
                }
                if (dt_tda != null)
                    _tienda = "50" + dt_tda.Rows[0]["C_sucu"].ToString();
            }
            catch
            {
                _existe_venta = false;                
                throw;
            }
            return _existe_venta;
        }

        private static void _register_vfpoledb()
        {

            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt_tda = null;
            string sqlquery = "";
            try
            {
                cn = new OleDbConnection(Variables._conexion_vfpoledb);
                //tienda
                sqlquery = "select C_sucu   from FPCTRL02";

                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                da = new OleDbDataAdapter(cmd);
                dt_tda = new DataTable();
                if (!_ejecute_reindex_dbf())
                {
                    da.Fill(dt_tda);
                    //if (dt_tda != null)
                    //    _tienda = "50" + dt_tda.Rows[0]["C_sucu"].ToString();
                }
            }
            catch (Exception exc)
            {
                _update_version_vfpoledbdll(true);
                //_tienda = null;
                //throw;
            }
            //_update_version_vfpoledbdll(true);
        }

        private static void _dbftienda()
        {

            OleDbConnection cn = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            DataTable dt_tda = null;            
            string sqlquery = "";
            try
            {
                cn = new OleDbConnection(Variables._conexion);
                //tienda
                sqlquery = "select C_sucu   from FPCTRL02";

                cmd = new OleDbCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;                
                da = new OleDbDataAdapter(cmd);
                dt_tda = new DataTable();
                if (!_ejecute_reindex_dbf())
                { 
                    da.Fill(dt_tda);                               
                    if (dt_tda != null)
                        _tienda = "50" + dt_tda.Rows[0]["C_sucu"].ToString();
                }
            }
            catch(Exception exc)
            {
                _tienda = null;
                throw;
            }
        }
        private static void tabla_FFACTD(DataTable dt)
        {
            try
            {
                DBF.NET.DBFNET venta_det = new DBF.NET.DBFNET();
                venta_det.tabla = "FFACTD";
                venta_det.addcol("fd_nint", DBF.NET.Tipo.Caracter, "8");
                venta_det.addcol("fd_tipo", DBF.NET.Tipo.Caracter, "1");
                venta_det.addcol("fd_arti", DBF.NET.Tipo.Caracter, "12");
                venta_det.addcol("fd_regl", DBF.NET.Tipo.Caracter, "4");
                venta_det.addcol("fd_colo", DBF.NET.Tipo.Caracter, "2");
                venta_det.addcol("fd_item", DBF.NET.Tipo.Caracter, "3");                
                venta_det.addcol("fd_icmb", DBF.NET.Tipo.Caracter, "1");                
                venta_det.addcol("fd_qfac", DBF.NET.Tipo.Numerico, "8,3");
                venta_det.addcol("fd_lpre", DBF.NET.Tipo.Caracter, "2");

                venta_det.addcol("fd_calm", DBF.NET.Tipo.Caracter, "4");
                venta_det.addcol("fd_pref", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_dref", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_prec", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_brut", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_pimp1", DBF.NET.Tipo.Numerico, "6,2");
                venta_det.addcol("fd_vimp1", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_subt1", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_pimp2", DBF.NET.Tipo.Numerico, "6,2");
                venta_det.addcol("fd_vimp2", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_subt2", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_pdct1", DBF.NET.Tipo.Numerico, "6,2");
                venta_det.addcol("fd_vdct1", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_subt3", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_vdct4", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_vdc23", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_vvta", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_pimp3", DBF.NET.Tipo.Numerico, "6,2");
                venta_det.addcol("fd_vimp3", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_pimp4", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_vimp4", DBF.NET.Tipo.Numerico, "14,4");

                venta_det.addcol("fd_total", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_cuse", DBF.NET.Tipo.Caracter, "3");
                venta_det.addcol("fd_muse", DBF.NET.Tipo.Caracter, "3");
                venta_det.addcol("fd_fcre", DBF.NET.Tipo.Fecha);
                venta_det.addcol("fd_fmod", DBF.NET.Tipo.Fecha);
                venta_det.addcol("fd_auto", DBF.NET.Tipo.Caracter, "6");
                venta_det.addcol("fd_dre2", DBF.NET.Tipo.Numerico, "14,4");
                venta_det.addcol("fd_asoc", DBF.NET.Tipo.Caracter, "13");

                venta_det.creardbf();
                venta_det.Insertar_tabla(dt);
            }
            catch
            {
                throw;
            }
        }

        private static void tabla_FNOTAA(DataTable dt)
        {
            try
            {
                ///crea tabla 
                ///
                DBF.NET.DBFNET venta_pago = new DBF.NET.DBFNET();
                venta_pago.tabla = "FNOTAA";
                venta_pago.addcol("na_nota", DBF.NET.Tipo.Caracter, "8");
                venta_pago.addcol("na_item", DBF.NET.Tipo.Caracter, "3");
                venta_pago.addcol("na_mone", DBF.NET.Tipo.Caracter, "2");
                venta_pago.addcol("na_tpag", DBF.NET.Tipo.Caracter, "2");
                venta_pago.addcol("na_tasa", DBF.NET.Tipo.Numerico, "10,4");
                venta_pago.addcol("na_cref", DBF.NET.Tipo.Caracter, "2");
                venta_pago.addcol("na_sref", DBF.NET.Tipo.Caracter, "4");
                venta_pago.addcol("na_nref", DBF.NET.Tipo.Caracter, "22");
                venta_pago.addcol("na_vref", DBF.NET.Tipo.Numerico, "14,4");
                venta_pago.addcol("na_vpag", DBF.NET.Tipo.Numerico, "14,4");
                venta_pago.addcol("na_esta", DBF.NET.Tipo.Caracter, "1");
                venta_pago.addcol("na_cier", DBF.NET.Tipo.Caracter, "1");
                venta_pago.addcol("na_fcre", DBF.NET.Tipo.Caracter, "30");
                venta_pago.addcol("na_fmod", DBF.NET.Tipo.Caracter, "30");
                venta_pago.creardbf();
                venta_pago.Insertar_tabla(dt);
            }
            catch
            {
                throw;
            }
        }
        private static void tabla_FFACTC(DataTable dt)
        {
            try
            { 
                DBF.NET.DBFNET venta_cab = new DBF.NET.DBFNET();
                venta_cab.tabla = "FFACTC";
                venta_cab.addcol("fc_nint", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_nnot", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_codi", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_suna", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_sfac", DBF.NET.Tipo.Caracter, "4");
                venta_cab.addcol("fc_nfac", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_ffac", DBF.NET.Tipo.Fecha);
                venta_cab.addcol("fc_nord", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_cref", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_sref", DBF.NET.Tipo.Caracter, "4");
                venta_cab.addcol("fc_nref", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_pvta", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_csuc", DBF.NET.Tipo.Caracter, "3");
                venta_cab.addcol("fc_gvta", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_zona", DBF.NET.Tipo.Caracter, "3");
                venta_cab.addcol("fc_clie", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_ncli", DBF.NET.Tipo.Caracter, "90");
                venta_cab.addcol("fc_nomb", DBF.NET.Tipo.Caracter, "30");
                venta_cab.addcol("fc_apep", DBF.NET.Tipo.Caracter, "30");
                venta_cab.addcol("fc_apem", DBF.NET.Tipo.Caracter, "30");
                venta_cab.addcol("fc_dcli", DBF.NET.Tipo.Caracter, "50");
                venta_cab.addcol("fc_cubi", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_ruc", DBF.NET.Tipo.Caracter, "11");
                venta_cab.addcol("fc_vuse", DBF.NET.Tipo.Caracter, "3");
                venta_cab.addcol("fc_vend", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_ipre", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_tint", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_pint", DBF.NET.Tipo.Numerico, "6,2");
                venta_cab.addcol("fc_lcsg", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_ncon", DBF.NET.Tipo.Caracter, "30");
                venta_cab.addcol("fc_dcon", DBF.NET.Tipo.Caracter, "30");
                venta_cab.addcol("fc_lcon", DBF.NET.Tipo.Caracter, "20");
                venta_cab.addcol("fc_lruc", DBF.NET.Tipo.Caracter, "11");
                venta_cab.addcol("fc_agen", DBF.NET.Tipo.Caracter, "20");
                venta_cab.addcol("fc_mone", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_tasa", DBF.NET.Tipo.Numerico, "10,4");
                venta_cab.addcol("fc_fpag", DBF.NET.Tipo.Caracter, "2");

                venta_cab.addcol("fc_nlet", DBF.NET.Tipo.Numerico, "2,0");
                venta_cab.addcol("fc_qtot", DBF.NET.Tipo.Numerico, "8,2");
                venta_cab.addcol("fc_pref", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_dref", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_brut", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vimp1", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vimp2", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vdct1", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vdct4", DBF.NET.Tipo.Numerico, "14,4");

                venta_cab.addcol("fc_pdc2", DBF.NET.Tipo.Numerico, "6,2");
                venta_cab.addcol("fc_pdc3", DBF.NET.Tipo.Numerico, "6,2");
                venta_cab.addcol("fc_vdc23", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vvta", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_vimp3", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_pimp4", DBF.NET.Tipo.Numerico, "6,2");

                venta_cab.addcol("fc_vimp4", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_total", DBF.NET.Tipo.Numerico, "14,4");
                venta_cab.addcol("fc_esta", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_tdoc", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_cuse", DBF.NET.Tipo.Caracter, "3");
                venta_cab.addcol("fc_muse", DBF.NET.Tipo.Caracter, "3");

                venta_cab.addcol("fc_fcre", DBF.NET.Tipo.Fecha);
                venta_cab.addcol("fc_fmod", DBF.NET.Tipo.Fecha);

                venta_cab.addcol("fc_hora", DBF.NET.Tipo.Caracter, "8");
                venta_cab.addcol("fc_auto", DBF.NET.Tipo.Caracter, "3");
                venta_cab.addcol("fc_ftx", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_estc", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_sexo", DBF.NET.Tipo.Caracter, "1");
                venta_cab.addcol("fc_mpub", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_edad", DBF.NET.Tipo.Caracter, "2");
                venta_cab.addcol("fc_regv", DBF.NET.Tipo.Caracter, "25");
                venta_cab.creardbf();
                venta_cab.Insertar_tabla(dt);
            }
            catch
            {
                throw;
            }
        }
        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }
        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }
        #endregion
        #region<ENVIAR PAQUETES TX DEL POS>
        public static void envia_paq_tx()
        {
            string ruta_tx_pos = @"D:\POS\TX";
            try
            {
                string[] _archivos_paq_array = Directory.GetFiles(@ruta_tx_pos, "*.*").OrderBy(d => new FileInfo(d).CreationTime).ToArray();

                if (_archivos_paq_array.Length > 0)
                {
                    for (Int32 a = 0; a < _archivos_paq_array.Length; ++a)
                    {
                        string _archivo = _archivos_paq_array[a].ToString();
                        string _nombrearchivo_paq = System.IO.Path.GetFileNameWithoutExtension(@_archivo);
                        
                        if (Left(_nombrearchivo_paq, 2) == "TD")
                        {                      
                            

                            string _ext = System.IO.Path.GetExtension(@_archivo);

                            string _nom_arc_ext = _nombrearchivo_paq + _ext;

                            byte[] _archivo_bytes = File.ReadAllBytes(@_archivo);


                            BataTransmision.bata_transaccionSoapClient envia_paq = new BataTransmision.bata_transaccionSoapClient();
                            BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                            conexion.user_name = "emcomer";
                            conexion.user_password = "Bata2013";

                            string[] valida = envia_paq.ws_transmision_ingreso(conexion, _archivo_bytes, _nom_arc_ext);

                            if (valida[0] == "1")
                                File.Delete(@_archivo);
                        }

                    }
                }


            }
            catch
            {


            }
        }
        #endregion

        #region<RECEPCION DE GUIAS DE ALMACEN>
        /// <summary>
        /// metodo que recepciona guias de almacen para las tda desde la WS
        /// </summary>
        public static void recepcion_guias_alma(string tda)
        {           
            try
            {
                /**/
                //_dbftienda();
                /**/
                //_tienda = "50140";
                //_tienda = tda;
                #region<EXTRAER POR WEB SERVICE LOS DATOS DE ALMACEN DESPACHOS>
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";
                BataTransmision.bata_transaccionSoapClient trans = new BataTransmision.bata_transaccionSoapClient();

                /*VALIDACION DE TIENDA AUTOMATIC RECIBIENDO GUIAS DE ALMACEN*/
                Boolean valida_guia_automatic = trans.ws_exists_guia_tienda_alm(conexion, _tienda);
                /*si es falso entonces no realiza ninguna operacion y retona al proceso*/
                if (!valida_guia_automatic) return;

                /*****************/

                var lista_data = trans.ws_get_guias_tienda_almacen(conexion, _tienda);
                
                foreach(var item_guia_cab in lista_data)
                {

                    FFCGUD02 FFC = new FFCGUD02()
                    {
                        gtc_tipo = "30",
                        gtc_alm = "",
                        gtc_nume = "",
                        gtc_femi = item_guia_cab.DESC_FDESP,
                        gtc_semi = item_guia_cab.DESC_SEM,
                        gtc_gudis = item_guia_cab.DESC_GUDIS,
                        gtc_tndcl = item_guia_cab.DESC_TDES,
                        gtc_estad = "T",
                        gtc_cal = Convert.ToInt32(item_guia_cab.DESC_UNCA),
                        gtc_calm = item_guia_cab.DESC_VACA,
                        gtc_acc = Convert.ToInt32(item_guia_cab.DESC_UNNC),
                        gtc_accm = item_guia_cab.DESC_VANC,
                        gtc_caj = Convert.ToInt32(item_guia_cab.DESC_CAJA),
                        gtc_cajm = item_guia_cab.DESC_VCAJ,
                        gtc_glosa = DateTime.Today.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToLongTimeString(),
                    };
                    List<FFDGUD02> LISTA_FFD = new List<FFDGUD02>();
                    foreach(DataRow item_guia_det in item_guia_cab.DT_FVDESPD.Select("DESD_GUDIS='" + item_guia_cab.DESC_GUDIS + "'"))
                    {
                        FFDGUD02 FFD = new FFDGUD02()
                        {
                            gtd_tipo = "30",
                            gtd_nume = "",
                            gtd_gudis = item_guia_det["DESD_GUDIS"].ToString(),
                            gtd_artic = item_guia_det["DESD_ARTIC"].ToString() + item_guia_det["DESD_CALID"].ToString(),
                            gtd_categ = item_guia_det["DESD_CATEG"].ToString(),
                            gtd_subca = item_guia_det["DESD_SUBCA"].ToString(),
                            gtd_cndme = item_guia_det["DESD_CNDME"].ToString(),
                            gtd_prvta =Convert.ToDecimal(item_guia_det["DESD_PRVTA"]),
                            gtd_impor= Convert.ToDecimal(item_guia_det["DESD_PRVTA"])*Convert.ToDecimal(item_guia_det["DESD_TOTAL"]),

                            gtd_med00 =Convert.ToInt32(item_guia_det["00"]),
                            gtd_med01 = Convert.ToInt32(item_guia_det["01"]),
                            gtd_med02 = Convert.ToInt32(item_guia_det["02"]),
                            gtd_med03 = Convert.ToInt32(item_guia_det["03"]),
                            gtd_med04 = Convert.ToInt32(item_guia_det["04"]),
                            gtd_med05 = Convert.ToInt32(item_guia_det["05"]),
                            gtd_med06 = Convert.ToInt32(item_guia_det["06"]),
                            gtd_med07 = Convert.ToInt32(item_guia_det["07"]),
                            gtd_med08 = Convert.ToInt32(item_guia_det["08"]),
                            gtd_med09 = Convert.ToInt32(item_guia_det["09"]),
                            gtd_med10 = Convert.ToInt32(item_guia_det["10"]),
                            gtd_med11 = Convert.ToInt32(item_guia_det["11"]),


                            gtd_total = Convert.ToInt32(item_guia_det["DESD_TOTAL"]),
                            gtd_conf="",
                        };
                        LISTA_FFD.Add(FFD);
                    }

                    FFC.gtd_lista = LISTA_FFD;

                    if (FFC.gtd_lista!=null)
                    {
                        /*en esta opcion quiere decir que la guias esta completo con el detalle*/
                        if (FFC.gtd_lista.Count>0)
                        {
                            /*en esta linea insertar a los dbf gudis si es ok true de lo contrario false*/
                            Boolean insertar_guia = insertar_dbf_gud(FFC);
                            /*EN ESTE CASO ES PORQUE LA GUIA SE GRABO CORRECTAMENTE Y ES TRUE*/
                            /*ENVIAMOS A LA WS */
                            if (insertar_guia)
                            {
                               Boolean update_ws= trans.ws_update_guia_tienda_almacen(conexion, _tienda, item_guia_cab.DESC_GUDIS);

                            }

                        }
                    }

                }    

                #endregion

            }
            catch(Exception exc)
            {
             
            }
        }     
        private static Boolean insertar_dbf_gud(FFCGUD02 FFC)
        {
            Boolean valida_insert = false;
            string sqlquery = "";
            string _FFCGUD02 = "FFCGUD02";
            string _FFDGUD02 = "FFDGUD02";
            Int32 _existe_guia = 0;
            try
            {
                using (OleDbConnection cn = new OleDbConnection(Variables._conexion))
                {
                    try
                    {

                        /*verificar si existe la guias*/
                        //1637314
                        // FFC.gtc_gudis
                        #region<VERIFICAR SI LA GUIA EXISTE >
                        sqlquery = "SELECT count(*) FROM " + _FFCGUD02 + " WHERE GTC_GUDIS='" + FFC.gtc_gudis + "' AND GTC_TIPO='30'";
                        using (OleDbCommand cmd = new OleDbCommand(sqlquery,cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.Text;
                            if (!_ejecute_reindex_dbf())                                                           
                            {
                                if (cn.State == 0) cn.Open();
                                _existe_guia =(Int32)cmd.ExecuteScalar();                            
                                if (cn.State == ConnectionState.Open) cn.Close();
                            }
                        }
                        #endregion
                        /*si la guia no existe entonces insertamos*/
                        /*si la guia existe entonces borramos para insertar de nuevo*/
                        if (_existe_guia>0)
                        {
                            #region<BORRAMOS LA GUIAS SI EXISTE CABEZERA Y DETALLE>
                            sqlquery = "delete from " + _FFCGUD02 + " WHERE GTC_GUDIS='" + FFC.gtc_gudis + "' AND GTC_TIPO='30'";

                            using (OleDbCommand cmd = new OleDbCommand(sqlquery, cn))
                            {
                                cmd.CommandTimeout = 0;
                                cmd.CommandType = CommandType.Text;
                                if (!_ejecute_reindex_dbf())
                                {
                                    if (cn.State == 0) cn.Open();
                                    cmd.ExecuteNonQuery();
                                    if (cn.State == ConnectionState.Open) cn.Close();
                                }
                            }
                            sqlquery = "delete from " + _FFDGUD02 + " WHERE GTD_GUDIS='" + FFC.gtc_gudis + "' AND GTD_TIPO='30'";
                            using (OleDbCommand cmd = new OleDbCommand(sqlquery, cn))
                            {
                                cmd.CommandTimeout = 0;
                                cmd.CommandType = CommandType.Text;
                                if (!_ejecute_reindex_dbf())
                                {
                                    if (cn.State == 0) cn.Open();
                                    cmd.ExecuteNonQuery();
                                    if (cn.State == ConnectionState.Open) cn.Close();
                                }
                            }


                            #endregion
                        }


                        //if (_existe_guia==0)
                        //{
                            #region<INSERTAMOS CABECERA DE LA GUIA>                    
                            sqlquery = "insert into " + _FFCGUD02 + "(gtc_tipo,gtc_alm,gtc_nume,gtc_femi,gtc_semi,gtc_gudis," +
                                                                                    "gtc_tndcl,gtc_estad,gtc_frect,gtc_cal,gtc_calm,gtc_acc," +
                                                                                    "gtc_accm,gtc_caj,gtc_cajm,gtc_glosa) " +
                                                                                    "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                            using (OleDbCommand cmd = new OleDbCommand(sqlquery, cn))
                            {
                                cmd.CommandTimeout = 0;
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Add("@gtc_tipo", OleDbType.Char).Value =FFC.gtc_tipo;
                                cmd.Parameters.Add("@gtc_alm", OleDbType.Char).Value = FFC.gtc_alm;
                                cmd.Parameters.Add("@gtc_nume", OleDbType.Char).Value =FFC.gtc_nume;
                                cmd.Parameters.Add("@gtc_femi", OleDbType.Date).Value =FFC.gtc_femi;
                                cmd.Parameters.Add("@gtc_semi", OleDbType.Char).Value =FFC.gtc_semi;
                                cmd.Parameters.Add("@gtc_gudis", OleDbType.Char).Value =FFC.gtc_gudis;
                                cmd.Parameters.Add("@gtc_tndcl", OleDbType.Char).Value =FFC.gtc_tndcl;
                                cmd.Parameters.Add("@gtc_estad", OleDbType.Char).Value =FFC.gtc_estad;
                                cmd.Parameters.Add("@gtc_frect", OleDbType.Date).Value = DBNull.Value;// _gtc_frect;
                                cmd.Parameters.Add("@gtc_cal", OleDbType.Decimal).Value =FFC.gtc_cal;
                                cmd.Parameters.Add("@gtc_calm", OleDbType.Decimal).Value = FFC.gtc_calm;
                                cmd.Parameters.Add("@gtc_acc", OleDbType.Decimal).Value = FFC.gtc_acc;
                                cmd.Parameters.Add("@gtc_accm", OleDbType.Decimal).Value = FFC.gtc_accm;
                                cmd.Parameters.Add("@gtc_caj", OleDbType.Decimal).Value = FFC.gtc_caj;
                                cmd.Parameters.Add("@gtc_cajm", OleDbType.Decimal).Value = FFC.gtc_cajm;
                                cmd.Parameters.Add("@gtc_glosa", OleDbType.Char).Value = FFC.gtc_glosa;

                                if (!_ejecute_reindex_dbf())
                                {
                                    if (cn.State == 0) cn.Open();
                                    cmd.ExecuteNonQuery();
                                    if (cn.State == ConnectionState.Open) cn.Close();
                                }

                            }

                            #endregion
                            #region<INSERTAMOS DETALLE DE LA GUIA>   
                            foreach(var gdet in FFC.gtd_lista)
                            {
                                sqlquery = "insert into " + _FFDGUD02 + "(gtd_tipo,gtd_nume,gtd_gudis,gtd_artic,gtd_categ,gtd_subca,gtd_cndme,gtd_prvta," +
                                                                                                        "gtd_impor,gtd_med00,gtd_med01,gtd_med02,gtd_med03,gtd_med04,gtd_med05,gtd_med06," +
                                                                                                        "gtd_med07,gtd_med08,gtd_med09,gtd_med10,gtd_med11,gtd_total,gtd_conf)" +
                                                                                                        "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                                using (OleDbCommand cmd = new OleDbCommand(sqlquery, cn))
                                {
                                    cmd.CommandTimeout = 0;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("@gtd_tipo", OleDbType.Char).Value =gdet.gtd_tipo;
                                    cmd.Parameters.Add("@gtd_nume", OleDbType.Char).Value = gdet.gtd_nume;
                                    cmd.Parameters.Add("@gtd_gudis", OleDbType.Char).Value = gdet.gtd_gudis;
                                    cmd.Parameters.Add("@gtd_artic", OleDbType.Char).Value = gdet.gtd_artic;
                                    cmd.Parameters.Add("@gtd_categ", OleDbType.Char).Value = gdet.gtd_categ;
                                    cmd.Parameters.Add("@gtd_subca", OleDbType.Char).Value = gdet.gtd_subca;
                                    cmd.Parameters.Add("@gtd_cndme", OleDbType.Char).Value = gdet.gtd_cndme;
                                    cmd.Parameters.Add("@gtd_prvta", OleDbType.Numeric).Value = gdet.gtd_prvta;
                                    cmd.Parameters.Add("@gtd_impor", OleDbType.Numeric).Value = gdet.gtd_impor;
                                    cmd.Parameters.Add("@gtd_med00", OleDbType.Numeric).Value = gdet.gtd_med00;
                                    cmd.Parameters.Add("@gtd_med01", OleDbType.Numeric).Value = gdet.gtd_med01;
                                    cmd.Parameters.Add("@gtd_med02", OleDbType.Numeric).Value = gdet.gtd_med02;
                                    cmd.Parameters.Add("@gtd_med03", OleDbType.Numeric).Value = gdet.gtd_med03;
                                    cmd.Parameters.Add("@gtd_med04", OleDbType.Numeric).Value = gdet.gtd_med04;
                                    cmd.Parameters.Add("@gtd_med05", OleDbType.Numeric).Value = gdet.gtd_med05;
                                    cmd.Parameters.Add("@gtd_med06", OleDbType.Numeric).Value = gdet.gtd_med06;
                                    cmd.Parameters.Add("@gtd_med07", OleDbType.Numeric).Value = gdet.gtd_med07;
                                    cmd.Parameters.Add("@gtd_med08", OleDbType.Numeric).Value = gdet.gtd_med08;
                                    cmd.Parameters.Add("@gtd_med09", OleDbType.Numeric).Value = gdet.gtd_med09;
                                    cmd.Parameters.Add("@gtd_med10", OleDbType.Numeric).Value = gdet.gtd_med10;
                                    cmd.Parameters.Add("@gtd_med11", OleDbType.Numeric).Value = gdet.gtd_med11;
                                    cmd.Parameters.Add("@gtd_total", OleDbType.Numeric).Value = gdet.gtd_total;
                                    cmd.Parameters.Add("@gtd_conf", OleDbType.Char).Value = gdet.gtd_conf;

                                    if (!_ejecute_reindex_dbf())
                                    {
                                        if (cn.State == 0) cn.Open();
                                        cmd.ExecuteNonQuery();
                                        if (cn.State == ConnectionState.Open) cn.Close();
                                    }

                                }
                            }

                           /*EN ESTE CASO QUIERE DECIR QUE NO HUBO ERRORES AL INSERTAR EN EL DBF DE LA GUIAS*/

                            valida_insert = true;

                            #endregion
                        //}


                    }
                    catch (Exception exc)
                    {                         
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }

                //    OleDbConnection cn = null;
                //OleDbCommand cmd = null;
                //OleDbDataAdapter da = null;

                //using

            }
            catch (Exception)
            {
                valida_insert = false;                
            }
            return valida_insert;
        }

        #endregion

        #region<CONEXIONES WEB SERVICES NUBE POS>

        #region<VARIABLES ESTATICOS>
        public static Boolean valida_time;
        public static DateTime activa_fecha_ini;
        public static DateTime activa_fecha_fin;
        public static Int32 intervalo_min;
        public static string ejecuciontime = "";
        #endregion
        public static void Obtener_Stock()
        {
            try
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine("INGRESO");
                tw.Flush();
                tw.Close();
                tw.Dispose();

                /*user y password*/
                BataPos.ValidateAcceso header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                BataPos.Bata_TransactionSoapClient batatran = new BataPos.Bata_TransactionSoapClient();

                

                /*list*/
                List<BataPos.Ent_Stock> result = new List<BataPos.Ent_Stock>();

                using (System.Data.OleDb.OleDbConnection dbConn = new System.Data.OleDb.OleDbConnection(Variables._conexion_vfpoledb))
                {
                    try
                    {

                        //-- Obtenemos datos abierto o ultimo cerrado
                        string sql_cierre = "SELECT MAX(CI_FECH) as CI_FECH FROM FCIERR02 ";
                        System.Data.OleDb.OleDbCommand dat_cierre = new System.Data.OleDb.OleDbCommand(sql_cierre, dbConn);
                        System.Data.OleDb.OleDbDataAdapter ada_cierre = new System.Data.OleDb.OleDbDataAdapter(dat_cierre);
                        DataTable datos_cierre = new DataTable();
                        if (!_ejecute_reindex_dbf())
                            ada_cierre.Fill(datos_cierre);

                        //-- Se convierten los datos obtenidos                       
                        string fecha_dbf = Convert.ToDateTime(datos_cierre.Rows[0]["CI_FECH"]).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(datos_cierre.Rows[0]["CI_FECH"]).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(datos_cierre.Rows[0]["CI_FECH"]).Year.ToString();
                        string mes = Convert.ToDateTime(datos_cierre.Rows[0]["CI_FECH"]).Month.ToString().PadLeft(2, '0');
                        string campo_stkg = "SG_SA" + mes;
                        string tabla_stkg = "FSTKG" + Right(Convert.ToDateTime(datos_cierre.Rows[0]["CI_FECH"].ToString()).Year.ToString(),1) + "02";

                        

                        //-- Obtenemos datos Periodo de abierto o ultimo cierre
                        string sql_prods = "SELECT stkg.sg_arti, stkg.sg_regl, sum(stkg." + campo_stkg + ") AS valor FROM " + tabla_stkg + " stkg ";
                        sql_prods = sql_prods + "WHERE stkg." + campo_stkg + " <> 0 GROUP BY stkg.sg_arti, stkg.sg_regl";
                        System.Data.OleDb.OleDbCommand dat_prods = new System.Data.OleDb.OleDbCommand(sql_prods, dbConn);
                        System.Data.OleDb.OleDbDataAdapter ada_prods = new System.Data.OleDb.OleDbDataAdapter(dat_prods);
                        DataTable datos_prods = new DataTable();
                        if (!_ejecute_reindex_dbf())
                            ada_prods.Fill(datos_prods);

                        result = (from DataRow row in datos_prods.Rows
                                  select new BataPos.Ent_Stock()
                                  {
                                      cod_tda = _tienda,
                                      art_cod = row["SG_ARTI"].ToString().Substring(0, 7),
                                      art_cal = row["SG_ARTI"].ToString().Substring(7, 1),
                                      art_talla = row["SG_REGL"].ToString(),
                                      art_pares = Convert.ToInt32(row["VALOR"]),
                                  }).ToList();

                    }
                    catch (Exception ex)
                    {
                        TextWriter tw1 = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                        tw1.WriteLine(ex.Message);
                        tw1.Flush();
                        tw1.Close();
                        tw1.Dispose();
                    }

                }

                if (result.Count > 0)
                {
                    var array = new BataPos.Ent_Lista_Stock();
                    array.lista_stock = result.ToArray();

                    BataPos.Ent_MsgTransac msg = batatran.ws_envia_stock_tda(header_user, array);

                    /*Nota*/
                    //msg.codigo = "0";
                    //msg.descripcion = "Se actualizo correctamente";

                    //msg.codigo = "1";
                    //msg.descripcion = "descripcion de error";
                }
            }
            catch (Exception ex)
            {
                TextWriter tw2 = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw2.WriteLine(ex.Message);
                tw2.Flush();
                tw2.Close();
                tw2.Dispose();
            }

        }
        public static void envio_stock()
        {
            try
            {
                /*si la variable es false entonces verifica el intervalo*/
                if (!valida_time)
                {
                    valida_time = true;
                    intervalo_min = timer_intervalo_min();
                    activa_fecha_ini= DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                    activa_fecha_fin = activa_fecha_ini.AddMinutes(intervalo_min);
                    ejecuciontime = "NO EJECUTANDO";
                }
                else
                {
                    DateTime fecha_hora_actual= DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);                   

                    if (fecha_hora_actual >= activa_fecha_fin)
                    {
                        Obtener_Stock();
                        ejecuciontime = "EJECUTANDO PROCESO";
                        valida_time =false;
                    }


                }
            }
            catch
            {

                
            }
        }
        /// <summary>
        /// return intervalo por minuto
        /// </summary>
        /// <returns></returns>
        public static Int32 timer_intervalo_min()
        {
            Int32 timer_min = 0;
            BataPos.ValidateAcceso header_user = null;

            BataPos.Bata_TransactionSoapClient batatran = null;
            try
            {                
                header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                batatran = new BataPos.Bata_TransactionSoapClient();
                var intervalo = batatran.ws_get_time_servicetrans(header_user,"01");
                if (intervalo!=null)
                {
                    timer_min = intervalo.cser_min;
                }

            }
            catch 
            {
                timer_min = 0;                
            }
            return timer_min;
        }

        public static void conexion_service_nube()
        {
            string msg = "";/*CONEXIONES DE TIENDAS*/
            BataPos.ValidateAcceso header_user = null;
            BataPos.Bata_TransactionSoapClient batatran = null;
            try
            {
                header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                batatran = new BataPos.Bata_TransactionSoapClient();

                var dd = batatran.HelloWorld(header_user,_tienda);

                if (dd!=null)
                {
                    msg = dd.descripcion;
                }
            }
            catch (Exception exc)
            {
                msg = exc.Message;               
            }
            //TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
            //tw.WriteLine(msg);
            //tw.Flush();
            //tw.Close();
            //tw.Dispose();
        }

        #region<ENVIAR VENTAS>
        public static String envia_transac_ventas(DataSet ds_venta)
        {
            string _error = "";
            try
            {
                /*user y password*/
                BataPos.ValidateAcceso header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                BataPos.Bata_TransactionSoapClient batatran = new BataPos.Bata_TransactionSoapClient();

                BataPos.Ent_MsgTransac  result = batatran.ws_envia_venta_tda(header_user, _tienda, ds_venta);

                if (result.codigo!="0")                
                    _error = result.descripcion;                

            }
            catch (Exception exc)
            {

                _error = exc.Message;
            }
            return _error;
        }

        public static String envia_transac_ventas_lista(BataPos.Ent_List_Ffactc ffactc,BataPos.Ent_List_Ffactd ffactd,BataPos.Ent_List_Fnotaa fnotaa)
        {
            string _error = "";
            try
            {
                /*user y password*/
                BataPos.ValidateAcceso header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                BataPos.Bata_TransactionSoapClient batatran = new BataPos.Bata_TransactionSoapClient();

                BataPos.Ent_MsgTransac result = batatran.ws_envia_venta_tda_lista(header_user, _tienda, ffactc, ffactd, fnotaa);

                if (result.codigo != "0")
                    _error = result.descripcion;

            }
            catch (Exception exc)
            {

                _error = exc.Message;
            }
            return _error;
        }

        public static String envia_transac_ventas_lista(BataPos.Ent_Venta_List lista_venta)
        {
            string _error = "";
            try
            {
                /*user y password*/
                BataPos.ValidateAcceso header_user = new BataPos.ValidateAcceso();
                header_user.Username = "3D4F4673-98EB-4EB5-A468-4B7FAEC0C721";
                header_user.Password = "566FDFF1-5311-4FE2-B3FC-0346923FE4B4";

                BataPos.Bata_TransactionSoapClient batatran = new BataPos.Bata_TransactionSoapClient();

                

                BataPos.Ent_MsgTransac result = batatran.ws_envia_venta_tda_list(header_user, _tienda, lista_venta);

                if (result.codigo != "0")
                    _error = result.descripcion;

            }
            catch (Exception exc)
            {

                _error = exc.Message;
            }
            return _error;
        }

        #endregion


        #endregion
    }

}
