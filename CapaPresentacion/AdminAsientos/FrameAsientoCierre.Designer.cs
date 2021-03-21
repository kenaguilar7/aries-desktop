namespace CapaPresentacion.AdminAsientos
{
    partial class FrameAsientoCierre
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
            this.groupBoxSalir = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnCerrarPeriodo = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtRegistros = new System.Windows.Forms.DataGridView();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.btnSeleccionarCuenta = new System.Windows.Forms.Button();
            this.txtBoxNombreCuenta = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSeleccione = new System.Windows.Forms.Label();
            this.lstAbrirMes = new System.Windows.Forms.ComboBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnReporte = new System.Windows.Forms.Button();
            this.groupBoxSalir.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).BeginInit();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSalir
            // 
            this.groupBoxSalir.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSalir.Location = new System.Drawing.Point(13, 405);
            this.groupBoxSalir.Name = "groupBoxSalir";
            this.groupBoxSalir.Size = new System.Drawing.Size(508, 55);
            this.groupBoxSalir.TabIndex = 18;
            this.groupBoxSalir.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCerrar);
            this.flowLayoutPanel1.Controls.Add(this.btnCerrarPeriodo);
            this.flowLayoutPanel1.Controls.Add(this.btnReporte);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(502, 34);
            this.flowLayoutPanel1.TabIndex = 3;
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
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnCerrarPeriodo
            // 
            this.btnCerrarPeriodo.FlatAppearance.BorderSize = 0;
            this.btnCerrarPeriodo.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnCerrarPeriodo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarPeriodo.Location = new System.Drawing.Point(291, 3);
            this.btnCerrarPeriodo.Name = "btnCerrarPeriodo";
            this.btnCerrarPeriodo.Size = new System.Drawing.Size(126, 30);
            this.btnCerrarPeriodo.TabIndex = 10;
            this.btnCerrarPeriodo.Text = "Cerrar Periodo";
            this.btnCerrarPeriodo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrarPeriodo.UseVisualStyleBackColor = true;
            this.btnCerrarPeriodo.Click += new System.EventHandler(this.btnCerrarPeriodo_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtRegistros);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(13, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 265);
            this.groupBox2.TabIndex = 17;
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
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.btnSeleccionarCuenta);
            this.groupBoxOptions.Controls.Add(this.txtBoxNombreCuenta);
            this.groupBoxOptions.Controls.Add(this.label1);
            this.groupBoxOptions.Controls.Add(this.txtSeleccione);
            this.groupBoxOptions.Controls.Add(this.lstAbrirMes);
            this.groupBoxOptions.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBoxOptions.Location = new System.Drawing.Point(13, 22);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(508, 105);
            this.groupBoxOptions.TabIndex = 19;
            this.groupBoxOptions.TabStop = false;
            // 
            // btnSeleccionarCuenta
            // 
            this.btnSeleccionarCuenta.FlatAppearance.BorderSize = 0;
            this.btnSeleccionarCuenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionarCuenta.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionarCuenta.ForeColor = System.Drawing.Color.Red;
            this.btnSeleccionarCuenta.Location = new System.Drawing.Point(340, 60);
            this.btnSeleccionarCuenta.Name = "btnSeleccionarCuenta";
            this.btnSeleccionarCuenta.Size = new System.Drawing.Size(30, 28);
            this.btnSeleccionarCuenta.TabIndex = 102;
            this.btnSeleccionarCuenta.TabStop = false;
            this.btnSeleccionarCuenta.Text = "...";
            this.btnSeleccionarCuenta.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSeleccionarCuenta.UseVisualStyleBackColor = true;
            this.btnSeleccionarCuenta.Click += new System.EventHandler(this.btnSeleccionarCuenta_Click);
            // 
            // txtBoxNombreCuenta
            // 
            this.txtBoxNombreCuenta.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtBoxNombreCuenta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxNombreCuenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxNombreCuenta.Location = new System.Drawing.Point(103, 63);
            this.txtBoxNombreCuenta.Name = "txtBoxNombreCuenta";
            this.txtBoxNombreCuenta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBoxNombreCuenta.Size = new System.Drawing.Size(233, 22);
            this.txtBoxNombreCuenta.TabIndex = 101;
            this.txtBoxNombreCuenta.Text = "                                 ";
            this.txtBoxNombreCuenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Cuenta:";
            // 
            // txtSeleccione
            // 
            this.txtSeleccione.AutoSize = true;
            this.txtSeleccione.Location = new System.Drawing.Point(15, 28);
            this.txtSeleccione.Name = "txtSeleccione";
            this.txtSeleccione.Size = new System.Drawing.Size(73, 17);
            this.txtSeleccione.TabIndex = 2;
            this.txtSeleccione.Text = "Seleccione:";
            // 
            // lstAbrirMes
            // 
            this.lstAbrirMes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstAbrirMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstAbrirMes.FormattingEnabled = true;
            this.lstAbrirMes.Location = new System.Drawing.Point(103, 24);
            this.lstAbrirMes.Name = "lstAbrirMes";
            this.lstAbrirMes.Size = new System.Drawing.Size(233, 24);
            this.lstAbrirMes.TabIndex = 5;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // btnReporte
            // 
            this.btnReporte.FlatAppearance.BorderSize = 0;
            this.btnReporte.Image = global::CapaPresentacion.Properties.Resources.icons8_lista_de_ingredientes_25;
            this.btnReporte.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReporte.Location = new System.Drawing.Point(178, 3);
            this.btnReporte.Name = "btnReporte";
            this.btnReporte.Size = new System.Drawing.Size(107, 30);
            this.btnReporte.TabIndex = 11;
            this.btnReporte.Text = "Ver reporte";
            this.btnReporte.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReporte.UseVisualStyleBackColor = true;
            this.btnReporte.Click += new System.EventHandler(this.btnReporte_Click);
            // 
            // FrameAsientoCierre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 467);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxSalir);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrameAsientoCierre";
            this.Text = "FrameAsientoCierre";
            this.groupBoxSalir.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtRegistros)).EndInit();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSalir;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dtRegistros;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button btnCerrarPeriodo;
        private System.Windows.Forms.ComboBox lstAbrirMes;
        private System.Windows.Forms.Label txtSeleccione;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button btnSeleccionarCuenta;
        private System.Windows.Forms.Label txtBoxNombreCuenta;
        private System.Windows.Forms.Button btnReporte;
    }
}