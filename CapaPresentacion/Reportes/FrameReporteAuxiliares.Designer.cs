namespace CapaPresentacion.Reportes
{
    partial class FrameReporteAuxiliares
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnGenerarExcel = new System.Windows.Forms.Button();
            this.lstMesFinal = new System.Windows.Forms.ComboBox();
            this.lstMesInicio = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtGridReportContainer = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridReportContainer)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnGenerar);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.btnGenerarExcel);
            this.groupBox1.Controls.Add(this.lstMesFinal);
            this.groupBox1.Controls.Add(this.lstMesInicio);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(1245, 154);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnGenerar
            // 
            this.btnGenerar.FlatAppearance.BorderSize = 0;
            this.btnGenerar.Image = global::CapaPresentacion.Properties.Resources.icons8_historial_de_pedidos_25;
            this.btnGenerar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerar.Location = new System.Drawing.Point(678, 51);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(131, 46);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "&Generar";
            this.btnGenerar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(1114, 51);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(123, 46);
            this.btnSalir.TabIndex = 5;
            this.btnSalir.Text = "&Cerrar";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.BtnSalir_Click);
            // 
            // btnGenerarExcel
            // 
            this.btnGenerarExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerarExcel.FlatAppearance.BorderSize = 0;
            this.btnGenerarExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnGenerarExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerarExcel.Location = new System.Drawing.Point(971, 53);
            this.btnGenerarExcel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnGenerarExcel.Name = "btnGenerarExcel";
            this.btnGenerarExcel.Size = new System.Drawing.Size(135, 46);
            this.btnGenerarExcel.TabIndex = 4;
            this.btnGenerarExcel.Text = "&Exportar";
            this.btnGenerarExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGenerarExcel.UseVisualStyleBackColor = true;
            this.btnGenerarExcel.Click += new System.EventHandler(this.btnGenerarExcel_Click);
            // 
            // lstMesFinal
            // 
            this.lstMesFinal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMesFinal.FormattingEnabled = true;
            this.lstMesFinal.Location = new System.Drawing.Point(480, 57);
            this.lstMesFinal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstMesFinal.Name = "lstMesFinal";
            this.lstMesFinal.Size = new System.Drawing.Size(190, 36);
            this.lstMesFinal.TabIndex = 3;
            // 
            // lstMesInicio
            // 
            this.lstMesInicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMesInicio.FormattingEnabled = true;
            this.lstMesInicio.Location = new System.Drawing.Point(142, 57);
            this.lstMesInicio.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstMesInicio.Name = "lstMesInicio";
            this.lstMesInicio.Size = new System.Drawing.Size(190, 36);
            this.lstMesInicio.TabIndex = 2;
            this.lstMesInicio.SelectedIndexChanged += new System.EventHandler(this.lstMesInicio_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(375, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 28);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mes final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mes inicial:";
            // 
            // dtGridReportContainer
            // 
            this.dtGridReportContainer.AllowUserToAddRows = false;
            this.dtGridReportContainer.AllowUserToDeleteRows = false;
            this.dtGridReportContainer.AllowUserToOrderColumns = true;
            this.dtGridReportContainer.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dtGridReportContainer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dtGridReportContainer.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtGridReportContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtGridReportContainer.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dtGridReportContainer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGridReportContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtGridReportContainer.EnableHeadersVisualStyles = false;
            this.dtGridReportContainer.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dtGridReportContainer.Location = new System.Drawing.Point(3, 22);
            this.dtGridReportContainer.Name = "dtGridReportContainer";
            this.dtGridReportContainer.ReadOnly = true;
            this.dtGridReportContainer.RowHeadersVisible = false;
            this.dtGridReportContainer.RowHeadersWidth = 62;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtGridReportContainer.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dtGridReportContainer.RowTemplate.Height = 28;
            this.dtGridReportContainer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtGridReportContainer.Size = new System.Drawing.Size(1239, 424);
            this.dtGridReportContainer.TabIndex = 4;
            this.dtGridReportContainer.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dtGridReportContainer);
            this.groupBox2.Location = new System.Drawing.Point(14, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1245, 449);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // FrameReporteAuxiliares
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1279, 634);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrameReporteAuxiliares";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Auxiliares";
            this.Load += new System.EventHandler(this.FrameReporteAuxiliares_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridReportContainer)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox lstMesFinal;
        private System.Windows.Forms.ComboBox lstMesInicio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerarExcel;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.DataGridView dtGridReportContainer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnGenerar;
    }
}