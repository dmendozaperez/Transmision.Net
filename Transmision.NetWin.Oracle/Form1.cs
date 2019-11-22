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
using Transmision.Net.Basico.Oracle.BataTransaction;
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
                Ent_Acceso_BD.server = txtserver.Text;
                Ent_Acceso_BD.user = txtusuario.Text;
                Ent_Acceso_BD.password = txtpassword.Text;
                Ent_Acceso_BD.port =Convert.ToInt32(txtport.Text);
                Ent_Acceso_BD.sid = txtsid.Text;
                Ent_Acceso_BD.nom_tabla = "BAT_TK_RETURN";
                Dat_Ora_Data data_ora = new Dat_Ora_Data();
                #endregion
                #region<ORACLE CREACION Y EXISTENCIA>
                Boolean existe = data_ora.existe_tabla();
                if (!existe)
                {
                    data_ora.crear_table();
                }
                #endregion

                #region<REGION DE TRANSACCIONES AL TEMPORAL>
                DataTable dtventa_ora = data_ora.get_documento_TRN_TRANS(fecha);

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

                            /*VERIFICAR QUE NO EXISTA EL DOCUMENTO*/
                            Boolean existe_tmp = data_ora.existe_tabla_temp(param);
                            if (!existe_tmp)
                            { 
                                Boolean insert = data_ora.inserta_tabla_temp(param);
                                /*si el insert es correcto entonces hacemos un update*/
                                if (insert)
                                {
                                    data_ora.update_documento_TRN_TRANS(param);
                                }
                            }
                            else
                            {
                                /*realizar el update en la tabla principal*/
                                data_ora.update_documento_TRN_TRANS(param);
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
                Ent_Acceso_BD.server = txtserver.Text;
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

                            Ent_Tk_Return env = data_ora.ws_envio_param(param);
                            /*en este caso quiere decir que no hay errores en el envio*/
                            if (env.estado_error.Length==0)
                            {
                                Int32 RTL_LOC_ID =Convert.ToInt32(fila["RTL_LOC_ID"]);
                                Int32 WKSTN_ID = Convert.ToInt32(fila["WKSTN_ID"]);
                                Int32 TRANS_SEQ = Convert.ToInt32(fila["TRANS_SEQ"]);
                                string FISCAL_NUMBER = fila["FISCAL_NUMBER"].ToString();

                                /*en este caso vemos que se genero el cupon*/
                                //if (env.genera_cupon==1)
                                //{
                                data_ora.update_tmp_ora(env.cupon_imprimir, RTL_LOC_ID, WKSTN_ID, TRANS_SEQ, FISCAL_NUMBER);
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
    }
}
