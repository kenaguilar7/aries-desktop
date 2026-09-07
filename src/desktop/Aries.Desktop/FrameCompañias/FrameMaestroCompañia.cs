using Aries.Reporting.Verificaciones;
using Aries.Reporting.Textos;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Aries.Desktop.Reportes;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using Aries.Desktop.Utils;
using System.Threading.Tasks;

namespace Aries.Desktop.FrameCompañias
{
    public partial class FrameMaestroCompañia : Form
    {
        private readonly IAdministrationService _administrationService;
        List<Company> lst = new List<Company>();

        public FrameMaestroCompañia(IAdministrationService administrationService)
        {
            InitializeComponent();
            _administrationService = administrationService;
        }

        private async void FrameMaestroCompañia_Load(object sender, EventArgs e)
        {
            this.lstCompanias.DataSource = new List<Company>();

            lstTipoId.SelectedIndex = 0;

            var lstCompanies = (await _administrationService.GetAllCompaniesAsync(GlobalConfig.User)).ToList();
            var companyNewCode = await _administrationService.GetCompanyConsecutiveAsync();
            txtCodigoCia.Text = companyNewCode; 

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
            this.txtBoxWeb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxMail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);
            this.txtBoxObservaciones.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SiguienteEnter);

            this.lstCompanias.SelectedIndexChanged -= new System.EventHandler(this.LstCompanias_SelectedIndexChanged);

            lst = (from alias in lstCompanies orderby alias.Code descending select alias).ToList<Company>();
            this.lstMovimientosRegistro.SelectedIndex = 0;
            this.lstCompanias.DataSource = lst;

            var lstMCuentas = new Company[lst.Count + 1];
            lstMCuentas[0] = new Company() { Name = "", Code = "POR DEFECTO" };
            lst.CopyTo(lstMCuentas, 1);

            lstCopiarMaestroCuentas.DataSource = lstMCuentas;
            this.lstCompanias.SelectedIndex = -1;
            this.lstCompanias.SelectedIndexChanged += new System.EventHandler(this.LstCompanias_SelectedIndexChanged);

