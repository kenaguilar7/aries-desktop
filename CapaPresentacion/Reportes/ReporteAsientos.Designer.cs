namespace CapaPresentacion.Reportes
{
    partial class ReporteAsientos
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstEndPeriod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstStarPeriod = new System.Windows.Forms.ComboBox();
            this.CheckBoxShowAllPeriods = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.btnCerrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrar.Location = new System.Drawing.Point(1153, 25);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(76, 30);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.CloseWindows);
            // 
            // btnExcel
            // 
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(1056, 25);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(91, 30);
            this.btnExcel.TabIndex = 2;
            this.btnExcel.Text = "Exportar";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.BtnExportToExcel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GridDatos);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1247, 456);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // GridDatos
            // 
            this.GridDatos.AllowUserToAddRows = false;
            this.GridDatos.AllowUserToDeleteRows = false;
            this.GridDatos.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GridDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridDatos.EnableHeadersVisualStyles = false;
            this.GridDatos.GridColor = System.Drawing.SystemColors.ControlLight;
            this.GridDatos.Location = new System.Drawing.Point(3, 21);
            this.GridDatos.Name = "GridDatos";
            this.GridDatos.ReadOnly = true;
            this.GridDatos.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.GridDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridDatos.Size = new System.Drawing.Size(1241, 432);
            this.GridDatos.TabIndex = 4;
            this.GridDatos.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstEndPeriod);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnCerrar);
            this.groupBox2.Controls.Add(this.lstStarPeriod);
            this.groupBox2.Controls.Add(this.btnExcel);
            this.groupBox2.Controls.Add(this.CheckBoxShowAllPeriods);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(13, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1245, 65);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "HERRAMIENTAS";
            // 
            // lstEndPeriod
            // 
            this.lstEndPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstEndPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEndPeriod.FormattingEnabled = true;
            this.lstEndPeriod.Location = new System.Drawing.Point(327, 27);
            this.lstEndPeriod.Name = "lstEndPeriod";
            this.lstEndPeriod.Size = new System.Drawing.Size(130, 24);
            this.lstEndPeriod.TabIndex = 16;
            this.lstEndPeriod.SelectedIndexChanged += new System.EventHandler(this.LstEndPostingPeriod_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Mes Final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Mes Inicial:";
            // 
            // lstStarPeriod
            // 
            this.lstStarPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstStarPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstStarPeriod.FormattingEnabled = true;
            this.lstStarPeriod.Location = new System.Drawing.Point(101, 27);
            this.lstStarPeriod.Name = "lstStarPeriod";
            this.lstStarPeriod.Size = new System.Drawing.Size(130, 24);
            this.lstStarPeriod.TabIndex = 13;
            this.lstStarPeriod.SelectedIndexChanged += new System.EventHandler(this.LstFirstPostingPeriod_SelectedIndexChanged);
            // 
            // CheckBoxShowAllPeriods
            // 
            this.CheckBoxShowAllPeriods.AutoSize = true;
            this.CheckBoxShowAllPeriods.Location = new System.Drawing.Point(476, 28);
            this.CheckBoxShowAllPeriods.Name = "CheckBoxShowAllPeriods";
            this.CheckBoxShowAllPeriods.Size = new System.Drawing.Size(159, 21);
            this.CheckBoxShowAllPeriods.TabIndex = 0;
            this.CheckBoxShowAllPeriods.Text = "Traer todos los meses";
            this.CheckBoxShowAllPeriods.UseVisualStyleBackColor = true;
            this.CheckBoxShowAllPeriods.CheckedChanged += new System.EventHandler(this.CheckBoxShowAllPeriods_Checked);
            // 
            // ReporteAsientos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 553);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ReporteAsientos";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Asientos";
            this.Load += new System.EventHandler(this.ReporteAsientos_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox CheckBoxShowAllPeriods;
        private System.Windows.Forms.ComboBox lstStarPeriod;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstEndPeriod;
        private System.Windows.Forms.Label label2;
    }
}