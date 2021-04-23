using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.cods;
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
    public partial class FrameSeleccionCuenta : Form, ICallingForm
    {
        private List<Cuenta> LstCuentas { get; set; }
        private ICallingForm _getCuenta;
        public FrameSeleccionCuenta(ICallingForm callingForm)
        {
            _getCuenta = callingForm as ICallingForm;
            InitializeComponent();
            CargarCuentas();
        }
        private void CargarCuentas()
        {
            LstCuentas = new CuentaCL().GetAll(GlobalConfig.Company);
            treeCuentas.Nodes.AddRange(TreeViewCuentas.CrearTreeView(LstCuentas));
        }
        private void SeleccionaCuentaEnTreeView(object sender, EventArgs e)
        {
            DevolverCuenta();
        }
        private void DevolverCuenta()
        {
            try
            {
                Cuenta sele = (Cuenta)treeCuentas.SelectedNode.Tag;
                sele.PathDirection = treeCuentas.SelectedNode.FullPath;

                if (_getCuenta.TransferirCuenta(sele))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cuenta no valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }
        private void SeleccionarClick(object sender, EventArgs e)
        {
            if (treeCuentas.SelectedNode != null)
            {
                DevolverCuenta();
            }
            else
            {
                MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void ExpandirArbol(object sender, EventArgs e)
        {
            treeCuentas.ExpandAll();
        }
        private void ContraerArbol(object sender, EventArgs e)
        {
            treeCuentas.CollapseAll();
        }
        private void CrearNuevaCuenta(object sender, EventArgs e)
        {
            try
            {
                if (treeCuentas.SelectedNode is null)
                {
                    MessageBox.Show("Seleccione una cuenta ", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;

                }
                else if (!(treeCuentas.SelectedNode.Tag is Cuenta cuenta) || cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    MessageBox.Show("No se puede crear cuentas en este nivel", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    FrameNuevaCuenta nv = new FrameNuevaCuenta(this, cuenta);
                    nv.lstCuentas = LstCuentas;
                    nv.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BucarNodes(object sender, KeyEventArgs e)
        {
            var t = txtNombreCuenta.Text;
            var nod = TreeViewCuentas.BuscarNodo(txtNombreCuenta.Text, this.treeCuentas);

            if (nod != null && nod.Count > 0)
            {
                treeCuentas.SelectedNode = nod[0];
                treeCuentas.SelectedNode.BackColor = Color.CornflowerBlue;
            }
            else
            {
                treeCuentas.SelectedNode = null;
            }
        }
        private void TxtNombreCuentaKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                DevolverCuenta();
            }
        }

        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, LstCuentas);
                return true;
            }
            else { return false; }
        }
    }
}
