using CapaEntidad.Entidades.Compañias;
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

        List<Compañia> compañias = new List<Compañia>();
        List<Compañia> actualList;

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

                        this.LlenarLista((from c in compañias where c.TipoId != TipoID.CEDULA_JURIDICA select c).ToList<Compañia>());

                    }
                    else
                    {
                        //GridDatos.DataSource = compañiaCL.GetDataTable((TipoID)lstIds.SelectedIndex + 1);
                        this.LlenarLista((from c in compañias where c.TipoId == ((TipoID)lstIds.SelectedIndex + 1) select c).ToList<Compañia>());
                    }

                }
                else
                {
                    lstIds.Enabled = false;
                    this.LlenarLista((from c in compañias where c.TipoId == TipoID.CEDULA_JURIDICA select c).ToList<Compañia>());
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

        private void LlenarLista(List<Compañia> lst)
        {
            actualList = lst;

            GridDatos.Rows.Clear();
            var list = new BindingList<Compañia>(lst);

            foreach (Compañia comp in lst)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = comp;
                row.Cells[0].Value = comp.Codigo;
                row.Cells[1].Value = comp.TipoId.ToString().Replace('_', ' '); ;
                row.Cells[2].Value = comp.NumeroCedula;
                row.Cells[3].Value = comp.Nombre;
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
                row.Cells[6].Value = comp.Direccion;
                row.Cells[7].Value = comp.Web;
                row.Cells[8].Value = comp.Correo;
                row.Cells[9].Value = comp.Telefono[0];
                row.Cells[10].Value = comp.Telefono[1];
                row.Cells[11].Value = comp.Observaciones;
                row.Cells[12].Value = comp.TipoMoneda.ToString().Replace('_', ' ');
                row.Cells[13].Value = (comp.Activo) ? "Activa" : "Desactiva";


                GridDatos.Rows.Add(row);

            }
        }

        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close(); 
        }
    }
}
