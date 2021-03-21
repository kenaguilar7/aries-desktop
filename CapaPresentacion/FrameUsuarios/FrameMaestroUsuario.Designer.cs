namespace CapaPresentacion.Seguridad
{
    partial class FrameMaestroUsuario
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.opnPanel = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbnSalir = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnVerContraseña = new System.Windows.Forms.Button();
            this.txtErrorNombre = new System.Windows.Forms.Label();
            this.txtErrorCedula = new System.Windows.Forms.Label();
            this.txtErrorCorreo = new System.Windows.Forms.Label();
            this.txtErrorUserName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rdbUsuarioNormal = new System.Windows.Forms.RadioButton();
            this.rdbUsuarioAdmin = new System.Windows.Forms.RadioButton();
            this.txtBoxUsuario = new System.Windows.Forms.TextBox();
            this.estado = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxNombre = new System.Windows.Forms.TextBox();
            this.txtBoxCLave = new System.Windows.Forms.TextBox();
            this.txtBoxID = new System.Windows.Forms.TextBox();
            this.txtOp1 = new System.Windows.Forms.Label();
            this.txtBoxOp1 = new System.Windows.Forms.TextBox();
            this.txtBoxTelefono = new System.Windows.Forms.MaskedTextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtBoxMail = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtOp2 = new System.Windows.Forms.Label();
            this.txtBoxObservaciones = new System.Windows.Forms.TextBox();
            this.txtBoxOp2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBoxBuscar = new GlobalTech.TextBoxControl.TextBoxUniversal();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstUsuarios = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.opnPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.opnPanel);
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(968, 422);
            this.panel1.TabIndex = 0;
            // 
            // opnPanel
            // 
            this.opnPanel.Controls.Add(this.flowLayoutPanel1);
            this.opnPanel.Location = new System.Drawing.Point(12, 356);
            this.opnPanel.Name = "opnPanel";
            this.opnPanel.Size = new System.Drawing.Size(944, 55);
            this.opnPanel.TabIndex = 3;
            this.opnPanel.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tbnSalir);
            this.flowLayoutPanel1.Controls.Add(this.btnGuardar);
            this.flowLayoutPanel1.Controls.Add(this.btnActualizar);
            this.flowLayoutPanel1.Controls.Add(this.btnLimpiar);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(938, 36);
            this.flowLayoutPanel1.TabIndex = 2;
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
            this.tbnSalir.Location = new System.Drawing.Point(860, 3);
            this.tbnSalir.Name = "tbnSalir";
            this.tbnSalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbnSalir.Size = new System.Drawing.Size(75, 30);
            this.tbnSalir.TabIndex = 5;
            this.tbnSalir.Text = "&Cerrar";
            this.tbnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tbnSalir.UseVisualStyleBackColor = true;
            this.tbnSalir.Click += new System.EventHandler(this.Salir);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = global::CapaPresentacion.Properties.Resources.guardar25;
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(767, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(87, 30);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.GuardarUsuario);
            // 
            // btnActualizar
            // 
            this.btnActualizar.Enabled = false;
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizar.Image = global::CapaPresentacion.Properties.Resources.icons8_renovar_suscripción_25;
            this.btnActualizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnActualizar.Location = new System.Drawing.Point(661, 3);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 4;
            this.btnActualizar.Text = "&Actualizar";
            this.btnActualizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Visible = false;
            this.btnActualizar.Click += new System.EventHandler(this.ActualizarUsuario);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Image = global::CapaPresentacion.Properties.Resources.icons8_limpiaparabrisas_25;
            this.btnLimpiar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLimpiar.Location = new System.Drawing.Point(570, 3);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(85, 30);
            this.btnLimpiar.TabIndex = 6;
            this.btnLimpiar.Text = "&Limpiar";
            this.btnLimpiar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.LimpiarFormulario);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel2);
            this.groupBox5.Controls.Add(this.btnVerContraseña);
            this.groupBox5.Controls.Add(this.txtErrorNombre);
            this.groupBox5.Controls.Add(this.txtErrorCedula);
            this.groupBox5.Controls.Add(this.txtErrorCorreo);
            this.groupBox5.Controls.Add(this.txtErrorUserName);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.rdbUsuarioNormal);
            this.groupBox5.Controls.Add(this.rdbUsuarioAdmin);
            this.groupBox5.Controls.Add(this.txtBoxUsuario);
            this.groupBox5.Controls.Add(this.estado);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label24);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtBoxNombre);
            this.groupBox5.Controls.Add(this.txtBoxCLave);
            this.groupBox5.Controls.Add(this.txtBoxID);
            this.groupBox5.Controls.Add(this.txtOp1);
            this.groupBox5.Controls.Add(this.txtBoxOp1);
            this.groupBox5.Controls.Add(this.txtBoxTelefono);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.txtBoxMail);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.txtOp2);
            this.groupBox5.Controls.Add(this.txtBoxObservaciones);
            this.groupBox5.Controls.Add(this.txtBoxOp2);
            this.groupBox5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(12, 89);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(944, 254);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Nuevo Usuario";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Red;
            this.panel2.Location = new System.Drawing.Point(754, 217);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(89, 23);
            this.panel2.TabIndex = 72;
            this.panel2.Visible = false;
            // 
            // btnVerContraseña
            // 
            this.btnVerContraseña.FlatAppearance.BorderSize = 0;
            this.btnVerContraseña.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerContraseña.Image = global::CapaPresentacion.Properties.Resources.icons8_visible_20;
            this.btnVerContraseña.Location = new System.Drawing.Point(397, 213);
            this.btnVerContraseña.Name = "btnVerContraseña";
            this.btnVerContraseña.Size = new System.Drawing.Size(33, 23);
            this.btnVerContraseña.TabIndex = 76;
            this.btnVerContraseña.UseVisualStyleBackColor = true;
            this.btnVerContraseña.Click += new System.EventHandler(this.VerContraseña);
            // 
            // txtErrorNombre
            // 
            this.txtErrorNombre.AutoSize = true;
            this.txtErrorNombre.ForeColor = System.Drawing.Color.Red;
            this.txtErrorNombre.Location = new System.Drawing.Point(396, 92);
            this.txtErrorNombre.Name = "txtErrorNombre";
            this.txtErrorNombre.Size = new System.Drawing.Size(14, 17);
            this.txtErrorNombre.TabIndex = 75;
            this.txtErrorNombre.Text = "*";
            this.txtErrorNombre.Visible = false;
            // 
            // txtErrorCedula
            // 
            this.txtErrorCedula.AutoSize = true;
            this.txtErrorCedula.ForeColor = System.Drawing.Color.Red;
            this.txtErrorCedula.Location = new System.Drawing.Point(396, 60);
            this.txtErrorCedula.Name = "txtErrorCedula";
            this.txtErrorCedula.Size = new System.Drawing.Size(14, 17);
            this.txtErrorCedula.TabIndex = 74;
            this.txtErrorCedula.Text = "*";
            this.txtErrorCedula.Visible = false;
            // 
            // txtErrorCorreo
            // 
            this.txtErrorCorreo.AutoSize = true;
            this.txtErrorCorreo.ForeColor = System.Drawing.Color.Red;
            this.txtErrorCorreo.Location = new System.Drawing.Point(926, 59);
            this.txtErrorCorreo.Name = "txtErrorCorreo";
            this.txtErrorCorreo.Size = new System.Drawing.Size(14, 17);
            this.txtErrorCorreo.TabIndex = 73;
            this.txtErrorCorreo.Text = "*";
            this.txtErrorCorreo.Visible = false;
            // 
            // txtErrorUserName
            // 
            this.txtErrorUserName.AutoSize = true;
            this.txtErrorUserName.ForeColor = System.Drawing.Color.Red;
            this.txtErrorUserName.Location = new System.Drawing.Point(396, 27);
            this.txtErrorUserName.Name = "txtErrorUserName";
            this.txtErrorUserName.Size = new System.Drawing.Size(14, 17);
            this.txtErrorUserName.TabIndex = 72;
            this.txtErrorUserName.Text = "*";
            this.txtErrorUserName.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(437, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 17);
            this.label7.TabIndex = 71;
            this.label7.Text = "Estado de Usuario:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(11, 179);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(393, 17);
            this.label6.TabIndex = 70;
            this.label6.Text = "_____________________________________________________________________________";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(437, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 17);
            this.label4.TabIndex = 67;
            this.label4.Text = "Tipo de Usuario:";
            // 
            // rdbUsuarioNormal
            // 
            this.rdbUsuarioNormal.AutoSize = true;
            this.rdbUsuarioNormal.Checked = true;
            this.rdbUsuarioNormal.Location = new System.Drawing.Point(574, 122);
            this.rdbUsuarioNormal.Name = "rdbUsuarioNormal";
            this.rdbUsuarioNormal.Size = new System.Drawing.Size(76, 21);
            this.rdbUsuarioNormal.TabIndex = 8;
            this.rdbUsuarioNormal.TabStop = true;
            this.rdbUsuarioNormal.Text = "Usuario ";
            this.rdbUsuarioNormal.UseVisualStyleBackColor = true;
            // 
            // rdbUsuarioAdmin
            // 
            this.rdbUsuarioAdmin.AutoSize = true;
            this.rdbUsuarioAdmin.Location = new System.Drawing.Point(663, 122);
            this.rdbUsuarioAdmin.Name = "rdbUsuarioAdmin";
            this.rdbUsuarioAdmin.Size = new System.Drawing.Size(113, 21);
            this.rdbUsuarioAdmin.TabIndex = 9;
            this.rdbUsuarioAdmin.TabStop = true;
            this.rdbUsuarioAdmin.Text = "Administrador";
            this.rdbUsuarioAdmin.UseVisualStyleBackColor = true;
            // 
            // txtBoxUsuario
            // 
            this.txtBoxUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxUsuario.Location = new System.Drawing.Point(149, 25);
            this.txtBoxUsuario.MaxLength = 20;
            this.txtBoxUsuario.Name = "txtBoxUsuario";
            this.txtBoxUsuario.Size = new System.Drawing.Size(245, 22);
            this.txtBoxUsuario.TabIndex = 0;
            this.txtBoxUsuario.Leave += new System.EventHandler(this.TxtBoxUsuario_Leave);
            // 
            // estado
            // 
            this.estado.AutoSize = true;
            this.estado.Checked = true;
            this.estado.CheckState = System.Windows.Forms.CheckState.Checked;
            this.estado.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.estado.Location = new System.Drawing.Point(574, 154);
            this.estado.Name = "estado";
            this.estado.Size = new System.Drawing.Size(115, 21);
            this.estado.TabIndex = 10;
            this.estado.Text = "Usuario Activo";
            this.estado.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 17);
            this.label2.TabIndex = 61;
            this.label2.Text = "Nombre Usuario:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(23, 60);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(91, 17);
            this.label24.TabIndex = 35;
            this.label24.Text = "Identificación:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(23, 92);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 17);
            this.label23.TabIndex = 38;
            this.label23.Text = "Nombre:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 58;
            this.label1.Text = "Contraseña:";
            // 
            // txtBoxNombre
            // 
            this.txtBoxNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxNombre.Location = new System.Drawing.Point(149, 89);
            this.txtBoxNombre.MaxLength = 50;
            this.txtBoxNombre.Name = "txtBoxNombre";
            this.txtBoxNombre.Size = new System.Drawing.Size(245, 22);
            this.txtBoxNombre.TabIndex = 2;
            this.txtBoxNombre.Leave += new System.EventHandler(this.TxtBoxNombre_Leave);
            // 
            // txtBoxCLave
            // 
            this.txtBoxCLave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxCLave.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxCLave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxCLave.Location = new System.Drawing.Point(149, 214);
            this.txtBoxCLave.MaxLength = 50;
            this.txtBoxCLave.Name = "txtBoxCLave";
            this.txtBoxCLave.Size = new System.Drawing.Size(245, 22);
            this.txtBoxCLave.TabIndex = 11;
            this.txtBoxCLave.UseSystemPasswordChar = true;
            // 
            // txtBoxID
            // 
            this.txtBoxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxID.Location = new System.Drawing.Point(149, 57);
            this.txtBoxID.MaxLength = 20;
            this.txtBoxID.Name = "txtBoxID";
            this.txtBoxID.Size = new System.Drawing.Size(245, 22);
            this.txtBoxID.TabIndex = 1;
            this.txtBoxID.Leave += new System.EventHandler(this.TxtBoxID_Leave);
            // 
            // txtOp1
            // 
            this.txtOp1.AutoSize = true;
            this.txtOp1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOp1.Location = new System.Drawing.Point(23, 124);
            this.txtOp1.Name = "txtOp1";
            this.txtOp1.Size = new System.Drawing.Size(104, 17);
            this.txtOp1.TabIndex = 40;
            this.txtOp1.Text = "Primer Apellido:";
            // 
            // txtBoxOp1
            // 
            this.txtBoxOp1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxOp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxOp1.Location = new System.Drawing.Point(149, 121);
            this.txtBoxOp1.MaxLength = 50;
            this.txtBoxOp1.Name = "txtBoxOp1";
            this.txtBoxOp1.Size = new System.Drawing.Size(245, 22);
            this.txtBoxOp1.TabIndex = 3;
            // 
            // txtBoxTelefono
            // 
            this.txtBoxTelefono.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxTelefono.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxTelefono.HidePromptOnLeave = true;
            this.txtBoxTelefono.Location = new System.Drawing.Point(574, 24);
            this.txtBoxTelefono.Mask = "(000)0000-0000";
            this.txtBoxTelefono.Name = "txtBoxTelefono";
            this.txtBoxTelefono.RejectInputOnFirstFailure = true;
            this.txtBoxTelefono.ResetOnSpace = false;
            this.txtBoxTelefono.Size = new System.Drawing.Size(99, 22);
            this.txtBoxTelefono.TabIndex = 5;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(437, 28);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 17);
            this.label19.TabIndex = 45;
            this.label19.Text = "Teléfono:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(437, 63);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(122, 17);
            this.label17.TabIndex = 51;
            this.label17.Text = "Correo Electrónico:";
            // 
            // txtBoxMail
            // 
            this.txtBoxMail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxMail.Location = new System.Drawing.Point(574, 57);
            this.txtBoxMail.MaxLength = 50;
            this.txtBoxMail.Name = "txtBoxMail";
            this.txtBoxMail.Size = new System.Drawing.Size(350, 22);
            this.txtBoxMail.TabIndex = 6;
            this.txtBoxMail.Leave += new System.EventHandler(this.TxtBoxMail_Leave);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(437, 92);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 17);
            this.label16.TabIndex = 53;
            this.label16.Text = "Observaciones:";
            // 
            // txtOp2
            // 
            this.txtOp2.AutoSize = true;
            this.txtOp2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOp2.Location = new System.Drawing.Point(23, 156);
            this.txtOp2.Name = "txtOp2";
            this.txtOp2.Size = new System.Drawing.Size(118, 17);
            this.txtOp2.TabIndex = 57;
            this.txtOp2.Text = "Segundo Apellido:";
            // 
            // txtBoxObservaciones
            // 
            this.txtBoxObservaciones.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxObservaciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxObservaciones.Location = new System.Drawing.Point(574, 89);
            this.txtBoxObservaciones.MaxLength = 100;
            this.txtBoxObservaciones.Name = "txtBoxObservaciones";
            this.txtBoxObservaciones.Size = new System.Drawing.Size(350, 22);
            this.txtBoxObservaciones.TabIndex = 7;
            // 
            // txtBoxOp2
            // 
            this.txtBoxOp2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxOp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxOp2.Location = new System.Drawing.Point(149, 153);
            this.txtBoxOp2.MaxLength = 50;
            this.txtBoxOp2.Name = "txtBoxOp2";
            this.txtBoxOp2.Size = new System.Drawing.Size(245, 22);
            this.txtBoxOp2.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBoxBuscar);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lstUsuarios);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(943, 66);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Buscar";
            // 
            // txtBoxBuscar
            // 
            this.txtBoxBuscar.AcceptDecimal = false;
            this.txtBoxBuscar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBuscar.Location = new System.Drawing.Point(758, 25);
            this.txtBoxBuscar.MaxLength = 20;
            this.txtBoxBuscar.Name = "txtBoxBuscar";
            this.txtBoxBuscar.NumeroDecimales = ((short)(2));
            this.txtBoxBuscar.Size = new System.Drawing.Size(166, 25);
            this.txtBoxBuscar.SombrearTexto = false;
            this.txtBoxBuscar.TabIndex = 9;
            this.txtBoxBuscar.TipoControl = GlobalTech.TextBoxControl.TextBoxUniversal.TipoDato.AlfaNumerico;
            this.txtBoxBuscar.WaterMarkColor = System.Drawing.Color.Gray;
            this.txtBoxBuscar.WaterMarkText = "";
            this.txtBoxBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Buscar_KeyPress);
            this.txtBoxBuscar.Leave += new System.EventHandler(this.BoxBuscar_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(629, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "Buscar por Código";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(29, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Seleccione:";
            // 
            // lstUsuarios
            // 
            this.lstUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstUsuarios.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstUsuarios.FormattingEnabled = true;
            this.lstUsuarios.Location = new System.Drawing.Point(102, 25);
            this.lstUsuarios.Name = "lstUsuarios";
            this.lstUsuarios.Size = new System.Drawing.Size(510, 24);
            this.lstUsuarios.TabIndex = 0;
            this.lstUsuarios.SelectedIndexChanged += new System.EventHandler(this.LstUsuarios_SelectedIndexChanged);
            // 
            // FrameMaestroUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 422);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrameMaestroUsuario";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Maestro de Usuario";
            this.panel1.ResumeLayout(false);
            this.opnPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox estado;
        private System.Windows.Forms.TextBox txtBoxCLave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtOp2;
        private System.Windows.Forms.TextBox txtBoxOp2;
        private System.Windows.Forms.TextBox txtBoxObservaciones;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBoxMail;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.MaskedTextBox txtBoxTelefono;
        private System.Windows.Forms.TextBox txtBoxOp1;
        private System.Windows.Forms.Label txtOp1;
        private System.Windows.Forms.TextBox txtBoxNombre;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtBoxID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox opnPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button tbnSalir;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.ComboBox lstUsuarios;
        private System.Windows.Forms.RadioButton rdbUsuarioNormal;
        private System.Windows.Forms.RadioButton rdbUsuarioAdmin;
        private System.Windows.Forms.TextBox txtBoxUsuario;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label txtErrorUserName;
        private System.Windows.Forms.Label txtErrorCorreo;
        private System.Windows.Forms.Label txtErrorCedula;
        private System.Windows.Forms.Label txtErrorNombre;
        private System.Windows.Forms.Button btnVerContraseña;
        private GlobalTech.TextBoxControl.TextBoxUniversal txtBoxBuscar;
        private System.Windows.Forms.Label label8;
    }
}