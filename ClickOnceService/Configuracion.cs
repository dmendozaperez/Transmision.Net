using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace ClickOnceService
{
    public partial class Configuracion : Form
    {
        public Configuracion()
        {
            InitializeComponent();
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnactualizar_Click(object sender, EventArgs e)
        {
            _actualizar();
        }
        private void _actualizar()
        {
            string _ruta_produccion = @"\\172.28.7.9\inetpub\wwwroot\service_windows_tda";
            string _ruta_desarrollo = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Actualizar";
            try
            {
                string version_pro = "";
                if (File.Exists(@_ruta_produccion + "\\" + "Transmision.Net.Basico.dll"))
                { 
                    var fvi_pro = FileVersionInfo.GetVersionInfo(@_ruta_produccion + "\\" + "Transmision.Net.Basico.dll");
                    version_pro = fvi_pro.FileVersion;
                }
                var fvi_des = FileVersionInfo.GetVersionInfo(@_ruta_desarrollo + "\\" + "Transmision.Net.Basico.dll");
                var version_des = fvi_des.FileVersion;

                if (version_des != version_pro)
                {
                    DialogResult resulado = MessageBox.Show("Esta seguro Actualizar la version de la dll del servicio a Produccion", "Bata Peru Sistema...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (resulado == DialogResult.OK)
                    {
                        string _DBFNETdll = "DBF.NET.dll"; ;
                        string _GlobalSoluciondll= "GlobalSolucion.dll";
                        string _TransmisionNetBasicodll = "Transmision.Net.Basico.dll";



                        Byte[] _archivo_DBFNET = null;
                        _archivo_DBFNET = File.ReadAllBytes(_ruta_desarrollo + "\\" + _DBFNETdll);

                        Byte[] _archivo_GlobalSolucion = null;
                        _archivo_GlobalSolucion = File.ReadAllBytes(_ruta_desarrollo + "\\" + _GlobalSoluciondll);

                        Byte[] _TransmisionNetBasico = null;
                        _TransmisionNetBasico = File.ReadAllBytes(_ruta_desarrollo + "\\" + _TransmisionNetBasicodll);

                        File.WriteAllBytes(@_ruta_produccion + "\\" + _DBFNETdll, _archivo_DBFNET);
                        File.WriteAllBytes(@_ruta_produccion + "\\" + _GlobalSoluciondll, _archivo_GlobalSolucion);
                        File.WriteAllBytes(@_ruta_produccion + "\\" + _TransmisionNetBasicodll, _TransmisionNetBasico);

                        cargar();
                        MessageBox.Show("Enviado a produccion correctamente", "Bata Peru Sistema...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No se puede actualizar a la misma version de la dll basico", "Bata Peru Sistema...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message,"Bata Peru Sistema...",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
           
        }

        private void Configuracion_Load(object sender, EventArgs e)
        {
            cargar();
        }
        private void cargar()
        {
            Cursor.Current = Cursors.WaitCursor;
            string _ruta_produccion = @"\\172.28.7.9\inetpub\wwwroot\service_windows_tda\Transmision.Net.Basico.dll";
            string _ruta_desarrollo = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Actualizar\\Transmision.Net.Basico.dll";
            try
            {
                if (File.Exists(@_ruta_produccion))
                { 
                    FileInfo infofile_pro = new FileInfo(@_ruta_produccion);
                    var fvi_pro = FileVersionInfo.GetVersionInfo(@_ruta_produccion);
                    var version_pro = fvi_pro.FileVersion;

                    lblproduccion.Text =infofile_pro.Name + " "+ version_pro;
                }
                FileInfo infofile_des = new FileInfo(@_ruta_desarrollo);
                var fvi_des = FileVersionInfo.GetVersionInfo(@_ruta_desarrollo);
                var version_des = fvi_des.FileVersion;

                lbldesarrollo.Text = infofile_des.Name + " " +  version_des;
            }
            catch(Exception exc)
            {

            }
            Cursor.Current = Cursors.Default;
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            cargar();
        }
    }
}
