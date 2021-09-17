using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.cods;
using CapaPresentacion.Reportes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameMaestroCuenta : Form, ICallingForm
    {
        private CuentaCL _cuentaCL { get; } = new CuentaCL();
        private FechaTransaccionCL _fechaTransaccionCL { get; } = new FechaTransaccionCL();
        private List<Cuenta> _lstCuentas { get; set; } = new List<Cuenta>();
        private List<FechaTransaccion> _lstFechas { get; set; } = new List<FechaTransaccion>();
        private Cuenta CuentaActual { get; set; }
        public FrameMaestroCuenta()
        {
            InitializeComponent();
            CargarDatos();
            CargarDatosAListas();
            //treeCuentas.ExpandAll(); 
        }
        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, _lstCuentas);
                return true;
            }
            else { return false; }
        }

        #region Carga de datos
        private void CargarDatos()
        {
            _lstCuentas.Clear();
            //_lstCuentas = await Task.Run(() => _cuentaCL.GetAll(GlobalConfig.Compañia            
            //_lstCuentas = await Task.Run(() => _cuentaCL.GetAll(GlobalConfig.Company));
            _lstCuentas = _cuentaCL.GetAll(GlobalConfig.Company);
            treeCuentas.Nodes.AddRange(TreeViewCuentas.CrearTreeView(_lstCuentas));
            // CargarDatosAListas();
        }
        private void CargarDatosAListas()
        {
            AFechaFinal.SelectedIndexChanged -= this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged -= this.BFechaFinalSelectedIndexChanged;

            _lstFechas = _fechaTransaccionCL.GetAllActive(GlobalConfig.Company, GlobalConfig.Usuario);
            ///Se añaden interfaces / copias (las interfaces no son copias) 
            ///la idea con eso es que cada item y de cada combo box tenga diferente hash
            var lstBfchFnl = new List<FechaTransaccion> { (from c1 in _lstFechas select c1).OrderByDescending(x => x.Fecha).LastOrDefault() };
            AFechaInicio.DataSource = lstBfchFnl;
            AFechaFinal.DataSource = (from c1 in _lstFechas select c1).ToList();
            AFechaFinal.SelectedIndex = -1;

            BFechaInicio.DataSource = (from c1 in _lstFechas select c1).ToList();
            BFechaFinal.DataSource = (from c1 in _lstFechas where c1.Fecha >= ((FechaTransaccion)BFechaInicio.SelectedItem).Fecha select c1).ToList();
            BFechaFinal.SelectedIndex = -1;

            AFechaFinal.SelectedIndexChanged += this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged += this.BFechaFinalSelectedIndexChanged;

        }
        private void CargarDatosAlPanelDeInformacion()
        {
            try
            {
                this.txtNombreInfo.Text = CuentaActual.Nombre;
                this.txtTipoInfo.Text = CuentaActual.TipoCuenta.TipoCuenta.ToString().Replace('_', ' ');
                this.txtIndicadorInfo.Text = CuentaActual.Indicador.ToString().Replace('_', ' ');
                this.txtBoxDetalle.Text = CuentaActual.Detalle;
                this.infoPanel.Tag = CuentaActual;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void CargarDatosPanelA()
        {

            if (AFechaFinal.SelectedIndex == -1)
            {
                return;
            }
            #region Obtenemos las Fechas
            DateTime fch1 = ((FechaTransaccion)AFechaInicio.SelectedItem).Fecha;
            DateTime fch2 = ((FechaTransaccion)AFechaFinal.SelectedItem).Fecha;
            DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            ///Ponemos el ultimo dia del mes en la fecha final
            par2 = (par2.AddMonths(1)).AddDays(-1);
            #endregion
            ///vamos a hacer un metodo que solo mande la cuenta con sus hijas y los datos de la cuenta que devulva 
            ///van a ser los utilizados

            //Solo vamos a llenar las cuentas que se seleccione, de esta manera evitamos traer
            //todos los datos t mejoramos rendimiento

            // var lstcntshjs = TreeViewCuentas.GetCuentasHIjas(CuentaActual, _lstCuentas);

            _cuentaCL.LLenarConSaldos(par1, par2, _lstCuentas, GlobalConfig.Company);
            //TreeCuentasAfterSelect(null, null);
            CargarGridA();
        }
        private void CargarDatosPanelB()
        {
            if (BFechaFinal.SelectedIndex == -1)
            {
                return;
            }
            #region Obtenemos las Fechas
            DateTime fch1 = ((FechaTransaccion)BFechaInicio.SelectedItem).Fecha;
            DateTime fch2 = ((FechaTransaccion)BFechaFinal.SelectedItem).Fecha;
            DateTime par1 = new DateTime(fch1.Year, fch1.Month, 1);
            DateTime par2 = new DateTime(fch2.Year, fch2.Month, 1);
            ///Ponemos el ultimo dia del mes en la fecha final
            par2 = (par2.AddMonths(1)).AddDays(-1);
            #endregion
            // var lstcntshjs = TreeViewCuentas.GetCuentasHIjas(CuentaActual, _lstCuentas);
            _cuentaCL.LLenarConSaldos(par1, par2, _lstCuentas, GlobalConfig.Company);

            CargarGridB();

        }
        #endregion

        #region CargarDatosAlGrid
        private void CargarGridA()
        {
            if (CuentaActual is null)
            {
                return;
            }
            gridDatosA.Rows.Clear();
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(gridDatosA);
            row.Cells[0].Value = CuentaActual.SaldoAnteriorColones;
            row.Cells[1].Value = CuentaActual.DebitosColones;
            row.Cells[2].Value = CuentaActual.CreditosColones;
            row.Cells[3].Value = CuentaActual.SaldoActualColones;
            gridDatosA.Rows.Add(row);
        }
        private void CargarGridB()
        {
            if (CuentaActual is null)
            {
                return;
            }
            gridDatosB.Rows.Clear();
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(gridDatosB);

            row.Cells[0].Value = CuentaActual.DebitosColones;
            row.Cells[1].Value = CuentaActual.CreditosColones;
            row.Cells[2].Value = CuentaActual.SaldoMensualColones;

            gridDatosB.Rows.Add(row);
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Evento que ocurre despues de seleccionar una cuenta 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.txtNombreInfo.ReadOnly = true;
            this.txtBoxDetalle.ReadOnly = true;
            this.btnGuardarNuevoNombre.Enabled = false;
            this.btnGuardarNuevoNombre.Visible = false;
            CuentaActual = (Cuenta)e.Node.Tag;
            CargarDatosAlPanelDeInformacion();

            if (tabControlGeneral.SelectedIndex == 0)
            {
                CargarGridA();
            }
            else
            {
                CargarGridB();
            }
        }
        /// <summary>
        /// Actualiza el nombre a de la cuenta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarNuevoNombre(object sender, EventArgs e)
        {
            try
            {


                Cuenta cEdita = ((Cuenta)treeCuentas.SelectedNode.Tag);


                cEdita.Detalle = this.txtBoxDetalle.Text;

                if (_cuentaCL.Update(ref cEdita, GlobalConfig.Usuario, txtNombreInfo.Text, GlobalConfig.Company, txtBoxDetalle.Text, out String mensaje))
                {
                    MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    treeCuentas.SelectedNode.Text = cEdita.Nombre;

                    this.btnGuardarNuevoNombre.Enabled = false;
                    this.btnGuardarNuevoNombre.Visible = false;

                    this.txtNombreInfo.ReadOnly = true;
                    this.txtBoxDetalle.ReadOnly = true;

                }
                else
                {
                    MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LstMesInicioSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 1)
            {

                BFechaFinal.DataSource = (from n in _lstFechas where n.Fecha >= ((FechaTransaccion)BFechaInicio.SelectedItem).Fecha select n).ToList<FechaTransaccion>();
            }
        }
        private void TabControlGeneralSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 0)
            {
                CargarDatosPanelA();
                //BFechaInicio.SelectedItem = _lstFechas.LastOrDefault();
            }
            else
            {
                CargarDatosPanelB();
            }
        }
        private void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {

            CargarDatosPanelA();

        }
        private void BFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {

            CargarDatosPanelB();

        }
        private void Eliminar_Click(object sender, EventArgs e)
        {
            if (CuentaActual is null)
            {
                MessageBox.Show("Seleccione una cuenta", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (MessageBox.Show("Esta acción no se puede deshacer ¿desea continuar de todos modos?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                if (_cuentaCL.Deleted(CuentaActual, GlobalConfig.Usuario, out String mensaje))
                {
                    MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _lstCuentas.Remove(CuentaActual);
                    var padre = treeCuentas.SelectedNode.Parent;
                    treeCuentas.Nodes.Remove(treeCuentas.SelectedNode);
                    treeCuentas.SelectedNode = padre;
                }
                else
                {
                    MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }

        }

        #endregion

        #region Llamada a otras ventanas
        /// <summary>
        /// Abre la ventana para listar las cuentas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listar(object sender, EventArgs e)
        {
            ReporteCuenta reporte = new ReporteCuenta(GlobalConfig.Company, GlobalConfig.Usuario);
            reporte.MdiParent = this.MdiParent;
            reporte.Show();

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
                    nv.lstCuentas = _lstCuentas;
                    nv.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        /// <summary>
        /// Evento que ocurre cuando se presiona el boton de editar una cuenta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditarCuenta(object sender, EventArgs e)
        {
            try
            {
                if (CuentaActual != null && CuentaActual.Editable)
                {
                    // var cuenta = (Cuenta)infoPanel.Tag;
                    this.txtNombreInfo.ReadOnly = false;
                    this.txtNombreInfo.Focus();
                    this.txtBoxDetalle.ReadOnly = false;
                    VisualizarOpcionesDeEdicion();
                }
                else
                {
                    MessageBox.Show("Esta cuenta no puede ser editada", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Metodos poco importante
        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ExpandirArbol(object sender, EventArgs e)
        {
            treeCuentas.ExpandAll();
        }
        private void ColapsarArbol(object sender, EventArgs e)
        {
            treeCuentas.CollapseAll();
        }
        /// <summary>
        /// Metodo para establecer los valores de los controles del panel editar cuenta
        /// </summary>
        /// <param name="tag"></param>
        private void VisualizarOpcionesDeEdicion()
        {

            btnGuardarNuevoNombre.Enabled = true;
            btnGuardarNuevoNombre.Visible = true;
        }
        /// <summary>
        /// Si el usuario presiona enter el sistema lo convierte en tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsuarioKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        private void btnMovimientosCuenta_Click(object sender, EventArgs e)
        {
            ReporteMovimientosCuenta frame = new ReporteMovimientosCuenta();
            if (frame.TransferirCuenta(CuentaActual))
            {
                frame.MdiParent = this.MdiParent;
                frame.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una cuenta valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        #endregion

    }
}