            var user = GlobalConfig.User.UserType;
            btnDelete.Enabled = (user == UserType.Administrador) ? true : false;

        }

        private void CargarCompaniaFormulario(Company compania)
        {
            lstTipoId.SelectedIndex = Convert.ToInt16(compania.IdType) - 1;
            lstCopiarMaestroCuentas.SelectedIndex = -1;
            lstCopiarMaestroCuentas.Enabled = false;
            btnActualizar.Tag = compania;
            this.txtBoxID.Text = compania.IdNumber;
            this.txtBoxID.ReadOnly = true;
            this.txtBoxNombre.Text = compania.Name;
            this.txtBoxDireccion.Text = compania.Address;
            this.txtBoxTelefono1.Text = compania.PhoneNumber1;
            this.txtBoxTelefono2.Text = compania.PhoneNumber2;
            this.ttCodigo.Text = compania.Code;
            this.groupCodigo.Visible = true;
            this.txtBoxWeb.Text = compania.Web;
            this.txtBoxMail.Text = compania.Mail;
            this.txtBoxObservaciones.Text = compania.Memo;
            this.chekActive.Enabled = true;
            this.chekActive.Checked = compania.Active;
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
            this.lstMovimientosRegistro.SelectedIndex = Convert.ToInt32(compania.CurrencyType) - 1;

            if (compania.CurrencyType == CurrencyTypeCompany.Solo_Colones)
            {
                lstMovimientosRegistro.Enabled = false;
            }
            this.btnActualizar.Visible = true;
            this.btnActualizar.Enabled = true;
            this.btnGuardar.Enabled = false;
            this.btnGuardar.Visible = false;
            this.lstTipoId.Enabled = false;

        }

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

        #region Events
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
                CargarCompaniaFormulario((Company)this.lstCompanias.SelectedItem);
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
            txtBoxID.Mask = VerificaString.MascaraIdentificacion((IdType)lstTipoId.SelectedIndex + 1);

        }



        private void BtnLimpiar(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
        }
        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
        private async void GuardarNuevaCómpaña(object sender, EventArgs e)
        {

            var copiarde = (Company)lstCopiarMaestroCuentas.SelectedItem;

            if (copiarde == null)
            {
                copiarde = ((Company)lstCopiarMaestroCuentas.Items[0]);
            }

            if (MessageBox.Show("Se guardara la compañia, ¿Desea continuar?", "Aries", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                
                try
                {
                    IdType tipo = (IdType)lstTipoId.SelectedIndex + 1;
                    Company persona;
                    if (lstTipoId.SelectedIndex == 0)
                    {
                        persona = new PersonaJuridica(
                                            numeroId: txtBoxID.Text,
                                            tipoID: tipo,
                                            nombre: txtBoxNombre.Text,
                                            TipoMoneda: (CurrencyTypeCompany)lstMovimientosRegistro.SelectedIndex + 1,
                                            representanteLegal: txtBoxOp1.Text,
                                            IDRepresentante: txtBoxOp2.Text,
                                            direccion: txtBoxDireccion.Text,
                                            web: txtBoxWeb.Text,
                                            correo: txtBoxMail.Text,
                                            observaciones: txtBoxObservaciones.Text,
                                            telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text }
                                                                );
                    }
                    else
                    {
                        persona = new PersonaFisica(
                                            numeroId: txtBoxID.Text,
                                            tipoID: tipo,
                                            nombre: txtBoxNombre.Text,
                                            TipoMoneda: (CurrencyTypeCompany)lstMovimientosRegistro.SelectedIndex + 1,
                                            apellidoPaterno: txtBoxOp1.Text,
                                            apellidoMaterno: txtBoxOp2.Text,
                                            direccion: txtBoxDireccion.Text,
                                            web: txtBoxWeb.Text,
                                            correo: txtBoxMail.Text,
                                            observaciones: txtBoxObservaciones.Text,
                                            telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text }
                                                                );
                    }

                    persona.CopyFrom = copiarde.Code;
                    persona.CreatedBy = GlobalConfig.User.Id;
                    await UiBusy.Run(this, () => _administrationService.CreateCompanyAsync(persona));
                    MessageBox.Show("Se registro la compañia correctamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lst.Add(persona);
                    this.LimpiarFormulario();
                    FrameMaestroCompañia_Load(null, null);
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

                List<Company> salida = (from c in lst where c.IdNumber == this.txtBoxID.Text select c).Take(1).ToList<Company>();

                if (salida.Count != 0)
                {
                    CargarCompaniaFormulario(salida[0]);
                }
                else
                {
                    if (!VerificaString.VerificarID(txtBoxID.Text, (IdType)lstTipoId.SelectedIndex + 1, out String mensaje))
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

        private async void ActualizarCompañia(object sender, EventArgs e)
        {
            if (MessageBox.Show("Se actualizaran los datos, ¿Desea continuar?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                try
                {

                    var com = (Company)btnActualizar.Tag;

                    com.Name = txtBoxNombre.Text;
                    com.Address = txtBoxDireccion.Text;
                    com.Web = txtBoxWeb.Text;
                    com.Mail = txtBoxMail.Text;
                    com.Memo = txtBoxObservaciones.Text;
                    com.PhoneNumber1 = this.txtBoxTelefono1.Text; 
                    com.PhoneNumber2 = this.txtBoxTelefono2.Text;
                    com.Active = chekActive.Checked;
                    com.CurrencyType = (CurrencyTypeCompany)lstMovimientosRegistro.SelectedIndex + 1;

                    if (com is PersonaFisica)
                    {
                        ((PersonaFisica)com).MyApellidoPaterno = txtBoxOp1.Text;
                        ((PersonaFisica)com).MyApellidoMaterno = txtBoxOp2.Text;
                    }
                    else if (com is PersonaJuridica)
                    {
                        ((PersonaJuridica)com).MyRepresentanteLegal = txtBoxOp1.Text;
                        ((PersonaJuridica)com).MyIDRepresentanteLegal = txtBoxOp2.Text;
                    }

                    com.CreatedBy = GlobalConfig.User.Id;
                    await _administrationService.UpdateCompanyAsync(com);
                    MessageBox.Show("Se actualizo la compañia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Algo salió mal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        private void TxtBoxBuscarLeave(object sender, EventArgs e)
        {
            try
            {

                if (int.TryParse(txtBoxBuscar.Text, out int num))
                {
                    //Le decimos que me devuelva un String con el formto del parametro
                    var cod = "C" + num.ToString("000");

                    List<Company> salida = (from c in (List<Company>)lstCompanias.DataSource where c.Code == cod select c).Take(1).ToList<Company>();

                    if (salida.Count != 0)
                    {
                        CargarCompaniaFormulario(salida[0]);
                    }
                }
                else
                {
                    List<Company> salida = (from c in (List<Company>)lstCompanias.DataSource where c.Code == txtBoxBuscar.Text select c).Take(1).ToList<Company>();
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
            ReporteCompañia c = new ReporteCompañia(_administrationService);
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

        private async void bntDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Este cambio es irreversible. \n ¿Desea continuar de todas formas?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {

                await _administrationService.DeleteCompanyAsync((Company)btnActualizar.Tag);
                this.LimpiarFormulario(); 
                FrameMaestroCompañia_Load(null, null); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp,MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

    }
}
