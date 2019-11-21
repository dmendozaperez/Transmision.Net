using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            }
            catch (Exception exc)
            {

                MessageBox.Show(exc.Message, "Mensaje..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
