namespace Transmision.NetWin.Oracle
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtsid = new System.Windows.Forms.TextBox();
            this.txtport = new System.Windows.Forms.TextBox();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.txtusuario = new System.Windows.Forms.TextBox();
            this.txtserver = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnconectar = new System.Windows.Forms.Button();
            this.btnenvio_ws = new System.Windows.Forms.Button();
            this.btnprueba_servicio = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_updservice = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnejecuta_envio_poslog = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtsid);
            this.groupBox1.Controls.Add(this.txtport);
            this.groupBox1.Controls.Add(this.txtpassword);
            this.groupBox1.Controls.Add(this.txtusuario);
            this.groupBox1.Controls.Add(this.txtserver);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(28, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CONEXION";
            // 
            // txtsid
            // 
            this.txtsid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsid.Location = new System.Drawing.Point(120, 134);
            this.txtsid.Name = "txtsid";
            this.txtsid.Size = new System.Drawing.Size(126, 20);
            this.txtsid.TabIndex = 9;
            this.txtsid.Text = "XSTOREDB";
            // 
            // txtport
            // 
            this.txtport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtport.Location = new System.Drawing.Point(120, 106);
            this.txtport.Name = "txtport";
            this.txtport.Size = new System.Drawing.Size(100, 20);
            this.txtport.TabIndex = 8;
            this.txtport.Text = "1521";
            // 
            // txtpassword
            // 
            this.txtpassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpassword.Location = new System.Drawing.Point(120, 77);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '*';
            this.txtpassword.Size = new System.Drawing.Size(100, 20);
            this.txtpassword.TabIndex = 7;
            this.txtpassword.Text = "dtv";
            // 
            // txtusuario
            // 
            this.txtusuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtusuario.Location = new System.Drawing.Point(120, 48);
            this.txtusuario.Name = "txtusuario";
            this.txtusuario.Size = new System.Drawing.Size(100, 20);
            this.txtusuario.TabIndex = 6;
            this.txtusuario.Text = "dtv";
            // 
            // txtserver
            // 
            this.txtserver.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtserver.Location = new System.Drawing.Point(120, 20);
            this.txtserver.Name = "txtserver";
            this.txtserver.Size = new System.Drawing.Size(160, 20);
            this.txtserver.TabIndex = 5;
            this.txtserver.Text = "172.19.4.40";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "SERVER";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "SID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PORT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "PASSWORD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "USUARIO";
            // 
            // btnconectar
            // 
            this.btnconectar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnconectar.Location = new System.Drawing.Point(370, 9);
            this.btnconectar.Name = "btnconectar";
            this.btnconectar.Size = new System.Drawing.Size(251, 48);
            this.btnconectar.TabIndex = 1;
            this.btnconectar.Text = "Transacciones Locales ORA";
            this.btnconectar.UseVisualStyleBackColor = true;
            this.btnconectar.Click += new System.EventHandler(this.btnconectar_Click);
            // 
            // btnenvio_ws
            // 
            this.btnenvio_ws.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnenvio_ws.Location = new System.Drawing.Point(370, 74);
            this.btnenvio_ws.Name = "btnenvio_ws";
            this.btnenvio_ws.Size = new System.Drawing.Size(250, 44);
            this.btnenvio_ws.TabIndex = 2;
            this.btnenvio_ws.Text = "Transacciones Envio WEB SERVICE";
            this.btnenvio_ws.UseVisualStyleBackColor = true;
            this.btnenvio_ws.Click += new System.EventHandler(this.btnenvio_ws_Click);
            // 
            // btnprueba_servicio
            // 
            this.btnprueba_servicio.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnprueba_servicio.Location = new System.Drawing.Point(369, 134);
            this.btnprueba_servicio.Name = "btnprueba_servicio";
            this.btnprueba_servicio.Size = new System.Drawing.Size(248, 42);
            this.btnprueba_servicio.TabIndex = 3;
            this.btnprueba_servicio.Text = "PRUEBA SERVICIO CONFIG";
            this.btnprueba_servicio.UseVisualStyleBackColor = true;
            this.btnprueba_servicio.Click += new System.EventHandler(this.btnprueba_servicio_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(369, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(248, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "IMPRIMIR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_updservice
            // 
            this.btn_updservice.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_updservice.Location = new System.Drawing.Point(29, 215);
            this.btn_updservice.Name = "btn_updservice";
            this.btn_updservice.Size = new System.Drawing.Size(219, 23);
            this.btn_updservice.TabIndex = 5;
            this.btn_updservice.Text = "UPDATE SERVICE";
            this.btn_updservice.UseVisualStyleBackColor = true;
            this.btn_updservice.Click += new System.EventHandler(this.btn_updservice_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnejecuta_envio_poslog);
            this.groupBox2.Location = new System.Drawing.Point(28, 288);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(325, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Procesos de envio POSLOG";
            // 
            // btnejecuta_envio_poslog
            // 
            this.btnejecuta_envio_poslog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnejecuta_envio_poslog.Location = new System.Drawing.Point(15, 33);
            this.btnejecuta_envio_poslog.Name = "btnejecuta_envio_poslog";
            this.btnejecuta_envio_poslog.Size = new System.Drawing.Size(289, 29);
            this.btnejecuta_envio_poslog.TabIndex = 0;
            this.btnejecuta_envio_poslog.Text = "ejecuta_envio_poslog";
            this.btnejecuta_envio_poslog.UseVisualStyleBackColor = true;
            this.btnejecuta_envio_poslog.Click += new System.EventHandler(this.btnejecuta_envio_poslog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 430);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_updservice);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnprueba_servicio);
            this.Controls.Add(this.btnenvio_ws);
            this.Controls.Add(this.btnconectar);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ACCESO BD ORACLE";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtsid;
        private System.Windows.Forms.TextBox txtport;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.TextBox txtusuario;
        private System.Windows.Forms.TextBox txtserver;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnconectar;
        private System.Windows.Forms.Button btnenvio_ws;
        private System.Windows.Forms.Button btnprueba_servicio;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_updservice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnejecuta_envio_poslog;
    }
}

