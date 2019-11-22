using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using Transmision.Net.Basico.Oracle;

namespace Genera_Transmision_Oracle
{
    public partial class Service_Trans_Oracle : ServiceBase
    {
        Timer tmservicio_ora_data = null;
        private Int32 _valida_ora_data = 0;
        private string ruta_log = @"D:\Transmision.net.ORACLE\LOG.TXT";
        public Service_Trans_Oracle()
        {
            //5000=5 segundos
            InitializeComponent();
            tmservicio_ora_data = new Timer(5000);
            tmservicio_ora_data.Elapsed += new ElapsedEventHandler(tmservicio_ora_data_Elapsed);
        }
        void tmservicio_ora_data_Elapsed(object sender, ElapsedEventArgs e)
        {           

            Int32 _valor = 0;
            try
            {

         
                if (_valida_ora_data == 0)
                {

                    _valor = 1;
                    _valida_ora_data = 1;

                    Basico procesar = new Basico();

                    string _error = procesar.ejecuta_proceso_oracle();                                        
                    if (_error.Length > 0)
                    {
                        TextWriter tw = new StreamWriter(ruta_log, true);
                        tw.WriteLine(_error);
                        tw.Flush();
                        tw.Close();
                        tw.Dispose();
                    }
                    _valida_ora_data = 0;
                }
                //****************************************************************************
            }
            catch (Exception EXC)
            {
                TextWriter tw = new StreamWriter(ruta_log, true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_ora_data = 0;
            }

            if (_valor == 1)
            {
                _valida_ora_data = 0;
            }


        }

        protected override void OnStart(string[] args)
        {
            tmservicio_ora_data.Start();
        }

        protected override void OnStop()
        {
            tmservicio_ora_data.Stop();
        }
    }
}
