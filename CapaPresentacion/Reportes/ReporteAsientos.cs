using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Reportes;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteAsientos : Form
    {
        private List<Asiento> _listaDeAsientos;
        public List<FechaTransaccion> fechaTransaccions { get; set; } = new List<FechaTransaccion>(); 
        public void Commit(){ lstMesesAbiertos.DataSource = fechaTransaccions; } 
        private List<Asiento> ListaDeAsientos
        {
            get { return _listaDeAsientos; }
            set { _listaDeAsientos = value; }
        }
        private AsientoCL _asientoCL = new AsientoCL();
        
        public ReporteAsientos()
        {
            InitializeComponent();

            if (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            {
                money_chance.Visible = false;
                balance_usd.Visible = false;
                money_type.Visible = false; 
            }

        }

        //private async Task CargarDatos()
        //{
        //    lstMesesAbiertos.DataSource = await Task.Run(() => _fechaTransaccion.GetAllActive(GlobalConfig.Compañia, GlobalConfig.Usuario));
        //    //lstMesesAbiertos.DataSource = _fechaTransaccion.GetAllActive(GlobalConfig.Compañia, GlobalConfig.Usuario);
        //}
        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            GridDatos.DataSource = _asientoCL.ReporteAsientos(GlobalConfig.Compañia, (FechaTransaccion)lstMesesAbiertos.SelectedItem, false);
            
        }

        private void CargarTabla()
        {

            GridDatos.Rows.Clear();
            foreach (var c in ListaDeAsientos)
            {
                
                foreach (var item in c.Transaccions)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(GridDatos);

                    row.Cells[0].Value = c.FechaAsiento;
                    row.Cells[1].Value = c.NumeroAsiento;
                    row.Cells[2].Value = item.CuentaDeAsiento;
                    row.Cells[3].Value = item.Referencia;
                    row.Cells[4].Value = item.Detalle;
                    row.Cells[5].Value = item.FechaFactura;
                    row.Cells[6].Value = (item.ComportamientoCuenta == Comportamiento.Debito) ? item.Monto : 0.00m;
                    row.Cells[7].Value = (item.ComportamientoCuenta == Comportamiento.Credito) ? item.Monto : 0.00m;
                    row.Cells[8].Value = item.TipoCambio;
                    row.Cells[9].Value = item.MontoTipoCambio;
                    row.Cells[10].Value = (item.TipoCambio == CapaEntidad.Enumeradores.TipoCambio.Dolares) ? item.Monto / item.MontoTipoCambio : 0.00m;

                    GridDatos.Rows.Add(row);
                }

            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {

            try
            {
                
                //if (ListaDeAsientos == null)
                //{
                //    throw new Exception("La lista se encuentra vacia!");
                //}
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE DE ASIENTOS {GlobalConfig.Compañia.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteAsiento.GenerarReporte(ListaDeAsientos, GlobalConfig.Compañia, GlobalConfig.Usuario, GlobalConfig.Compañia.TipoMoneda, sfd.FileName, ((DataTable)GridDatos.DataSource));
                        Convert(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                lstMesesAbiertos.Enabled = false;
                GridDatos.DataSource = GridDatos.DataSource = _asientoCL.ReporteAsientos(GlobalConfig.Compañia, (FechaTransaccion)lstMesesAbiertos.SelectedItem, true);
            }
            else
            {
                lstMesesAbiertos.Enabled = true;
                GridDatos.DataSource = _asientoCL.ReporteAsientos(GlobalConfig.Compañia, (FechaTransaccion)lstMesesAbiertos.SelectedItem, false);
            }
            
        }
        private void Convert() {

            foreach (DataRow item in ((DataTable)GridDatos.DataSource).Rows)
            {
                Object[] vs = item.ItemArray;
                
            }
        }
    }

}
