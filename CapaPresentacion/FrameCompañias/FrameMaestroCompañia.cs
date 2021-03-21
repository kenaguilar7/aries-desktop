using CapaLogica;
using System;
using System.Windows.Forms;
using CapaEntidad.Enumeradores;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Verificaciones;
using System.Linq;
using CapaEntidad.Entidades.Usuarios;
using System.Collections.Generic;
using CapaEntidad.Textos;
using CapaPresentacion.Reportes;
using System.Drawing;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CapaPresentacion.FrameCompañias
{
    public partial class FrameMaestroCompañia : Form
    {
        public FrameMaestroCompañia()
        {
            InitializeComponent();
            CargarDatos();
        }
        CompañiaCL compañiaCL = new CompañiaCL();

        //BindingList<Compañia> lst = new BindingList<Compañia>();
        List<Compañia> lst = new List<Compañia>();



        /// <summary>
        /// Carga datos y eventos al formulario
        /// </summary>
        /// <param name="usuario"></param>
        public async Task CargarDatos()
        {
            this.lstCompanias.DataSource = new List<Compañia>();

            lstTipoId.SelectedIndex = 0;
            txtCodigoCia.Text = await Task.Run(() => compañiaCL.NuevoCodigo());
            var lstCompanies = await Task.Run(() => compañiaCL.GetAll(GlobalConfig.Usuario));

            //eventos
            this.lstTipoId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxOp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxOp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxDireccion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxTelefono1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxTelefono2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.lstMovimientosRegistro.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            // this.panelOps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            //this.btnAgregaTelefono.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxWeb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxMail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxObservaciones.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            //this.txtBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxID_KeyPress);

            lst = (from alias in lstCompanies orderby alias.Codigo descending select alias).ToList<Compañia>();
            this.lstMovimientosRegistro.SelectedIndex = 0;
            this.lstCompanias.DataSource = lst;

            /**
             * 
             * en eL paso de asignar la lista de compañias al control de donde se podra seleccionar para crear un maestro de
             * cuentas a partir de la compañia seleccionada, se tuvo que duplicar la copia, porque el copilador interpreta esta como solo una
             * y cuando se selecciona una opcion en un control en el otro cambia, esto porque ambas listas apuntas al mismo espacio en memoria
             * 
             */

            ///Creamos una nueva lista para almacenar las compañias que podran ser usadas para duplicar su maestro de cuentas
            ///Y agregamos una nueva compañia con el nombre maestro  de cuentas por defecto, esta sera la opcion que el 
            ///usuario puede marcar para que no duplique de ninguna otra compañia
            var lstMCuentas = new Compañia[lst.Count + 1];
            lstMCuentas[0] = new Compañia() { Nombre = "", Codigo = "POR DEFECTO" };
            ///Copiamos la lista de cuentas a la nueva lista para guardarla en el seleccionador de maestros de cuenta
            lst.CopyTo(lstMCuentas, 1);
            lstCopiarMaestroCuentas.DataSource = lstMCuentas;
            this.lstCompanias.SelectedIndex = -1;
            this.lstCompanias.SelectedIndexChanged += new System.EventHandler(this.LstCompanias_SelectedIndexChanged);
        }
        /// <summary>
        /// Carga la compañia pasada por parametros al forumulario
        /// </summary>
        /// <param name="compania"></param>
        private void CargarCompaniaFormulario(Compañia compania)
        {
            /**
             * la lista lstTipoId tiene como primer indice 0; mientras que 
             * los unum de tipo id tiene como primer indice 1
             * en este caso le restamos 1 
             */
            lstTipoId.SelectedIndex = Convert.ToInt16(compania.TipoId) - 1;
            lstCopiarMaestroCuentas.SelectedIndex = -1;
            lstCopiarMaestroCuentas.Enabled = false;
            btnActualizar.Tag = compania;
            this.txtBoxID.Text = compania.NumeroCedula;
            this.txtBoxID.ReadOnly = true;
            this.txtBoxNombre.Text = compania.Nombre;
            this.txtBoxDireccion.Text = compania.Direccion;
            this.txtBoxTelefono1.Text = compania.Telefono[0];
            this.txtBoxTelefono2.Text = compania.Telefono[1];
            this.ttCodigo.Text = compania.Codigo;
            this.groupCodigo.Visible = true;
            this.txtBoxWeb.Text = compania.Web;
            this.txtBoxMail.Text = compania.Correo;
            this.txtBoxObservaciones.Text = compania.Observaciones;
            this.chekActive.Enabled = true;
            this.chekActive.Checked = compania.Activo;
            if (compania is PersonaFisica)
            {
                txtBoxOp1.Text = ((PersonaFisica)compania).MyApellidoPaterno;
                txtBoxOp2.Text = ((PersonaFisica)compania).MyApellidoMaterno;
            }
            if (compania is PersonaJuridica)
            {

                txtBoxOp1.Text = ((PersonaJuridica)compania).MyRepresentanteLegal;
                txtBoxOp2.Text = ((PersonaJuridica)compania).MyIDRepresentanteLegal;

            }
            this.lstMovimientosRegistro.SelectedIndex = Convert.ToInt32(compania.TipoMoneda) - 1;

            if (compania.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            {
                lstMovimientosRegistro.Enabled = false;
            }
            this.btnActualizar.Visible = true;
            this.btnActualizar.Enabled = true;
            this.btnGuardar.Enabled = false;
            this.btnGuardar.Visible = false;
            this.lstTipoId.Enabled = false;

        }
        /// <summary>
        /// Limpia el formulario
        /// </summary>
        private void LimpiarFormulario()
        {

            this.lstTipoId.Enabled = true;
            this.txtBoxID.Clear();
            this.txtBoxNombre.Clear();
            this.txtBoxOp1.Clear();
            this.txtBoxOp2.Clear();
            this.txtBoxWeb.Clear();
            this.txtBoxMail.Clear();
            this.txtBoxTelefono1.Clear();
            this.txtBoxTelefono2.Clear();
            this.txtBoxDireccion.Clear();
            this.txtBoxObservaciones.Clear();
            this.btnGuardar.Enabled = true;
            this.btnGuardar.Visible = true;
            this.btnActualizar.Enabled = false;
            this.btnActualizar.Visible = false;
            this.btnActualizar.Tag = null;
            this.txtBoxID.ReadOnly = false;
            this.groupCodigo.Visible = false;
            this.chekActive.Enabled = false;
            this.txtBoxBuscar.Clear();
            this.lstMovimientosRegistro.Enabled = true;
            this.lstMovimientosRegistro.SelectedIndex = 0;
            txtErorId.Visible = false;
            txtErrorNombre.Visible = false;
            txtErrorCorreo.Visible = false;
            lstCopiarMaestroCuentas.Enabled = true;

            //lstCopiarMaestroCuentas.SelectedIndex = 1;
        }

        #region Eventos
        private void SiguienteEnter(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)(Keys.Enter))
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }


        }
        private void LstCompanias_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CargarCompaniaFormulario((Compañia)this.lstCompanias.SelectedItem);
                // btnActualizar.Tag = (Compañia)this.lstCompanias.SelectedItem;
            }
            catch (Exception)
            {

            }
        }

        private void TipoIdSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Convert.ToString((((DataRowView)lstTipoId.SelectedItem).Row.ItemArray)[0]) == "CEDULA JURIDICA")
            this.LimpiarFormulario();
            if (lstTipoId.SelectedIndex != 0)
            {
                txtOp1.Text = "Primer Apellido:";
                txtOp2.Text = "Segundo Apellido:";
            }
            else
            {
                txtOp1.Text = "Representante Legal:";
                txtOp2.Text = "ID Representante Legal:";
            }


            txtBoxID.Enabled = true;
            txtBoxID.Mask = VerificaString.MascaraIdentificacion((TipoID)lstTipoId.SelectedIndex + 1);

        }



        private void BtnLimpiar(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
        }
        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Obtiene los datos del formulario los valida y registra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarNuevaCómpaña(object sender, EventArgs e)
        {

            var copiarde = (Compañia)lstCopiarMaestroCuentas.SelectedItem;

            if (copiarde == null)
            {
                copiarde = ((Compañia)lstCopiarMaestroCuentas.Items[0]);
            }

            if (MessageBox.Show("Se guardara la compañia, ¿Desea continuar?", "Aries", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                
                try
                {

                

                    TipoID tipo = (TipoID)lstTipoId.SelectedIndex + 1;

                    if (lstTipoId.SelectedIndex == 0)
                    {

                        var Persona = new PersonaJuridica(
                                            numeroId: txtBoxID.Text,
                                            tipoID: tipo,
                                            nombre: txtBoxNombre.Text,
                                            TipoMoneda: (TipoMonedaCompañia)lstMovimientosRegistro.SelectedIndex + 1,
                                            representanteLegal: txtBoxOp1.Text,
                                            IDRepresentante: txtBoxOp2.Text,
                                            direccion: txtBoxDireccion.Text,
                                            web: txtBoxWeb.Text,
                                            correo: txtBoxMail.Text,
                                            observaciones: txtBoxObservaciones.Text,
                                            telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text }
                                                                );

                        if (compañiaCL.Insert(Persona, GlobalConfig.Usuario, copiarde, out String mensaje))
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lst.Add(Persona);
                            this.LimpiarFormulario();
                            //CargarDatos(); 
                        }
                        else
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        var Persona = new PersonaFisica(
                                            numeroId: txtBoxID.Text,
                                            tipoID: tipo,
                                            nombre: txtBoxNombre.Text,
                                            TipoMoneda: (TipoMonedaCompañia)lstMovimientosRegistro.SelectedIndex + 1,
                                            apellidoPaterno: txtBoxOp1.Text,
                                            apellidoMaterno: txtBoxOp2.Text,
                                            direccion: txtBoxDireccion.Text,
                                            web: txtBoxWeb.Text,
                                            correo: txtBoxMail.Text,
                                            observaciones: txtBoxObservaciones.Text,
                                            telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text }
                                                                );
                        if (compañiaCL.Insert(Persona, GlobalConfig.Usuario, copiarde, out String mensaje))
                        {

                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lst.Add(Persona);
                            LimpiarFormulario();
                            //CargarDatos(); 
                        }
                        else
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void TxtBoxIDLeave(object sender, EventArgs e)
        {

            try
            {
                if (!this.Visible)
                {
                    return;
                }

                List<Compañia> salida = (from c in lst where c.NumeroCedula == this.txtBoxID.Text select c).Take(1).ToList<Compañia>();

                if (salida.Count != 0)
                {
                    CargarCompaniaFormulario(salida[0]);
                }
                else
                {
                    if (!VerificaString.VerificarID(txtBoxID.Text, (TipoID)lstTipoId.SelectedIndex + 1, out String mensaje))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        txtErorId.Visible = true;
                    }
                    else
                    {
                        txtErorId.Visible = false;
                    }
                }

            }

            catch (Exception)
            {

            }
        }

        private void ActualizarCompañia(object sender, EventArgs e)
        {
            if (MessageBox.Show("Se actualizaran los datos, ¿Desea continuar?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                try
                {

                    var com = (Compañia)btnActualizar.Tag;

                    com.Nombre = txtBoxNombre.Text;
                    com.Direccion = txtBoxDireccion.Text;
                    com.Web = txtBoxWeb.Text;
                    com.Correo = txtBoxMail.Text;
                    com.Observaciones = txtBoxObservaciones.Text;
                    com.Telefono = new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text };
                    com.Activo = chekActive.Checked;
                    com.TipoMoneda = (TipoMonedaCompañia)lstMovimientosRegistro.SelectedIndex + 1;

                    if (com is PersonaFisica)
                    {
                        ((PersonaFisica)com).MyApellidoPaterno = txtBoxOp1.Text;
                        ((PersonaFisica)com).MyApellidoMaterno = txtBoxOp2.Text;
                        if (compañiaCL.Update(com, GlobalConfig.Usuario, out String mensaje))
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }

                    }
                    else
                    {
                        ((PersonaJuridica)com).MyRepresentanteLegal = txtBoxOp1.Text;
                        ((PersonaJuridica)com).MyIDRepresentanteLegal = txtBoxOp2.Text;
                        if (compañiaCL.Update(com, GlobalConfig.Usuario, out String mensaje))
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Algo salió mal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region METODOS DE SOPORTE
        //mejorar esto!!!!!!!!!!!!!!!!!!

        #endregion

        private void TxtBoxBuscarLeave(object sender, EventArgs e)
        {
            try
            {

                if (int.TryParse(txtBoxBuscar.Text, out int num))
                {
                    //Le decimos que me devuelva un String con el formto del parametro
                    var cod = "C" + num.ToString("000");

                    List<Compañia> salida = (from c in (List<Compañia>)lstCompanias.DataSource where c.Codigo == cod select c).Take(1).ToList<Compañia>();

                    if (salida.Count != 0)
                    {
                        CargarCompaniaFormulario(salida[0]);
                    }
                }
                else
                {
                    List<Compañia> salida = (from c in (List<Compañia>)lstCompanias.DataSource where c.Codigo == txtBoxBuscar.Text select c).Take(1).ToList<Compañia>();
                    if (salida.Count != 0)
                    {
                        CargarCompaniaFormulario(salida[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Reporte(object sender, EventArgs e)
        {
            ReporteCompañia c = new ReporteCompañia();
            c.MdiParent = this.MdiParent;
            c.Show();

        }

        private void TxtBoxNombreLeave(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!VerificaString.IsNullOrWhiteSpace(txtBoxNombre.Text, "Nombre", out String mensaje))
                {
                    MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorNombre.Visible = true;
                }
                else
                {
                    txtErrorNombre.Visible = false;
                }
            }
        }

        private void TxtBoxMailLeave(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!VerificaString.ValidarEmail(txtBoxMail.Text))
                {
                    MessageBox.Show("Correo electronico no valido", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtErrorCorreo.Visible = true;
                }
                else
                {
                    txtErrorCorreo.Visible = false;

                }
            }
        }
    }
}
