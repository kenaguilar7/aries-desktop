using AriesContador.Core.Models.Companies;
using AriesContador.Core.Services;
using Aries.Reporting.Entidades.Cuentas;
using Aries.Reporting.Entidades.FechaTransacciones;
using Aries.Reporting.Entidades.Usuarios;
using Aries.Reporting.Mappers;
using Aries.Reporting.Reportes;
using Aries.Reporting.Textos;
using Aries.Desktop.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.Reportes
{
    public partial class ReporteCuenta : Form
    {

        private Company _compania = new Company();
        private Usuario _usuario = new Usuario();
        private List<Cuenta> _lstCuentas = new List<Cuenta>();
        private List<Cuenta> _lstCuentasFiltradas = new List<Cuenta>();
        private List<FechaTransaccion> lstFechas = new List<FechaTransaccion>();
        private readonly IFinancialService _financialService;
        int cont = 0;
        private Boolean ConSaldo { set { LlenarTabla(value); } }
        public ReporteCuenta(Company compañia, Usuario usuario, IFinancialService financialService)
        {
            _financialService = financialService;
            InitializeComponent();
            _compania = compañia;
            _usuario = usuario;
            Load += ReporteCuenta_Load;
        }

        private async void ReporteCuenta_Load(object sender, EventArgs e)
        {
            await CargarDatosAsync();
        }

        /// <summary>
        /// Carga los datos!!!
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="compañia"></param>
        /// <param name="usuario"></param>
        private async Task CargarDatosAsync()
        {
            _lstCuentas = await ReportAccountLoader.LoadAsync(_financialService, _compania);
            lstFechas = CuentaMapper.ToFechaTransaccionList(
                await _financialService.GetPostingPeriodsAsync(_compania.Code));
            this.lstMesesAbiertos.DataSource = lstFechas;
            LlenarTabla(false);
        }
        /// <summary>
        /// LLena la tabla del panel, 
        /// </summary>
        private void LlenarTabla(Boolean ConSaldo)
        {

            ///Elimina todos los datos de la tabla
            GridDatos.Rows.Clear();
            GridDatos.Refresh();


            // _lstCuentas = ReporteadorCuenta.Ordernar(_lstCuentas);
            _lstCuentasFiltradas = new List<Cuenta>();
            _lstCuentas.ForEach((Cuenta => _lstCuentasFiltradas.Add(Cuenta.DeepCopy())));


            if (!checkCuentasConSaldo.Checked)
            {
                _lstCuentasFiltradas = ReportAccountLoader.WithoutEmptyBalances(_lstCuentas);
            }
            CrearColumnasParaNombre();
            foreach (var c in _lstCuentasFiltradas)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);

                var name = c.GetNombreParaExcel(_lstCuentas);

                row.Cells[name.Length - 1].Value = name.Last();


                if (ConSaldo)
                {
                    row.Cells[cont ].Value = c.SaldoAnteriorColones;
                    row.Cells[cont + 1].Value = c.DebitosColones;
                    row.Cells[cont + 2].Value = c.CreditosColones;
                    row.Cells[cont + 3].Value = c.SaldoActualColones;

                    GridDatos.Rows.Add(row);
                }
                else
                {
                    GridDatos.Rows.Add(row);
                    c.SaldoAnteriorColones = 0.00m;
                    c.DebitosColones = 0.00m;
                    c.CreditosColones = 0.00m;
                    c.DebitosDolares = 0.00m;
                    c.CreditosDolares = 0.00m;
                }





            }
        }
        /// <summary>
        /// Retorna el indice de la colu
        /// </summary>
        /// <param name="_lstCuentasFiltradas"></param>
        /// <returns>Indice de la ultima comlumna del nombre</returns>
        private void CrearColumnasParaNombre()
        {

            ///Sacamos el numero maximo de la lista y creamos esa cantidad de columnas - 1 
            ///-1, porque ya tenemos una

            ///Esta varibable se va a utilizar para al final del ciclo saber si se deben eliminar columnas o no
            var intrCont = 1;

            _lstCuentasFiltradas.ForEach((Cuenta) =>
            {

                var cnt = Cuenta.GetNombreParaExcel(_lstCuentasFiltradas).Length;

                intrCont = (cnt > intrCont) ? cnt : intrCont;

                if (cnt > cont)
                {
                    DataGridViewTextBoxColumn column1P = new DataGridViewTextBoxColumn
                    {
                        HeaderText = "",
                        Name = $"columnN{cont}",
                        ReadOnly = true
                    };
                    GridDatos.Columns.Insert(cont, column1P);

                    cont = cnt;
                }

            });

            if (intrCont < cont)
            {
                for (int i = 0; i < (cont - intrCont); i++)
                {
                    GridDatos.Columns.Remove(GridDatos.Columns[i]);
                }

            }
            cont = intrCont; 

        }

        private void BtnExcel_Click(object sender, EventArgs e)
        {

            foreach (var item in _lstCuentas)
            {
                var pp = item.GetNombreParaExcel(_lstCuentas);
            }

            try
            {

                if (_lstCuentas == null)
                {
                    throw new Exception("La lista se encuentra vacia!");
                }
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"MAESTRO DE CUENTAS DE {GlobalConfig.Company.ToString()}"  })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ReporteMaestroCuenta.GenerarReporte(_lstCuentasFiltradas, sfd.FileName, GlobalConfig.Company, GlobalConfig.Usuario,  (FechaTransaccion)lstMesesAbiertos.SelectedItem, checkImprimirSaldo.Checked);


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Set_Imprimir_Saldo(object sender, EventArgs e)
        {
            ConSaldo = this.checkImprimirSaldo.Checked;
            lstMesesAbiertos.Enabled = checkImprimirSaldo.Checked;
        }
        /// <summary>
        /// Esta funcion no ayuda a buscar cual es el rango que 
        /// queremos imprimir, buscamos los datos y los cargamos a la tabla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Imprimir_Saldo(object sender, EventArgs e)
        {
            ///Obtenemos el seleccionado
            var mes = (FechaTransaccion)lstMesesAbiertos.SelectedItem;
            // _lstCuentas = ReporteadorCuenta.CuentasActualizadasPorRango(mes, _compania, checkImprimirSaldo.Checked);
            ///Buscamos el mes mams viejo 
            ///

            var mesFinal = lstFechas.OrderBy(x => x.Fecha).ToList()[0];

            await ReportAccountLoader.FillBalancesAsync(_financialService, _lstCuentas, mesFinal.Fecha, mes.Fecha);

            LlenarTabla(ConSaldo: true);
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkCuentasConSaldo_CheckedChanged(object sender, EventArgs e)
        {
            ConSaldo = checkImprimirSaldo.Checked;
        }

        private void lstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            Imprimir_Saldo(null, null);
        }
    }



}

