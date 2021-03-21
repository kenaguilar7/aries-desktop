using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAdministrarMeses : Form
    {
        private CuentaCL _cuentaCL = new CuentaCL();
        private FechaTransaccionCL fechaCL = new FechaTransaccionCL();
        private IEnumerable<Cuenta> Cuentas { get; set; }

        public FrameAdministrarMeses()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {

            //List<FechaTransaccion> lst = fechaCL.GetAll(compania, usuario);
            DataTable dt = fechaCL.GetDataTable(GlobalConfig.Compañia, GlobalConfig.Usuario);
            Cuentas = _cuentaCL.GetAll(GlobalConfig.Compañia);

            dtRegistros.DataSource = dt;

            lstAbrirMes.DataSource = fechaCL.FechaAbrirMes(GlobalConfig.Compañia, GlobalConfig.Usuario);
            lstCerrarMes.DataSource = fechaCL.GetAllActive(GlobalConfig.Compañia, GlobalConfig.Usuario);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Se abrira un mes ¿Desea continuar?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (fechaCL.Insert((FechaTransaccion)lstAbrirMes.SelectedItem, GlobalConfig.Compañia, GlobalConfig.Usuario, out String mensaje))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void BtnCerrarMes_Click(object sender, EventArgs e)
        {
            try
            {
                
                if ((lstCerrarMes.Items.Count > 0) && MessageBox.Show("¿Desea cerrar este mes?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    FechaTransaccion fechaTransaccion = (FechaTransaccion)lstCerrarMes.SelectedItem;
                    fechaTransaccion.Cerrada = true;

                    if (fechaCL.CerrarMes(fechaTransaccion, GlobalConfig.Compañia, GlobalConfig.Usuario, out string mensaje))
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
