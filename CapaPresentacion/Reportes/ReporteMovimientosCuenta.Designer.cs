namespace CapaPresentacion.Reportes
{
    partial class ReporteMovimientosCuenta
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
            this.components = new System.ComponentModel.Container();
            this.groupBoxSeleccionCuenta = new System.Windows.Forms.GroupBox();
            this.btnSeleccionarCuenta = new System.Windows.Forms.Button();
            this.tbnSalir = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtBoxCuentaSeleccionada = new System.Windows.Forms.Label();
            this.chckBxMostarDolares = new System.Windows.Forms.CheckBox();
            this.labelSeleccioneLaCuenta = new System.Windows.Forms.Label();
            this.groupBoxListado = new System.Windows.Forms.GroupBox();
            this.GridDatos = new System.Windows.Forms.DataGridView();
            this.txtCuentaToolT = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxSeleccionCuenta.SuspendLayout();
            this.groupBoxListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxSeleccionCuenta
            // 
            this.groupBoxSeleccionCuenta.Controls.Add(this.btnSeleccionarCuenta);
            this.groupBoxSeleccionCuenta.Controls.Add(this.tbnSalir);
            this.groupBoxSeleccionCuenta.Controls.Add(this.btnExcel);
            this.groupBoxSeleccionCuenta.Controls.Add(this.panel1);
            this.groupBoxSeleccionCuenta.Controls.Add(this.txtBoxCuentaSeleccionada);
            this.groupBoxSeleccionCuenta.Controls.Add(this.chckBxMostarDolares);
            this.groupBoxSeleccionCuenta.Controls.Add(this.labelSeleccioneLaCuenta);
            this.groupBoxSeleccionCuenta.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSeleccionCuenta.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSeleccionCuenta.Name = "groupBoxSeleccionCuenta";
            this.groupBoxSeleccionCuenta.Size = new System.Drawing.Size(1220, 64);
            this.groupBoxSeleccionCuenta.TabIndex = 0;
            this.groupBoxSeleccionCuenta.TabStop = false;
            // 
            // btnSeleccionarCuenta
            // 
            this.btnSeleccionarCuenta.FlatAppearance.BorderSize = 0;
            this.btnSeleccionarCuenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionarCuenta.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionarCuenta.ForeColor = System.Drawing.Color.Red;
            this.btnSeleccionarCuenta.Location = new System.Drawing.Point(402, 22);
            this.btnSeleccionarCuenta.Name = "btnSeleccionarCuenta";
            this.btnSeleccionarCuenta.Size = new System.Drawing.Size(30, 28);
            this.btnSeleccionarCuenta.TabIndex = 100;
            this.btnSeleccionarCuenta.TabStop = false;
            this.btnSeleccionarCuenta.Text = "...";
            this.btnSeleccionarCuenta.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.txtCuentaToolT.SetToolTip(this.btnSeleccionarCuenta, "CTRL + S");
            this.btnSeleccionarCuenta.UseVisualStyleBackColor = true;
            this.btnSeleccionarCuenta.Click += new System.EventHandler(this.btnSeleccionarCuenta_Click);
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
            this.tbnSalir.Location = new System.Drawing.Point(1128, 21);
            this.tbnSalir.Name = "tbnSalir";
            this.tbnSalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbnSalir.Size = new System.Drawing.Size(75, 30);
            this.tbnSalir.TabIndex = 71;
            this.tbnSalir.Text = "&Cerrar";
            this.tbnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbnSalir.UseVisualStyleBackColor = true;
            this.tbnSalir.Click += new System.EventHandler(this.CerrarFormulario);
            // 
            // btnExcel
            // 
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.Image = global::CapaPresentacion.Properties.Resources.icons8_ms_excel_25;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(567, 21);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(91, 30);
            this.btnExcel.TabIndex = 70;
            this.btnExcel.Text = "Exportar";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.GenerarExcel);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Red;
            this.panel1.Location = new System.Drawing.Point(1204, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(15, 50);
            this.panel1.TabIndex = 69;
            this.panel1.Visible = false;
            // 
            // txtBoxCuentaSeleccionada
            // 
            this.txtBoxCuentaSeleccionada.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtBoxCuentaSeleccionada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxCuentaSeleccionada.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxCuentaSeleccionada.Location = new System.Drawing.Point(156, 25);
            this.txtBoxCuentaSeleccionada.Name = "txtBoxCuentaSeleccionada";
            this.txtBoxCuentaSeleccionada.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBoxCuentaSeleccionada.Size = new System.Drawing.Size(240, 22);
            this.txtBoxCuentaSeleccionada.TabIndex = 68;
            this.txtBoxCuentaSeleccionada.Text = "                                 ";
            this.txtBoxCuentaSeleccionada.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chckBxMostarDolares
            // 
            this.chckBxMostarDolares.AutoSize = true;
            this.chckBxMostarDolares.Checked = true;
            this.chckBxMostarDolares.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chckBxMostarDolares.Location = new System.Drawing.Point(440, 26);
            this.chckBxMostarDolares.Name = "chckBxMostarDolares";
            this.chckBxMostarDolares.Size = new System.Drawing.Size(119, 21);
            this.chckBxMostarDolares.TabIndex = 3;
            this.chckBxMostarDolares.Text = "Mostar Dolares";
            this.chckBxMostarDolares.UseVisualStyleBackColor = true;
            this.chckBxMostarDolares.CheckedChanged += new System.EventHandler(this.MostarDolares);
            // 
            // labelSeleccioneLaCuenta
            // 
            this.labelSeleccioneLaCuenta.AutoSize = true;
            this.labelSeleccioneLaCuenta.Location = new System.Drawing.Point(20, 28);
            this.labelSeleccioneLaCuenta.Name = "labelSeleccioneLaCuenta";
            this.labelSeleccioneLaCuenta.Size = new System.Drawing.Size(129, 17);
            this.labelSeleccioneLaCuenta.TabIndex = 0;
            this.labelSeleccioneLaCuenta.Text = "Seleccione la cuenta";
            // 
            // groupBoxListado
            // 
            this.groupBoxListado.Controls.Add(this.GridDatos);
            this.groupBoxListado.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxListado.Location = new System.Drawing.Point(6, 75);
            this.groupBoxListado.Name = "groupBoxListado";
            this.groupBoxListado.Size = new System.Drawing.Size(1220, 400);
            this.groupBoxListado.TabIndex = 1;
            this.groupBoxListado.TabStop = false;
            // 
            // GridDatos
            // 
            this.GridDatos.AllowUserToAddRows = false;
            this.GridDatos.AllowUserToOrderColumns = true;
            this.GridDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.GridDatos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.GridDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GridDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridDatos.EnableHeadersVisualStyles = false;
            this.GridDatos.GridColor = System.Drawing.SystemColors.ControlLight;
            this.GridDatos.Location = new System.Drawing.Point(3, 21);
            this.GridDatos.Name = "GridDatos";
            this.GridDatos.ReadOnly = true;
            this.GridDatos.RowHeadersVisible = false;
            this.GridDatos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.GridDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridDatos.Size = new System.Drawing.Size(1214, 376);
            this.GridDatos.TabIndex = 1;
            // 
            // ReporteMovimientosCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 482);
            this.Controls.Add(this.groupBoxListado);
            this.Controls.Add(this.groupBoxSeleccionCuenta);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ReporteMovimientosCuenta";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Movimientos de Cuentas";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrameReporteMovimientosCuenta_KeyPress);
            this.groupBoxSeleccionCuenta.ResumeLayout(false);
            this.groupBoxSeleccionCuenta.PerformLayout();
            this.groupBoxListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridDatos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSeleccionCuenta;
        private System.Windows.Forms.Label labelSeleccioneLaCuenta;
        private System.Windows.Forms.GroupBox groupBoxListado;
        private System.Windows.Forms.CheckBox chckBxMostarDolares;
        private System.Windows.Forms.DataGridView GridDatos;
        private System.Windows.Forms.Label txtBoxCuentaSeleccionada;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button tbnSalir;
        private System.Windows.Forms.Button btnSeleccionarCuenta;
        private System.Windows.Forms.ToolTip txtCuentaToolT;
    }
}