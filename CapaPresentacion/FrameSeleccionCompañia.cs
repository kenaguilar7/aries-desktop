using CapaEntidad.Entidades.Compañias;
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
using CapaEntidad.Textos;
using CapaPresentacion.Utils;

namespace CapaPresentacion
{
    public partial class FrameSeleccionCompañia : Form
    {

        FrameMenu fm = null;
        public FrameSeleccionCompañia(FrameMenu fm)
        {
            this.fm = fm as FrameMenu;
            InitializeComponent();
            CargarCompañias();
        }

        private void CargarCompañias()
        {

            var _lstCompanies = (from c in new CompañiaCL().GetAll(GlobalConfig.Usuario) where c.Activo == true orderby c.Nombre select c).ToList<Compañia>();

            lstCompanias.DataSource = _lstCompanies;
            lstCompanias.SelectedIndex = -1;
            this.lstCompanias.SelectedIndexChanged += new System.EventHandler(this.lstCompanias_SelectedIndexChanged);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                var c = (Compañia)btnAceptar.Tag;
                if (c != null)
                {

                    if(!IsAvalibleToChangeCompany())return;
                    GlobalConfig.Company = c;
                    fm.comParametro = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("DEBE DE SELECIONAR UNA COMPAÑIA", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public bool IsAvalibleToChangeCompany()
        {
            var isAvalibleToChangeCompany = true; 
            foreach (Form form in Application.OpenForms)
            {
                
                if (form is INeedValidatedForClose && !((INeedValidatedForClose) form).IsAvalibleToClose())
                isAvalibleToChangeCompany = false;
            }

            if (isAvalibleToChangeCompany)
            {
                for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                {
                    if (Application.OpenForms[i].Name != "FrameMenu")
                        Application.OpenForms[i].Close();
                }
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al tratar de cambiar de compañia \nAlgunas ventanas que no se pudieron cerrar. ", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            return isAvalibleToChangeCompany; 
        }

        private void CargarCompaniaFormulario(Compañia compañia)
        {
            // lstCompanias.SelectedIndex = -1;
            txtCompaniaBuscada.Text = compañia.ToString();
            txtCompaniaBuscada.Visible = true;
            txtIdentificacion.Text = compañia.NumeroCedula;
            txtIdentificacion.Visible = true;
            btnAceptar.Tag = compañia;

        }

        private void TxtBoxBuscar_Leave(object sender, EventArgs e)
        {
            try
            {
                /// Editamos el codigo de la compañia
                // /

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
            catch (Exception)
            {
                throw;
            }
        }

        private void lstCompanias_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtCompaniaBuscada.Visible = false;
            var c = (Compañia)lstCompanias.SelectedItem;
            CargarCompaniaFormulario(c);
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBoxBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                TxtBoxBuscar_Leave(null, null);
            }
        }

        private void FrameSeleccionCompañia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                btnAceptar_Click(null, null);
            }
        }
    }
}
