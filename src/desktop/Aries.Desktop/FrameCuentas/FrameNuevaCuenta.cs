using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Enumeradores;
using Aries.Reporting.Interfaces;
using Aries.Reporting.Mappers;
using Aries.Reporting.Textos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.FrameCuentas
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

        private async void CrearCuenta(object sender, EventArgs e)
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
                var (ok, msg) = await _financialService.EvaluateParentForNewChildAsync(parentAccount);
                if (!ok)
                {
                    if (MessageBox.Show(msg, TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    {
                        return;
                    }
                }

                var account = CuentaMapper.ToAccount(nuevaCuenta);
                account.UpdatedBy = GlobalConfig.Usuario.Id;
                await _financialService.CreateAccountAsync(account, parentAccount);
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
