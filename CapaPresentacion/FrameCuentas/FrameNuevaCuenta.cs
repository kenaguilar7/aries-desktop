using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaPresentacion.Utils;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        private Account CuentaPadre { get; set; } = new Account();
        private ICallingForm FormParaEnviarCuenta = null;
        private readonly IFinancialService _financialService;
        public FrameNuevaCuenta(ICallingForm callingFrom, Account cuenta)
        {
            FormParaEnviarCuenta = callingFrom as ICallingForm;
            CuentaPadre = cuenta;
            InitializeComponent();
            txtCuentaPadre.Text = CuentaPadre.Name;

            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
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
                Account nuevaCuenta = new Account
                {
                    Name = txtBoxNombre.Text,
                    AccountType = AccountType.Cuenta_Auxiliar,
                    CompanyId = CuentaPadre.CompanyId,
                    AccountTag = CuentaPadre.AccountTag,
                    DebOCred = CuentaPadre.DebOCred,
                    Memo = txtBoxDetalle.Text,
                    FatherAccount = CuentaPadre.Id,
                    Editable = true
                };
                
                _financialService.CreateAccount(nuevaCuenta);
                MessageBox.Show("Cuenta creada exitosamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //FormParaEnviarCuenta?.TransferirCuenta(nuevaCuenta);
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
