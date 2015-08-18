namespace Services
{
    partial class Principal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.conexaoPn = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.conServTb = new System.Windows.Forms.TextBox();
            this.conUserTb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.conPass1Tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.conPass2Tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.conConectaBt = new System.Windows.Forms.Button();
            this.conexaoPn.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // conexaoPn
            // 
            this.conexaoPn.Controls.Add(this.conConectaBt);
            this.conexaoPn.Controls.Add(this.conPass2Tb);
            this.conexaoPn.Controls.Add(this.label5);
            this.conexaoPn.Controls.Add(this.conPass1Tb);
            this.conexaoPn.Controls.Add(this.label4);
            this.conexaoPn.Controls.Add(this.conUserTb);
            this.conexaoPn.Controls.Add(this.label3);
            this.conexaoPn.Controls.Add(this.conServTb);
            this.conexaoPn.Controls.Add(this.label2);
            this.conexaoPn.Controls.Add(this.panel1);
            this.conexaoPn.Location = new System.Drawing.Point(302, 136);
            this.conexaoPn.Name = "conexaoPn";
            this.conexaoPn.Size = new System.Drawing.Size(194, 218);
            this.conexaoPn.TabIndex = 0;
            this.conexaoPn.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 31);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Banco de Dados";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Servidor";
            // 
            // conServTb
            // 
            this.conServTb.Dock = System.Windows.Forms.DockStyle.Top;
            this.conServTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conServTb.Location = new System.Drawing.Point(0, 44);
            this.conServTb.Name = "conServTb";
            this.conServTb.Size = new System.Drawing.Size(194, 23);
            this.conServTb.TabIndex = 2;
            this.conServTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // conUserTb
            // 
            this.conUserTb.Dock = System.Windows.Forms.DockStyle.Top;
            this.conUserTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conUserTb.Location = new System.Drawing.Point(0, 80);
            this.conUserTb.Name = "conUserTb";
            this.conUserTb.Size = new System.Drawing.Size(194, 23);
            this.conUserTb.TabIndex = 4;
            this.conUserTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Usuario";
            // 
            // conPass1Tb
            // 
            this.conPass1Tb.Dock = System.Windows.Forms.DockStyle.Top;
            this.conPass1Tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conPass1Tb.Location = new System.Drawing.Point(0, 116);
            this.conPass1Tb.Name = "conPass1Tb";
            this.conPass1Tb.PasswordChar = '*';
            this.conPass1Tb.Size = new System.Drawing.Size(194, 23);
            this.conPass1Tb.TabIndex = 6;
            this.conPass1Tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Senha";
            // 
            // conPass2Tb
            // 
            this.conPass2Tb.Dock = System.Windows.Forms.DockStyle.Top;
            this.conPass2Tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conPass2Tb.Location = new System.Drawing.Point(0, 152);
            this.conPass2Tb.Name = "conPass2Tb";
            this.conPass2Tb.PasswordChar = '*';
            this.conPass2Tb.Size = new System.Drawing.Size(194, 23);
            this.conPass2Tb.TabIndex = 8;
            this.conPass2Tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(0, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Senha (repetir)";
            // 
            // conConectaBt
            // 
            this.conConectaBt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conConectaBt.Location = new System.Drawing.Point(0, 175);
            this.conConectaBt.Name = "conConectaBt";
            this.conConectaBt.Size = new System.Drawing.Size(194, 43);
            this.conConectaBt.TabIndex = 9;
            this.conConectaBt.Text = "Conecar";
            this.conConectaBt.UseVisualStyleBackColor = true;
            this.conConectaBt.Click += new System.EventHandler(this.conConectaBt_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.conexaoPn);
            this.Name = "Principal";
            this.Text = "Services";
            this.conexaoPn.ResumeLayout(false);
            this.conexaoPn.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel conexaoPn;
        private System.Windows.Forms.Button conConectaBt;
        private System.Windows.Forms.TextBox conPass2Tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox conPass1Tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox conUserTb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox conServTb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}

