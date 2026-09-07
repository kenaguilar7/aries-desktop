using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Services;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Mappers;
using CapaEntidad.Textos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Seguridad
{
    public partial class FormPermisoUsuario : Form
    {

        private readonly IAdministrationService _administrationService;
        private readonly IPermissionService _permissionService;
        private List<Usuario> TodosLosUsuarios = new List<Usuario>();
        private List<Company> CompañiasDelUsuario = new List<Company>();
        private List<Company> TodasLasCompañias = new List<Company>();
        private List<Modulo> modulos = new List<Modulo>();

        public FormPermisoUsuario(IAdministrationService administrationService, IPermissionService permissionService)
        {
            _administrationService = administrationService;
            _permissionService = permissionService;
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            ///Cargamos los usuarios,
            TodosLosUsuarios = _administrationService.GetAllUsers().Select(UserMapper.ToUsuario).ToList();
            lstUsuarios.DataSource = TodosLosUsuarios;
            lstUsuarios.SelectedIndex = -1;
        }
        /// <summary>
        /// Agrega las compañias seleccionadas en la lista (compañias sin asignar)
        /// a la lista (compañias asignadas)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AgregarCompania_Click(object sender, EventArgs e)
        {
            var lst = (listCompañiasSinAsignar.SelectedItems.Cast<Company>()).ToList();
            lst.ForEach((compañia) =>
            {
                listCompañiasAsignadas.Items.Add(compañia);
                listCompañiasSinAsignar.Items.Remove(compañia);
            });
        }
        /// <summary>
        /// Agrega las compañias seleccionadas en la lista (Compañias asignadas)
        /// a la lista (compañias por asignar)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoverCompañia_Click(object sender, EventArgs e)
        {
            var lst = (listCompañiasAsignadas.SelectedItems.Cast<Company>()).ToList();
            lst.ForEach((compañia) =>
            {
                listCompañiasSinAsignar.Items.Add(compañia);
                listCompañiasAsignadas.Items.Remove(compañia);
            });
        }
        /// <summary>
        /// Cargar los datos del usuario seleccionado
        /// </summary>
        /// <param name="usuario"></param>
        private void CargarUsuario(Usuario usuario)
        {

            ///Traigo todas las compañias que el usuario tenga asignado
            listCompañiasAsignadas.Items.Clear();
            listCompañiasSinAsignar.Items.Clear();

            TodasLasCompañias = _administrationService.GetAllCompanies().GetAwaiter().GetResult().ToList();
            var target = _administrationService.GetAllUsers().FirstOrDefault(u => u.Id.ToString() == usuario.UsuarioId)
                         ?? new User { Id = int.TryParse(usuario.UsuarioId, out var parsed) ? parsed : usuario.Id, UserType = (UserType)usuario.TipoUsuario };
            CompañiasDelUsuario = _administrationService.GetAllCompanies(target).GetAwaiter().GetResult().ToList();

            ///Buscamos todas las compañias
            TodasLasCompañias.ForEach((Compañia) =>
            {
                if (CompañiasDelUsuario.Find(x => x.Code == Compañia.Code) == null)
                {
                    ///si la busqueda fue nula
                    ///quiere decir que el usuario no tiene asginada la compañia
                    listCompañiasSinAsignar.Items.Add(Compañia);
                }
                else
                {
                    listCompañiasAsignadas.Items.Add(Compañia);
                }

            });
            CargarModulos();
        }
        /// <summary>
        /// Evento que ocurre cuando la lista que contiene los usuarios cambia de indice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                var user = (Usuario)lstUsuarios.SelectedItem;
                if (user != null)
                {
                    CargarUsuario(user);
                }
                else
                {
                    MessageBox.Show("No se encontro ningun usurario", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        /// <summary>
        /// Carga los modulos disponible
        /// y selecciona los asignados al usuario
        /// y los que no tiene asignados
        /// </summary>
        private void CargarModulos()
        {
            treeViewModulos.Nodes.Clear();
            var user = (Usuario)lstUsuarios.SelectedItem;
            ///El usuario admin puede tener acceso a todas las compañias??? si es asi entonces 
            ///no ponerlos en la lista
            panelAsignacionModulos.Enabled = (user.TipoUsuario == TipoUsuario.Administrador) ? false : true;
            var userId = int.TryParse(user.UsuarioId, out var parsed) ? parsed : user.Id;
            modulos = PermissionMapper.ToModulos(_permissionService.GetModules(userId));

            foreach (var item in modulos)
            {
                var x = new TreeNode(item.ToString())
                {
                    Tag = item,
                    Checked = item.TienePermiso
                };

                CargarVentanasAlNodo(item, ref x);

                treeViewModulos.Nodes.Add(x);
            }

        }
        /// <summary>
        /// Carga las lista de ventanas al su respectivo modulo asignado al nodo
        /// </summary>
        /// <param name="modulo"></param>
        /// <param name="treeNode"></param>
        private void CargarVentanasAlNodo(Modulo modulo, ref TreeNode treeNode)
        {
            foreach (var item in modulo.LstVentanas)
            {
                var x = new TreeNode(item.NombreExterno)
                {
                    Tag = item,
                    Checked = item.TienePermiso
                };
                x.Checked = item.TienePermiso;
                x.Nodes.Add(new TreeNode(item.CRUDInsert.Nombre.ToString()) { Tag = item.CRUDInsert, Checked = item.CRUDInsert.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDUpdate.Nombre.ToString()) { Tag = item.CRUDUpdate, Checked = item.CRUDUpdate.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDLIst.Nombre.ToString()) { Tag = item.CRUDLIst, Checked = item.CRUDLIst.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDDeleted.Nombre.ToString()) { Tag = item.CRUDDeleted, Checked = item.CRUDDeleted.TienePermiso });

                treeNode.Nodes.Add(x);
                ///
            }
        }
        /// <summary>
        /// Guarda los datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            ///Primero guarda las compañias 

            var nuevas = (listCompañiasAsignadas.Items.Cast<Company>()).ToList();
            var remover = (listCompañiasSinAsignar.Items.Cast<Company>()).ToList();

            var user = (Usuario)lstUsuarios.SelectedItem;
            if (user != null)
            {

                var targetId = int.TryParse(user.UsuarioId, out var parsed) ? parsed : user.Id;
                var updaterId = GlobalConfig.User != null ? GlobalConfig.User.Id : 0;
                _permissionService.AssignCompanies(nuevas.Select(c => c.Code), targetId, updaterId);
                _permissionService.RemoveCompanies(remover.Select(c => c.Code), targetId, updaterId);
                _permissionService.UpdateWindowPermissions(PermissionMapper.ToModulePermissions(modulos), targetId, updaterId);
                var ss = modulos;
                MessageBox.Show("Usuario actulizado correctamente", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un usuario", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        /// <summary>
        /// Evento que ocurre cuando se checa un modulo 
        /// se hace un casteo del modulo checado y se le asigna el estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewModulos_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ((IPermiso)e.Node.Tag).TienePermiso = e.Node.Checked;
        }
        /// <summary>
        /// Se sale de la venatana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Evento que ocurre cuando se presiona un tecla en el cuadro de texto 
        /// de buscar usuarios 
        /// es basicamente por si el usuario presiona enter poder hacer el tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Buscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                BoxBuscar_Leave(null, null);
            }
        }
        /// <summary>
        /// Evento que ocurre cuando el control que busca los usuarios por el codigo deja el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoxBuscar_Leave(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxBuscar.Text.Length > 0)
            {
                var tyxt = txtBoxBuscar.Text;

                var r = TodosLosUsuarios.Find(x => x.UserName.Equals(txtBoxBuscar.Text, StringComparison.OrdinalIgnoreCase));

                if (r != null)
                {
                    lstUsuarios.SelectedItem = r;
                    //CargarUsuario(r); 
                }
                else
                {
                    MessageBox.Show($"No se encontro ningun usuario con el usuario {tyxt}", TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        /// <summary>
        /// Evento que ocurre cuando el la lista de usuarios deja el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Usuarios_Leave(object sender, EventArgs e)
        {
            LstUsuarios_SelectedIndexChanged(null, null);
        }
    }
}
