using AriesContador.Core;
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
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Data;
using AriesContador.Services;
using CapaEntidad.Entidades.JournalEntries;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAdministrarMeses : Form
    {
        private CuentaCL _cuentaCL = new CuentaCL();
        private FechaTransaccionCL fechaCL = new FechaTransaccionCL();
        //private IEnumerable<Cuenta> Cuentas { get; set; }

        private readonly IFinancialService _financialService;
        private readonly IFinancialReportService _financialReportService; 

        public FrameAdministrarMeses()
        {
            InitializeComponent();
            IUnitOfWork unit = new UnitOfWork(GlobalConfig.ConnectionString);
            _financialService = new FinancialService(unit);
            _financialReportService = new FinancialReportService(unit); 
            CargarDatos();
        }

        private void FrameAdministrarMeses_Load(object sender, EventArgs e)
        {
            var postingPeriods = _financialService.GetPostingPeriods(GlobalConfig.Company.Codigo);
            var toList = new List<PostingPeriod>()
            {
                postingPeriods.FirstOrDefault().DeepClone()
            };

            lstFromPeriod.DataSource = toList; 
            lstToPeriod.DataSource = postingPeriods.DeepClone();
        }

        private void CargarDatos()
        {

            //List<FechaTransaccion> lst = fechaCL.GetAll(compania, usuario);
            DataTable dt = fechaCL.GetDataTable(GlobalConfig.Company, GlobalConfig.Usuario);
            //Cuentas = _cuentaCL.GetAll(GlobalConfig.Company);

            dtRegistros.DataSource = dt;

            lstAbrirMes.DataSource = fechaCL.FechaAbrirMes(GlobalConfig.Company, GlobalConfig.Usuario);
            lstToPeriod.DataSource = fechaCL.GetAllActive(GlobalConfig.Company, GlobalConfig.Usuario);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Se abrira un mes ¿Desea continuar?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (fechaCL.Insert((FechaTransaccion)lstAbrirMes.SelectedItem, GlobalConfig.Company, GlobalConfig.Usuario, out String mensaje))
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
            var amount = ReporteEstadoResultadoIntegralData();

            if (MessageBox.Show($"Se creará una cuenta con un saldo de {amount.Amount}", TextoGeneral.NombreApp,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ///Create new account
                /// Mark Posting period as closed
            } 


            //try
            //{

            //    if ((lstFromPeriod.Items.Count > 0) && MessageBox.Show("¿Desea cerrar este mes?", TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        FechaTransaccion fechaTransaccion = (FechaTransaccion)lstFromPeriod.SelectedItem;
            //        fechaTransaccion.Cerrada = true;

            //        if (fechaCL.CerrarMes(fechaTransaccion, GlobalConfig.Company, GlobalConfig.Usuario, out string mensaje))
            //        {
            //            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            CargarDatos();
            //        }
            //        else
            //        {
            //            MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }

        private ClosurePostingPeriodBalance ReporteEstadoResultadoIntegralData()
        {
            var firstDate = lstFromPeriod.SelectedItem as PostingPeriod;
            var endDate = lstToPeriod.SelectedItem as PostingPeriod;

            var reportParamns = new BasicReportParam()
            {
                CompanyId = GlobalConfig.Company.Codigo,
                FirstDate = $"{firstDate.Date.Year}{string.Format("{0, 0:D2}", firstDate.Date.Month)}",
                EndDate = $"{endDate.Date.Year}{string.Format("{0, 0:D2}", endDate.Date.Month)}"
            };

            return  _financialReportService.PreviousClosurePostingPeriodBalance(reportParamns);
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
