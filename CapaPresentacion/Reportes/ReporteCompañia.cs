using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using CapaEntidad.Enumeradores;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteCompañia : Form
    {
        CompañiaCL compañiaCL = new CompañiaCL();

        List<Company> compañias = new List<Company>();
        List<Company> actualList;

        public ReporteCompañia()
        {
            InitializeComponent();
            CargarDatos();
            //GridDatos.AllowUserToResizeRows = true;


        }

        private async Task CargarDatos()
        {
            lstIds.SelectedIndex = 0;
            compañias = await Task.Run(()=>compañiaCL.GetAll(GlobalConfig.Usuario));
            RadiosbuttonChanceStatus(null, null);
        }

        private void RadiosbuttonChanceStatus(object sender, EventArgs e)
        {
            try
            {
                if (rbtFisicas.Checked)
                {
                    lstIds.Enabled = true;
                    
                    if (lstIds.SelectedIndex == 0)
                    {

                        this.LlenarLista((from c in compañias where c.IdType != IdType.CEDULA_JURIDICA select c).ToList<Company>());

                    }
                    else
                    {
                        //GridDatos.DataSource = compañiaCL.GetDataTable((TipoID)lstIds.SelectedIndex + 1);
                        this.LlenarLista((from c in compañias where c.IdType == ((AriesContador.Core.Models.Utils.IdType)lstIds.SelectedIndex + 1) select c).ToList());
                    }

                }
                else
                {
                    lstIds.Enabled = false;
                    this.LlenarLista((from c in compañias where c.IdType == IdType.CEDULA_JURIDICA select c).ToList<Company>());
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstIds_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadiosbuttonChanceStatus(null, null);
        }

        private void BtnExcel_Click(object sender, EventArgs e)
        {

            try
            {

                if (actualList == null)
                {
                    throw new Exception("La lista se encuentra vacia!");
                }
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = "REPORTE DE COMPAÑIAS" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        // GridDatos.DataSource = this.Records;
                        

                        CapaEntidad.Reportes.ReporteCompañia.GenerarReporte(actualList, sfd.FileName, GlobalConfig.Usuario); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void LlenarLista(List<Company> lst)
        {
            actualList = lst;

            GridDatos.Rows.Clear();
            var list = new BindingList<Company>(lst);

            foreach (Company comp in lst)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = comp;
                row.Cells[0].Value = comp.Code;
                row.Cells[1].Value = comp.IdType.ToString().Replace('_', ' '); ;
                row.Cells[2].Value = comp.IdNumber;
                row.Cells[3].Value = comp.Name;
                if (comp is PersonaFisica)
                {
                    Column4.HeaderText = "Apellido Paterno";
                    Column5.HeaderText = "Apellido Materno";
                    row.Cells[4].Value = ((PersonaFisica)comp).MyApellidoMaterno;
                    row.Cells[5].Value = ((PersonaFisica)comp).MyApellidoPaterno;

                }
                else
                {
                    Column4.HeaderText = "Representante Legal";
                    Column5.HeaderText = "ID Representante";
                    row.Cells[4].Value = ((PersonaJuridica)comp).MyRepresentanteLegal;
                    row.Cells[5].Value = ((PersonaJuridica)comp).MyIDRepresentanteLegal;
                }
                row.Cells[6].Value = comp.Address;
                row.Cells[7].Value = comp.Web;
                row.Cells[8].Value = comp.Mail;
                row.Cells[9].Value = comp.PhoneNumber1;
                row.Cells[10].Value = comp.PhoneNumber2;
                row.Cells[11].Value = comp.Memo;
                row.Cells[12].Value = comp.CurrencyType.ToString().Replace('_', ' ');
                row.Cells[13].Value = (comp.Active) ? "Activa" : "Desactiva";


                GridDatos.Rows.Add(row);

            }
        }

        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close(); 
        }
    }
}
