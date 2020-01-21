using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transmision.Net.Basico.Oracle;
using Transmision.Net.Basico.Oracle.BataPos;
//using Transmision.Net.Basico.Oracle.BataTransaction;
using Transmision.Net.Basico.Oracle.CapaDato;
using Transmision.Net.Basico.Oracle.CapaEntidad;

namespace Transmision.NetWin.Oracle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnconectar_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {

                DateTime fecha = DateTime.Today;/*fecha de validacion*/

                #region<CONEXIONES DE ORACLE>
                Ent_Acceso_BD.server = "10.10.10.209";// txtserver.Text;
                Ent_Acceso_BD.user = txtusuario.Text;
                Ent_Acceso_BD.password = txtpassword.Text;
                Ent_Acceso_BD.port =Convert.ToInt32(txtport.Text);
                Ent_Acceso_BD.sid = txtsid.Text;
                Ent_Acceso_BD.nom_tabla = "BAT_TK_RETURN";
                Dat_Ora_Data data_ora = new Dat_Ora_Data();
                #endregion
                #region<ORACLE CREACION Y EXISTENCIA>
                string error = "";
                Boolean existe = data_ora.existe_tabla(ref error);
                if (!existe)
                {
                    data_ora.crear_table();
                }
                #endregion

                #region<REGION DE TRANSACCIONES AL TEMPORAL>
                DataTable dtventa_ora = data_ora.get_documento_TRN_TRANS(fecha,ref error);

                if (dtventa_ora!=null)
                {
                    if (dtventa_ora.Rows.Count>0)
                    {
                        foreach(DataRow fila in dtventa_ora.Rows)
                        {
                            Ent_Bat_Tk_Return param = new Ent_Bat_Tk_Return();
                            param.RTL_LOC_ID =Convert.ToDecimal(fila["RTL_LOC_ID"]);
                            param.BUSINESS_DATE = Convert.ToDateTime(fila["BUSINESS_DATE"]);
                            param.WKSTN_ID = Convert.ToDecimal(fila["WKSTN_ID"]);
                            param.TRANS_SEQ = Convert.ToDecimal(fila["TRANS_SEQ"]);
                            param.TOTAL = Convert.ToDecimal(fila["TOTAL"]);
                            param.FISCAL_NUMBER = fila["FISCAL_NUMBER"].ToString();
                            //string error = "";
                            /*VERIFICAR QUE NO EXISTA EL DOCUMENTO*/
                            Boolean existe_tmp = data_ora.existe_tabla_temp(param,ref  error);
                            if (!existe_tmp)
                            { 
                                Boolean insert = data_ora.inserta_tabla_temp(param,ref error);
                                /*si el insert es correcto entonces hacemos un update*/
                                if (insert)
                                {
                                    data_ora.update_documento_TRN_TRANS(param,ref error);
                                }
                            }
                            else
                            {
                                /*realizar el update en la tabla principal*/
                                data_ora.update_documento_TRN_TRANS(param,ref error);
                            }

                        }
                    }
                }
                #endregion


