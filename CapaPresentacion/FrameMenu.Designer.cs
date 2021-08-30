namespace CapaPresentacion
{
    partial class FrameMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameMenu));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.compañiasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maestroDeCompañiasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seleccionarParaTrabajarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaestroDeUsuariotoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maestroDeCuentasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asientosContablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.administrarMesesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cierreDePeriodoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.balanceDeComprobaciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.balanceDeAuxiliaresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movimientosDeCuentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perdiasYGananciasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.balanceDeSituaciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nbalanceDeComprobaciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sistemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PermisosDeUsuarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionDeCorreosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementosEliminadosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.txtUsuario = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.txtCompaniaNombre = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compañiasToolStripMenuItem,
            this.usuariosToolStripMenuItem,
            this.contableToolStripMenuItem,
            this.reportesToolStripMenuItem,
            this.sistemaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(906, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // compañiasToolStripMenuItem
            // 
            this.compañiasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maestroDeCompañiasToolStripMenuItem,
            this.seleccionarParaTrabajarToolStripMenuItem});
            this.compañiasToolStripMenuItem.Name = "compañiasToolStripMenuItem";
            this.compañiasToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.compañiasToolStripMenuItem.Text = "&Compañias";
            // 
            // maestroDeCompañiasToolStripMenuItem
            // 
            this.maestroDeCompañiasToolStripMenuItem.Name = "maestroDeCompañiasToolStripMenuItem";
            this.maestroDeCompañiasToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.maestroDeCompañiasToolStripMenuItem.Text = "Maestro de Compañias";
            this.maestroDeCompañiasToolStripMenuItem.Click += new System.EventHandler(this.MaestroDeCompañiasToolStripMenuItem_Click);
            // 
            // seleccionarParaTrabajarToolStripMenuItem
            // 
            this.seleccionarParaTrabajarToolStripMenuItem.Name = "seleccionarParaTrabajarToolStripMenuItem";
            this.seleccionarParaTrabajarToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.seleccionarParaTrabajarToolStripMenuItem.Text = "Seleccionar para Trabajar";
            this.seleccionarParaTrabajarToolStripMenuItem.Click += new System.EventHandler(this.MaestroDeCompañia);
            // 
            // usuariosToolStripMenuItem
            // 
            this.usuariosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaestroDeUsuariotoolStripMenuItem});
            this.usuariosToolStripMenuItem.Name = "usuariosToolStripMenuItem";
            this.usuariosToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.usuariosToolStripMenuItem.Text = "&Usuarios";
            // 
            // MaestroDeUsuariotoolStripMenuItem
            // 
            this.MaestroDeUsuariotoolStripMenuItem.Name = "MaestroDeUsuariotoolStripMenuItem";
            this.MaestroDeUsuariotoolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.MaestroDeUsuariotoolStripMenuItem.Text = "&Maestro de Usuarios";
            this.MaestroDeUsuariotoolStripMenuItem.Click += new System.EventHandler(this.MaestroDeUsaurio);
            // 
            // contableToolStripMenuItem
            // 
            this.contableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maestroDeCuentasToolStripMenuItem,
            this.asientosContablesToolStripMenuItem,
            this.administrarMesesToolStripMenuItem,
            this.cierreDePeriodoToolStripMenuItem});
            this.contableToolStripMenuItem.Name = "contableToolStripMenuItem";
            this.contableToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.contableToolStripMenuItem.Text = "C&ontable";
            // 
            // maestroDeCuentasToolStripMenuItem
            // 
            this.maestroDeCuentasToolStripMenuItem.Name = "maestroDeCuentasToolStripMenuItem";
            this.maestroDeCuentasToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.maestroDeCuentasToolStripMenuItem.Text = "&Maestro de Cuentas";
            this.maestroDeCuentasToolStripMenuItem.Click += new System.EventHandler(this.MaestroDeCuentasToolStripMenuItem_Click);
            // 
            // asientosContablesToolStripMenuItem
            // 
            this.asientosContablesToolStripMenuItem.Name = "asientosContablesToolStripMenuItem";
            this.asientosContablesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.asientosContablesToolStripMenuItem.Text = "Asientos Contables";
            this.asientosContablesToolStripMenuItem.Click += new System.EventHandler(this.AsientosContablesToolStripMenuItem_Click);
            // 
            // administrarMesesToolStripMenuItem
            // 
            this.administrarMesesToolStripMenuItem.Name = "administrarMesesToolStripMenuItem";
            this.administrarMesesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.administrarMesesToolStripMenuItem.Text = "Administrar Meses";
            this.administrarMesesToolStripMenuItem.Click += new System.EventHandler(this.MaestroDeMeses);
            // 
            // cierreDePeriodoToolStripMenuItem
            // 
            this.cierreDePeriodoToolStripMenuItem.Name = "cierreDePeriodoToolStripMenuItem";
            this.cierreDePeriodoToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.cierreDePeriodoToolStripMenuItem.Text = "Cierre de periodo";
            this.cierreDePeriodoToolStripMenuItem.Visible = false;
            this.cierreDePeriodoToolStripMenuItem.Click += new System.EventHandler(this.cierreDePeriodoToolStripMenuItem_Click);
            // 
            // reportesToolStripMenuItem
            // 
            this.reportesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.balanceDeComprobaciónToolStripMenuItem,
            this.balanceDeAuxiliaresToolStripMenuItem,
            this.movimientosDeCuentaToolStripMenuItem,
            this.perdiasYGananciasToolStripMenuItem,
            this.balanceDeSituaciónToolStripMenuItem,
            this.nbalanceDeComprobaciónToolStripMenuItem});
            this.reportesToolStripMenuItem.Name = "reportesToolStripMenuItem";
            this.reportesToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.reportesToolStripMenuItem.Text = "&Reportes";
            // 
            // balanceDeComprobaciónToolStripMenuItem
            // 
            this.balanceDeComprobaciónToolStripMenuItem.Name = "balanceDeComprobaciónToolStripMenuItem";
            this.balanceDeComprobaciónToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.balanceDeComprobaciónToolStripMenuItem.Text = "Balance de Comprobación";
            this.balanceDeComprobaciónToolStripMenuItem.Click += new System.EventHandler(this.balanceDeComprobaciónToolStripMenuItem_Click);
            // 
            // balanceDeAuxiliaresToolStripMenuItem
            // 
            this.balanceDeAuxiliaresToolStripMenuItem.Name = "balanceDeAuxiliaresToolStripMenuItem";
            this.balanceDeAuxiliaresToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.balanceDeAuxiliaresToolStripMenuItem.Text = "Balance de Auxiliares";
            this.balanceDeAuxiliaresToolStripMenuItem.Click += new System.EventHandler(this.balanceDeAuxiliaresToolStripMenuItem_Click);
            // 
            // movimientosDeCuentaToolStripMenuItem
            // 
            this.movimientosDeCuentaToolStripMenuItem.Name = "movimientosDeCuentaToolStripMenuItem";
            this.movimientosDeCuentaToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.movimientosDeCuentaToolStripMenuItem.Text = "Movimientos de cuenta";
            this.movimientosDeCuentaToolStripMenuItem.Click += new System.EventHandler(this.movimientosDeCuentaToolStripMenuItem_Click);
            // 
            // perdiasYGananciasToolStripMenuItem
            // 
            this.perdiasYGananciasToolStripMenuItem.Name = "perdiasYGananciasToolStripMenuItem";
            this.perdiasYGananciasToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.perdiasYGananciasToolStripMenuItem.Text = "Estado de resultado integral";
            this.perdiasYGananciasToolStripMenuItem.Click += new System.EventHandler(this.perdiasYGananciasToolStripMenuItem_Click);
            // 
            // balanceDeSituaciónToolStripMenuItem
            // 
            this.balanceDeSituaciónToolStripMenuItem.Name = "balanceDeSituaciónToolStripMenuItem";
            this.balanceDeSituaciónToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.balanceDeSituaciónToolStripMenuItem.Text = "Balance de Situación";
            this.balanceDeSituaciónToolStripMenuItem.Click += new System.EventHandler(this.balanceDeSituaciónToolStripMenuItem_Click);
            // 
            // nbalanceDeComprobaciónToolStripMenuItem
            // 
            this.nbalanceDeComprobaciónToolStripMenuItem.Name = "nbalanceDeComprobaciónToolStripMenuItem";
            this.nbalanceDeComprobaciónToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.nbalanceDeComprobaciónToolStripMenuItem.Text = "Nbalance de Comprobación";
            this.nbalanceDeComprobaciónToolStripMenuItem.Visible = false;
            // 
            // sistemaToolStripMenuItem
            // 
            this.sistemaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PermisosDeUsuarioToolStripMenuItem,
            this.gestionDeCorreosToolStripMenuItem,
            this.elementosEliminadosToolStripMenuItem,
            this.salirToolStripMenuItem});
            this.sistemaToolStripMenuItem.Name = "sistemaToolStripMenuItem";
            this.sistemaToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.sistemaToolStripMenuItem.Text = "Sistema";
            // 
            // PermisosDeUsuarioToolStripMenuItem
            // 
            this.PermisosDeUsuarioToolStripMenuItem.Name = "PermisosDeUsuarioToolStripMenuItem";
            this.PermisosDeUsuarioToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.PermisosDeUsuarioToolStripMenuItem.Text = "Permisos de usuario";
            this.PermisosDeUsuarioToolStripMenuItem.Click += new System.EventHandler(this.gestorDeVentanasToolStripMenuItem_Click);
            // 
            // gestionDeCorreosToolStripMenuItem
            // 
            this.gestionDeCorreosToolStripMenuItem.Name = "gestionDeCorreosToolStripMenuItem";
            this.gestionDeCorreosToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.gestionDeCorreosToolStripMenuItem.Text = "Gestion de correos";
            this.gestionDeCorreosToolStripMenuItem.Click += new System.EventHandler(this.gestionDeCorreosToolStripMenuItem_Click);
            // 
            // elementosEliminadosToolStripMenuItem
            // 
            this.elementosEliminadosToolStripMenuItem.Name = "elementosEliminadosToolStripMenuItem";
            this.elementosEliminadosToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.elementosEliminadosToolStripMenuItem.Text = "Elementos eliminados";
            this.elementosEliminadosToolStripMenuItem.Click += new System.EventHandler(this.elementosEliminadosToolStripMenuItem_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.SalirToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.txtUsuario,
            this.toolStripButton2,
            this.txtCompaniaNombre,
            this.toolStripSeparator1,
            this.ProgressBar,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 425);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(906, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Click += new System.EventHandler(this.SeleccioneCompañiaParaTrabajar);
            // 
            // txtUsuario
            // 
            this.txtUsuario.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(59, 22);
            this.txtUsuario.Text = "USUARIO ";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            // 
            // txtCompaniaNombre
            // 
            this.txtCompaniaNombre.Name = "txtCompaniaNombre";
            this.txtCompaniaNombre.Size = new System.Drawing.Size(155, 22);
            this.txtCompaniaNombre.Text = "No se ha cargado compañia";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ProgressBar
            // 
            this.ProgressBar.MarqueeAnimationSpeed = 30;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(400, 22);
            this.ProgressBar.Step = 0;
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressBar.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // FrameMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 450);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrameMenu";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aries ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem compañiasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maestroDeCompañiasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MaestroDeUsuariotoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maestroDeCuentasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asientosContablesToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel txtUsuario;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem administrarMesesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seleccionarParaTrabajarToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel txtCompaniaNombre;
        private System.Windows.Forms.ToolStripMenuItem reportesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balanceDeComprobaciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balanceDeAuxiliaresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sistemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PermisosDeUsuarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem movimientosDeCuentaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perdiasYGananciasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balanceDeSituaciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cierreDePeriodoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripProgressBar ProgressBar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem nbalanceDeComprobaciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionDeCorreosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementosEliminadosToolStripMenuItem;
    }
}