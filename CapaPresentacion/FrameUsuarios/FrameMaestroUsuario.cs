using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Textos;
using CapaEntidad.Verificaciones;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion.Seguridad
{
    public partial class FrameMaestroUsuario : Form
    {
        private UsuarioCL usuarioCL = new UsuarioCL();
        private List<Usuario> ListUsuarios = new List<Usuario>();
        private Usuario userCur;
        public FrameMaestroUsuario()
        {
            InitializeComponent();
            CargarEventos();
            CargarDatos();
        }
        /// <summary>
        /// Carga los eventos 
        /// </summary>
        private void CargarEventos()
        {
            this.txtBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxOp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxOp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxTelefono.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxMail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxObservaciones.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxCLave.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.estado.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            this.txtBoxUsuario.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            rdbUsuarioNormal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);
            rdbUsuarioAdmin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ENTER_KeyPress);

        }
        /// <summary>
        /// Carga datos 
        /// </summary>
        /// <param name="usuario"></param>
        private void CargarDatos()
        {
            lstUsuarios.Items.Clear();
            var lst = usuarioCL.GetAll();
            lstUsuarios.Items.AddRange(lst.ToArray());
            ListUsuarios = lst;

        }
        /// <summary>
        /// Guarda un nuevo usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarUsuario(object sender, EventArgs e)
        {
            try
            {
                //if (MessageBox.Show("¿Desea guardar un nuevo usuario?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                //    == DialogResult.Yes)
                //{

                    var usuario = new Usuario(
                          myCedula: this.txtBoxID.Text,
                          myNombre: this.txtBoxNombre.Text,
                          username: this.txtBoxUsuario.Text,
                          tipoUsuario: TipoUsuario.Usuario,
                          myApellidoPaterno: this.txtBoxOp1.Text,
                          myApellidoMaterno: this.txtBoxOp2.Text,
                          myTelefono: this.txtBoxTelefono.Text,
                          myMail: this.txtBoxMail.Text,
                          myNotas: this.txtBoxObservaciones.Text,
                          myClave: this.txtBoxCLave.Text,
                          myActivo: this.estado.Checked
                          );

                    if (rdbUsuarioAdmin.Checked)
                    {
                        usuario.TipoUsuario = TipoUsuario.Administrador;
                    }

                    if (usuarioCL.Insert(usuario, GlobalConfig.Usuario, out String mensaje))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario(null, null);
                    }
                    else
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Carga el usuario al formulario
        /// </summary>
        private void CargarUsuarioAPanel()
        {
            this.txtBoxID.Text = userCur.MyCedula;
            this.txtBoxNombre.Text = userCur.MyNombre;
            this.txtBoxUsuario.Text = userCur.UserName;
            this.txtBoxOp1.Text = userCur.MyApellidoPaterno;
            this.txtBoxOp2.Text = userCur.MyApellidoMaterno;
            this.txtBoxTelefono.Text = userCur.MyTelefono;
            this.txtBoxMail.Text = userCur.MyMail;
            this.txtBoxObservaciones.Text = userCur.MyNotas;
            this.estado.Checked = userCur.MyActivo;
            this.rdbUsuarioAdmin.Checked = (userCur.TipoUsuario == TipoUsuario.Administrador) ? true : false;
            this.rdbUsuarioNormal.Checked = (userCur.TipoUsuario == TipoUsuario.Usuario) ? true : false;
            this.txtBoxCLave.Text = userCur.MyClave;
            this.btnActualizar.Enabled = true;
            this.btnActualizar.Visible = true;
            this.btnGuardar.Visible = false;
            this.txtBoxUsuario.Enabled = false;
            this.txtBoxID.Enabled = false; 

        }
        /// <summary>
        /// Actualiza el usuario caragado al formulario 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActualizarUsuario(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("¿Desea actualizar este usuario?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    var userUpd = new Usuario(
                        myID: userCur.UsuarioId,
                         myCedula: this.txtBoxID.Text,
                         myNombre: this.txtBoxNombre.Text,
                         username: this.txtBoxUsuario.Text,
                         myApellidoPaterno: this.txtBoxOp1.Text,
                         myApellidoMaterno: this.txtBoxOp2.Text,
                         myTelefono: this.txtBoxTelefono.Text,
                         myMail: this.txtBoxMail.Text,
                         myNotas: this.txtBoxObservaciones.Text,
                         myClave: this.txtBoxCLave.Text,
                         myUpdated: GlobalConfig.Usuario.UsuarioId,
                         myActivo: this.estado.Checked,
                         //myAdmin: this.IsAdmin.Checked,
                         tipoUsuario: TipoUsuario.Usuario
                         );

                    //if (rdbUsuarioAdmin.Checked)
                    //{
                    //    userUpd.TipoUsuario = TipoUsuario.Administrador; 
                    //}

                    userUpd.TipoUsuario = (rdbUsuarioAdmin.Checked) ? TipoUsuario.Administrador : TipoUsuario.Usuario;

                    if (usuarioCL.Update(userUpd, GlobalConfig.Usuario, out String mensaje))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario(null, null);
                    }
                    else
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Sale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        #region Eventos
        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Limpia el formulario y agrega el autoincrementador del codigo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarFormulario(object sender, EventArgs e)
        {
            this.txtErrorUserName.Visible = false;
            this.txtErrorCedula.Visible = false;
            this.txtErrorCorreo.Visible = false;
            this.txtErrorNombre.Visible = false;

            this.txtBoxID.Clear();
            this.txtBoxNombre.Clear();
            this.txtBoxOp1.Clear();
            this.txtBoxOp2.Clear();
            this.txtBoxTelefono.Clear();
            this.txtBoxMail.Clear();
            this.txtBoxObservaciones.Clear();
            this.txtBoxCLave.Clear();
            this.btnActualizar.Visible = false;
            this.btnActualizar.Enabled = false;
            this.btnGuardar.Visible = true;
            this.txtBoxUsuario.Clear();
            this.txtBoxUsuario.Enabled = true;
            this.txtBoxID.Enabled = true; 
            CargarDatos();
        }
        /// <summary>
        /// Esta funcion la llamamos cada vez que alguien presione una tecla
        /// si esta es el enter pasa de control// hace el  tap 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ENTER_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)(Keys.Enter))
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        /// <summary>
        /// Muestra o oculta la contraseña
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerContraseña(object sender, EventArgs e)
        {
            if (txtBoxCLave.UseSystemPasswordChar)
            {
                txtBoxCLave.UseSystemPasswordChar = false;
                this.btnVerContraseña.Image = global::CapaPresentacion.Properties.Resources.icons8_invisible_20;
            }
            else
            {

                txtBoxCLave.UseSystemPasswordChar = true;
                this.btnVerContraseña.Image = global::CapaPresentacion.Properties.Resources.icons8_visible_20;
            }
        }
        /// <summary>
        /// Evento que ocurre cuando el control de busqueda deja el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoxBuscar_Leave(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxBuscar.Text.Length > 0)
            {
                var tyxt = txtBoxBuscar.Text;

                var r = ListUsuarios.Find(x => x.UserName.Equals(txtBoxBuscar.Text, StringComparison.OrdinalIgnoreCase));

                if (r != null)
                {
                    lstUsuarios.SelectedItem = r;
                    //CargarUsuarioEvent();
                }
                else
                {
                    MessageBox.Show($"No se encontro ningun usuario con el usuario {tyxt}", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        /// <summary>
        /// Evento que ocurre cada vez que se presiona una tecla en el cuadro de busqueda
        /// esto con el fin de identificar los enter
        /// y en caso de identificar uno, manda el evento para que deje el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Buscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                BoxBuscar_Leave(null, null);
            }
        }
        /// <summary>
        /// Evento que ocurre cuando la lista de usuarios cambia de item
        /// una vez cambie carga el nuevo usuario al panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            userCur = (Usuario)lstUsuarios.SelectedItem;
            CargarUsuarioAPanel();
        }
        #endregion

        #region Verificaciones
        /// <summary>
        /// Verifica que el username pueda ser asignado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxUsuario_Leave(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxUsuario.Enabled)
            {
                if (usuarioCL.VerificarUserName(txtBoxUsuario.Text) || !VerificaString.IsNullOrWhiteSpace(txtBoxUsuario.Text, "nombre de usuario", out String mensaje))
                {
                    MessageBox.Show("Nombre de usuario no valido", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorUserName.Visible = true;
                }
                else
                {
                    txtErrorUserName.Visible = false;
                }
            }
        }
        /// <summary>
        /// Verifica que el correo electronico sea valido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxMail_Leave(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!VerificaString.ValidarEmail(txtBoxMail.Text))
                {
                    MessageBox.Show("Formato de correo electronico no valido", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorCorreo.Visible = true;
                }
                else
                {
                    txtErrorCorreo.Visible = false;

                }
            }
        }
        /// <summary>
        /// Verifica el número de cédula del usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxID_Leave(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxID.Enabled)
            {
                if (!VerificaString.IsNullOrWhiteSpace(txtBoxID.Text, "Identificación", out String mensaje))
                {
                    MessageBox.Show("Formato de cédula no valido", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorCedula.Visible = true;
                }
                else if ((ListUsuarios.Where(x=> x.MyCedula == txtBoxID.Text).Count()) > 0) {
                    MessageBox.Show("Esta número de identificación se encuentra ocupado", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorCedula.Visible = true;
                }
                else
                {
                    txtErrorCedula.Visible = false;

                }
            }
        }
        /// <summary>
        /// verifica que el nombre del usuario sea valido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxNombre_Leave(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!VerificaString.IsNullOrWhiteSpace(txtBoxNombre.Text, "Nombre", out String mensaje))
                {
                    MessageBox.Show("Nombre de usuario no valido", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorNombre.Visible = true;
                }
                else
                {
                    txtErrorNombre.Visible = false;
                }
            }
        }
        #endregion
    }
}
