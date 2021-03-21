namespace CapaPresentacion.Reportes
{
    partial class ReporteBalanceSituacion
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBoxListado = new System.Windows.Forms.GroupBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.groupBoxSeleccionCuenta = new System.Windows.Forms.GroupBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.AFechaFinal = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AFechaInicio = new System.Windows.Forms.ComboBox();
            this.tbnSalir = new System.Windows.Forms.Button();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.groupBoxListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).BeginInit();
            this.groupBoxSeleccionCuenta.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxListado
            // 
            this.groupBoxListado.Controls.Add(this.GridDatos);
            this.groupBoxListado.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxListado.Location = new System.Drawing.Point(10, 77);
            this.groupBoxListado.Name = "groupBoxListado";
            this.groupBoxListado.Size = new System.Drawing.Size(1076, 393);
            this.groupBoxListado.TabIndex = 3;
            this.groupBoxListado.TabStop = false;
            // 
            // GridDatos
            // 
            this.GridDatos.AllowUserToAddRows = false;
            this.GridDatos.AllowUserToDeleteRows = false;
            this.GridDatos.AllowUserToOrderColumns = true;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridDatos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridDatos.DefaultCellStyle = dataGridViewCellStyle14;
            this.GridDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridDatos.EnableHeadersVisualStyles = false;
            this.GridDatos.GridColor = System.Drawing.SystemColors.ControlLight;
            this.GridDatos.Location = new System.Drawing.Point(3, 21);
            this.GridDatos.Name = "GridDatos";
            this.GridDatos.ReadOnly = true;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GridDatos.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.GridDatos.RowHeadersVisible = false;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridDatos.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.GridDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridDatos.Size = new System.Drawing.Size(1070, 369);
            this.GridDatos.TabIndex = 39;
            this.GridDatos.TabStop = false;
            // 
            // groupBoxSeleccionCuenta
            // 
            this.groupBoxSeleccionCuenta.Controls.Add(this.btnExcel);
            this.groupBoxSeleccionCuenta.Controls.Add(this.label10);
            this.groupBoxSeleccionCuenta.Controls.Add(this.AFechaFinal);
            this.groupBoxSeleccionCuenta.Controls.Add(this.label2);
            this.groupBoxSeleccionCuenta.Controls.Add(this.AFechaInicio);
            this.groupBoxSeleccionCuenta.Controls.Add(this.tbnSalir);
            this.groupBoxSeleccionCuenta.Controls.Add(this.btnCalcular);
            this.groupBoxSeleccionCuenta.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSeleccionCuenta.Location = new System.Drawing.Point(8, 4);
            this.groupBoxSeleccionCuenta.Name = "groupBoxSeleccionCuenta";
            this.groupBoxSeleccionCuenta.Size = new System.Drawing.Size(1077, 67);
            this.groupBoxSeleccionCuenta.TabIndex = 4;
            this.groupBoxSeleccionCuenta.TabStop = false;
            // 
            // btnExcel
            // 
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(637, 23);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(80, 30);
            this.btnExcel.TabIndex = 55;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(13, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 17);
            this.label10.TabIndex = 46;
            this.label10.Text = "Ver Saldos desde:";
            // 
            // AFechaFinal
            // 
            this.AFechaFinal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AFechaFinal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AFechaFinal.FormattingEnabled = true;
            this.AFechaFinal.Location = new System.Drawing.Point(348, 26);
            this.AFechaFinal.Name = "AFechaFinal";
            this.AFechaFinal.Size = new System.Drawing.Size(128, 25);
            this.AFechaFinal.TabIndex = 47;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(283, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 17);
            this.label2.TabIndex = 49;
            this.label2.Text = "Hasta:";
            // 
            // AFechaInicio
            // 
            this.AFechaInicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AFechaInicio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AFechaInicio.FormattingEnabled = true;
            this.AFechaInicio.Location = new System.Drawing.Point(130, 26);
            this.AFechaInicio.Name = "AFechaInicio";
            this.AFechaInicio.Size = new System.Drawing.Size(128, 25);
            this.AFechaInicio.TabIndex = 48;
            // 
            // tbnSalir
            // 
            this.tbnSalir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbnSalir.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.tbnSalir.FlatAppearance.BorderSize = 0;
            this.tbnSalir.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbnSalir.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbnSalir.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.tbnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tbnSalir.Location = new System.Drawing.Point(984, 23);
            this.tbnSalir.Name = "tbnSalir";
            this.tbnSalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbnSalir.Size = new System.Drawing.Size(75, 30);
            this.tbnSalir.TabIndex = 4;
            this.tbnSalir.Text = "&Cerrar";
            this.tbnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbnSalir.UseVisualStyleBackColor = true;
            this.tbnSalir.Click += new System.EventHandler(this.tbnSalir_Click);
            // 
            // btnCalcular
            // 
            this.btnCalcular.Image = global::CapaPresentacion.Properties.Resources.icons8_calculadora_25;
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(514, 23);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(94, 30);
            this.btnCalcular.TabIndex = 3;
            this.btnCalcular.Text = "C&alcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = true;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcular);
            // 
            // ReporteBalanceSituacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 487);
            this.Controls.Add(this.groupBoxSeleccionCuenta);
            this.Controls.Add(this.groupBoxListado);
            this.MaximizeBox = false;
            this.Name = "ReporteBalanceSituacion";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Balance Situación";
            this.groupBoxListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).EndInit();
            this.groupBoxSeleccionCuenta.ResumeLayout(false);
            this.groupBoxSeleccionCuenta.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxListado;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.GroupBox groupBoxSeleccionCuenta;
        private System.Windows.Forms.Button tbnSalir;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox AFechaFinal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox AFechaInicio;
        private System.Windows.Forms.Button btnExcel;
    }
}