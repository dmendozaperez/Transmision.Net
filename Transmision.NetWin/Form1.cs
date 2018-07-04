using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Transmision.Net.Basico;
namespace Transmision.NetWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            //Basico._genera_ventas();
        }
        public static void Registar_Dlls()
        {
            try
            {
                //'/s' : indicates regsvr32.exe to run silently.
                string fileinfo = "/s" + " " + "\"" + @"C:\Windows\System32\vfpoledb.dll" + "\"";

                Process reg = new Process();
                reg.StartInfo.FileName = "regsvr32.exe";
                reg.StartInfo.Arguments = fileinfo;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.Start();
                reg.WaitForExit();
                reg.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string sqlquery = "select s_fpro from FMSUCU02";
            //OleDbConnection cn = null;
            //OleDbCommand cmd = null;
            //OleDbDataAdapter da = null;
            //DataTable dt = null;

            //try
            //{
            //    Registar_Dlls();

            //    //"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + @"D:\POS" + ";Extended Properties=dBASE IV;User ID=Admin;"

            //    cn = new OleDbConnection("Provider=VFPOLEDB;Data Source=" + @"D:\POS" + ";Exclusive=No");
            //    cmd = new OleDbCommand(sqlquery, cn);
            //    cmd.CommandTimeout = 0;
            //    da = new OleDbDataAdapter(cmd);
            //    dt = new DataTable();
            //    da.Fill(dt);

            //    sqlquery = "SELECT * FROM " + "fmc05602" + " WHERE  (v_cfor='32' or v_cfor='31' )";
            //    cmd = new OleDbCommand(sqlquery, cn);
            //    cmd.CommandTimeout = 0;
            //    da = new OleDbDataAdapter(cmd);
            //    dt = new DataTable();

            //    da.Fill(dt);

            //    MessageBox.Show("ok", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //catch(Exception exc)
            //{
            //    MessageBox.Show(exc.Message,"Aviso",MessageBoxButtons.OK,MessageBoxIcon.Error);
            //}
            Cursor.Current = Cursors.WaitCursor;
            string _error = "";
            Basico._genera_transmision(ref _error);
            string cc = Basico.ejecuciontime.ToString();
            MessageBox.Show(cc);
            //MessageBox.Show("ok==>>" + _error);
            Cursor.Current = Cursors.Default;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //this.Hide();
            //this.Visible = false;
        }

        private void btnrecepcion_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                string codtda = "50" + txttda.Text;         
                Basico.recepcion_guias_alma(codtda);
                MessageBox.Show("termino", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message,"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
