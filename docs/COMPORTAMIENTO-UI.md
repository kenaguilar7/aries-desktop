# Comportamiento de CapaPresentacion

Red de caracterización (fase -1). Complementa [`PLAN-MIGRACION.md`](PLAN-MIGRACION.md) y [`ARQUITECTURA.md`](ARQUITECTURA.md).

Los tests **no** clican WinForms. Fijan la lógica que cada form ya llama. Si un merge cambia asientos o Excel, estos contratos deben fallar.

**23 forms.** Cero UserControls.

---

## Arranque y sesión

| Form | Backend | Contrato |
|---|---|---|
| `FrameMenu` | HttpAdmin (inyectado) | Abre `LoginForm`. Si `GlobalConfig.User` es null, `Application.Exit`. `HideOptions` oculta menús según permisos. |
| `LoginForm` | HttpAdmin `POST auth/login` | Guarda token + usuario. Sin credenciales válidas no entra al MDI. |
| `FrameSeleccionCompañia` | HttpAdmin `GetAllCompanies` | Antes de cambiar compañía recorre MDI children `INeedValidatedForClose`. Hoy solo `FrameAsientos` lo implementa (asiento persistido debe estar cuadrado). |

`IFinancialService` **no** está en DI. Forms financieros hacen `new FinancialService(new UnitOfWork(GlobalConfig.ConnectionString))`.

---

## Compañías, usuarios, permisos, correo

| Form | Backend | Invariantes |
|---|---|---|
| `FrameMaestroCompañia` | **Mixto:** HTTP list/código/delete + `CompañiaCL` insert/update | Cédula (`VerificaString.VerificarID` por longitud), nombre no vacío, email. Copiar plan = **todas** las cuentas del origen, no `Id <= 57`. Jurídica vs física: `PersonaJuridica` / `PersonaFisica` (`op1`/`op2`). |
| `ReporteCompañia` | `CompañiaCL` | Listado Excel ClosedXML. |
| `FrameMaestroUsuario` | `UsuarioCL` | Nombre y username no en blanco. Username único (DAO). |
| `Correo` | `CorreoCL` | Log `usuarios_correo`. No hay tests de SMTP. |
| `FormPermisoUsuario` | `PermisoCL` + `Guachi` | Admin (`TipoUsuario.Administrador`) bypasea. Usuario: `modules` / `windows` / `windows_permission` / `companies_permission`. |

Validación de cédula (longitud del string, con guiones): jurídica 12, nacional 11, DIMEX 12, NITE 10.

---

## Plan de cuentas

| Form | Backend | Invariantes |
|---|---|---|
| `FrameMaestroCuenta` | `CuentaCL`, `FechaTransaccionCL` | Árbol `Ordernar`: título → hijas recursivo. No borrar `Editable == false` ni no-auxiliar. Nombre único por compañía. |
| `FrameNuevaCuenta` | `CuentaCL` | Auxiliar bajo auxiliar: `HeredarSaldosSiPadreEsAuxiliar`. `VerificarSiEsApta` avisa si el padre auxiliar tiene movimientos. |
| `FrameSeleccionCuenta` | `CuentaCL` | Devuelve cuenta por `ICallingForm.TransferirCuenta`. |
| `ReporteCuenta` | `CuentaCL` | Maestro Excel (`ReporteMaestroCuenta`). |

`LLenarConSaldos`: DAO llena auxiliares; `AplicarRollUpHaciaPadres` suma débitos/créditos hacia arriba (**no** saldo anterior). `QuitarCuentasSinSaldos` deja títulos aunque no tengan movimiento.

---

## Periodos

| Form | Backend | Invariantes |
|---|---|---|
| `FrameAdministrarMeses` | `FinancialService` + `FinancialReportService` | No duplicar mes (`PeriodExist`: mismo año+mes). Disponibles: `PostingPeriodCreator` (si hay movimientos, solo mes siguiente; si no, anterior y siguiente). Cierre nuevo: `PreviousClosurePostingPeriodBalance` (total ERI) + `ClosePostingPeriod`. |
| Reportes viejos / cierre viejo | `FechaTransaccionCL` | `FechaAbrirMes`: si no hay meses → mes actual; si hay → mes anterior al más viejo **y** siguiente al más nuevo. `FechaCerrarMes`: extremos abiertos. `Insert` rechaza mes duplicado. |

Hay **dos** caminos de cierre. No unificar en esta fase.

---

## Asientos y líneas (P0)

Camino vivo: `FrameAsientos` → `FinancialService` → SPs. `AsientoCL` / `TransaccionCL` están comentados.

### `FrameAsientos`

