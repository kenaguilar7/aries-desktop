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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstMesesAbiertos = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.ColumnMesContable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAsiento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dsd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.debitos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.money_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.money_chance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_usd = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.btnCerrar.Click += new System.EventHandler(this.CerrarVentana);
            // 
            // btnExcel
            // 
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(523, 25);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(91, 30);
            this.btnExcel.TabIndex = 2;
            this.btnExcel.Text = "Exportar";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GridDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnMesContable,
            this.ColumnAsiento,
            this.dataGridViewButtonColumn1,
            this.dsd,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.debitos,
            this.dataGridViewTextBoxColumn2,
            this.money_type,
            this.money_chance,
            this.balance_usd});
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
            this.GridDatos.Size = new System.Drawing.Size(1241, 432);
            this.GridDatos.TabIndex = 4;
            this.GridDatos.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnCerrar);
            this.groupBox2.Controls.Add(this.lstMesesAbiertos);
            this.groupBox2.Controls.Add(this.btnExcel);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(13, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1245, 65);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "HERRAMIENTAS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(277, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Seleccione mes:";
            // 
            // lstMesesAbiertos
            // 
            this.lstMesesAbiertos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMesesAbiertos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstMesesAbiertos.FormattingEnabled = true;
            this.lstMesesAbiertos.Location = new System.Drawing.Point(386, 28);
            this.lstMesesAbiertos.Name = "lstMesesAbiertos";
            this.lstMesesAbiertos.Size = new System.Drawing.Size(130, 24);
            this.lstMesesAbiertos.TabIndex = 13;
            this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.lstMesesAbiertos_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(25, 28);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(159, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Traer todos los meses";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ColumnMesContable
            // 
            this.ColumnMesContable.DataPropertyName = "month_report";
            this.ColumnMesContable.HeaderText = "Mes Contable";
            this.ColumnMesContable.Name = "ColumnMesContable";
            this.ColumnMesContable.ReadOnly = true;
            // 
            // ColumnAsiento
            // 
            this.ColumnAsiento.DataPropertyName = "entry_id";
            this.ColumnAsiento.HeaderText = "Asiento";
            this.ColumnAsiento.Name = "ColumnAsiento";
            this.ColumnAsiento.ReadOnly = true;
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.DataPropertyName = "account_name";
            this.dataGridViewButtonColumn1.FillWeight = 110.7445F;
            this.dataGridViewButtonColumn1.HeaderText = "Cuenta";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.ReadOnly = true;
            this.dataGridViewButtonColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewButtonColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dsd
            // 
            this.dsd.DataPropertyName = "reference";
            this.dsd.FillWeight = 110.7445F;
            this.dsd.HeaderText = "Referencia";
            this.dsd.Name = "dsd";
            this.dsd.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "detail";
            this.dataGridViewTextBoxColumn1.FillWeight = 110.7445F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Detalle";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "bill_date";
            dataGridViewCellStyle2.Format = "D";
            dataGridViewCellStyle2.NullValue = null;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn3.FillWeight = 110.7445F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Fecha de documento";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // debitos
            // 
            this.debitos.DataPropertyName = "debit";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "₡#,0.00";
            dataGridViewCellStyle3.NullValue = null;
            this.debitos.DefaultCellStyle = dataGridViewCellStyle3;
            this.debitos.FillWeight = 110.7445F;
            this.debitos.HeaderText = "Debitos";
            this.debitos.Name = "debitos";
            this.debitos.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "credit";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "₡#,0.00";
            dataGridViewCellStyle4.NullValue = null;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn2.FillWeight = 110.7445F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Creditos";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // money_type
            // 
            this.money_type.DataPropertyName = "money_type";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.money_type.DefaultCellStyle = dataGridViewCellStyle5;
            this.money_type.HeaderText = "Moneda";
            this.money_type.Name = "money_type";
            this.money_type.ReadOnly = true;
            // 
            // money_chance
            // 
            this.money_chance.DataPropertyName = "money_chance";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "₡#,0.00";
            dataGridViewCellStyle6.NullValue = null;
            this.money_chance.DefaultCellStyle = dataGridViewCellStyle6;
            this.money_chance.HeaderText = "Tipo Cambio";
            this.money_chance.Name = "money_chance";
            this.money_chance.ReadOnly = true;
            // 
            // balance_usd
            // 
            this.balance_usd.DataPropertyName = "balance_usd";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "$#,0.00";
            dataGridViewCellStyle7.NullValue = null;
            this.balance_usd.DefaultCellStyle = dataGridViewCellStyle7;
            this.balance_usd.HeaderText = "Monto Dolares";
            this.balance_usd.Name = "balance_usd";
            this.balance_usd.ReadOnly = true;
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
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox lstMesesAbiertos;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMesContable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAsiento;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dsd;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitos;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn money_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn money_chance;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_usd;
    }
}