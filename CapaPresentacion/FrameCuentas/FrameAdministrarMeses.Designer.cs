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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtRegistros = new System.Windows.Forms.DataGridView();
            this.lstAbrirMes = new System.Windows.Forms.ComboBox();
            this.txtSeleccione = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtBoxUserNotes = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtGridClosingPeriodsReport = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.lstFromPeriod = new System.Windows.Forms.ComboBox();
            this.btnCerrarMes = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lstToPeriod = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridClosingPeriodsReport)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(10, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1119, 553);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.btnGuardar);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.lstAbrirMes);
            this.tabPage1.Controls.Add(this.txtSeleccione);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1111, 520);
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
            this.btnGuardar.Click += new System.EventHandler(this.Btn_Create_PostingPeriod);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dtRegistros);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1099, 441);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // dtRegistros
            // 
            this.dtRegistros.AllowUserToAddRows = false;
            this.dtRegistros.AllowUserToDeleteRows = false;
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
            this.dtRegistros.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dtRegistros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtRegistros.Size = new System.Drawing.Size(1093, 420);
            this.dtRegistros.TabIndex = 0;
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
            this.tabPage2.Controls.Add(this.txtBoxUserNotes);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.lstFromPeriod);
            this.tabPage2.Controls.Add(this.btnCerrarMes);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.lstToPeriod);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1111, 520);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Cerrar";
            // 
            // txtBoxUserNotes
            // 
            this.txtBoxUserNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxUserNotes.Location = new System.Drawing.Point(74, 67);
            this.txtBoxUserNotes.MaxLength = 100;
            this.txtBoxUserNotes.Multiline = true;
            this.txtBoxUserNotes.Name = "txtBoxUserNotes";
            this.txtBoxUserNotes.Size = new System.Drawing.Size(347, 50);
            this.txtBoxUserNotes.TabIndex = 54;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(24, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 17);
            this.label16.TabIndex = 55;
            this.label16.Text = "Notas";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dtGridClosingPeriodsReport);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 148);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1099, 333);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // dtGridClosingPeriodsReport
            // 
            this.dtGridClosingPeriodsReport.AllowUserToAddRows = false;
            this.dtGridClosingPeriodsReport.AllowUserToDeleteRows = false;
            this.dtGridClosingPeriodsReport.AllowUserToOrderColumns = true;
            this.dtGridClosingPeriodsReport.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dtGridClosingPeriodsReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtGridClosingPeriodsReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtGridClosingPeriodsReport.EnableHeadersVisualStyles = false;
            this.dtGridClosingPeriodsReport.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dtGridClosingPeriodsReport.Location = new System.Drawing.Point(3, 18);
            this.dtGridClosingPeriodsReport.Name = "dtGridClosingPeriodsReport";
            this.dtGridClosingPeriodsReport.ReadOnly = true;
            this.dtGridClosingPeriodsReport.RowHeadersVisible = false;
            this.dtGridClosingPeriodsReport.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dtGridClosingPeriodsReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtGridClosingPeriodsReport.Size = new System.Drawing.Size(1093, 312);
            this.dtGridClosingPeriodsReport.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(248, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Hasta";
            // 
            // lstFromPeriod
            // 
            this.lstFromPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstFromPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstFromPeriod.FormattingEnabled = true;
            this.lstFromPeriod.Location = new System.Drawing.Point(73, 17);
            this.lstFromPeriod.Name = "lstFromPeriod";
            this.lstFromPeriod.Size = new System.Drawing.Size(152, 24);
            this.lstFromPeriod.TabIndex = 10;
            this.lstFromPeriod.SelectedIndexChanged += new System.EventHandler(this.lstFromPeriod_SelectedIndexChanged);
            // 
            // btnCerrarMes
            // 
            this.btnCerrarMes.FlatAppearance.BorderSize = 0;
            this.btnCerrarMes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarMes.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnCerrarMes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarMes.Location = new System.Drawing.Point(439, 16);
            this.btnCerrarMes.Name = "btnCerrarMes";
            this.btnCerrarMes.Size = new System.Drawing.Size(130, 23);
            this.btnCerrarMes.TabIndex = 9;
            this.btnCerrarMes.Text = "Cerrar Período";
            this.btnCerrarMes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrarMes.UseVisualStyleBackColor = true;
            this.btnCerrarMes.Click += new System.EventHandler(this.BtnCerrarMes_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Desde:";
            // 
            // lstToPeriod
            // 
            this.lstToPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstToPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstToPeriod.FormattingEnabled = true;
            this.lstToPeriod.Location = new System.Drawing.Point(297, 17);
            this.lstToPeriod.Name = "lstToPeriod";
            this.lstToPeriod.Size = new System.Drawing.Size(124, 24);
            this.lstToPeriod.TabIndex = 5;
            // 
            // FrameAdministrarMeses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 577);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameAdministrarMeses";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administar Meses";
            this.Load += new System.EventHandler(this.FrameAdministrarMeses_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtGridClosingPeriodsReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label txtSeleccione;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCerrarMes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox lstToPeriod;
        private System.Windows.Forms.ComboBox lstAbrirMes;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstFromPeriod;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dtRegistros;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dtGridClosingPeriodsReport;
        private System.Windows.Forms.TextBox txtBoxUserNotes;
        private System.Windows.Forms.Label label16;
    }
}