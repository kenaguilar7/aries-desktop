using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.Reportes;


namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAsientos : Form, ICallingForm
    {

        private Asiento _asiento;
        private Transaccion TransaccionOnEdit = new Transaccion();
        private AsientoCL _asientoCL = new AsientoCL();
        private FechaTransaccionCL _fechaTransaccion = new FechaTransaccionCL();
        private TransaccionCL _transaccionCL = new TransaccionCL();
        private int PreventMesesAbiertosIndex = 0;
        private int ProventAsientoIndex = 0; 
        public FrameAsientos()
        {
            InitializeComponent();
            AgregarEventos();
            CargarDatos();
            //txtValido.TextChanged += new EventHandler(tb_TextChanged);
            txtMontoTotalTransaccion.TextChanged += new EventHandler(tb_TextChanged);
            //Controls.Add(maskedmaskedTextBox1);

        }
        /// <summary>
        /// Carga los enventos que luego serviran para cargar datos a las listas
        /// </summary>
        private void AgregarEventos()
        {
            //this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
            //this.lstNumeroAsientos.SelectedIndexChanged += new System.EventHandler(this.LstNumeroAsientos_SelectedIndexChanged);
            this.txtMontoTotalTransaccion.KeyPress += UsuarioKeyPress;
            this.txtTipoCambio.KeyPress += UsuarioKeyPress;

        }
        /// <summary>
        /// Carga los datos necesarios
        /// </summary>
        /// <param name="compañia"></param>
        /// <param name="user"></param>
        private async Task CargarDatos()
        {

            if (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            {
                ColumnTipoCambio.Visible = false;
                ColumnMontoDolares.Visible = false;
            }

            lstMesesAbiertos.DataSource = await Task.Run(() => _fechaTransaccion.GetAllActive(GlobalConfig.Compañia, GlobalConfig.Usuario));
            lstTipoCambio.SelectedIndex = 0;
            lstTipoCambio.SelectedIndex = 0;

            lstTipoCambio.Enabled = (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Dolares_y_Colones) ? true : DummyTipoCambio();

            Boolean DummyTipoCambio()
            {
                lstTipoCambio.SelectedIndex = (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Solo_Colones) ? 0 : 1;
                return false;
            }

            if (lstMesesAbiertos.Items.Count == 0)
            {
                btnAgregarTransa.Enabled = false;
            }
            //GridDatos.ScrollBars = ScrollBars.Both;

        }
        /// <summary>
        /// Evento que ocurre cuando se agrega una tranaccion al panel de transacciones
        /// 1. Se verifica que la transaccion sea apto //no valores nulos
        /// 2. Se revisa el estado del asiento
        ///     2.1 De ser necesario se guarda el asiento
        ///     2.2 De ser necesario se actualiza la variable <Estado> eso es si el asiento se encuentra cuadrado
        /// 3. Se inserta la transaccion a la base de datos
        /// 4. se actualiza la tabla 
        /// 5. se limpia el panel de la transaccion. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAgregarTransaccion(object sender, EventArgs e)
        {
            try
            {

                ///Verifica todos los campos y lo asigna al objeto pasado por parametro
                ///Como el objeto es de salida, si el metodo retorna true entonces quiere  decir que fue 
                ///llenado correctamente y se puede usar
                ///El ref se podria ignorar pero vamos a dejarlo de momento
                ///para efectos practicos
                if (VerificarYAsignarCampos(ref TransaccionOnEdit))
                {

                    ///Si el id del asiento(variable global) es igual a cero quiere decir que 
                    ///no esta guardado entonces va y se guarda en la base de datos
                    if (_asiento.Id == 0)
                    {
                        ///Preguntar si el indice es cero poder asegurar  que las tramsa de verdad no se hay insertado
                        if ((_asiento = _asientoCL.Insert(_asiento, GlobalConfig.Usuario, out String mensaje)).Id == 0)
                        {
                            MessageBox.Show(mensaje, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    ///Si el asiento 
                    if (_asiento.Transaccions.Exists(x => x.Id == TransaccionOnEdit.Id))
                    {
                        if (!_transaccionCL.Update(TransaccionOnEdit, GlobalConfig.Usuario, out String mensaje))
                        {
                            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        TransaccionOnEdit = _transaccionCL.Insert(TransaccionOnEdit, _asiento.Id, GlobalConfig.Usuario);
                        this._asiento.Transaccions.Add(TransaccionOnEdit);
                    }



                    /**
                     * Si el asiento es igual a proceso quiere decir que:
                     * 1. Esta todavia reciendo transacciones
                     * por lo tanto no hay que actualizarlo
                     * 
                     * 2. si el asiento es desgial a proceso, quiere decir
                     * que si hay que actualizalo para que en la bbdd cambie 
                     * su estado a proceso y asi no se incluya en los saldo
                     * 
                     */
                    if (_asiento.Cuadrado)
                    {
                        _asiento.Estado = EstadoAsiento.Aprobado;//se cambia a proceso dentro del if. 
                        _asientoCL.Update(_asiento, GlobalConfig.Usuario, out string mensaje);
                        ///este ya esta configurado para respuesta bool mensaje 
                        ///talvez se pueda implementar mostrando el mensaje en el banner de la app
                    }
                    else
                    {
                        _asiento.Estado = EstadoAsiento.Proceso;//se cambia a proceso dentro del if. 
                        _asientoCL.Update(_asiento, GlobalConfig.Usuario, out string mensaje);
                    }


                    TransaccionOnEdit = new Transaccion();
                    this.UpdateView();

                    this.LimpiarPanelDatosAsiento();
                    txtBoxReferencia.Focus();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.UpdateView();
            }

        }
        private Boolean VerificarYAsignarCampos(ref Transaccion tr)
        {

            //tr = new Transaccion();

            #region Aqui verificamos la cuenta

            ///Extraemos la cuenta de la lista 
            ///Este casteo se puede hacer mejor
            tr.CuentaDeAsiento = (txtBoxNombreCuenta.Tag != null) ? (Cuenta)txtBoxNombreCuenta.Tag : null;

            if (tr.CuentaDeAsiento == null || tr.CuentaDeAsiento.Indicador != IndicadorCuenta.Cuenta_Auxiliar)
            {
                MessageBox.Show($"Seleccione un cuenta valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            #endregion

            #region Aqui verificamos la referencia que no este vacia

            ///Si la asignacion es nula o la cadena esta en blanco
            if ((tr.Referencia = txtBoxReferencia.Text) == null || String.IsNullOrWhiteSpace(tr.Referencia))
            {
                MessageBox.Show("Referencia no puede ir en blanco", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            #endregion

            #region Aqui verificamos el detalle, que no este vacio


            if ((tr.Detalle = txtBoxDetalle.Text) == null || String.IsNullOrWhiteSpace(tr.Detalle))
            {
                MessageBox.Show("Detalle no puede ir en blanco", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }



            #endregion

            #region Aqui verificamos la fecha de la factura

            if (DateTime.TryParse(txtBoxFechaFactura.Text, out DateTime dateTime))
            {
                if (dateTime.Year < 1000 || dateTime.Year > 9999)
                {
                    MessageBox.Show("Ingrese una fecha valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    tr.FechaFactura = dateTime;

                }
            }
            else
            {
                MessageBox.Show("Ingrese una fecha valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtBoxFechaFactura.Focus();
                return false;
            }

            #endregion

            #region Aqui vamos a verificar el tipo de cambio


            var rsldo = false;
            if (txtTipoCambio.Text.Length == 0 || !(rsldo = decimal.TryParse(txtTipoCambio.Text, out decimal tpCambio)) || tpCambio == 0.00m)
            {
                MessageBox.Show(((!rsldo) ? "Formato tipo cambio incorrecto" : "El tipo de cambio no puede ser cero"), TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (lstTipoCambio.SelectedIndex == 0)
            {
                tr.TipoCambio = TipoCambio.Colones;
                tr.MontoTipoCambio = 1.00m;
                tr.Monto = Convert.ToDecimal(txtMontoTotalTransaccion.Text);

            }
            else
            {
                tr.TipoCambio = TipoCambio.Dolares;
                tr.MontoTipoCambio = Convert.ToDecimal(txtTipoCambio.Text);
                tr.Monto = Convert.ToDecimal(txtMontoTotalTransaccion.Text) * tr.MontoTipoCambio; ///pasamos los dolares a colones

            }


            #endregion

            #region Aqui vamos a verificar el comportamiento de la cuenta
            if (rDebitos.Checked)
            {
                tr.ComportamientoCuenta = Comportamiento.Debito;
            }
            else if (rCreditos.Checked)
            {
                tr.ComportamientoCuenta = Comportamiento.Credito;
            }
            #endregion

            var monto = Convert.ToDecimal(txtMontoTotalTransaccion.Text) * Convert.ToDecimal(txtTipoCambio.Text);

            if (monto > 9999999999999999.99m)
            {
                MessageBox.Show("Longitud de monto muy larga", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Limpia el panel de edicion del transacciones
        /// </summary>
        private void LimpiarPanelDatosAsiento()
        {
            //this.txtBoxReferencia.Clear();
            //this.txtBoxDetalle.Clear();
            //this.fechaFactura.Clear();
            this.txtMontoTotalTransaccion.Clear();
            this.btnAgregarTransa.Text = "Agregar";
        }
        /// <summary>
        /// Evento que abre una nueva ventana con las cuentas para poder seleccionar una y usarla en las transaccion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CuentaLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrameSeleccionCuenta n = new FrameSeleccionCuenta(this);

            n.ShowDialog();
        }
        /// <summary>
        /// Actualiza la vista con la información de la variable local (el asiento)
        /// </summary>
        private void UpdateView()
        {
            GridDatos.Rows.Clear();

            decimal debitos = 0;
            decimal creditos = 0;

            foreach (var tr in _asiento.Transaccions)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = tr;
                row.Cells[0].Value = tr.CuentaDeAsiento;
                row.Cells[1].Value = tr.Referencia;
                row.Cells[2].Value = tr.Detalle;
                row.Cells[3].Value = tr.FechaFactura;

                if (tr.ComportamientoCuenta == Comportamiento.Debito)
                {
                    row.Cells[4].Value = tr.Monto;
                    debitos += Convert.ToDecimal(tr.Monto);
                }
                else if (tr.ComportamientoCuenta == Comportamiento.Credito)
                {
                    row.Cells[5].Value = tr.Monto;
                    creditos += Convert.ToDecimal(tr.Monto);
                }
                row.Cells[6].Value = tr.TipoCambio.ToString();
                row.Cells[7].Value = tr.MontoTipoCambio;
                if (tr.TipoCambio == TipoCambio.Dolares)
                {
                    row.Cells[8].Value = tr.Monto / tr.MontoTipoCambio;
                }
                else
                {
                    row.Cells[8].Value = 00.00;
                }
                GridDatos.Rows.Add(row);

            }
            SetColorBalance();
            SetDiferenciaLabel();
        }
        /// <summary>
        /// Establece los colores de los textos del panelbalance
        /// </summary>
        /// <param name="debitos"></param>
        /// <param name="creditos"></param>
        private void SetColorBalance()
        {

            this.txtTotalDebitos.Text = string.Format("{0:₡###,###,###,##0.00}", _asiento.DebitosColones);
            this.txtTotalCreditos.Text = string.Format("{0:₡###,###,###,##0.00}", _asiento.CreditosColones);

            if (_asiento.Cuadrado && (_asiento.Transaccions.Count != 0))
            {
                txtTotalCreditos.ForeColor = Color.Green;
                txtTotalDebitos.ForeColor = Color.Green;

                this.labelDiferencia.Visible = false;
                this.txtDiferenciaSaldo.Visible = false;
                this.txtDiferenciaSaldo.Text = string.Format("{0:₡###,###,###,##0.00}", 0);

            }
            else
            {
                txtTotalCreditos.ForeColor = Color.Black;
                txtTotalDebitos.ForeColor = Color.Black;
            }
        }

        private void SetDiferenciaLabel()
        {
            var diferencia = _asiento?.DebitosColones - _asiento?.CreditosColones;
            if (diferencia != null && diferencia != 0)
            { 
                this.labelDiferencia.Visible = true;
                this.txtDiferenciaSaldo.Visible = true;
                this.txtDiferenciaSaldo.Text = string.Format("{0:₡###,###,###,##0.00}", diferencia);
            }
        }

        /// <summary>
        /// Evento que ocurre cuando se sale del control de fecha, este valida que la fecha se correcta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        /// <summary>
        /// Evento que ocurre cuando se cambia el indice de los meses abiertos. 
        /// Se carga en la lista de asientos los asientos correspondientes a ese mes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AsientoCuadrado())
            {
                List<Asiento> lst = _asientoCL.GetPorFecha((FechaTransaccion)lstMesesAbiertos.SelectedItem, GlobalConfig.Compañia);
                lstNumeroAsientos.DataSource = lst;
                this.PreventMesesAbiertosIndex = lstMesesAbiertos.SelectedIndex;
            }
            else
            {
                this.lstMesesAbiertos.SelectedIndexChanged -= new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
                 lstMesesAbiertos.SelectedIndex = this.PreventMesesAbiertosIndex;
                this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);

            }

        }
        /// <summary>
        /// Evento que ocurre cuando cambiamos el indice de la lista de asientos
        /// lo que hace es ir y buscar todas las transacciones de ese asiento
        /// y cargarlas en el panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstNumeroAsientos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AsientoCuadrado())
            {
                _asiento = (Asiento) lstNumeroAsientos.SelectedItem;
                _asiento.Transaccions = _transaccionCL.GetCompleto(_asiento);
                UpdateView();
                this.ProventAsientoIndex = lstNumeroAsientos.SelectedIndex;
            }
            else
            {
                this.lstNumeroAsientos.SelectedIndexChanged -= new System.EventHandler(this.LstNumeroAsientos_SelectedIndexChanged);
                lstNumeroAsientos.SelectedIndex = ProventAsientoIndex;
                this.lstNumeroAsientos.SelectedIndexChanged += new System.EventHandler(this.LstNumeroAsientos_SelectedIndexChanged);

            }

        }
        /// <summary>
        /// Se limpia el panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (AsientoCuadrado())
            {
                this.GridDatos.Rows.Clear();
                this.SetColorBalance();
                this.SetDiferenciaLabel();
                this.txtBoxReferencia.Clear();
                this.txtBoxDetalle.Clear();
                this.lstTipoCambio.SelectedIndex = 0;
                this.txtMontoTotalTransaccion.Clear();
                this.labelRutaNuevaCuenta.Text = "Ruta:";
                lstNumeroAsientos.SelectedIndex = 0;
                txtBoxNombreCuenta.Text = "";
                txtBoxNombreCuenta.Tag = null;
                btnAgregarTransa.Text = "Agregar";
                if (_asiento.Id != 0)
                {
                    btnNuevoAsiento_Click(null, null);
                }
            }
        }
        /// <summary>
        /// Elimina el asiento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_asiento.Id == 0)
                {
                    MessageBox.Show("No se puede eliminar este asiento porque aun no ha sido registrado",
                        TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MessageBox.Show("Se eliminara este asiento desea continuar", TextoGeneral.NombreApp,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _asientoCL.Delete(_asiento, GlobalConfig.Usuario);
                    MessageBox.Show("Asiento eliminado correctamente", TextoGeneral.NombreApp, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    _asiento = null;
                    SetDiferenciaLabel();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
        /// <summary>
        /// Evento ue ocurre cuando el usuario presiona doble click en una transaccion de la tabla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        /// <summary>
        /// Carga los datos al panel de edicion
        /// </summary>
        /// <param name="dummy"></param>
        private void CargarDatosPanelTransaction(Transaccion dummy)
        {

            TransaccionOnEdit = dummy;
            TransferirCuenta(dummy.CuentaDeAsiento);
            //establece cual radio button estara checado.
            if (dummy.ComportamientoCuenta == Comportamiento.Debito)
            { rDebitos.PerformClick(); }
            else { rCreditos.PerformClick(); }

            txtBoxReferencia.Text = dummy.Referencia;
            txtBoxDetalle.Text = dummy.Detalle;
            txtBoxFechaFactura.Text = dummy.FechaFactura.ToShortDateString();

            if (dummy.TipoCambio == TipoCambio.Dolares)
            {
                txtMontoTotalTransaccion.Text = (dummy.Monto / dummy.MontoTipoCambio).ToString();
                lstTipoCambio.SelectedIndex = 1;
                txtTipoCambio.Text = dummy.MontoTipoCambio.ToString();
            }
            else
            {
                txtMontoTotalTransaccion.Text = dummy.Monto.ToString();
                lstTipoCambio.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Abre la ventana de reporte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReporte_Click(object sender, EventArgs e)
        {
            try
            {
                ReporteAsientos n = new ReporteAsientos();
                n.MdiParent = this.MdiParent;
                foreach (var item in lstMesesAbiertos.Items)
                {
                    n.fechaTransaccions.Add((FechaTransaccion)item);
                }
                n.Commit();
                n.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Evento que ocurre cuando cambiamos el 
        /// index de la lista tipo de cambio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstTipoCambio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTipoCambio.SelectedIndex == 0)
            {
                txtTipoCambio.Enabled = false;
                txtTipoCambio.Text = "1.00";
            }
            else
            {
                txtTipoCambio.Enabled = true;
                txtTipoCambio.Clear();
            }

        }
        /// <summary>
        /// Evento  para cambiar de control cuando el usuario 
        /// presione enter
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
        private void Cerrar(object sender, EventArgs e)
        {
            this.Close();
        }
        //private void LinkAddCuenta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    FrameSeleccionCuenta n = new FrameSeleccionCuenta(this);

        //    n.ShowDialog();
        //}

        private void btnNuevoAsiento_Click(object sender, EventArgs e)
        {
            try
            {
                ///Nos aseguramos que el asiento que este seleccionado sea el ultimo
                lstNumeroAsientos.SelectedIndex = 0;
                if (_asiento.Id == 0)
                {
                    if ((_asiento = _asientoCL.Insert(_asiento, GlobalConfig.Usuario, out String mensaje)).Id == 0)
                    {
                        MessageBox.Show(mensaje, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                LstMesesAbiertos_SelectedIndexChanged(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtbEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                ///Preguntamos si de verdad desea eliminar los asientos
                var adummy = this.GridDatos.SelectedRows;
                if ((adummy.Count > 0) && MessageBox.Show($"Se van a eliminar {adummy.Count} elementos ¿Desea continuar?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    var dummy = this.GridDatos.SelectedRows;

                    for (int i = 0; i < dummy.Count; i++)
                    {
                        _transaccionCL.Delete(new List<Transaccion> { (Transaccion)dummy[i].Tag }, _asiento.Id, GlobalConfig.Usuario);
                        _asiento.Transaccions.Remove((Transaccion)dummy[i].Tag);
                    }


                    if (_asiento.Cuadrado)
                    {
                        _asiento.Estado = EstadoAsiento.Aprobado;//se cambia a proceso dentro del if. 
                        _asientoCL.Update(_asiento, GlobalConfig.Usuario, out string mensaje);
                    }
                    else
                    {
                        _asiento.Estado = EstadoAsiento.Proceso;//se cambia a proceso dentro del if. 
                        _asientoCL.Update(_asiento, GlobalConfig.Usuario, out string mensaje);
                    }
                    this.UpdateView();
                }
                else if (adummy.Count == 0)
                {
                    MessageBox.Show("Seleccione una transacción", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEditarLinea_Click(object sender, EventArgs e)
        {
            var adummy = this.GridDatos.SelectedRows;

            if (adummy.Count != 1)
            {
                MessageBox.Show("Seleccione una transacción", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var dummy = (Transaccion)this.GridDatos.SelectedRows[0].Tag;
                CargarDatosPanelTransaction(dummy);
                btnAgregarTransa.Text = "Actualizar";
            }


        }
        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                this.txtBoxNombreCuenta.Tag = cuenta;
                this.txtBoxNombreCuenta.Text = cuenta.Nombre;

                var cuentaPath = txtPathCuenta.Text = $"Ruta: {cuenta.PathDirection}";
                this.labelRutaNuevaCuenta.Text = cuentaPath.Substring(0, 40) + ((cuentaPath.Length > 40 )?"...":"");

                ToolTip toolTip1 = new ToolTip();
                toolTip1.AutoPopDelay = 5000;
                toolTip1.InitialDelay = 1000;
                toolTip1.ReshowDelay = 500;
                toolTip1.ShowAlways = true;

                toolTip1.SetToolTip(this.labelRutaNuevaCuenta, cuentaPath);

                this.rDebitos.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }
        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private void tb_TextChanged(object sender, EventArgs e)
        {
            ///si se llega a este punto es porque si era un numero o un . (punto)

            string value = txtMontoTotalTransaccion.Text.Replace(",", "");
            value = (value == ".") ? "0." : value;
            if (decimal.TryParse(value, out decimal ul))
            {
                txtMontoTotalTransaccion.TextChanged -= tb_TextChanged;

                ///Primero todo lo que hace si comienza con cero

                if (ul == 0)
                {
                    if (value.StartsWith("."))
                    {
                        txtMontoTotalTransaccion.Text = string.Format("{0:0.}", ul);
                        txtMontoTotalTransaccion.Text += ".";
                        txtMontoTotalTransaccion.SelectionStart = txtMontoTotalTransaccion.Text.Length;
                    }
                    else if (value.EndsWith("."))
                    {
                        txtMontoTotalTransaccion.Text = string.Format("{0:0.}", ul);
                        txtMontoTotalTransaccion.Text += ".";
                        txtMontoTotalTransaccion.SelectionStart = txtMontoTotalTransaccion.Text.Length;
                    }
                    else if (value.IndexOf('.') != 1 && ul == 0)
                    {
                        txtMontoTotalTransaccion.Text = string.Format("{0:0}", ul);
                        txtMontoTotalTransaccion.SelectionStart = txtMontoTotalTransaccion.Text.Length;
                    }

                }
                else if (!value.Contains("."))
                {
                    txtMontoTotalTransaccion.Text = string.Format("{0:#,#}", ul);
                    txtMontoTotalTransaccion.SelectionStart = txtMontoTotalTransaccion.Text.Length;
                }
                else
                {

                    if (value.IndexOf(".") + 2 < value.Length - 1)
                    {
                        var ss = value.IndexOf('.');
                        txtMontoTotalTransaccion.Text = string.Format("{0:#,#.##}", Convert.ToDecimal(value.Substring(0, value.Length - 1)));
                        //if (value.EndsWith("0"))
                        //{
                        //    textBox1.Text += ".00";
                        //}

                        txtMontoTotalTransaccion.SelectionStart = txtMontoTotalTransaccion.Text.Length;
                    }
                }
                txtMontoTotalTransaccion.TextChanged += tb_TextChanged;
            }
        }
        private void txtTipoCambio_TextChanged(object sender, EventArgs e)
        {

            if (decimal.TryParse(txtTipoCambio.Text, out decimal ul))
            {
                if (txtTipoCambio.Text.Contains("."))
                {
                    var slp = txtTipoCambio.Text.Split(new char[] { '.' });
                    txtTipoCambio.MaxLength = slp[0].Length + 3;

                }
                else
                {
                    txtTipoCambio.MaxLength = 4;
                }

            }
        }
        private void btnSeleccionarCuenta_Click(object sender, EventArgs e)
        {
            FrameSeleccionCuenta n = new FrameSeleccionCuenta(this);

            n.ShowDialog();
        }
        private void FrameAsientos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\u0013'))
            {
                ///CTR + S
                btnSeleccionarCuenta_Click(null, null);

            }
            else if (e.KeyChar == '\u0004')
            {
                ///CTR + D
                rDebitos.Checked = true;
            }
            else if (e.KeyChar == '\u0003')
            {
                ///CTR + C
                rCreditos.Checked = true;
            }
            else if (e.KeyChar == '\u0001')
            {
                this.BtnAgregarTransaccion(null, null);
            }


        }
        private void GridDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (GridDatos.SelectedRows.Count > 0)
            {

                txtPathCuenta.Text = $"Ruta: {((Transaccion)this.GridDatos.SelectedRows[0].Tag).CuentaDeAsiento.PathDirection}";
            }
            else
            {
                txtPathCuenta.Text = "";
            }
        }
        private void FrameAsientos_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !AsientoCuadrado();
        }
        private bool AsientoCuadrado()
        {
            if (_asiento != null && _asiento.Id != 0 && !_asiento.Cuadrado)
            {
                this.WindowState = FormWindowState.Normal;
                MessageBox.Show("Este asiento se encuentra descuadrado, cuadre el asiento antes de salir", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Validations
        private void txtBoxReferencia_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxReferencia.Text))
            {
                SetErrorMessage(txtBoxReferencia, "Referencia no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxReferencia, string.Empty, ref e, false);
            }
        }


        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
        #endregion

        private void txtBoxDetalle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoxDetalle.Text))
            {
                SetErrorMessage(txtBoxDetalle, "Detalle no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxDetalle, string.Empty, ref e, false);
            }
        }

        private void fechaFactura_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.TryParse(txtBoxFechaFactura.Text, out DateTime sr))
            {
                if (sr.Year < 1000 || sr.Year > 9999)
                {
                    SetErrorMessage(txtBoxFechaFactura, "Formato de fecha incorrecto", ref e, true);
                }
                else
                {
                    SetErrorMessage(txtBoxFechaFactura, string.Empty, ref e, false);
                }
            }
            else
            {
                SetErrorMessage(txtBoxFechaFactura, "Formato de fecha incorrecto", ref e, true);
            }
        }

        private void txtTipoCambio_Validating(object sender, CancelEventArgs e)
        {
            var tipoCambioString = txtTipoCambio.Text;
            var canParse = decimal.TryParse(tipoCambioString, out decimal tipoCambio);

            if (canParse)
            {
                ////Buscar para que con el tipo de cambio se defina la cantidad de numeros 
                if (tipoCambio <= 0)
                {
                    SetErrorMessage(txtTipoCambio, "El tipo de cambio no puede ser cero o menor a cero", ref e, true);
                }
                else
                {
                    SetErrorMessage(txtTipoCambio, string.Empty, ref e, false);
                }
            }
            else
            {
                SetErrorMessage(txtTipoCambio, "El tipo de cambio no puede ser cero o menor a cero", ref e, true);
            }
        }

        private void TxtBoxMontoTotal_Validating(object sender, CancelEventArgs e)
        {
            if (txtMontoTotalTransaccion.Text.Length == 0)
            {
                txtMontoTotalTransaccion.TextChanged -= tb_TextChanged;
                txtMontoTotalTransaccion.Text = "0.00";
                txtMontoTotalTransaccion.TextChanged += tb_TextChanged;
            }
            if (decimal.TryParse(txtMontoTotalTransaccion.Text, out decimal dummys))
            {
                SetErrorMessage(txtMontoTotalTransaccion, string.Empty, ref e, false);
            }
            else
            {
                SetErrorMessage(txtMontoTotalTransaccion, "Formato de monto incorrecto", ref e, true);
            }
        }

    }
}
