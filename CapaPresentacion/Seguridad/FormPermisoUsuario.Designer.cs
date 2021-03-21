namespace CapaPresentacion.Seguridad
{
    partial class FormPermisoUsuario
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBoxBuscar = new GlobalTech.TextBoxControl.TextBoxUniversal();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRemoverCompañia = new System.Windows.Forms.Button();
            this.btnAgregarCompania = new System.Windows.Forms.Button();
            this.listCompañiasAsignadas = new System.Windows.Forms.ListBox();
            this.listCompañiasSinAsignar = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbnSalir = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.treeViewModulos = new System.Windows.Forms.TreeView();
            this.panelAsignacionModulos = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstUsuarios = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panelAsignacionModulos.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstUsuarios);
            this.groupBox2.Controls.Add(this.txtBoxBuscar);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1096, 67);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BUSCAR USUARIO";
            // 
            // txtBoxBuscar
            // 
            this.txtBoxBuscar.AcceptDecimal = false;
            this.txtBoxBuscar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBuscar.Location = new System.Drawing.Point(889, 25);
            this.txtBoxBuscar.MaxLength = 20;
            this.txtBoxBuscar.Name = "txtBoxBuscar";
            this.txtBoxBuscar.NumeroDecimales = ((short)(2));
            this.txtBoxBuscar.Size = new System.Drawing.Size(179, 25);
            this.txtBoxBuscar.SombrearTexto = false;
            this.txtBoxBuscar.TabIndex = 7;
            this.txtBoxBuscar.TipoControl = GlobalTech.TextBoxControl.TextBoxUniversal.TipoDato.AlfaNumerico;
            this.txtBoxBuscar.WaterMarkColor = System.Drawing.Color.Gray;
            this.txtBoxBuscar.WaterMarkText = "";
            this.txtBoxBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Buscar_KeyPress);
            this.txtBoxBuscar.Leave += new System.EventHandler(this.BoxBuscar_Leave);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Red;
            this.panel1.Location = new System.Drawing.Point(885, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(5, 15);
            this.panel1.TabIndex = 5;
            this.panel1.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(761, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Buscar por Código";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Seleccione";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnRemoverCompañia);
            this.groupBox1.Controls.Add(this.btnAgregarCompania);
            this.groupBox1.Controls.Add(this.listCompañiasAsignadas);
            this.groupBox1.Controls.Add(this.listCompañiasSinAsignar);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(381, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(727, 340);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Asignación de Compañias";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(419, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Compañias asignadas";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(14, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Compañias sin asignar";
            // 
            // btnRemoverCompañia
            // 
            this.btnRemoverCompañia.Location = new System.Drawing.Point(326, 130);
            this.btnRemoverCompañia.Name = "btnRemoverCompañia";
            this.btnRemoverCompañia.Size = new System.Drawing.Size(75, 23);
            this.btnRemoverCompañia.TabIndex = 10;
            this.btnRemoverCompañia.Text = "<<";
            this.btnRemoverCompañia.UseVisualStyleBackColor = true;
            this.btnRemoverCompañia.Click += new System.EventHandler(this.RemoverCompañia_Click);
            // 
            // btnAgregarCompania
            // 
            this.btnAgregarCompania.Location = new System.Drawing.Point(326, 94);
            this.btnAgregarCompania.Name = "btnAgregarCompania";
            this.btnAgregarCompania.Size = new System.Drawing.Size(75, 23);
            this.btnAgregarCompania.TabIndex = 9;
            this.btnAgregarCompania.Text = ">>";
            this.btnAgregarCompania.UseVisualStyleBackColor = true;
            this.btnAgregarCompania.Click += new System.EventHandler(this.AgregarCompania_Click);
            // 
            // listCompañiasAsignadas
            // 
            this.listCompañiasAsignadas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listCompañiasAsignadas.FormattingEnabled = true;
            this.listCompañiasAsignadas.ItemHeight = 15;
            this.listCompañiasAsignadas.Location = new System.Drawing.Point(416, 40);
            this.listCompañiasAsignadas.Name = "listCompañiasAsignadas";
            this.listCompañiasAsignadas.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listCompañiasAsignadas.Size = new System.Drawing.Size(300, 289);
            this.listCompañiasAsignadas.TabIndex = 8;
            // 
            // listCompañiasSinAsignar
            // 
            this.listCompañiasSinAsignar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listCompañiasSinAsignar.FormattingEnabled = true;
            this.listCompañiasSinAsignar.ItemHeight = 15;
            this.listCompañiasSinAsignar.Location = new System.Drawing.Point(12, 42);
            this.listCompañiasSinAsignar.Name = "listCompañiasSinAsignar";
            this.listCompañiasSinAsignar.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listCompañiasSinAsignar.Size = new System.Drawing.Size(300, 289);
            this.listCompañiasSinAsignar.TabIndex = 7;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tbnSalir);
            this.flowLayoutPanel1.Controls.Add(this.btnGuardar);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1090, 34);
            this.flowLayoutPanel1.TabIndex = 6;
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
            this.tbnSalir.Location = new System.Drawing.Point(1007, 3);
            this.tbnSalir.Name = "tbnSalir";
            this.tbnSalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbnSalir.Size = new System.Drawing.Size(80, 30);
            this.tbnSalir.TabIndex = 1;
            this.tbnSalir.Text = "&Cerrar";
            this.tbnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbnSalir.UseVisualStyleBackColor = true;
            this.tbnSalir.Click += new System.EventHandler(this.BbnSalir_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(914, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(87, 30);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // treeViewModulos
            // 
            this.treeViewModulos.BackColor = System.Drawing.SystemColors.Menu;
            this.treeViewModulos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewModulos.CheckBoxes = true;
            this.treeViewModulos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewModulos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewModulos.Location = new System.Drawing.Point(3, 21);
            this.treeViewModulos.Name = "treeViewModulos";
            this.treeViewModulos.Size = new System.Drawing.Size(344, 316);
            this.treeViewModulos.TabIndex = 0;
            this.treeViewModulos.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewModulos_AfterCheck);
            // 
            // panelAsignacionModulos
            // 
            this.panelAsignacionModulos.Controls.Add(this.treeViewModulos);
            this.panelAsignacionModulos.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelAsignacionModulos.Location = new System.Drawing.Point(12, 88);
            this.panelAsignacionModulos.Name = "panelAsignacionModulos";
            this.panelAsignacionModulos.Size = new System.Drawing.Size(350, 340);
            this.panelAsignacionModulos.TabIndex = 3;
            this.panelAsignacionModulos.TabStop = false;
            this.panelAsignacionModulos.Text = "Asiganación de Sistemas";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.flowLayoutPanel1);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 439);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1096, 55);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            // 
            // lstUsuarios
            // 
            this.lstUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstUsuarios.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstUsuarios.FormattingEnabled = true;
            this.lstUsuarios.Location = new System.Drawing.Point(98, 25);
            this.lstUsuarios.Name = "lstUsuarios";
            this.lstUsuarios.Size = new System.Drawing.Size(647, 25);
            this.lstUsuarios.TabIndex = 13;
            this.lstUsuarios.SelectedIndexChanged += new System.EventHandler(this.LstUsuarios_SelectedIndexChanged);
            // 
            // FormPermisoUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 506);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panelAsignacionModulos);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPermisoUsuario";
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Permisos de usuario";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panelAsignacionModulos.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeViewModulos;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button tbnSalir;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnRemoverCompañia;
        private System.Windows.Forms.Button btnAgregarCompania;
        private System.Windows.Forms.ListBox listCompañiasAsignadas;
        private System.Windows.Forms.ListBox listCompañiasSinAsignar;
        private System.Windows.Forms.GroupBox panelAsignacionModulos;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private GlobalTech.TextBoxControl.TextBoxUniversal txtBoxBuscar;
        private System.Windows.Forms.ComboBox lstUsuarios;
    }
}