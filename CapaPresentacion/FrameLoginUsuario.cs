using CapaEntidad.Entidades.Usuarios;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class FrameLoginUsuario : Form
    {

       public FrameLoginUsuario()
        {

            InitializeComponent();
            AddVersionNumber(); 
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = new UsuarioCL().Login(txtBoxUsuario.Text,txtBoxClave.Text);
                if (usuario != null)
                {
                    GlobalConfig.Usuario = usuario;
                    this.Close();
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

        private void VerClave_Click(object sender, EventArgs e)
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
