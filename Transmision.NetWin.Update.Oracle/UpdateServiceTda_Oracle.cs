using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Transmision.NetWin.Update.Oracle
{
    public partial class UpdateServiceTda_Oracle : Form
    {
        public UpdateServiceTda_Oracle()
        {
            InitializeComponent();
        }
        private static string _tienda;
        //private static string _path_default = @"D:\POS";
        private static void copiar_archivo_service(string _name)
        {
            try
            {
                string _ruta_local_service = @"D:\Transmision.Net.Oracle\" + _name;

                BataTransmision.bata_transaccionSoapClient updateversion = new BataTransmision.bata_transaccionSoapClient();
                BataTransmision.Autenticacion conexion = new BataTransmision.Autenticacion();
                conexion.user_name = "emcomer";
                conexion.user_password = "Bata2013";

                byte[] _archivo = updateversion.ws_dll_service_tda(conexion, _name);
                System.IO.File.WriteAllBytes(_ruta_local_service, _archivo);

                if (_name == "Transmision.Net.Basico.Oracle.dll")
                {
                    //_dbftienda();
                    //if (_tienda == null)
                    //{
                    //    if (Environment.GetEnvironmentVariable("codtda") != null)
                    //        _tienda = "50" + Environment.GetEnvironmentVariable("codtda").ToString();
                    //}
                    FileInfo infofile_pro = new FileInfo(@_ruta_local_service);
                    var fvi_pro = FileVersionInfo.GetVersionInfo(@_ruta_local_service);
                    var version_pro = fvi_pro.FileVersion;

                    string _act = updateversion.ws_update_versiondll_net(conexion, _tienda, infofile_pro.Name, version_pro);
                }


            }
            catch
            {
                throw;
            }
        }
        private static void habilitando_servicio()
        {
            try
            {
                ServiceController[] service;
                service = (ServiceController[])ServiceController.GetServices();
                for (Int32 s = 0; s < service.Length; ++s)
                {
                    string nameservicio = service[s].ServiceName;
                    if (nameservicio == "Service Transmision.Oracle (Bata)")
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

        private static Boolean deshabilitando_servicio()
        {
            Boolean _valida = false;
            try
            {
                ServiceController[] service;
                service = (ServiceController[])ServiceController.GetServices();
                for (Int32 s = 0; s < service.Length; ++s)
                {
                    string nameservicio = service[s].ServiceName;
                    if (nameservicio == "Service Transmision.Oracle (Bata)")
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
            catch (Exception exc)
            {
                _valida = false;
            }
            return _valida;
        }
        private static new string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }
        private static new string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }
        private void UpdateServiceTda_Oracle_Load(object sender, EventArgs e)
        {

        }

        private void UpdateServiceTda_Oracle_Activated(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
            try
            {
                string nom_pc = Environment.MachineName;  /*"TIENDA-143-1"*/

                _tienda = nom_pc.Substring(nom_pc.IndexOf('-') + 1, nom_pc.Length - (nom_pc.IndexOf('-') + 1));
                _tienda = _tienda.Substring(0, _tienda.Length - 2);
                if (_tienda.Length == 3) _tienda = "50" + _tienda;

                //verificar y deshabilitar el servicio 
                if (deshabilitando_servicio())
                {
                    for (Int32 i = 0; i < 4; ++i)
                    {
                        switch (i)
                        {
                            //case 0:
                            //    copiar_archivo_service("DBF.NET.dll");
                            //    break;
                            //case 1:
                            //    copiar_archivo_service("GlobalSolucion.dll");
                            //    break;
                            case 0:
                                copiar_archivo_service("Transmision.Net.Basico.Oracle.dll");
                                break;
                            case 1:
                                copiar_archivo_service("Genera_Transmision_Oracle.exe.config");
                                break;
                            case 2:
                                copiar_archivo_service("Genera_Transmision_Oracle.exe");
                                break;
                            case 3:
                                copiar_archivo_service("BarcodeLib.dll");
                                break;
                                //case 3:
                                //    copiar_archivo_service("ICSharpCode.SharpZLib.dll");
                                //    break;
                                //case 4:
                                //    copiar_archivo_service("Genera_Transmision.exe.config");
                                //    break;
                                //case 5:
                                //    copiar_archivo_service("Genera_Transmision.exe");
                                //    break;
                        }

                    }


                }
                habilitando_servicio();
                Application.Exit();
                this.Close();

            }
            catch (Exception exc)
            {
                habilitando_servicio();
                TextWriter tw = new StreamWriter(@"D:\Transmision.Net.Oracle\LOG.txt", true);
                tw.WriteLine(exc.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                //MessageBox.Show(exc.Message);
                //en este caso si hay algun error entonces volvemos activar el servicio
                habilitando_servicio();
                Application.Exit();
            }
        }
    }
}
