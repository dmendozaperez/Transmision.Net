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


        #region<timers cv>
        Timer tmservicioCS = null;
        Timer tmservicioEG = null;
        Timer tmservicioER = null;
        Timer tmservicioRG = null;

        private Int32 _valida_serviceCS = 0;
        private Int32 _valida_serviceEG = 0;
        private Int32 _valida_serviceER = 0;
        private Int32 _valida_serviceRG = 0;
        #endregion

        public Service_Trans()
        {
            //5000=5 segundos
            InitializeComponent();
            tmservicio = new Timer(5000);
            tmservicio.Elapsed += new ElapsedEventHandler(tmpServicio_Elapsed);


            #region<timers cv>
            tmservicioCS = new Timer(1000);
            tmservicioEG = new Timer(1000);
            tmservicioER = new Timer(5000);
            tmservicioRG = new Timer(1000);

            tmservicioCS.Elapsed += new ElapsedEventHandler(tmpServicioCS_Elapsed);
            tmservicioEG.Elapsed += new ElapsedEventHandler(tmpServicioEG_Elapsed);
            tmservicioER.Elapsed += new ElapsedEventHandler(tmpServicioER_Elapsed);
            tmservicioRG.Elapsed += new ElapsedEventHandler(tmpServicioRG_Elapsed);
            #endregion

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
        #region Canal de Ventas
        void tmpServicioCS_Elapsed(object sender, ElapsedEventArgs e)
        {
            //string varchivov = "c://valida_hash.txt";
            Int32 _valor = 0;
            try
            {

                //if (!(System.IO.File.Exists(varchivov)))
                if (_valida_serviceCS == 0)
                {

                    _valor = 1;
                    _valida_serviceCS = 1;
                    string _error = "";
                    Basico._consultar_stock_otra_tda(ref _error);// Basico._ejecuta_proceso(ref _error);                    
                    _valida_serviceCS = 0;
                    if (_error.Length > 0)
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
            catch (Exception EXC)
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_serviceCS = 0;
            }

            if (_valor == 1)
            {
                _valida_serviceCS = 0;
            }


        }
        void tmpServicioEG_Elapsed(object sender, ElapsedEventArgs e)
        {
            //string varchivov = "c://valida_hash.txt";
            Int32 _valor = 0;
            try
            {

                //if (!(System.IO.File.Exists(varchivov)))
                if (_valida_serviceEG == 0)
                {

                    _valor = 1;
                    _valida_serviceEG = 1;
                    string _error = "";
                    Basico._enviar_guias_cv(ref _error);// Basico._ejecuta_proceso(ref _error);                    
                    _valida_serviceEG = 0;
                    if (_error.Length > 0)
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
            catch (Exception EXC)
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_serviceEG = 0;
            }

            if (_valor == 1)
            {
                _valida_serviceEG = 0;
            }


        }
        void tmpServicioER_Elapsed(object sender, ElapsedEventArgs e)
        {
            //string varchivov = "c://valida_hash.txt";
            Int32 _valor = 0;
            try
            {

                //if (!(System.IO.File.Exists(varchivov)))
                if (_valida_serviceER == 0)
                {

                    _valor = 1;
                    _valida_serviceER = 1;
                    string _error = "";
                    Basico._estado_recepcionar(ref _error);// Basico._ejecuta_proceso(ref _error);                    
                    _valida_serviceER = 0;
                    if (_error.Length > 0)
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
            catch (Exception EXC)
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_serviceER = 0;
            }

            if (_valor == 1)
            {
                _valida_serviceER = 0;
            }


        }
        void tmpServicioRG_Elapsed(object sender, ElapsedEventArgs e)
        {
            //string varchivov = "c://valida_hash.txt";
            Int32 _valor = 0;
            try
            {

                //if (!(System.IO.File.Exists(varchivov)))
                if (_valida_serviceRG == 0)
                {

                    _valor = 1;
                    _valida_serviceRG = 1;
                    string _error = "";
                    Basico._recepcionar_guias(ref _error);// Basico._ejecuta_proceso(ref _error);                    
                    _valida_serviceRG = 0;
                    if (_error.Length > 0)
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
            catch (Exception EXC)
            {
                TextWriter tw = new StreamWriter(@"D:\POS\Transmision.net\ERROR.txt", true);
                tw.WriteLine(EXC.Message);
                tw.Flush();
                tw.Close();
                tw.Dispose();
                _valida_serviceRG = 0;
            }

            if (_valor == 1)
            {
                _valida_serviceRG = 0;
            }


        }
        #endregion
        protected override void OnStart(string[] args)
        {
            tmservicio.Start();
            #region timers canal de venta
            tmservicioCS.Start();
            tmservicioEG.Start();
            tmservicioER.Start();
            tmservicioRG.Start();
            #endregion
        }

        protected override void OnStop()
        {
            tmservicio.Stop();
            #region timers canal de venta
            tmservicioCS.Stop();
            tmservicioEG.Stop();
            tmservicioER.Stop();
            tmservicioRG.Stop();
            #endregion
        }
    }
}
