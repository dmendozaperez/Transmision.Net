using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Transmision.Net.Basico.Oracle
{
    public class Basico_Update
    {
        private string _tienda = "";
        public string ejecuta_update_service()
        {
            string error = "";
            try
            {
                string nom_pc = Environment.MachineName;// "TIENDA-143-1";//Environment.MachineName;

                _tienda = nom_pc.Substring(nom_pc.IndexOf('-') + 1, nom_pc.Length - (nom_pc.IndexOf('-') + 1));
                _tienda = _tienda.Substring(0, _tienda.Length - 2);
                if (_tienda.Length == 3) _tienda = "50" + _tienda;

                //ACTUALIZAR EXE DE WIN UPDATE
                _update_version_winupdateexe();
                //***actualizar si ha alguna version nueva de la ddl basico
                _update_version_serviowin();

            }
            catch (Exception exc)
            {
                error = exc.Message;
            }
            return error;
        }
        private void _update_version_winupdateexe()
        {
            try
            {
                string _assembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string _exe_servicio = "Transmision.NetWin.Update.Oracle.exe";
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
                    copiar_archivo_service(_exe_servicio);
                }

            }
            catch
            {

            }
        }
        private void copiar_archivo_service(string _name)
        {
            try
            {
                //string _ruta_local_service = @"D:\POS\Transmision.net\" + _name;

                string _ruta_local_service = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + _name;

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
        private void _update_version_serviowin()
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

                string _exe_servicio = "Transmision.Net.Basico.Oracle.dll";
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

                    string _exe_name = "Transmision.NetWin.Update.Oracle.exe";
                    string _path = _assembly + "\\" + _exe_name;
                    System.Diagnostics.Process.Start(@_path);
                }

            }
            catch
            {
                //throw;
            }
        }
    }
    
}
