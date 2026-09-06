using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Mappers;
using CapaEntidad.Textos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        private readonly IFinancialService _financialService;
        private Cuenta CuentaPadre { get; set; } = new Cuenta();
        public List<Cuenta> lstCuentas = new List<Cuenta>();
        private ICallingForm FormParaEnviarCuenta = null;

        public FrameNuevaCuenta(ICallingForm callingFrom, Cuenta cuenta, IFinancialService financialService)
        {
            FormParaEnviarCuenta = callingFrom as ICallingForm;
            CuentaPadre = cuenta;
            _financialService = financialService;
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

                var parentAccount = CuentaMapper.ToAccount(CuentaPadre);
                if (!_financialService.EvaluateParentForNewChild(parentAccount, out String Mensaje))
                {
                    if (MessageBox.Show(Mensaje, TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    {
                        return;
                    }
                }

                var account = CuentaMapper.ToAccount(nuevaCuenta);
                account.UpdatedBy = GlobalConfig.Usuario.Id;
                _financialService.CreateAccount(account, parentAccount);
                CuentaMapper.CopyBalancesToCuenta(account, nuevaCuenta);
                CuentaPadre.Indicador = (IndicadorCuenta)parentAccount.AccountType;

                MessageBox.Show(AccountRules.CreateSuccessMessage, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (FormParaEnviarCuenta != null)
                {
                    FormParaEnviarCuenta.TransferirCuenta(nuevaCuenta);
                }

                this.Close();
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
