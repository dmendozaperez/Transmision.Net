using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Transmision.Net.Basico;
using Transmision.Net.Basico.BataPos;

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

        private void button2_Click(object sender, EventArgs e)
        {
            Basico ejecuta = new Basico();
            Ent_Tk_Return tk = new Ent_Tk_Return();
            tk.cupon_imprimir = "BTRBF0LH01RL00DYI5";
            tk.text1_cup = "* BATA TE REGALA *";
            tk.text2_cup = "S/30.00 DSCTO EN TU PROXIMA COMPRA";
            tk.text3_cup = "";
            tk.text4_cup = "Por compras mayores o iguales a S/100.00 Soles en toda la tienda, realizadas del 28 de Noviembre al 02 de Diciembre del 2019 en una sola transacción, BATA te regala un cupón de descuento de S/30.00 Soles para ser utilizado durante el 03 al 09 de Diciembre del 2019 en tu siguiente compra mayor o igual a S/100.00 Soles en una sola transacción. Para hacer efectiva la promoción se hará entrega del cupón de descuento (impreso en el presente ticket de compra). Aplica para todas las tiendas BATA a nivel nacional. No acumulable con otras promociones. Solo aplica un descuento por transacción y por cliente. El cupón no puede ser canjeado por efectivo. Promoción sujeta a cambio sin previo aviso.";

            string error = "";
            Basico.imprimir(tk, "Ticket");

            //Basico._ticket_retorno(ref error);

            //ejecuta. ._ticket_retorno();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Basico ejecuta = new Basico();
            Ent_Tk_Return tk = new Ent_Tk_Return();
            tk.cupon_imprimir = "BTRBF0LH01RL00DYI5";
            tk.text1_cup = "* BATA TE REGALA *";
            tk.text2_cup = "S/30.00 DSCTO EN TU PROXIMA COMPRA";
            tk.text3_cup = "";
            tk.text4_cup = "Por compras mayores o iguales a S/100.00 Soles en toda la tienda, realizadas del 28 de Noviembre al 02 de Diciembre del 2019 en una sola transacción, BATA te regala un cupón de descuento de S/30.00 Soles para ser utilizado durante el 03 al 09 de Diciembre del 2019 en tu siguiente compra mayor o igual a S/100.00 Soles en una sola transacción. Para hacer efectiva la promoción se hará entrega del cupón de descuento (impreso en el presente ticket de compra). Aplica para todas las tiendas BATA a nivel nacional. No acumulable con otras promociones. Solo aplica un descuento por transacción y por cliente. El cupón no puede ser canjeado por efectivo. Promoción sujeta a cambio sin previo aviso.";

            string error = "";
            //Basico.imprimir2(tk, "Ticket");

            //String impresora = "ticket";
            //string barra =  "BTRBF0LH01RL00DYI5";
            //CrearTicket tk = new CrearTicket();
            //tk.TextoCentro("* BATA TE REGALA *");
            //tk.lineasGuio();
            //tk.TextoCentro("S/30.00 DSCTO EN TU PROXIMA COMPRA");
            //tk.lineasGuio();
            //tk.TextoIzquierda("Por compras mayores o iguales a S/100.00 Soles en toda la tienda, realizadas del 28 de Noviembre al 02 de Diciembre del 2019 en una sola transacción, BATA te regala un cupón de descuento de S/30.00 Soles para ser utilizado durante el 03 al 09 de Diciembre del 2019 en tu siguiente compra mayor o igual a S/100.00 Soles en una sola transacción. Para hacer efectiva la promoción se hará entrega del cupón de descuento (impreso en el presente ticket de compra). Aplica para todas las tiendas BATA a nivel nacional. No acumulable con otras promociones. Solo aplica un descuento por transacción y por cliente. El cupón no puede ser canjeado por efectivo. Promoción sujeta a cambio sin previo aviso.");
            //tk.lineasGuio();
            //tk.TextoCentro(barra);
            //tk.lineasGuio();
            //tk.ImprimirTicket(impresora);
            //tk.lineasGuio();

            //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
            //Codigo.IncludeLabel = true;
            //System.Drawing.Image im = Codigo.Encode(BarcodeLib.TYPE.CODE128, Barra, System.Drawing.Color.Black, System.Drawing.Color.White, 400, 100);

            //tk.HeaderImage = im;

            //tk.PrintQR(impresora);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string error = "";
            Basico._ticket_retorno(ref error);
            if (error.Length > 0)
            {
                MessageBox.Show(error);
            }
        }

        private void print_qr_Click(object sender, EventArgs e)
        {
            Basico ejecuta = new Basico();
            Ent_Tk_Return tk = new Ent_Tk_Return();
            tk.cupon_imprimir = "";
            tk.text1_cup = "* BATA TE REGALA *";
            tk.text2_cup = "*2X1 EN ENTRADAS AL CINE EN CINEMARK*";
            tk.text3_cup = "** USALO YA ** ";
            tk.text4_cup = "*Por compras mayores o igual a S/ 99.00 Soles en toda la tienda, realizadas del 01 al 29 de Febrero de 2020 en una sola transacción, Bata te regala un cupón de 2x1 en entradas 2D al cine en cualquier local de Cinemark para ser utilizado durante el 01 de febrero al 15 de marzo del 2020. La promoción es válida en los locales Cinemark de Lima, Arequipa, Huancayo y Trujillo. No aplica para asientos DBOX ni sala XD. No válido para películas con la restricción del distribuidor según el tiempo que indiquen. Para hacer efectiva la promoción se hará entrega del cupón en cualquier boletería de Cinemark. No valido para pre ventas, pre estrenos, contenido alternativo ni compra online. Indispensable presentar este cupon en la boletería para acceder al descuento. No aplica con otros descuentos y/ promociones. Promoción válida solo para personas mayores de 18 años.";

            string error = "";
            Basico.imprimir_qr(tk, "Ticket");
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btnreimporimir_Click(object sender, EventArgs e)
        {
            Basico ejecuta = new Basico();
            //Ent_Tk_Return tk = new Ent_Tk_Return();
            //tk.cupon_imprimir = "";
            //tk.text1_cup = "";
            //tk.text2_cup = "";
            //tk.text3_cup = "";
            //tk.text4_cup = "";

            Ent_Tk_Return tk = reim("50143");

            string error = "";
            Basico.imprimir(tk, "Ticket");
        }
        private Ent_Tk_Return reim(string tienda)
        {
            string sqlquery = "[USP_BATA_GET_TKRETURN_REIMPR_BK]";
            string con = "Server=172.28.7.14;Database=BDPOS;User ID=pos_oracle;Password=Bata2018**;Trusted_Connection=False;";
            Ent_Tk_Return tk = null;
            try
            {
                tk = new Ent_Tk_Return();
                using (SqlConnection cn = new SqlConnection(con))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COD_TDA", tienda);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            tk.cupon_imprimir = dt.Rows[0]["cup_rtn_barra"].ToString();
                            tk.text1_cup= dt.Rows[0]["tex1_cup"].ToString();
                            tk.text2_cup= dt.Rows[0]["tex2_cup"].ToString();
                            tk.text3_cup = dt.Rows[0]["tex3_cup"].ToString();
                            tk.text4_cup = dt.Rows[0]["tex4_cup"].ToString();
                            

                        }
                    }
                }
            }
            catch (Exception)
            {

               
            }
            return tk;
        }

        private void btnruleta_Click(object sender, EventArgs e)
        {
            string _error = "";
            //Basico ejecuta = new Basico();
            Basico._ruleta_bata(ref _error);

            //ejecuta.  _ruleta_bata(ref _error);
        }
    }
}
