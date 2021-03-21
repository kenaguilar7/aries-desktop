namespace CapaPresentacion.Reportes
{
    partial class ReporteCompañia
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rbtJuridicas = new System.Windows.Forms.RadioButton();
            this.rbtFisicas = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lstIds = new System.Windows.Forms.ComboBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dsd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.debitos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnMoneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Activo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelExporta = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).BeginInit();
            this.panelExporta.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbtJuridicas
            // 
            this.rbtJuridicas.AutoSize = true;
            this.rbtJuridicas.Checked = true;
            this.rbtJuridicas.Location = new System.Drawing.Point(20, 27);
            this.rbtJuridicas.Name = "rbtJuridicas";
            this.rbtJuridicas.Size = new System.Drawing.Size(136, 21);
            this.rbtJuridicas.TabIndex = 3;
            this.rbtJuridicas.TabStop = true;
            this.rbtJuridicas.Text = "Personas Juridicas";
            this.rbtJuridicas.UseVisualStyleBackColor = true;
            this.rbtJuridicas.CheckedChanged += new System.EventHandler(this.RadiosbuttonChanceStatus);
            // 
            // rbtFisicas
            // 
            this.rbtFisicas.AutoSize = true;
            this.rbtFisicas.Location = new System.Drawing.Point(166, 27);
            this.rbtFisicas.Name = "rbtFisicas";
            this.rbtFisicas.Size = new System.Drawing.Size(123, 21);
            this.rbtFisicas.TabIndex = 2;
            this.rbtFisicas.Text = "Personas Fisicas";
            this.rbtFisicas.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(317, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Filtrar";
            // 
            // lstIds
            // 
            this.lstIds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstIds.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstIds.FormattingEnabled = true;
            this.lstIds.Items.AddRange(new object[] {
            "Ver Todos",
            "Cédula Fisica",
            "Dimex",
            "Nite"});
            this.lstIds.Location = new System.Drawing.Point(368, 25);
            this.lstIds.Name = "lstIds";
            this.lstIds.Size = new System.Drawing.Size(133, 25);
            this.lstIds.TabIndex = 0;
            this.lstIds.SelectedIndexChanged += new System.EventHandler(this.LstIds_SelectedIndexChanged);
            // 
            // GridDatos
            // 
            this.GridDatos.AllowUserToAddRows = false;
            this.GridDatos.AllowUserToDeleteRows = false;
            this.GridDatos.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GridDatos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GridDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewButtonColumn1,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.dsd,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.debitos,
            this.ColumnMoneda,
            this.Activo});
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
            this.GridDatos.Size = new System.Drawing.Size(1159, 386);
            this.GridDatos.TabIndex = 4;
            this.GridDatos.TabStop = false;
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.FillWeight = 137.0074F;
            this.dataGridViewButtonColumn1.HeaderText = "Código";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.ReadOnly = true;
            this.dataGridViewButtonColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewButtonColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewButtonColumn1.Width = 57;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 76.566F;
            this.Column1.HeaderText = "Tipo ID";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 75;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 76.566F;
            this.Column2.HeaderText = "Número Identicación";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 154;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 76.566F;
            this.Column3.HeaderText = "Nombre";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 82;
            // 
            // Column4
            // 
            this.Column4.FillWeight = 76.566F;
            this.Column4.HeaderText = "Primer Apellido";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 123;
            // 
            // Column5
            // 
            this.Column5.FillWeight = 76.566F;
            this.Column5.HeaderText = "Segundo Apellido ";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 141;
            // 
            // Column6
            // 
            this.Column6.FillWeight = 76.566F;
            this.Column6.HeaderText = "Dirección";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 87;
            // 
            // Column7
            // 
            this.Column7.FillWeight = 76.566F;
            this.Column7.HeaderText = "Sitio Web";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 88;
            // 
            // dsd
            // 
            this.dsd.FillWeight = 84.79263F;
            this.dsd.HeaderText = "Correro Electronico";
            this.dsd.Name = "dsd";
            this.dsd.ReadOnly = true;
            this.dsd.Width = 147;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 84.79263F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Teléfono";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 83;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle2.Format = "D";
            dataGridViewCellStyle2.NullValue = null;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn3.FillWeight = 84.79263F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Teléfono";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 83;
            // 
            // debitos
            // 
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.debitos.DefaultCellStyle = dataGridViewCellStyle3;
            this.debitos.FillWeight = 84.79263F;
            this.debitos.HeaderText = "Observaciones";
            this.debitos.Name = "debitos";
            this.debitos.ReadOnly = true;
            this.debitos.Width = 119;
            // 
            // ColumnMoneda
            // 
            this.ColumnMoneda.HeaderText = "Moneda";
            this.ColumnMoneda.Name = "ColumnMoneda";
            this.ColumnMoneda.ReadOnly = true;
            this.ColumnMoneda.Width = 82;
            // 
            // Activo
            // 
            this.Activo.FillWeight = 76.566F;
            this.Activo.HeaderText = "Activo";
            this.Activo.Name = "Activo";
            this.Activo.ReadOnly = true;
            this.Activo.Width = 68;
            // 
            // panelExporta
            // 
            this.panelExporta.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panelExporta.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelExporta.Controls.Add(this.rbtJuridicas);
            this.panelExporta.Controls.Add(this.button5);
            this.panelExporta.Controls.Add(this.rbtFisicas);
            this.panelExporta.Controls.Add(this.btnExcel);
            this.panelExporta.Controls.Add(this.label1);
            this.panelExporta.Controls.Add(this.lstIds);
            this.panelExporta.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelExporta.Location = new System.Drawing.Point(6, 5);
            this.panelExporta.Name = "panelExporta";
            this.panelExporta.Size = new System.Drawing.Size(1165, 67);
            this.panelExporta.TabIndex = 3;
            this.panelExporta.TabStop = false;
            this.panelExporta.Text = "EXPORTAR";
            // 
            // button5
            // 
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.Location = new System.Drawing.Point(1076, 22);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(76, 30);
            this.button5.TabIndex = 2;
            this.button5.Text = "Cerrar";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.CerrarVentana);
            // 
            // btnExcel
            // 
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(510, 22);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(91, 30);
            this.btnExcel.TabIndex = 2;
            this.btnExcel.Text = "Exportar";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.BtnExcel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GridDatos);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1165, 410);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // ReporteCompañia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 500);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelExporta);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ReporteCompañia";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Compañias";
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).EndInit();
            this.panelExporta.ResumeLayout(false);
            this.panelExporta.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RadioButton rbtJuridicas;
        private System.Windows.Forms.RadioButton rbtFisicas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstIds;
        private System.Windows.Forms.GroupBox panelExporta;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dsd;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitos;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMoneda;
        private System.Windows.Forms.DataGridViewTextBoxColumn Activo;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}