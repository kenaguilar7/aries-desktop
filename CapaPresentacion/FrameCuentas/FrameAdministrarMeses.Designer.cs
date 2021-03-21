namespace CapaPresentacion.FrameCuentas
{
    partial class FrameAdministrarMeses
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtRegistros = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.lstAbrirMes = new System.Windows.Forms.ComboBox();
            this.txtSeleccione = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnCerrarMes = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lstCerrarMes = new System.Windows.Forms.ComboBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.groupBoxSalir = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.groupBoxSalir.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtRegistros);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(7, 133);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 265);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // dtRegistros
            // 
            this.dtRegistros.AllowUserToAddRows = false;
            this.dtRegistros.AllowUserToOrderColumns = true;
            this.dtRegistros.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dtRegistros.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtRegistros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtRegistros.EnableHeadersVisualStyles = false;
            this.dtRegistros.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dtRegistros.Location = new System.Drawing.Point(3, 18);
            this.dtRegistros.Name = "dtRegistros";
            this.dtRegistros.ReadOnly = true;
            this.dtRegistros.RowHeadersVisible = false;
            this.dtRegistros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtRegistros.Size = new System.Drawing.Size(502, 244);
            this.dtRegistros.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(5, 11);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(502, 88);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.btnGuardar);
            this.tabPage1.Controls.Add(this.lstAbrirMes);
            this.tabPage1.Controls.Add(this.txtSeleccione);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(494, 55);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Abrir";
            // 
            // btnGuardar
            // 
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(352, 17);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(97, 23);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Abrir mes";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // lstAbrirMes
            // 
            this.lstAbrirMes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstAbrirMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstAbrirMes.FormattingEnabled = true;
            this.lstAbrirMes.Location = new System.Drawing.Point(100, 16);
            this.lstAbrirMes.Name = "lstAbrirMes";
            this.lstAbrirMes.Size = new System.Drawing.Size(250, 24);
            this.lstAbrirMes.TabIndex = 5;
            // 
            // txtSeleccione
            // 
            this.txtSeleccione.AutoSize = true;
            this.txtSeleccione.Location = new System.Drawing.Point(19, 21);
            this.txtSeleccione.Name = "txtSeleccione";
            this.txtSeleccione.Size = new System.Drawing.Size(73, 17);
            this.txtSeleccione.TabIndex = 2;
            this.txtSeleccione.Text = "Seleccione:";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.btnCerrarMes);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.lstCerrarMes);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(494, 55);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Cerrar";
            // 
            // btnCerrarMes
            // 
            this.btnCerrarMes.FlatAppearance.BorderSize = 0;
            this.btnCerrarMes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarMes.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnCerrarMes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarMes.Location = new System.Drawing.Point(352, 17);
            this.btnCerrarMes.Name = "btnCerrarMes";
            this.btnCerrarMes.Size = new System.Drawing.Size(105, 23);
            this.btnCerrarMes.TabIndex = 9;
            this.btnCerrarMes.Text = "Cerrar mes";
            this.btnCerrarMes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrarMes.UseVisualStyleBackColor = true;
            this.btnCerrarMes.Click += new System.EventHandler(this.BtnCerrarMes_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Seleccione: ";
            // 
            // lstCerrarMes
            // 
            this.lstCerrarMes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstCerrarMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCerrarMes.FormattingEnabled = true;
            this.lstCerrarMes.Location = new System.Drawing.Point(100, 16);
            this.lstCerrarMes.Name = "lstCerrarMes";
            this.lstCerrarMes.Size = new System.Drawing.Size(250, 24);
            this.lstCerrarMes.TabIndex = 5;
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.btnCerrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrar.Location = new System.Drawing.Point(423, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(76, 30);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "&Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.BtnCerrar_Click);
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.tabControl1);
            this.groupBoxOptions.Location = new System.Drawing.Point(7, 14);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(508, 105);
            this.groupBoxOptions.TabIndex = 3;
            this.groupBoxOptions.TabStop = false;
            // 
            // groupBoxSalir
            // 
            this.groupBoxSalir.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSalir.Location = new System.Drawing.Point(7, 409);
            this.groupBoxSalir.Name = "groupBoxSalir";
            this.groupBoxSalir.Size = new System.Drawing.Size(508, 55);
            this.groupBoxSalir.TabIndex = 12;
            this.groupBoxSalir.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCerrar);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(502, 34);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // FrameAdministrarMeses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 474);
            this.Controls.Add(this.groupBoxSalir);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameAdministrarMeses";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administar Meses";
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxSalir.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dtRegistros;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label txtSeleccione;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCerrarMes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox lstCerrarMes;
        private System.Windows.Forms.ComboBox lstAbrirMes;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.GroupBox groupBoxSalir;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}