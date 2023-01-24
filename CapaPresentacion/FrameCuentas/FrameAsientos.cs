using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Core;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaEntidad.Utils;
using CapaLogica;
using CapaPresentacion.Reportes;
using CapaPresentacion.Utils;


namespace CapaPresentacion.FrameCuentas 
{
    public partial class FrameAsientos : Form, ICallingForm, INeedValidatedForClose
    {
        private JournalEntry _journalEntry;
        private JournalEntryLine _journalEntryLineOnEdit = new JournalEntryLine();
        private readonly IFinancialService _financialSercie;

        
        private int PreventMesesAbiertosIndex = 0;
        private int ProventAsientoIndex = 0;
        private Cuenta AccountInTxtBoxNombreCuenta
        {
            //Test
            get
            {
                if (txtBoxNombreCuenta?.Tag is Cuenta myCuenta) return myCuenta;
                return null;
            }
        }

        private PostingPeriod PostingPeriodSelected => (PostingPeriod) lstMesesAbiertos.SelectedItem;

        public FrameAsientos()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialSercie = new FinancialService(unit);
        }

        private void FrameAsientos_Load(object sender, EventArgs e)
        {
            ConfigExchangeController(GlobalConfig.Company.TipoMoneda);
            LoadAccountingPeriodList();
        }

        private void LoadAccountingPeriodList()
        {
            try
            {
                lstMesesAbiertos.DataSource =  _financialSercie.GetPostingPeriods(GlobalConfig.Company.Codigo).OrderByDescending(p=>p.Date).ToList(); 
                lstTipoCambio.SelectedIndex = 0;
                lstTipoCambio.SelectedIndex = 0;
            
                if (lstMesesAbiertos.Items.Count == 0)
                {
                    btnAgregarTransa.Enabled = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void ConfigExchangeController(TipoMonedaCompañia moneyType)
        {
            lstTipoCambio.DataSource = Enum.GetValues(typeof(Currency));

            switch (moneyType)
            {
                case TipoMonedaCompañia.Dolares_y_Colones:
                    ColumnTipoCambio.Visible = true;
                    ColumnMontoDolares.Visible = true;
                    lstTipoCambio.SelectedItem = Currency.colones;
                    lstTipoCambio.Enabled = true;
                    break;
                case TipoMonedaCompañia.Solo_Colones:
                    ColumnTipoCambio.Visible = false;
                    ColumnMontoDolares.Visible = false;
                    lstTipoCambio.SelectedItem = Currency.colones;
                    lstTipoCambio.Enabled = false;
                    break;
                case TipoMonedaCompañia.Solo_Dolares:
                    ColumnTipoCambio.Visible = true;
                    ColumnMontoDolares.Visible = true;
                    lstTipoCambio.SelectedItem = Currency.dolares;
                    lstTipoCambio.Enabled = false;
                    break;
                default:
                    break;
            }
            //if (GlobalConfig.Company.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            //{
            //    ColumnTipoCambio.Visible = false;
            //    ColumnMontoDolares.Visible = false;
            //}
        }

        private IEnumerable<JournalEntry> ConfigAsientoBorrador(IEnumerable<JournalEntry> asientos)
        {
            var newEntryNum = _financialSercie.CreateJournalEntryConsecutive(PostingPeriodSelected.Id);

            var newJEnt = new JournalEntry()
            {
                Number = newEntryNum,
                PostingPeriodId = PostingPeriodSelected.Id,
                JournalEntryStatus = JournalEntryStatus.Progress,
                UpdatedBy = GlobalConfig.Usuario.Id,
                CreatedBy = GlobalConfig.Usuario.Id,
                Active = true
            };

            List<JournalEntry> jEntries = asientos.ToList();
            jEntries.Add(newJEnt);

            return jEntries.OrderByDescending(x => x.Number).ToArray();
        }

        private void LstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EqualDebAndCredONJournalEntry())
            {
                var lst = _financialSercie.GetJournalEntries(PostingPeriodSelected.Id);
                lstNumeroAsientos.DataSource = ConfigAsientoBorrador(lst);
                this.PreventMesesAbiertosIndex = lstMesesAbiertos.SelectedIndex;
            }
            else
            {
                this.lstMesesAbiertos.SelectedIndexChanged -= new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
                 lstMesesAbiertos.SelectedIndex = this.PreventMesesAbiertosIndex;
                this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);

            }
        }


