namespace Transmision.NetWin
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnrecepcion = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txttda = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTipo = new System.Windows.Forms.TextBox();
            this.txtAncho = new System.Windows.Forms.TextBox();
            this.txtAlto = new System.Windows.Forms.TextBox();
            this.txtCalidad = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "ejecutar ws";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnrecepcion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txttda);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(25, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 140);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recepcion de Guias";
            // 
            // btnrecepcion
            // 
            this.btnrecepcion.Location = new System.Drawing.Point(58, 102);
            this.btnrecepcion.Name = "btnrecepcion";
            this.btnrecepcion.Size = new System.Drawing.Size(167, 23);
            this.btnrecepcion.TabIndex = 2;
            this.btnrecepcion.Text = "Recepcion de Guia";
            this.btnrecepcion.UseVisualStyleBackColor = true;
            this.btnrecepcion.Click += new System.EventHandler(this.btnrecepcion_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tienda";
            // 
            // txttda
            // 
            this.txttda.Location = new System.Drawing.Point(58, 51);
            this.txttda.Name = "txttda";
            this.txttda.Size = new System.Drawing.Size(100, 20);
            this.txttda.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(296, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(169, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "prueba reimp";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(296, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(169, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "BTRBF0LH01RL00DYI5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(296, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tipo codigo";
            // 
            // txtTipo
            // 
            this.txtTipo.Location = new System.Drawing.Point(365, 65);
            this.txtTipo.Name = "txtTipo";
            this.txtTipo.Size = new System.Drawing.Size(100, 20);
            this.txtTipo.TabIndex = 5;
            this.txtTipo.Text = "30";
            // 
            // txtAncho
            // 
            this.txtAncho.Location = new System.Drawing.Point(365, 91);
            this.txtAncho.Name = "txtAncho";
            this.txtAncho.Size = new System.Drawing.Size(100, 20);
            this.txtAncho.TabIndex = 6;
            this.txtAncho.Text = "240";
            // 
            // txtAlto
            // 
            this.txtAlto.Location = new System.Drawing.Point(365, 118);
            this.txtAlto.Name = "txtAlto";
            this.txtAlto.Size = new System.Drawing.Size(100, 20);
            this.txtAlto.TabIndex = 7;
            this.txtAlto.Text = "40";
            // 
            // txtCalidad
            // 
            this.txtCalidad.Location = new System.Drawing.Point(365, 145);
            this.txtCalidad.Name = "txtCalidad";
            this.txtCalidad.Size = new System.Drawing.Size(100, 20);
            this.txtCalidad.TabIndex = 8;
            this.txtCalidad.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Alto";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(300, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Ancho";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(296, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Calidad";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 262);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCalidad);
            this.Controls.Add(this.txtAlto);
            this.Controls.Add(this.txtAncho);
            this.Controls.Add(this.txtTipo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnrecepcion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txttda;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTipo;
        private System.Windows.Forms.TextBox txtAncho;
        private System.Windows.Forms.TextBox txtAlto;
        private System.Windows.Forms.TextBox txtCalidad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

