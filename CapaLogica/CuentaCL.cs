using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using CapaDatos.Daos;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Mappers;

namespace CapaLogica
{
    /// <summary>
    /// Excel reports and old closing still call this by design (fase 4).
    /// Account CRUD for WinForms goes through IFinancialService.
    /// Pure rules delegate to AccountRules so there is one implementation.
    /// </summary>
    [Obsolete("Prefer IFinancialService for account CRUD. Classic Excel reports still use this.")]
    public class CuentaCL
    {
        private CuentaDao _cuentaDao;
        private FechaTransaccionCL _fechaTransaccionCL;

        private CuentaDao cuentaDao
        {
            get
            {
                if (_cuentaDao == null) _cuentaDao = new CuentaDao();
                return _cuentaDao;
            }
        }

        private FechaTransaccionCL FechaTransaccion
        {
            get
            {
                if (_fechaTransaccionCL == null) _fechaTransaccionCL = new FechaTransaccionCL();
                return _fechaTransaccionCL;
            }
        }
        public Boolean Deleted(Cuenta cuenta, Usuario usuario, out String mensaje)
        {
            var account = CuentaMapper.ToAccount(cuenta);
            if (!AccountRules.CanDelete(account, out mensaje))
            {
                return false;
            }
            return cuentaDao.Deleted(cuenta, usuario, out mensaje);
        }
        public List<Cuenta> GetAll(Company t)
        {
            var cuentas = cuentaDao.GetAll(t);
            return Ordernar(cuentas);
        }
        public Boolean Insert(ref Cuenta nuevaCuenta, Cuenta cuentaPadre, out String Mensaje, Usuario user)
        {
            if (VerificarNombre(nuevaCuenta, nuevaCuenta.Nombre, out Mensaje, nuevaCuenta.MyCompania))
            {
                HeredarSaldosSiPadreEsAuxiliar(nuevaCuenta, cuentaPadre);

                if (cuentaDao.Insert(ref nuevaCuenta, cuentaPadre, user, out Mensaje))
                {
                    Mensaje = AccountRules.CreateSuccessMessage;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public DataTable GetInfoCompleta(Cuenta cuenta)
        {
            var retorno = new DataTable();
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                retorno = cuentaDao.GetInfoCompletaCuentaAux(cuenta);
            }
            else
            {
                retorno = cuentaDao.GetInforCompletaCuentaMayor(cuenta);
            }

            decimal lastSaldoActual = 0m;
            foreach (DataRow item in retorno.Rows)
            {
                var rw = item["Saldo Actual"];
                decimal debito = String.IsNullOrWhiteSpace(item["Debito"].ToString()) ? 0m : Convert.ToDecimal(item["Debito"]);
                ITipoCuenta tpcnta = Cuenta.GenerarTipoCuenta(Convert.ToInt32(rw));
                lastSaldoActual = tpcnta.SaldoActual(saldo: lastSaldoActual, debito: debito, credito: string.IsNullOrWhiteSpace(item["Credito"].ToString()) ? 0m : Convert.ToDecimal(item["Credito"]));

                item["Saldo Actual"] = string.Format("{0:n}", lastSaldoActual);
            }
            return retorno;
        }
        public Boolean Update(ref Cuenta cuenta, Usuario user, String nuevoNombre, Company compañia, String nuevaDesc, out String mensaje)
        {
            try
            {
                if (!AccountRules.ValidateName(nuevoNombre, out mensaje))
                {
                    return false;
                }
                else
                {
                    mensaje = "";

                    if (cuentaDao.VerificarNombre(cuenta, nuevoNombre, compañia))
                    {
                        cuenta.Nombre = nuevoNombre;
                        cuenta.Detalle = nuevaDesc;


                        return cuentaDao.UpdateNameInfo(cuenta, user, out mensaje);


                    }
                    else
                    {
                        mensaje = AccountRules.NameTakenMessage;
                        return false;
                    }


                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }
        public Boolean UpdatesettInfo(Cuenta t, Usuario user, out String mensaje)
        {
            return cuentaDao.UpdatesettInfo(t, user, out mensaje); 
        }

        public Boolean VerificarNombre(Cuenta cuenta, String nombre, out String mensaje, Company compañia)
        {
            if (!AccountRules.ValidateName(nombre, out mensaje))
            {
                return false;
            }
            else
            {
                if (cuentaDao.VerificarNombre(cuenta, nombre, compañia))
                {
                    mensaje = "El nombre puede ser utilizado";
                    return true;
                }
                else
                {
                    mensaje = AccountRules.NameCannotBeUsedMessage;
                    return false;
                }
            }
        }
        public void LLenarConSaldos(DateTime fechaInicio, DateTime fechaFinal, List<Cuenta> lst, Company compañia)
        {
            lst.ForEach(delegate (Cuenta c)
            {
                c.DebitosColones = 0.00m;
                c.CreditosColones = 0.00m;
                c.DebitosDolares = 0.00m;
                c.CreditosDolares = 0.00m;
            });

            cuentaDao.CuentaConSaldos(lst, compañia, fechaInicio, fechaFinal);
            AplicarRollUpHaciaPadres(lst);
        }

        public void HeredarSaldosSiPadreEsAuxiliar(Cuenta nuevaCuenta, Cuenta cuentaPadre)
        {
            var child = CuentaMapper.ToAccount(nuevaCuenta);
            var parent = CuentaMapper.ToAccount(cuentaPadre);
            AccountRules.InheritBalancesIfParentIsAuxiliar(child, parent);
            CuentaMapper.CopyBalancesToCuenta(child, nuevaCuenta);
        }

        public void AplicarRollUpHaciaPadres(List<Cuenta> lst)
        {
            var accounts = lst.Select(CuentaMapper.ToAccount).ToList();
            AccountRules.ApplyRollUp(accounts);
            CuentaMapper.CopyBalancesToCuentas(accounts, lst);
        }
        public Cuenta BuscarCuentaPadre(List<Cuenta> lst, Cuenta cuentaHija)
        {
            if (lst.Count != 0)
            {
                foreach (Cuenta item in lst)
                {
                    if (item.Id == cuentaHija.Padre)
                    {
                        return item;
                    }

                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public List<Cuenta> Ordernar(List<Cuenta> lst)
        {
            var accounts = lst.Select(CuentaMapper.ToAccount).ToList();
            var ordered = AccountRules.OrderByTree(accounts);
            var byId = lst.ToDictionary(c => c.Id);
            return ordered.Select(a => byId[a.Id]).ToList();
        }
        public Boolean VerificarSiEsApta(Cuenta cuentaPadre, out String Mensaje)
        {
            List<FechaTransaccion> meses = FechaTransaccion.GetAllActive(cuentaPadre.MyCompania, null);

            if (meses.Count != 0)
            {
                var cuentaDummy = cuentaPadre.DeepCopy();
                var dummy = new List<Cuenta> { cuentaDummy };

                LLenarConSaldos(meses[0].Fecha, meses.Last().Fecha, dummy, cuentaPadre.MyCompania);

                var dummyAccount = CuentaMapper.ToAccount(cuentaDummy);
                if (dummyAccount.AccountType == AccountType.Cuenta_Auxiliar && AccountRules.HasMovement(dummyAccount))
                {
                    Mensaje = AccountRules.ParentHasMovementsWarning(dummyAccount);
                    return false;
                }
            }

            Mensaje = "";
            return true;
        }

        public List<Cuenta> QuitarCuentasSinSaldos(List<Cuenta> lis)
        {
            var accounts = lis.Select(CuentaMapper.ToAccount).ToList();
            var filtered = AccountRules.RemoveAccountsWithoutBalances(accounts);
            var byId = lis.ToDictionary(c => c.Id);
            return filtered.Select(a => byId[a.Id].DeepCopy()).ToList();
        }

        public Boolean GenerarSaldosEnCeroParaCierreDeAsieto(Cuenta cuentaSaldoAsiento, Company compañia, Usuario usuario, int limitSec) {
            return cuentaDao.GenerarSaldosEnCeroParaCierreDeAsieto(cuentaSaldoAsiento, compañia, usuario, limitSec);
        }

    }

}
