using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Transmision.Net.Basico;

namespace Genera_Transmision
{
    public partial class Service_Trans : ServiceBase
    {
        Timer tmservicio = null;
        private Int32 _valida_service = 0;
        public Service_Trans()
        {
            //5000=5 segundos
            InitializeComponent();
            tmservicio = new Timer(5000);
            tmservicio.Elapsed += new ElapsedEventHandler(tmpServicio_Elapsed);
        }
        void tmpServicio_Elapsed(object sender, ElapsedEventArgs e)
        {
            //string varchivov = "c://valida_hash.txt";
            Int32 _valor = 0;
            try
            {

                //if (!(System.IO.File.Exists(varchivov)))
                if (_valida_service == 0)
                {

                    _valor = 1;
                    _valida_service = 1;                  
                    string _error = "";
                    Basico._genera_transmision(ref _error);// Basico._ejecuta_proceso(ref _error);                    
                    _valida_service = 0;
                    if (_error.Length>0)
                    { 
                    TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                    tw.WriteLine(_error);
                    tw.Flush();
                    tw.Close();
                    tw.Dispose();
                    }
                }
                //****************************************************************************
            }
            catch(Exception EXC)
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_service = 0;             
            }

            if (_valor == 1)
            {                
                _valida_service = 0;             
            }
            

        }
        protected override void OnStart(string[] args)
        {
            tmservicio.Start();
        }

        protected override void OnStop()
        {
            tmservicio.Stop();
        }
    }
}
