namespace CapaPresentacion.Reportes
{
    partial class FrameReporteComprobacion
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.chekImprimirSaldosCero = new System.Windows.Forms.CheckBox();
            this.lstMesesAbiertos = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ColumnSaldoAnteriorDebitosColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSaldoAnteriorCreditosColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSaldoMesColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSaldoMesCredtiosColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSaldoActualDebitosColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSaldoActualCreditosColones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GridDatos);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(5, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1247, 421);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // GridDatos
            // 
            this.GridDatos.AllowUserToAddRows = false;
            this.GridDatos.AllowUserToDeleteRows = false;
            this.GridDatos.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GridDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSaldoAnteriorDebitosColones,
            this.ColumnSaldoAnteriorCreditosColones,
            this.ColumnSaldoMesColones,
            this.ColumnSaldoMesCredtiosColones,
            this.ColumnSaldoActualDebitosColones,
            this.ColumnSaldoActualCreditosColones});
            this.GridDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridDatos.EnableHeadersVisualStyles = false;
            this.GridDatos.GridColor = System.Drawing.SystemColors.ControlLight;
            this.GridDatos.Location = new System.Drawing.Point(3, 21);
            this.GridDatos.Name = "GridDatos";
            this.GridDatos.ReadOnly = true;
            this.GridDatos.RowHeadersVisible = false;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.GridDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridDatos.Size = new System.Drawing.Size(1241, 397);
            this.GridDatos.TabIndex = 4;
            this.GridDatos.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSalir);
            this.groupBox2.Controls.Add(this.btnExcel);
            this.groupBox2.Controls.Add(this.chekImprimirSaldosCero);
            this.groupBox2.Controls.Add(this.lstMesesAbiertos);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(12, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1236, 73);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // btnSalir
            // 
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(1133, 29);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(80, 30);
            this.btnSalir.TabIndex = 13;
            this.btnSalir.Text = "&Cerrar";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.CerrarVentana);
            // 
            // btnExcel
            // 
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(641, 29);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(101, 30);
            this.btnExcel.TabIndex = 12;
            this.btnExcel.Text = "&Exportar ";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.BtnExcel_Click);
            // 
            // chekImprimirSaldosCero
            // 
            this.chekImprimirSaldosCero.AutoSize = true;
            this.chekImprimirSaldosCero.Location = new System.Drawing.Point(18, 34);
            this.chekImprimirSaldosCero.Name = "chekImprimirSaldosCero";
            this.chekImprimirSaldosCero.Size = new System.Drawing.Size(221, 21);
            this.chekImprimirSaldosCero.TabIndex = 2;
            this.chekImprimirSaldosCero.Text = "Imprimir solo cuentas con saldo";
            this.chekImprimirSaldosCero.UseVisualStyleBackColor = true;
            this.chekImprimirSaldosCero.CheckedChanged += new System.EventHandler(this.ChekImprimirSaldosCero_CheckedChanged);
            // 
            // lstMesesAbiertos
            // 
            this.lstMesesAbiertos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMesesAbiertos.FormattingEnabled = true;
            this.lstMesesAbiertos.Location = new System.Drawing.Point(423, 32);
            this.lstMesesAbiertos.Name = "lstMesesAbiertos";
            this.lstMesesAbiertos.Size = new System.Drawing.Size(207, 25);
            this.lstMesesAbiertos.TabIndex = 1;
            this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(320, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Balance al mes:";
            // 
            // ColumnSaldoAnteriorDebitosColones
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "₡#,0.00";
            dataGridViewCellStyle2.NullValue = null;
            this.ColumnSaldoAnteriorDebitosColones.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnSaldoAnteriorDebitosColones.HeaderText = "Saldo Anterior Debitos";
            this.ColumnSaldoAnteriorDebitosColones.Name = "ColumnSaldoAnteriorDebitosColones";
            this.ColumnSaldoAnteriorDebitosColones.ReadOnly = true;
            // 
            // ColumnSaldoAnteriorCreditosColones
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "₡#,0.00";
            dataGridViewCellStyle3.NullValue = null;
            this.ColumnSaldoAnteriorCreditosColones.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnSaldoAnteriorCreditosColones.HeaderText = "Saldo Anterior Creditos";
            this.ColumnSaldoAnteriorCreditosColones.Name = "ColumnSaldoAnteriorCreditosColones";
            this.ColumnSaldoAnteriorCreditosColones.ReadOnly = true;
            // 
            // ColumnSaldoMesColones
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "₡#,0.00";
            dataGridViewCellStyle4.NullValue = null;
            this.ColumnSaldoMesColones.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnSaldoMesColones.HeaderText = "Saldo Mes Debitos";
            this.ColumnSaldoMesColones.Name = "ColumnSaldoMesColones";
            this.ColumnSaldoMesColones.ReadOnly = true;
            // 
            // ColumnSaldoMesCredtiosColones
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "₡#,0.00";
            dataGridViewCellStyle5.NullValue = null;
            this.ColumnSaldoMesCredtiosColones.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnSaldoMesCredtiosColones.HeaderText = "Saldo Mes Creditos";
            this.ColumnSaldoMesCredtiosColones.Name = "ColumnSaldoMesCredtiosColones";
            this.ColumnSaldoMesCredtiosColones.ReadOnly = true;
            // 
            // ColumnSaldoActualDebitosColones
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "₡#,0.00";
            dataGridViewCellStyle6.NullValue = null;
            this.ColumnSaldoActualDebitosColones.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColumnSaldoActualDebitosColones.HeaderText = "Saldo Actual Debitos";
            this.ColumnSaldoActualDebitosColones.Name = "ColumnSaldoActualDebitosColones";
            this.ColumnSaldoActualDebitosColones.ReadOnly = true;
            // 
            // ColumnSaldoActualCreditosColones
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "₡#,0.00";
            dataGridViewCellStyle7.NullValue = null;
            this.ColumnSaldoActualCreditosColones.DefaultCellStyle = dataGridViewCellStyle7;
            this.ColumnSaldoActualCreditosColones.HeaderText = "Saldo Actual Creditos";
            this.ColumnSaldoActualCreditosColones.Name = "ColumnSaldoActualCreditosColones";
            this.ColumnSaldoActualCreditosColones.ReadOnly = true;
            // 
            // FrameReporteComprobacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1259, 523);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrameReporteComprobacion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Balance de Comprobacion";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox lstMesesAbiertos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chekImprimirSaldosCero;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoAnteriorDebitosColones;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoAnteriorCreditosColones;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoMesColones;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoMesCredtiosColones;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoActualDebitosColones;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSaldoActualCreditosColones;
    }
}