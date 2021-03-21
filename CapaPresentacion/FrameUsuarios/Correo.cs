using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Textos;
using CapaLogica;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.FrameUsuarios
{
    public partial class Correo : Form
    {


        CorreoCL CorreoCL = new CorreoCL();
        public Correo()
        {
            InitializeComponent();
            CargarDatos();
        }
        public void CargarDatos()
        {
            //this.dataGridView1.Columns.Clear();
            DataTable dt = CorreoCL.Get();

            foreach (DataRow row in dt.Rows)
            {
                object[] vs = row.ItemArray;
                dataGridView1.Rows.Add(vs);

            }

        }
        public void Insertar(UsuarioTemporal usuario)
        {
            CorreoCL.Insert(usuario);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            ReadConcact();
        }
        private void ReadConcact()
        {

            try
            {

                using (XLWorkbook workBook = new XLWorkbook(UlrExcelToRead()))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);
                    dataGridView1.Rows.Clear();
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        InsertRowToDataGrid(row.Cells().ToArray());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertRowToDataGrid(IEnumerable<IXLCell> cells)
        {

            var rs = cells.ToList();
            var line = new String[rs.Count];

            for (int i = 0; i < line.Count(); i++)
            {
                line[i] = rs[i].Value.ToString();
            }
            dataGridView1.Rows.Add(line);
        }

        private void InsertRowToDataGrid(IEnumerable<UsuarioTemporal> cells)
        {
            dataGridView1.Rows.Clear();

            foreach (var user in cells)
            {
                var line = new String[]{
                user.Nombre,
                user.Apellido,
                user.CorreoElectronico,
                user.CCopy,
                user.Titulo,
                user.Asunto,
                user.Cuerpo
                  };
                dataGridView1.Rows.Add(line);
            }

        }

        private String UlrExcelToRead()
        {
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return null;
        }

        private IEnumerable<UsuarioTemporal> ReadTable()
        {
            var retorno = new List<UsuarioTemporal>();

            try
            {

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    
                        retorno.Add(new UsuarioTemporal(
                            nombre: row.Cells[0].Value.ToString(),
                            apellido: row.Cells[1].Value.ToString(),
                            correoElectronico: row.Cells[2].Value.ToString(),
                            ccopy: row.Cells[3].Value.ToString(),
                            titulo: row.Cells[4].Value.ToString(),
                            asunto: row.Cells[5].Value.ToString(),
                            cuerpo: row.Cells[6].Value.ToString()
                            ));
                }

                return retorno;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return retorno;
            }

        }
        private async Task EnviarAsync()
        {
            dataGridView1.AllowUserToAddRows = false;
            var usuariosTemporales = ReadTable();
            if (usuariosTemporales.Count() > 0)
            {
                if (MessageBox.Show($"Enviara un total de {usuariosTemporales.Count()} ¿Desea continuar?",
                    TextoGeneral.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rejecList = await Task.Run(() => CorreoCL.SendMail(usuariosTemporales));
                    RouteRejed(rejecList);
                }
            }
            else
            {

                MessageBox.Show("Lista vacia", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            dataGridView1.AllowUserToAddRows = true;

        }

        private void RouteRejed(IEnumerable<UsuarioTemporal> rejecList)
        {
            if (rejecList.Count() > 0)
            {
                MessageBox.Show($"{rejecList.Count()} correos no pudieron ser enviados, a continuación se listaran listados", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                InsertRowToDataGrid(rejecList);
            }
            else
                MessageBox.Show("Lista enviada exitosamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarAsync();
        }

        //private void CreateColumns()
        //{
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column1 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column7 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column5 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column6 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column2 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column3 = new DataGridViewTextBoxColumn();
        //    System.Windows.Forms.DataGridViewTextBoxColumn Column4 = new DataGridViewTextBoxColumn();

        //    dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        //    Column1,
        //    Column7,
        //    Column5,
        //    Column6,
        //    Column2,
        //    Column3,
        //    Column4});
        //    Column1.HeaderText = "Nombre";
        //    Column1.Name = "nombre";
        //    Column7.HeaderText = "Apellido";
        //    Column7.Name = "apellido";
        //    Column5.HeaderText = "Correo Electronico";
        //    Column5.Name = "correo_electronico";
        //    Column6.HeaderText = "Copiar correo a ";
        //    Column6.Name = "correo_copia";
        //    Column2.HeaderText = "Titulo ";
        //    Column2.Name = "titulo";
        //    Column3.HeaderText = "Asunto";
        //    Column3.Name = "asunto";
        //    Column4.HeaderText = "Mensaje";
        //    Column4.Name = "mensaje";

        //}

    }
}
