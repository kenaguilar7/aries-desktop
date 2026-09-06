# Fase 3 — SPs de cuentas (copia de BD, nunca RDS producción)

`SP_InsertAccount` del dump se usa al **copiar el plan** en `CompanyRepository.Add`. No se modifica: no debe reasignar asientos.

Hueco vs `CuentaDao` (escritorio):

| Operación | DAO | Dump / Data antes de fase 3 |
|---|---|---|
| Alta interactiva (auxiliar bajo auxiliar) | Inserta nombre, inserta cuenta con `previous_balance_*`, mueve `transactions_accounting` al nuevo id, padre `account_guide = 2` | `SP_InsertAccount` solo inserta (remap de `FatherAccount` es de copia de compañía, no de líneas) |
| Unique name | `account_type = 1` (ACTIVO) | no hay SP |
| Delete | auxiliar + sin movimientos en meses **abiertos** | `SP_DesactivateAccount` (se llama después de las guardas en C#) |
| Saldos del maestro | vista `account_info` YYYYMM | `SP_AuxiliaryAccountsWithBalanceByDateRange` es otro camino (comprobación); no se asume paridad |

Scripts de esta carpeta cubren el alta interactiva, unique name, movimientos abiertos y saldos `account_info`. Aplicar en copia Docker/`AriesContabilidad_Local`.
