namespace CapaPresentacion.FrameCuentas
{
    partial class FrameSeleccionCuenta
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
            this.panelCuentas = new System.Windows.Forms.GroupBox();
            this.treeCuentas = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExpandir = new System.Windows.Forms.Button();
            this.btnContraer = new System.Windows.Forms.Button();
            this.btnNuevaCuenta = new System.Windows.Forms.Button();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNombreCuenta = new GlobalTech.TextBoxControl.TextBoxUniversal();
            this.label1 = new System.Windows.Forms.Label();
            this.panelCuentas.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCuentas
            // 
            this.panelCuentas.Controls.Add(this.treeCuentas);
            this.panelCuentas.Controls.Add(this.groupBox1);
            this.panelCuentas.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCuentas.Location = new System.Drawing.Point(6, 65);
            this.panelCuentas.Name = "panelCuentas";
            this.panelCuentas.Size = new System.Drawing.Size(522, 451);
            this.panelCuentas.TabIndex = 2;
            this.panelCuentas.TabStop = false;
            this.panelCuentas.Text = "CUENTAS";
            // 
            // treeCuentas
            // 
            this.treeCuentas.BackColor = System.Drawing.SystemColors.Menu;
            this.treeCuentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeCuentas.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeCuentas.Location = new System.Drawing.Point(5, 21);
            this.treeCuentas.Name = "treeCuentas";
            this.treeCuentas.Size = new System.Drawing.Size(510, 358);
            this.treeCuentas.TabIndex = 3;
            this.treeCuentas.DoubleClick += new System.EventHandler(this.SeleccionaCuentaEnTreeView);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(11, 388);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 55);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnExpandir);
            this.flowLayoutPanel1.Controls.Add(this.btnContraer);
            this.flowLayoutPanel1.Controls.Add(this.btnNuevaCuenta);
            this.flowLayoutPanel1.Controls.Add(this.btnSeleccionar);
            this.flowLayoutPanel1.Controls.Add(this.btnCerrar);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(494, 34);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // btnExpandir
            // 
            this.btnExpandir.FlatAppearance.BorderSize = 0;
            this.btnExpandir.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpandir.Image = global::CapaPresentacion.Properties.Resources.icons8_expandir_25;
            this.btnExpandir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExpandir.Location = new System.Drawing.Point(3, 3);
            this.btnExpandir.Name = "btnExpandir";
            this.btnExpandir.Size = new System.Drawing.Size(95, 30);
            this.btnExpandir.TabIndex = 3;
            this.btnExpandir.Text = "&Expandir";
            this.btnExpandir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExpandir.UseVisualStyleBackColor = true;
            this.btnExpandir.Click += new System.EventHandler(this.ExpandirArbol);
            // 
            // btnContraer
            // 
            this.btnContraer.FlatAppearance.BorderSize = 0;
            this.btnContraer.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContraer.Image = global::CapaPresentacion.Properties.Resources.icons8_achicar_25;
            this.btnContraer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContraer.Location = new System.Drawing.Point(104, 3);
            this.btnContraer.Name = "btnContraer";
            this.btnContraer.Size = new System.Drawing.Size(95, 30);
            this.btnContraer.TabIndex = 4;
            this.btnContraer.Text = "C&olapsar";
            this.btnContraer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnContraer.UseVisualStyleBackColor = true;
            this.btnContraer.Click += new System.EventHandler(this.ContraerArbol);
            // 
            // btnNuevaCuenta
            // 
            this.btnNuevaCuenta.FlatAppearance.BorderSize = 0;
            this.btnNuevaCuenta.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevaCuenta.Image = global::CapaPresentacion.Properties.Resources.icons8_añadir_25;
            this.btnNuevaCuenta.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNuevaCuenta.Location = new System.Drawing.Point(205, 3);
            this.btnNuevaCuenta.Name = "btnNuevaCuenta";
            this.btnNuevaCuenta.Size = new System.Drawing.Size(80, 30);
            this.btnNuevaCuenta.TabIndex = 1;
            this.btnNuevaCuenta.Text = "&Nueva";
            this.btnNuevaCuenta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNuevaCuenta.UseVisualStyleBackColor = true;
            this.btnNuevaCuenta.Click += new System.EventHandler(this.CrearNuevaCuenta);
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.FlatAppearance.BorderSize = 0;
            this.btnSeleccionar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionar.Image = global::CapaPresentacion.Properties.Resources.icons8_comprobado_25;
            this.btnSeleccionar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSeleccionar.Location = new System.Drawing.Point(291, 3);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(110, 30);
            this.btnSeleccionar.TabIndex = 6;
            this.btnSeleccionar.Text = "&Seleccionar";
            this.btnSeleccionar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            this.btnSeleccionar.Click += new System.EventHandler(this.SeleccionarClick);
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Image = global::CapaPresentacion.Properties.Resources.icons8_cerrar_ventana_25;
            this.btnCerrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrar.Location = new System.Drawing.Point(407, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(80, 30);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "&Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.CerrarClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtNombreCuenta);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(5, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(522, 56);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Buscar Cuenta";
            // 
            // txtNombreCuenta
            // 
            this.txtNombreCuenta.AcceptDecimal = false;
            this.txtNombreCuenta.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreCuenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtNombreCuenta.Location = new System.Drawing.Point(143, 24);
            this.txtNombreCuenta.Name = "txtNombreCuenta";
            this.txtNombreCuenta.NumeroDecimales = ((short)(2));
            this.txtNombreCuenta.Size = new System.Drawing.Size(369, 20);
            this.txtNombreCuenta.SombrearTexto = false;
            this.txtNombreCuenta.TabIndex = 1;
            this.txtNombreCuenta.TipoControl = GlobalTech.TextBoxControl.TextBoxUniversal.TipoDato.AlfaNumerico;
            this.txtNombreCuenta.WaterMarkColor = System.Drawing.Color.Gray;
            this.txtNombreCuenta.WaterMarkText = "";
            this.txtNombreCuenta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNombreCuentaKeyPress);
            this.txtNombreCuenta.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BucarNodes);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre de cuenta:";
            // 
            // FrameSeleccionCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 520);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panelCuentas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameSeleccionCuenta";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cuentas";
            this.panelCuentas.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox panelCuentas;
        private System.Windows.Forms.TreeView treeCuentas;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExpandir;
        private System.Windows.Forms.Button btnContraer;
        private System.Windows.Forms.Button btnNuevaCuenta;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnSeleccionar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private GlobalTech.TextBoxControl.TextBoxUniversal txtNombreCuenta;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}