                MessageBox.Show("Trancciones Exitosa", "Mensaje..", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Mensaje..", MessageBoxButtons.OK, MessageBoxIcon.Error);    
                
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnenvio_ws_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                #region<CONEXIONES DE ORACLE>
                Ent_Acceso_BD.server = "172.19.4.40";// txtserver.Text;
                Ent_Acceso_BD.user = txtusuario.Text;
                Ent_Acceso_BD.password = txtpassword.Text;
                Ent_Acceso_BD.port = Convert.ToInt32(txtport.Text);
                Ent_Acceso_BD.sid = txtsid.Text;
                Ent_Acceso_BD.nom_tabla = "BAT_TK_RETURN";
                Dat_Ora_Data data_ora = new Dat_Ora_Data();
                #endregion
                #region<REGION DE ENVIO DE DATA AL SERVER WEB SERVICE>
                DataTable dt_envio = data_ora.select_tmp_ora();/*seleccionamos datos del oracle*/
                if (dt_envio!=null)
                {
                    if (dt_envio.Rows.Count>0)
                    {
                        foreach(DataRow fila in dt_envio.Rows)
                        {
                            String FC_SUNA = fila["FISCAL_NUMBER"].ToString().Substring(0,1)=="B"?"03":"01";
                            String FC_SFAC = fila["FISCAL_NUMBER"].ToString().Substring(0, 4).ToString();
                            String FC_NFAC= fila["FISCAL_NUMBER"].ToString().Substring(5, fila["FISCAL_NUMBER"].ToString().Length - 5).ToString().PadLeft(8,'0');

                            Ent_Tk_Set_Parametro param = new Ent_Tk_Set_Parametro();
                            param.COD_TDA = fila["RTL_LOC_ID"].ToString();
                            param.FECHA =Convert.ToDateTime(fila["BUSINESS_DATE"]);
                            param.MONTO= Convert.ToDecimal(fila["TOTAL"]);
                            param.FC_SUNA = FC_SUNA;
                            param.SERIE = FC_SFAC;
                            param.NUMERO = FC_NFAC;

                            string error = "";

                            Ent_Tk_Return env = data_ora.ws_envio_param(param,ref error);
                            /*en este caso quiere decir que no hay errores en el envio*/
                            if (env.estado_error.Length==0)
                            {
                                Int32 RTL_LOC_ID =Convert.ToInt32(fila["RTL_LOC_ID"]);
                                Int32 WKSTN_ID = Convert.ToInt32(fila["WKSTN_ID"]);
                                Int32 TRANS_SEQ = Convert.ToInt32(fila["TRANS_SEQ"]);
                                string FISCAL_NUMBER = fila["FISCAL_NUMBER"].ToString();
                                //string error = "";
                                /*en este caso vemos que se genero el cupon*/
                                //if (env.genera_cupon==1)
                                //{
                                data_ora.update_tmp_ora(env.cupon_imprimir, RTL_LOC_ID, WKSTN_ID, TRANS_SEQ, FISCAL_NUMBER,ref error);
                                //}
                                ////else
                                ////{

                                ////}
                            }

                           // param.
                        }
                    }
                }

                #endregion
            }
            catch (Exception exc)
            {

                MessageBox.Show(exc.Message, "Mensaje..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnprueba_servicio_Click(object sender, EventArgs e)
        {
            Basico ejecuta = new Basico();
            string error = ejecuta.ejecuta_proceso_oracle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Basico ejecuta = new Basico();
            //Ent_Tk_Return tk = new Ent_Tk_Return();
            //tk.cupon_imprimir = "BTRBF0LH01RL00DYI5";
            //tk.text1_cup = "* BATA TE REGALA *";
            //tk.text2_cup = "S/30.00 DSCTO EN TU PROXIMA COMPRA";
            //tk.text3_cup = "";
            //tk.text4_cup = "Por compras mayores o iguales a S/100.00 Soles en toda la tienda, realizadas del 28 de Noviembre al 02 de Diciembre del 2019 en una sola transacción, BATA te regala un cupón de descuento de S/30.00 Soles para ser utilizado durante el 03 al 09 de Diciembre del 2019 en tu siguiente compra mayor o igual a S/100.00 Soles en una sola transacción. Para hacer efectiva la promoción se hará entrega del cupón de descuento (impreso en el presente ticket de compra). Aplica para todas las tiendas BATA a nivel nacional. No acumulable con otras promociones. Solo aplica un descuento por transacción y por cliente. El cupón no puede ser canjeado por efectivo. Promoción sujeta a cambio sin previo aviso.";

            //ejecuta.imprimir(tk);
        }

        private void btn_updservice_Click(object sender, EventArgs e)
        {
            Basico_Update ejecuta = new Basico_Update();
            ejecuta.ejecuta_update_service();
        }

        private void btnejecuta_envio_poslog_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Basico proc_pos = new Basico();
            proc_pos.ejecuta_envio_poslog();
            MessageBox.Show("Terminado el proceso");
            Cursor.Current = Cursors.Default;
        }
    }
}
