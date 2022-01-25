namespace CapaPresentacion.FrameCompañias
{
    partial class FrameMaestroCompañia
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
            this.txtBoxBuscar = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lstCompanias = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnListado = new System.Windows.Forms.Button();
            this.maestro = new System.Windows.Forms.GroupBox();
            this.txtBoxObservaciones = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lstCopiarMaestroCuentas = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtErrorCorreo = new System.Windows.Forms.Label();
            this.txtErrorNombre = new System.Windows.Forms.Label();
            this.txtErorId = new System.Windows.Forms.Label();
            this.lstTipoId = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txtOp2 = new System.Windows.Forms.Label();
            this.txtOp1 = new System.Windows.Forms.Label();
            this.groupCodigo = new System.Windows.Forms.GroupBox();
            this.ttCodigo = new System.Windows.Forms.Label();
            this.ttC = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lstMovimientosRegistro = new System.Windows.Forms.ComboBox();
            this.txtBoxOp2 = new System.Windows.Forms.TextBox();
            this.txtBoxTelefono2 = new System.Windows.Forms.MaskedTextBox();
            this.txtBoxTelefono1 = new System.Windows.Forms.MaskedTextBox();
            this.txtBoxOp1 = new System.Windows.Forms.TextBox();
            this.chekActive = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtBoxMail = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtBoxWeb = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtBoxDireccion = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtBoxID = new System.Windows.Forms.MaskedTextBox();
            this.txtBoxNombre = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtCodigoCia = new System.Windows.Forms.Label();
            this.opnPanel = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbnSalir = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.maestro.SuspendLayout();
            this.groupCodigo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.opnPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtBoxBuscar);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lstCompanias);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(11, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(791, 66);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BUSCAR";
            // 
            // txtBoxBuscar
            // 
            this.txtBoxBuscar.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBuscar.Location = new System.Drawing.Point(708, 27);
            this.txtBoxBuscar.MaxLength = 4;
            this.txtBoxBuscar.Name = "txtBoxBuscar";
            this.txtBoxBuscar.Size = new System.Drawing.Size(59, 22);
            this.txtBoxBuscar.TabIndex = 0;
            this.txtBoxBuscar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBoxBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxBuscar.Leave += new System.EventHandler(this.TxtBoxBuscarLeave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(582, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Buscar por Código";
            // 
            // lstCompanias
            // 
            this.lstCompanias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstCompanias.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCompanias.FormattingEnabled = true;
            this.lstCompanias.Location = new System.Drawing.Point(95, 24);
            this.lstCompanias.Name = "lstCompanias";
            this.lstCompanias.Size = new System.Drawing.Size(468, 25);
            this.lstCompanias.TabIndex = 1;
            this.lstCompanias.TabStop = false;
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Red;
            this.panel1.Location = new System.Drawing.Point(3, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 15);
            this.panel1.TabIndex = 4;
            this.panel1.Visible = false;
            // 
            // btnListado
            // 
            this.btnListado.FlatAppearance.BorderSize = 0;
            this.btnListado.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnListado.Image = global::CapaPresentacion.Properties.Resources.icons8_lista_de_ingredientes_25;
            this.btnListado.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnListado.Location = new System.Drawing.Point(628, 3);
            this.btnListado.Name = "btnListado";
            this.btnListado.Size = new System.Drawing.Size(90, 30);
            this.btnListado.TabIndex = 11;
            this.btnListado.Text = "&Ver Lista";
            this.btnListado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnListado.UseVisualStyleBackColor = true;
            this.btnListado.Click += new System.EventHandler(this.Reporte);
            // 
            // maestro
            // 
            this.maestro.BackColor = System.Drawing.SystemColors.Control;
            this.maestro.Controls.Add(this.txtBoxObservaciones);
            this.maestro.Controls.Add(this.label7);
            this.maestro.Controls.Add(this.lstCopiarMaestroCuentas);
            this.maestro.Controls.Add(this.label6);
            this.maestro.Controls.Add(this.txtErrorCorreo);
            this.maestro.Controls.Add(this.txtErrorNombre);
            this.maestro.Controls.Add(this.txtErorId);
            this.maestro.Controls.Add(this.lstTipoId);
            this.maestro.Controls.Add(this.label4);
            this.maestro.Controls.Add(this.label21);
            this.maestro.Controls.Add(this.txtOp2);
            this.maestro.Controls.Add(this.txtOp1);
            this.maestro.Controls.Add(this.groupCodigo);
            this.maestro.Controls.Add(this.label23);
            this.maestro.Controls.Add(this.label24);
            this.maestro.Controls.Add(this.lstMovimientosRegistro);
            this.maestro.Controls.Add(this.txtBoxOp2);
            this.maestro.Controls.Add(this.txtBoxTelefono2);
            this.maestro.Controls.Add(this.txtBoxTelefono1);
            this.maestro.Controls.Add(this.txtBoxOp1);
            this.maestro.Controls.Add(this.chekActive);
            this.maestro.Controls.Add(this.label1);
            this.maestro.Controls.Add(this.label16);
            this.maestro.Controls.Add(this.txtBoxMail);
            this.maestro.Controls.Add(this.label17);
            this.maestro.Controls.Add(this.txtBoxWeb);
            this.maestro.Controls.Add(this.label18);
            this.maestro.Controls.Add(this.label19);
            this.maestro.Controls.Add(this.txtBoxDireccion);
            this.maestro.Controls.Add(this.label20);
            this.maestro.Controls.Add(this.txtBoxID);
            this.maestro.Controls.Add(this.txtBoxNombre);
            this.maestro.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maestro.Location = new System.Drawing.Point(11, 78);
            this.maestro.Margin = new System.Windows.Forms.Padding(0);
            this.maestro.Name = "maestro";
            this.maestro.Size = new System.Drawing.Size(1098, 304);
            this.maestro.TabIndex = 1;
            this.maestro.TabStop = false;
            // 
            // txtBoxObservaciones
            // 
            this.txtBoxObservaciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxObservaciones.Location = new System.Drawing.Point(720, 156);
            this.txtBoxObservaciones.MaxLength = 100;
            this.txtBoxObservaciones.Multiline = true;
            this.txtBoxObservaciones.Name = "txtBoxObservaciones";
            this.txtBoxObservaciones.Size = new System.Drawing.Size(351, 50);
            this.txtBoxObservaciones.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label7.Location = new System.Drawing.Point(13, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(518, 17);
            this.label7.TabIndex = 67;
            this.label7.Text = "_________________________________________________________________________________" +
    "_____________________";
            // 
            // lstCopiarMaestroCuentas
            // 
            this.lstCopiarMaestroCuentas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstCopiarMaestroCuentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCopiarMaestroCuentas.FormattingEnabled = true;
            this.lstCopiarMaestroCuentas.Location = new System.Drawing.Point(179, 263);
            this.lstCopiarMaestroCuentas.Name = "lstCopiarMaestroCuentas";
            this.lstCopiarMaestroCuentas.Size = new System.Drawing.Size(351, 24);
            this.lstCopiarMaestroCuentas.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 267);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 17);
            this.label6.TabIndex = 64;
            this.label6.Text = "Maesto de cuentas:";
            // 
            // txtErrorCorreo
            // 
            this.txtErrorCorreo.AutoSize = true;
            this.txtErrorCorreo.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtErrorCorreo.ForeColor = System.Drawing.Color.Red;
            this.txtErrorCorreo.Location = new System.Drawing.Point(1074, 59);
            this.txtErrorCorreo.Name = "txtErrorCorreo";
            this.txtErrorCorreo.Size = new System.Drawing.Size(18, 24);
            this.txtErrorCorreo.TabIndex = 63;
            this.txtErrorCorreo.Text = "*";
            this.txtErrorCorreo.Visible = false;
            // 
            // txtErrorNombre
            // 
            this.txtErrorNombre.AutoSize = true;
            this.txtErrorNombre.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtErrorNombre.ForeColor = System.Drawing.Color.Red;
            this.txtErrorNombre.Location = new System.Drawing.Point(537, 89);
            this.txtErrorNombre.Name = "txtErrorNombre";
            this.txtErrorNombre.Size = new System.Drawing.Size(18, 24);
            this.txtErrorNombre.TabIndex = 62;
            this.txtErrorNombre.Text = "*";
            this.txtErrorNombre.Visible = false;
            // 
            // txtErorId
            // 
            this.txtErorId.AutoSize = true;
            this.txtErorId.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtErorId.ForeColor = System.Drawing.Color.Red;
            this.txtErorId.Location = new System.Drawing.Point(322, 59);
            this.txtErorId.Name = "txtErorId";
            this.txtErorId.Size = new System.Drawing.Size(18, 24);
            this.txtErorId.TabIndex = 61;
            this.txtErorId.Text = "*";
            this.txtErorId.Visible = false;
            // 
            // lstTipoId
            // 
            this.lstTipoId.BackColor = System.Drawing.SystemColors.Window;
            this.lstTipoId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstTipoId.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lstTipoId.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTipoId.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstTipoId.FormattingEnabled = true;
            this.lstTipoId.Items.AddRange(new object[] {
            "Cédula Juridica",
            "Cédula Nacional",
            "Dimex",
            "Nite"});
            this.lstTipoId.Location = new System.Drawing.Point(179, 25);
            this.lstTipoId.Name = "lstTipoId";
            this.lstTipoId.Size = new System.Drawing.Size(139, 25);
            this.lstTipoId.TabIndex = 0;
            this.lstTipoId.TabStop = false;
            this.lstTipoId.SelectedIndexChanged += new System.EventHandler(this.TipoIdSelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 17);
            this.label4.TabIndex = 60;
            this.label4.Text = "Moneda (Registro Mov):";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(17, 29);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(140, 17);
            this.label21.TabIndex = 41;
            this.label21.Text = "Tipo de Identificación:";
            // 
            // txtOp2
            // 
            this.txtOp2.AutoSize = true;
            this.txtOp2.Location = new System.Drawing.Point(17, 158);
            this.txtOp2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.txtOp2.Name = "txtOp2";
            this.txtOp2.Size = new System.Drawing.Size(151, 17);
            this.txtOp2.TabIndex = 57;
            this.txtOp2.Text = "ID Representante Legal:";
            // 
            // txtOp1
            // 
            this.txtOp1.AutoSize = true;
            this.txtOp1.Location = new System.Drawing.Point(17, 127);
            this.txtOp1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.txtOp1.Name = "txtOp1";
            this.txtOp1.Size = new System.Drawing.Size(134, 17);
            this.txtOp1.TabIndex = 40;
            this.txtOp1.Text = "Representante Legal:";
            // 
            // groupCodigo
            // 
            this.groupCodigo.Controls.Add(this.ttCodigo);
            this.groupCodigo.Controls.Add(this.ttC);
            this.groupCodigo.Location = new System.Drawing.Point(323, 15);
            this.groupCodigo.Name = "groupCodigo";
            this.groupCodigo.Size = new System.Drawing.Size(101, 37);
            this.groupCodigo.TabIndex = 58;
            this.groupCodigo.TabStop = false;
            this.groupCodigo.Visible = false;
            // 
            // ttCodigo
            // 
            this.ttCodigo.AutoSize = true;
            this.ttCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ttCodigo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ttCodigo.ForeColor = System.Drawing.Color.Green;
            this.ttCodigo.Location = new System.Drawing.Point(60, 13);
            this.ttCodigo.Name = "ttCodigo";
            this.ttCodigo.Size = new System.Drawing.Size(36, 17);
            this.ttCodigo.TabIndex = 1;
            this.ttCodigo.Text = "0000";
            // 
            // ttC
            // 
            this.ttC.AutoSize = true;
            this.ttC.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ttC.ForeColor = System.Drawing.Color.Red;
            this.ttC.Location = new System.Drawing.Point(5, 15);
            this.ttC.Name = "ttC";
            this.ttC.Size = new System.Drawing.Size(48, 15);
            this.ttC.TabIndex = 0;
            this.ttC.Text = "Código:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(17, 95);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 17);
            this.label23.TabIndex = 38;
            this.label23.Text = "Nombre:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(17, 63);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(91, 17);
            this.label24.TabIndex = 35;
            this.label24.Text = "Identificación:";
            // 
            // lstMovimientosRegistro
            // 
            this.lstMovimientosRegistro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstMovimientosRegistro.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstMovimientosRegistro.FormattingEnabled = true;
            this.lstMovimientosRegistro.Items.AddRange(new object[] {
            "Colones y Dolares",
            "Solo Colones",
            "Solo Dolares"});
            this.lstMovimientosRegistro.Location = new System.Drawing.Point(179, 187);
            this.lstMovimientosRegistro.Name = "lstMovimientosRegistro";
            this.lstMovimientosRegistro.Size = new System.Drawing.Size(246, 24);
            this.lstMovimientosRegistro.TabIndex = 5;
            // 
            // txtBoxOp2
            // 
            this.txtBoxOp2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxOp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxOp2.Location = new System.Drawing.Point(179, 156);
            this.txtBoxOp2.MaxLength = 50;
            this.txtBoxOp2.Name = "txtBoxOp2";
            this.txtBoxOp2.Size = new System.Drawing.Size(351, 21);
            this.txtBoxOp2.TabIndex = 4;
            // 
            // txtBoxTelefono2
            // 
            this.txtBoxTelefono2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxTelefono2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxTelefono2.HidePromptOnLeave = true;
            this.txtBoxTelefono2.Location = new System.Drawing.Point(965, 124);
            this.txtBoxTelefono2.Mask = "(000)0000-0000";
            this.txtBoxTelefono2.Name = "txtBoxTelefono2";
            this.txtBoxTelefono2.RejectInputOnFirstFailure = true;
            this.txtBoxTelefono2.ResetOnSpace = false;
            this.txtBoxTelefono2.Size = new System.Drawing.Size(106, 22);
            this.txtBoxTelefono2.TabIndex = 10;
            // 
            // txtBoxTelefono1
            // 
            this.txtBoxTelefono1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxTelefono1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxTelefono1.HidePromptOnLeave = true;
            this.txtBoxTelefono1.Location = new System.Drawing.Point(720, 124);
            this.txtBoxTelefono1.Mask = "(000)0000-0000";
            this.txtBoxTelefono1.Name = "txtBoxTelefono1";
            this.txtBoxTelefono1.RejectInputOnFirstFailure = true;
            this.txtBoxTelefono1.ResetOnSpace = false;
            this.txtBoxTelefono1.Size = new System.Drawing.Size(106, 22);
            this.txtBoxTelefono1.TabIndex = 9;
            // 
            // txtBoxOp1
            // 
            this.txtBoxOp1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxOp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxOp1.Location = new System.Drawing.Point(179, 124);
            this.txtBoxOp1.MaxLength = 50;
            this.txtBoxOp1.Name = "txtBoxOp1";
            this.txtBoxOp1.Size = new System.Drawing.Size(351, 22);
            this.txtBoxOp1.TabIndex = 3;
            // 
            // chekActive
            // 
            this.chekActive.AutoSize = true;
            this.chekActive.Checked = true;
            this.chekActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chekActive.Location = new System.Drawing.Point(719, 219);
            this.chekActive.Name = "chekActive";
            this.chekActive.Size = new System.Drawing.Size(129, 21);
            this.chekActive.TabIndex = 12;
            this.chekActive.Text = "Compañia Activa";
            this.chekActive.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(841, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 17);
            this.label1.TabIndex = 59;
            this.label1.Text = "Teléfono Celular:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(591, 158);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 17);
            this.label16.TabIndex = 53;
            this.label16.Text = "Observaciones:";
            // 
            // txtBoxMail
            // 
            this.txtBoxMail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxMail.Location = new System.Drawing.Point(720, 60);
            this.txtBoxMail.MaxLength = 50;
            this.txtBoxMail.Name = "txtBoxMail";
            this.txtBoxMail.Size = new System.Drawing.Size(351, 22);
            this.txtBoxMail.TabIndex = 7;
            this.txtBoxMail.Leave += new System.EventHandler(this.TxtBoxMailLeave);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(591, 63);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(122, 17);
            this.label17.TabIndex = 51;
            this.label17.Text = "Correo Electrónico:";
            // 
            // txtBoxWeb
            // 
            this.txtBoxWeb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxWeb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxWeb.Location = new System.Drawing.Point(720, 92);
            this.txtBoxWeb.MaxLength = 50;
            this.txtBoxWeb.Name = "txtBoxWeb";
            this.txtBoxWeb.Size = new System.Drawing.Size(351, 22);
            this.txtBoxWeb.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(591, 95);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(98, 17);
            this.label18.TabIndex = 49;
            this.label18.Text = "Dirección Web:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(591, 127);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(84, 17);
            this.label19.TabIndex = 45;
            this.label19.Text = "Teléfono Fijo";
            // 
            // txtBoxDireccion
            // 
            this.txtBoxDireccion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxDireccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxDireccion.Location = new System.Drawing.Point(720, 27);
            this.txtBoxDireccion.MaxLength = 100;
            this.txtBoxDireccion.Name = "txtBoxDireccion";
            this.txtBoxDireccion.Size = new System.Drawing.Size(351, 22);
            this.txtBoxDireccion.TabIndex = 6;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(591, 30);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(66, 17);
            this.label20.TabIndex = 42;
            this.label20.Text = "Dirección:";
            // 
            // txtBoxID
            // 
            this.txtBoxID.Enabled = false;
            this.txtBoxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxID.Location = new System.Drawing.Point(179, 60);
            this.txtBoxID.Name = "txtBoxID";
            this.txtBoxID.Size = new System.Drawing.Size(139, 22);
            this.txtBoxID.TabIndex = 1;
            this.txtBoxID.Leave += new System.EventHandler(this.TxtBoxIDLeave);
            // 
            // txtBoxNombre
            // 
            this.txtBoxNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBoxNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxNombre.Location = new System.Drawing.Point(179, 92);
            this.txtBoxNombre.MaxLength = 50;
            this.txtBoxNombre.Name = "txtBoxNombre";
            this.txtBoxNombre.Size = new System.Drawing.Size(351, 22);
            this.txtBoxNombre.TabIndex = 2;
            this.txtBoxNombre.Leave += new System.EventHandler(this.TxtBoxNombreLeave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(16, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(219, 21);
            this.label5.TabIndex = 11;
            this.label5.Text = "Código Próxima Compañia:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtCodigoCia);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.groupBox3.Location = new System.Drawing.Point(810, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 63);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // txtCodigoCia
            // 
            this.txtCodigoCia.AutoSize = true;
            this.txtCodigoCia.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoCia.ForeColor = System.Drawing.Color.Green;
            this.txtCodigoCia.Location = new System.Drawing.Point(232, 24);
            this.txtCodigoCia.Name = "txtCodigoCia";
            this.txtCodigoCia.Size = new System.Drawing.Size(46, 21);
            this.txtCodigoCia.TabIndex = 0;
            this.txtCodigoCia.Text = "0000";
            // 
            // opnPanel
            // 
            this.opnPanel.Controls.Add(this.flowLayoutPanel1);
            this.opnPanel.Location = new System.Drawing.Point(11, 383);
            this.opnPanel.Name = "opnPanel";
            this.opnPanel.Size = new System.Drawing.Size(1098, 55);
            this.opnPanel.TabIndex = 2;
            this.opnPanel.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tbnSalir);
            this.flowLayoutPanel1.Controls.Add(this.btnGuardar);
            this.flowLayoutPanel1.Controls.Add(this.btnActualizar);
            this.flowLayoutPanel1.Controls.Add(this.btnLimpiar);
            this.flowLayoutPanel1.Controls.Add(this.btnListado);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1092, 36);
            this.flowLayoutPanel1.TabIndex = 4;
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
            this.tbnSalir.Location = new System.Drawing.Point(1014, 3);
            this.tbnSalir.Name = "tbnSalir";
            this.tbnSalir.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbnSalir.Size = new System.Drawing.Size(75, 30);
            this.tbnSalir.TabIndex = 1;
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
            this.btnGuardar.Location = new System.Drawing.Point(921, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(87, 30);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.GuardarNuevaCómpaña);
            // 
            // btnActualizar
            // 
            this.btnActualizar.Enabled = false;
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizar.Image = global::CapaPresentacion.Properties.Resources.icons8_renovar_suscripción_25;
            this.btnActualizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnActualizar.Location = new System.Drawing.Point(815, 3);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 2;
            this.btnActualizar.Text = "&Actualizar";
            this.btnActualizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Visible = false;
            this.btnActualizar.Click += new System.EventHandler(this.ActualizarCompañia);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Image = global::CapaPresentacion.Properties.Resources.icons8_limpiaparabrisas_25;
            this.btnLimpiar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLimpiar.Location = new System.Drawing.Point(724, 3);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(85, 30);
            this.btnLimpiar.TabIndex = 3;
            this.btnLimpiar.Text = "&Limpiar";
            this.btnLimpiar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar);
            // 
            // FrameMaestroCompañia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1121, 447);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.opnPanel);
            this.Controls.Add(this.maestro);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrameMaestroCompañia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Maestro de Compañias";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.maestro.ResumeLayout(false);
            this.maestro.PerformLayout();
            this.groupCodigo.ResumeLayout(false);
            this.groupCodigo.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.opnPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBoxBuscar;
        private System.Windows.Forms.Button btnListado;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox lstCompanias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox maestro;
        private System.Windows.Forms.GroupBox groupCodigo;
        private System.Windows.Forms.Label ttCodigo;
        private System.Windows.Forms.Label ttC;
        private System.Windows.Forms.Label txtOp2;
        private System.Windows.Forms.TextBox txtBoxOp2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBoxMail;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtBoxWeb;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.MaskedTextBox txtBoxTelefono1;
        private System.Windows.Forms.TextBox txtBoxDireccion;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.MaskedTextBox txtBoxID;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtBoxOp1;
        private System.Windows.Forms.Label txtOp1;
        private System.Windows.Forms.TextBox txtBoxNombre;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox lstTipoId;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label txtCodigoCia;
        private System.Windows.Forms.GroupBox opnPanel;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button tbnSalir;
        private System.Windows.Forms.MaskedTextBox txtBoxTelefono2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chekActive;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox lstMovimientosRegistro;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label txtErrorCorreo;
        private System.Windows.Forms.Label txtErrorNombre;
        private System.Windows.Forms.Label txtErorId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox lstCopiarMaestroCuentas;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBoxObservaciones;
    }
}