        #region CRUD JournalEntry
        private void btnNuevoAsiento_Click(object sender, EventArgs e)
        {
            try
            {
                lstNumeroAsientos.SelectedIndex = 0;
                
                if (_journalEntry.Id == 0)
                {
                    _financialSercie.CreateJournalEntry(_journalEntry);
                }
                LstMesesAbiertos_SelectedIndexChanged(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private JournalEntry ValidateBookEntryForInsert(JournalEntry asiento)
        {
            if (asiento.Id == 0)
            {
                 _financialSercie.CreateJournalEntry(asiento);
            }
            return asiento;
        }

        #endregion

        #region JournalEntryLine

        private void DeleteJournalEntryLine_Event_Click(object sender, EventArgs e)
        {
            var selectedRows = this.GridDatos.SelectedRows;

            if (selectedRows.Count > 0)
            {
                if (MessageBox.Show($"Se van a eliminar {selectedRows.Count} elementos ¿Desea continuar?",
                    TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteTransactions(selectedRows);
                    ValidateEqualDebAndCred();
                    UpdateView();
                    RemoveAccountOnEdit();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una entrada de asiento", TextoGeneral.NombreApp, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void ValidateEqualDebAndCred()
        {
            if (_journalEntry.Cuadrado)
            {
                _journalEntry.JournalEntryStatus =
                    JournalEntryStatus.Approved; 
                _financialSercie.UpdateJournalEntry(_journalEntry);
            }
            else
            {
                _journalEntry.JournalEntryStatus =
                    JournalEntryStatus.Progress; 
                _financialSercie.UpdateJournalEntry(_journalEntry);
            }
        }

        private void DeleteTransactions(DataGridViewSelectedRowCollection selectedRows)
        {
            try
            {
                for (int i = 0; i < selectedRows.Count; i++)
                {
                    var jEnL = (JournalEntryLine) selectedRows[i].Tag;
                    _financialSercie.DeleteJournalEntryLine(jEnL);
                    _journalEntry.JournalEntryLines.Remove(jEnL);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private JournalEntryLine CreateJournalEntryLineModel()
        {
            var jELine = new JournalEntryLine()
            {
                AccountId = AccountInTxtBoxNombreCuenta.Id,
                AccountPath = AccountInTxtBoxNombreCuenta.PathDirection,
                AccountName = AccountInTxtBoxNombreCuenta.Nombre,
                Reference = txtBoxReferencia.Text,
                Memo = txtBoxDetalle.Text,
                Date = DateTime.Parse(txtBoxFechaFactura.Text),
                JournalEntryId = _journalEntry.Id,
                CreatedBy = GlobalConfig.Usuario.Id,
                UpdatedBy = GlobalConfig.Usuario.Id,
            };

            if (lstTipoCambio.SelectedItem is Currency.colones)
            {
                jELine.Currency = Currency.colones;
                jELine.RateAmount = 1.00m;
                jELine.Amount = Convert.ToDecimal(txtMontoTotalTransaccion.Text);
            }
            else if (lstTipoCambio.SelectedItem is Currency.dolares)
            {
                jELine.Currency = Currency.dolares;
                jELine.RateAmount = Convert.ToDecimal(txtTipoCambio.Text);
                jELine.ForeignAmount = Convert.ToDecimal(txtMontoTotalTransaccion.Text);

                var foreignAmount = jELine.ForeignAmount * jELine.RateAmount;
                jELine.Amount = Math.Truncate(100 * foreignAmount) / 100;
            }

            jELine.DebOrCred = (rDebitos.Checked) ? DebOrCred.Debito : DebOrCred.Credito;

            return jELine;
        }

        private void BtnAgregarTransaccion(object sender, EventArgs e)
        {
            try
            {
                if (IsAccountValid())
                {
                    if (ValidateChildren())
                    {
                        ValidateBookEntryForInsert(_journalEntry);
                        var newJEntry = CreateJournalEntryLineModel();
                        _financialSercie.CreateJournalEntryLine(newJEntry);
                        _journalEntry.JournalEntryLines.Add(newJEntry);

                        UpdateJournalEntryState();

                        _journalEntryLineOnEdit = new JournalEntryLine();
                        this.LimpiarPanelDatosAsiento();
                        txtBoxReferencia.Focus();
                        UpdateView();
                    }

                }
                else
                {
                    MessageBox.Show("Seleccione una cuenta valida", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.UpdateView();
            }

        }

        private void UpdateJournalEntryState()
        {
            if (_journalEntry.Cuadrado)
            {
                _journalEntry.JournalEntryStatus = JournalEntryStatus.Approved;
                _financialSercie.UpdateJournalEntry(_journalEntry);
            }
            else
            {
                _journalEntry.JournalEntryStatus = JournalEntryStatus.Progress;
                _financialSercie.UpdateJournalEntry(_journalEntry);
            }
        }

        private void LstNumeroAsientos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EqualDebAndCredONJournalEntry())
            {
                _journalEntry = (JournalEntry) lstNumeroAsientos.SelectedItem;
                _journalEntry.JournalEntryLines = _financialSercie.GetJournalEntryLineByJournalEntryId(_journalEntry.Id).ToList();

                //_journalEntry.Transaccions = _transaccionCL.GetCompleto(_journalEntry);
                ValidatePostingPeriodStatus(); 
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

        private void ValidatePostingPeriodStatus()
        {
            var postigPeriod = lstMesesAbiertos.SelectedItem as PostingPeriod;
            if (postigPeriod.Closed)
            {
                this.BtnEliminarLinea.Enabled = false;
                this.btnEditarLinea.Enabled = false;
                this.btnNuevoAsiento.Enabled = false;
                this.btnEliminar.Enabled = false;
                this.btnLimpiar.Enabled = false;
                this.layoutSaveTransaction.Enabled = false;
                this.labelPeriodoCerrado.Visible = true; 
            }
            else
            {
                this.BtnEliminarLinea.Enabled = true;
                this.btnEditarLinea.Enabled = true;
                this.btnNuevoAsiento.Enabled = true;
                this.btnEliminar.Enabled = true;
                this.btnLimpiar.Enabled = true;
                this.layoutSaveTransaction.Enabled = true;
                this.labelPeriodoCerrado.Visible = false;
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_journalEntry.Id == 0)
                {
                    MessageBox.Show("No se puede eliminar este asiento porque aun no ha sido registrado",
                        TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MessageBox.Show("Se eliminara este asiento desea continuar", TextoGeneral.NombreApp,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _financialSercie.DeleteJournalEntry(_journalEntry);
                    //_asientoCL.Delete(_journalEntry, GlobalConfig.Usuario);
                    MessageBox.Show("Asiento eliminado correctamente", TextoGeneral.NombreApp, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    _journalEntry = null;
                    FrameAsientos_Load(null,null);
                    SetDiferenciaLabel();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
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
                var dummy = (JournalEntryLine)this.GridDatos.SelectedRows[0].Tag;
                CargarDatosPanelTransaction(dummy);
                //btnAgregarTransa.Text = "Actualizar";
                btnAgregarTransa.Visible = false; 
                btnUpdateJELine.Visible = true; 
            }


        }
        private void btnUpdateJELine_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                var jenLine = CreateJournalEntryLineModel();
                jenLine.Id = _journalEntryLineOnEdit.Id;
                jenLine.JournalEntryId = _journalEntryLineOnEdit.JournalEntryId;
                jenLine.UpdatedBy = GlobalConfig.Usuario.Id;

                try
                {
                    _financialSercie.UpdateJournalEntryLine(jenLine);

                    var index = _journalEntry.JournalEntryLines.IndexOf(_journalEntryLineOnEdit);
                    _journalEntry.JournalEntryLines.Remove(_journalEntryLineOnEdit);
                    _journalEntry.JournalEntryLines.Insert(index, jenLine);

                    UpdateJournalEntryState();
                    this.LimpiarPanelDatosAsiento();
                    txtBoxReferencia.Focus();
                    btnUpdateJELine.Visible = false; 
                    btnAgregarTransa.Visible = true; 
                    UpdateView();

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        #endregion

        #region Dashoboard Events
        private void LimpiarPanelDatosAsiento()
        {
            this.txtMontoTotalTransaccion.Clear();
            this.btnAgregarTransa.Text = "Agregar";
        }


        private void UpdateView()
        {
            GridDatos.Rows.Clear();

            decimal debitos = 0;
            decimal creditos = 0;

            foreach (var tr in _journalEntry.JournalEntryLines)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = tr;
                row.Cells[0].Value = tr.AccountName;
                row.Cells[1].Value = tr.Reference;
                row.Cells[2].Value = tr.Memo;
                row.Cells[3].Value = tr.Date;

                if (tr.DebOrCred == DebOrCred.Debito)
                {
                    row.Cells[4].Value = tr.Amount;
                    debitos += Convert.ToDecimal(tr.Amount);
                }
                else if (tr.DebOrCred == DebOrCred.Credito)
                {
                    row.Cells[5].Value = tr.Amount;
                    creditos += Convert.ToDecimal(tr.Amount);
                }
                row.Cells[6].Value = tr.Currency.ToString();
                row.Cells[7].Value = tr.RateAmount;
                if (tr.Currency == Currency.dolares)
                {
                    row.Cells[8].Value = tr.ForeignAmount;
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
        private void SetColorBalance()
        {

            this.txtTotalDebitos.Text = string.Format("{0:₡###,###,###,##0.00}", _journalEntry.DebitosColones);
            this.txtTotalCreditos.Text = string.Format("{0:₡###,###,###,##0.00}", _journalEntry.CreditosColones);

            if (_journalEntry.Cuadrado && (_journalEntry.JournalEntryLines.Count != 0))
            {
                txtTotalCreditos.ForeColor = Color.Green;
                txtTotalDebitos.ForeColor = Color.Green;

                this.labelDiferencia.Visible = false;
                this.txtDiferenciaSaldo.Visible = false;
                this.txtDiferenciaSaldo.Text = string.Format("{0:₡###,###,###,##0.00}", 0);
                this.btnSwitchPeriod.Enabled = true; 
            }
            else
            {
                txtTotalCreditos.ForeColor = Color.Black;
                txtTotalDebitos.ForeColor = Color.Black;
                this.btnSwitchPeriod.Enabled = false;
            }
        }
        private void SetDiferenciaLabel()
        {
            var diferencia = _journalEntry?.DebitosColones - _journalEntry?.CreditosColones;
            if (diferencia != null && diferencia != 0)
            { 
                this.labelDiferencia.Visible = true;
                this.txtDiferenciaSaldo.Visible = true;
                this.txtDiferenciaSaldo.Text = string.Format("{0:₡###,###,###,##0.00}", diferencia);
            }
            else
            {
                this.labelDiferencia.Visible = false;
                this.txtDiferenciaSaldo.Visible = false;
            }
        }
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (EqualDebAndCredONJournalEntry())
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
                if (_journalEntry.Id != 0)
                {
                    btnNuevoAsiento_Click(null, null);
                }
            }
        }

        private void RemoveAccountOnEdit()
        {
            this.txtBoxReferencia.Clear();
            this.txtBoxDetalle.Clear();
            this.lstTipoCambio.SelectedIndex = 0;
            this.txtMontoTotalTransaccion.Clear();
            this.labelRutaNuevaCuenta.Text = "Ruta:";
            txtBoxNombreCuenta.Text = "";
            txtBoxNombreCuenta.Tag = null;
            btnAgregarTransa.Text = "Agregar";
        }

        private void CargarDatosPanelTransaction(JournalEntryLine dummy)
        {

            _journalEntryLineOnEdit = dummy;

            var account = _financialSercie.FindAccount(dummy.AccountId);

            var accountDTO = new Cuenta()
            {
                Id = account.Id,
                Nombre = account.Name,
                PathDirection = account.PathDirection,
                Indicador =  (IndicadorCuenta)(int)account.AccountType
            };

            TransferirCuenta(accountDTO);

            if (dummy.DebOrCred == DebOrCred.Debito)
            { rDebitos.PerformClick(); }
            else { rCreditos.PerformClick(); }

            txtBoxReferencia.Text = dummy.Reference;
            txtBoxDetalle.Text = dummy.Memo;
            txtBoxFechaFactura.Text = dummy.Date.ToShortDateString();

            if (dummy.Currency == Currency.dolares)
            {
                txtMontoTotalTransaccion.Text = (dummy.Amount / dummy.RateAmount).ToString();
                lstTipoCambio.SelectedIndex = 1;
                txtTipoCambio.Text = dummy.RateAmount.ToString();
            }
            else
            {
                txtMontoTotalTransaccion.Text = dummy.Amount.ToString();
                lstTipoCambio.SelectedIndex = 0;
            }
        }
        private void BtnReporte_Click(object sender, EventArgs e)
        {
            try
            {
                ReporteAsientos n = new ReporteAsientos();
                n.MdiParent = this.MdiParent;
                n.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
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
        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                this.txtBoxNombreCuenta.Tag = cuenta;
                this.txtBoxNombreCuenta.Text = cuenta.Nombre;

                var accountPath = txtPathCuenta.Text = $"Ruta: {cuenta.PathDirection}";

                accountPath = (accountPath.Length < 40)
                    ? accountPath
                    : string.Concat(accountPath.Substring(0, 40), "...");

                this.labelRutaNuevaCuenta.Text = accountPath; 

                ToolTip toolTip1 = new ToolTip();
                toolTip1.AutoPopDelay = 5000;
                toolTip1.InitialDelay = 1000;
                toolTip1.ReshowDelay = 500;
                toolTip1.ShowAlways = true;

                toolTip1.SetToolTip(this.labelRutaNuevaCuenta, accountPath);

                this.rDebitos.Focus();
                return true;
            }
            else
            {
                return false;
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
        private void FrameAsientos_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !EqualDebAndCredONJournalEntry();
        }
        private bool EqualDebAndCredONJournalEntry()
        {
            if (_journalEntry != null && _journalEntry.Id != 0 && !_journalEntry.Cuadrado)
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
        private void GridDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (GridDatos.SelectedRows.Count > 0)
            {

                txtPathCuenta.Text = $"Ruta: {((JournalEntryLine)this.GridDatos.SelectedRows[0].Tag).AccountPath}";
            }
            else
            {
                txtPathCuenta.Text = "";
            }
        }

        #endregion


        #region Validations
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

        private bool IsAccountValid()
        {
            return (txtBoxNombreCuenta.Tag is Cuenta);
        }
        #endregion


        public bool IsAvalibleToClose()
        {
            return EqualDebAndCredONJournalEntry(); 
        }

        private void BtnRefreshGrid(object sender, EventArgs e)
        {
            _journalEntry = (JournalEntry)lstNumeroAsientos.SelectedItem;
            if(_journalEntry != null)
                _journalEntry.JournalEntryLines = _financialSercie.GetJournalEntryLineByJournalEntryId(_journalEntry.Id).ToList();
            UpdateView();
        }

        private void btnSwitchPeriod_Click(object sender, EventArgs e)
        {
            var frame = new SwitchAccountEntryPeriod(_journalEntry, PostingPeriodSelected);
            frame.FinishProcess += ShowSelectedPeriod; 
            frame.ShowDialog();
        }

        private void ShowSelectedPeriod(JournalEntry journalEntry, PostingPeriod postingPeriod)
        {
            lstMesesAbiertos.SelectedIndex = lstMesesAbiertos.FindStringExact(postingPeriod.ToString());
            lstNumeroAsientos.SelectedIndex = lstNumeroAsientos.FindStringExact(journalEntry.Number.ToString());
        }
    }
}
