using AriesContador.Core;
using AriesContador.Core.Models;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class LoginForm : Form
    {
        private readonly IHttpAdministrationService _httpService;

        public LoginForm(IHttpAdministrationService httpAdministrationService)
        {
            InitializeComponent();
            AddVersionNumber();
            _httpService = httpAdministrationService;
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                var param = new Login()
                {
                    UserId = txtBoxUsuario.Text,
                    Password = txtBoxClave.Text
                };

                //var param = new Login()
                //{
                //    UserId = "kenneth",
                //    Password = "96321"
                //};

                var token = await _httpService.Login(param);

                if (token.Token != null)
                {
                    EnvironmentVariable.ApiToken = token;
                    GlobalConfig.User = token.User;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrecta", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBoxUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void txtBoxClave_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                if (!String.IsNullOrWhiteSpace(txtBoxUsuario.Text))
                {
                    btnAceptar_Click(null, null);
                }
                else
                {
                    txtBoxUsuario.Focus();
                }
            }
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
           this.Close();

        }

        private void TogglePasswordVisibilityButton_Click(object sender, EventArgs e)
        {
            if (txtBoxClave.UseSystemPasswordChar)
            {
                txtBoxClave.UseSystemPasswordChar = false;
                this.txtVerClave.Image = global::CapaPresentacion.Properties.Resources.icons8_invisible_20;
            }
            else
            {

                txtBoxClave.UseSystemPasswordChar = true;
                this.txtVerClave.Image = global::CapaPresentacion.Properties.Resources.icons8_visible_20;
            }



        }
        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += $" v.{versionInfo.FileVersion}";
        }

        private void FrameLoginUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Application.ExitThread();
            //Application.Exit(); 
        }
    }


}
