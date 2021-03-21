using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        private CuentaCL cuentaCL = new CuentaCL();
        private Cuenta CuentaPadre { get; set; } = new Cuenta();
        public List<Cuenta> lstCuentas = new List<Cuenta>();
        private ICallingForm FormParaEnviarCuenta = null;
        public FrameNuevaCuenta(ICallingForm callingFrom, Cuenta cuenta)
        {
            FormParaEnviarCuenta = callingFrom as ICallingForm;
            CuentaPadre = cuenta;
            InitializeComponent();
            txtCuentaPadre.Text = CuentaPadre.Nombre;

        }
        private void UsuarioKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        private void CrearCuenta(object sender, EventArgs e)
        {
            try
            {
                Cuenta nuevaCuenta = new Cuenta
                {
                    Nombre = txtBoxNombre.Text,
                    Indicador = IndicadorCuenta.Cuenta_Auxiliar,
                    MyCompania = CuentaPadre.MyCompania,
                    TipoCuenta = CuentaPadre.TipoCuenta,
                    Detalle = txtBoxDetalle.Text,
                    Padre = CuentaPadre.Id,
                    Editable = true
                };

                    if (!cuentaCL.VerificarSiEsApta(CuentaPadre, out String Mensaje))
                    {
                        if (MessageBox.Show(Mensaje, TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                        {
                            return;
                        }

                    }

                    if (cuentaCL.Insert(ref nuevaCuenta, CuentaPadre, out String mensaje, GlobalConfig.Usuario))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (FormParaEnviarCuenta != null)
                        {
                            FormParaEnviarCuenta.TransferirCuenta(nuevaCuenta);
                        }

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void CerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
