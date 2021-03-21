using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.cods
{
    public class TreeViewCuentas
    {

        public static TreeNode[] CrearTreeView(List<Cuenta> LstCuentas)
        {

            List<TreeNode> retorno = new List<TreeNode>();

            foreach (Cuenta item in LstCuentas)
            {
                if (item.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    retorno.Add(CrearCuenta(item, LstCuentas));
                }
            }

            return retorno.ToArray();
        }
        private static TreeNode CrearCuenta(Cuenta cuenta, List<Cuenta> lst)
        {


            TreeNode retorno = new TreeNode(cuenta.Nombre);
            retorno.Tag = cuenta;

            var sql = from c in lst where c.Padre == cuenta.Id select c;

            var cueHijas = sql.ToArray<Cuenta>();

            foreach (Cuenta item in cueHijas)
            {

                var l3 = (from c in lst where c.Padre == cuenta.Id select c).ToArray<Cuenta>();

                if (l3.Length != 0)
                {

                    TreeNode b = CrearCuenta(item, lst);
                    retorno.Nodes.Add(b);

                }

            }

            return retorno;
        }
        public static List<TreeNode> BuscarNodo(string param, TreeView treeCuentas)
        {

            List<TreeNode> list = new List<TreeNode>();

            foreach (TreeNode item in treeCuentas.Nodes)
            {
                item.BackColor = new Color();

                if (item.Text.Contains(param))

                    list.Add(item);

                if (item.Nodes.Count > 0)

                    list.AddRange(FindChild(item.Nodes, param));

            }

            return list;
        }
        private static List<TreeNode> FindChild(TreeNodeCollection nodes, string param)
        {
            List<TreeNode> list = new List<TreeNode>();

            foreach (TreeNode item in nodes)
            {
                item.BackColor = new Color();
                if (item.Text.Contains(param))
                    list.Add(item);

                if (item.Nodes.Count > 0)
                    list.AddRange(FindChild(item.Nodes, param));
            }

            return list;
        }
        public static void CargarCuentaAlTreeView(Cuenta cuenta, ref TreeView treeCuentas, List<Cuenta> LstCuentas)
        {

            TreeNode node = new TreeNode(cuenta.Nombre)
            {
                Tag = cuenta
            };
            treeCuentas.SelectedNode.Nodes.Add(node);
            LstCuentas.Add(cuenta);
            BuscarCuentaPadre(cuenta);

            void BuscarCuentaPadre(Cuenta cuentaHija)
            {
                if (LstCuentas.Count != 0)
                {
                    foreach (Cuenta item in LstCuentas)
                    {
                        if (item.Id == cuentaHija.Padre)
                        {
                            //item.SaldoAnteriorColones += cuenta.SaldoAnteriorColones;
                            if (item.Indicador != IndicadorCuenta.Cuenta_Titulo)
                            {
                                item.Indicador = IndicadorCuenta.Cuenta_De_Mayor;
                            }
                            BuscarCuentaPadre(item);
                            return;
                        }

                    }
                }
            }
        }
        public static List<Cuenta> GetCuentasHIjas(Cuenta cuentaPadre, List<Cuenta> cuentas)
        {

            var retorno = new List<Cuenta>();
            retorno.Add(cuentaPadre); 
            foreach (var item in cuentas)
            {
                if (item.Padre == cuentaPadre.Id)
                {
                    retorno.Add(item);

                    var hijas = from cc in cuentas where item.Id == cc.Padre select cc;

                    foreach (var hitem in hijas)
                    {
                        retorno.AddRange(GetCuentasHIjas(hitem, cuentas));
                    }
                }


            }

            return retorno;
        }
    }
}