- Consecutivo: `CreateJournalEntryConsecutive` → `SP_GetJournalEntryConsecutive`.
- Alta encabezado: `CreateJournalEntry` → `SP_InsertJournalEntry`. El **servicio no valida** cuadratura.
- Líneas: create/update/delete (`SP_DesactivateJournalEntryLine` = soft delete).
- Moneda: `JournalEntryLineAmount` — colones `RateAmount = 1` y `Amount` = monto digitado; dólares `ForeignAmount` = monto digitado y `Amount = Truncate(Foreign * Rate * 100) / 100`.
- Totales: `DebitosColones` / `CreditosColones` = suma de `Amount` por `DebOrCred`. `Cuadrado` si son iguales.
- Status al guardar: `ApplyStatusFromBalance` → `Approved` si cuadrado, si no `Progress`.
- Periodo cerrado: **solo UI** (`ValidatePostingPeriodStatus`). El servicio sí insertaría.
- Salir / cambiar asiento / `INeedValidatedForClose`: `CanNavigateAway` — Id 0 (borrador) se puede cerrar; persistido exige `Cuadrado`.
- Soft delete asiento: `DeleteJournalEntry` → `SP_DesactivateJournalEntry`.

### Otros

| Form | Backend | Contrato |
|---|---|---|
| `SwitchAccountEntryPeriod` | FS `UpdatedJournalEntryPeriod` | Nuevo consecutivo del periodo destino + `Update`. |
| `RestoreJournalEntry` | FS | Lista `GetAllJournalEntryDeleted` / líneas; restore `SP_RestoreJournalEntry*`. |
| `FrameAsientoCierre` | `FechaTransaccionCL.AsientoDeCierre` | Cierra meses, `LLenarConSaldos`, asiento de cierre a auxiliar elegida (`GenerarSaldosEnCeroParaCierreDeAsieto`). |
| `ReporteAsientos` | FRS `JournalEntryReport` | SP `SP_JournalEntryReportByDateRange` → ClosedXML en el form. |

---

## Reportes

Excel activo: **ClosedXML**. Interop está referenciado y no se usa en `.cs`.

### Stack nuevo (FinancialReportService → SP → ClosedXML)

| Form | Datos | Columnas / reglas |
|---|---|---|
| `FrameReporteComprobación` | `BalanceComprobacionReport` ← `SP_AuxiliaryAccountsWithBalanceByDateRange` + `BuildAccountsBalance` | Path `¡`. Deb/Cred según `DebOCred`: Prior / Monthly / Current. Árbol en columnas `AccountName{i}`. |
| `ReporteEstadoResultadoIntegral` | `EstadoResultadoIntegral` ← `SP_EstadoResultadoIntegralReport` | Omite auxiliar editable con `CurrentBalance == 0`. Título → `IsMainAccount` (Excel: `TOTAL {nombre}`). Total P&G = Ingreso − CostoVenta − Egreso (cuentas **título**). Fila `UTILIDAD/PERDIDA PERIODO`. |
| `ReporteAsientos` | `JournalEntryReport` | Mes, número, cuenta, referencia, detalle, fecha doc, débitos, créditos, moneda, tipo cambio, monto dólares. |

`AccountingReportService` (raíz WebServices) es stub. No es referencia.

### Stack viejo (CuentaCL.LLenarConSaldos → CapaEntidad)

| Form | Generador | Notas |
|---|---|---|
| `FrameReporteAuxiliares` | `ReporteAuxiliares` | Un mes = una hoja; `QuitarCuentasSinSaldos` conserva títulos. |
| `ReporteBalanceSituacion` | `ReporteExcel.ReporteUtilidadPerdida` | `ReporteBalanceSituacion` de Entidad está comentado en UI. |
| `ReporteMovimientosCuenta` | `ReporteMovimientoCuenta` | `CuentaCL.GetInfoCompleta` (DataTable). |
| `ReporteCuenta` | `ReporteMaestroCuenta` | |
| `ReporteCompañia` | `ReporteCompañia` | |

Clases Excel **sin caller:** `ReporteBalanceComprobacion`, `ReportePerdidasGanancias`, `ReporteAsiento` (método comentado). No testear.

Varios writers de Entidad aún leen `Company.CurrencyType` / `Name` (el modelo Core usa `MoneyType` / `CompanyName`). Documentado; no “arreglar” en -1.

---

## Hooks de test

- `INeedValidatedForClose.IsAvalibleToClose()` — `FrameAsientos` → `CanNavigateAway`.
- `ICallingForm.TransferirCuenta` — picker de cuentas.
- Helpers extraídos: `JournalEntryLineAmount`, `JournalEntry.ApplyStatusFromBalance`, `CuentaCL.AplicarRollUpHaciaPadres`, `CuentaCL.HeredarSaldosSiPadreEsAuxiliar`.
