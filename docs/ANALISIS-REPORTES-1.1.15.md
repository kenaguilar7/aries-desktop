# Análisis de reportes: Aries actual vs 1.1.15 / 1.1.18

**Fecha:** 7 sep 2026  
**Alcance:** solo lectura. No hay cambios de código.  
**Pregunta:** ¿los reportes de esta versión (`C:\Aries`, ensamblado **1.2.0**) coinciden en funcionalidad con los estables de **1.1.15 / 1.1.18**?

**Respuesta corta:** no se puede afirmar paridad. Los reportes **nuevos** (comprobación, estado de resultados, asientos) llaman los **mismos SPs** que 1.1.18 y el C# de armado/Excel es el mismo, así que deberían coincidir si la base tiene esas rutinas. Los reportes **clásicos** (auxiliares, balance de situación, maestro de cuentas) **cambiaron de origen de datos**: ya no usan `CuentaDao.GetAll` + `LLenarConSaldos`. Hoy pasan por `SP_GetAccountsByCompanyId`, que **intercambia Ingreso y Costo de venta**. Eso sí puede alterar saldos y totales.

---

## 1. Fuentes comparadas

| Fuente | Qué es | Versión / fecha | Qué se pudo leer |
|---|---|---|---|
| `C:\Aries` | código actual | `AssemblyVersion` **1.2.0** | Forms, servicios, migraciones, tests |
| `C:\Repos\AriesWindowsForms` | fuente del escritorio clásico | `AssemblyVersion` **1.1.4** (el número del exe instalado es mayor; el código es el de esa línea) | Forms, `CapaDatos` SQL, `FinancialReportService` |
| `C:\Users\Steve\AppData\Local\AriesUpdater\app-1.1.18` | exe instalado estable | carpeta **app-1.1.18**, DLLs del **1 ene 2024** | solo binarios (`.exe` / `.dll`). No hay `.cs` |
| Dump `C:\Users\Steve\Desktop\Aries DUMP\aries_routines.sql` | rutinas de RDS `aries` (8.0.35) | dump Workbench | SPs y vistas que **1.1.18 realmente ejecuta** |

`app-1.1.18` no se puede diffear como texto. El contrato de reportes estables es: **mismo C# que `AriesWindowsForms` + mismos SPs del dump**. El repo `AriesWindowsForms` es la fuente más cercana a ese exe.

---

## 2. Inventario (lo que el usuario puede abrir)

| Reporte | Menú / entrada | Stack 1.1.18 | Stack actual (1.2.0) | ¿Misma funcionalidad? |
|---|---|---|---|---|
| Balance de comprobación | Contable → Balance de comprobación | `FinancialReportService` → `SP_AuxiliaryAccountsWithBalanceByDateRange` | Igual (async) | **Muy probable** (mismo SP + mismo armado) |
| Estado de resultado integral | Contable → Pérdidas y ganancias | `SP_EstadoResultadoIntegralReport` + Excel `ReportResultadoIntegralActions` | Igual (async) | **Muy probable** |
| Asientos | Asientos / reporte de asientos | `SP_JournalEntryReportByDateRange` | Igual (async) | **Muy probable** |
| Auxiliares | Contable → Auxiliares | `CuentaCL.GetAll` + `LLenarConSaldos` → `account_info` | `GetAccounts` + `FillAccountsWithBalances` | **Riesgo alto** (mapeo Ingreso/Costo venta) |
| Balance de situación | Contable → Balance de situación | `CuentaCL` + `LLenarConSaldos` | `ReportAccountLoader` | **Riesgo alto** (mismo mapeo + lista de meses) |
| Movimientos de cuenta | Contable → Movimientos | `CuentaDao.GetInfoCompleta*` + `SET lc_time_names='es_MX'` | SQL Dapper casi igual, **sin** `lc_time_names` | **Casi**: meses en inglés; texto “tipo movimiento” distinto en cuentas padre |
| Maestro de cuentas | Maestro de cuentas → Excel | `CuentaCL` + `GetAllActive` | `GetAccounts` + `GetPostingPeriods` | **Riesgo alto** (mapeo) + meses `active=1` |
| Compañías | Maestro de compañías → Excel | `CompañiaCL.GetAll` | `IAdministrationService.GetAllCompanies` | Layout Excel igual; datos según el servicio de compañías |
| Periodos / cierres (admin meses) | Administrar meses | `SP_GetPostingPeriodReport` / `SP_GetClosingPostingPeriodReport` | Igual | **Muy probable** |

El layout Excel de auxiliares, maestro, movimientos, compañías y balance de situación (ClosedXML) es el mismo algoritmo que en `AriesWindowsForms`. El riesgo no está en el pintado; está en **qué números y qué tipo de cuenta** llegan al Excel.

---

## 3. Dos caminos de datos (esto explica casi todo)

```text
1.1.18
  Reportes nuevos  → Dapper → SP_Auxiliary… / SP_EstadoResultado… / SP_JournalEntry…
  Reportes clásicos → ADO.NET CuentaDao.GetAll (account_type+0 numérico)
                      → Cuenta.GenerarTipoCuenta(4=Ingreso, 5=Costo_Venta)
                      → SELECT SUM(*) FROM account_info …

Actual (1.2.0)
  Reportes nuevos  → mismos SPs
  Reportes clásicos → SP_GetAccountsByCompanyId (CASE 4='CostoVenta', 5='Ingreso')
                      → CuentaMapper.GenerarTipoCuenta((int)AccountTag)
                      → SP_GetAccountBalancesFromAccountInfo (misma suma sobre account_info)
```

El enum MySQL `accounts.account_type` es:

`ACTIVO=1, PASIVO=2, PATRIMONIO=3, INGRESO=4, COSTO VENTA=5, EGRESO=6`

`Cuenta.GenerarTipoCuenta` en 1.1.18 y en `Aries.Reporting` actual:

| Número BD | 1.1.18 clásico (`CuentaDao`) | `SP_GetAccountsByCompanyId` (dump y M010) | Efecto en clásicos actuales |
|---|---|---|---|
| 4 | Ingreso (crédito) | string `'CostoVenta'` → enum 5 → `CostoVenta` | **Ingreso tratado como costo de venta** |
| 5 | Costo de venta (débito) | string `'Ingreso'` → enum 4 → `Ingreso` | **Costo de venta tratado como ingreso** |

Los SPs de comprobación y ERI **no** tienen ese cruce: ahí 4 = `'Ingreso'` y 5 = `'CostoVenta'`. Por eso el stack nuevo y el clásico **no están alineados entre sí** en la versión actual.

Consecuencia en auxiliares / situación / maestro:

- `SaldoActualColones` usa `ITipoCuenta.SaldoActual`: ingreso = `saldo − débito + crédito`; costo de venta = `saldo + débito − crédito`.
- Si se intercambian los tipos, **cambia el signo** de esas cuentas.
- En balance de situación, `ListaCuentasBalancePerdida` vs `ListaCuentasBalanceSitucion` se arma por `TipoCuenta`. Ingreso y costo de venta se mezclarían en el bloque de P&amp;G y el total `UTILIDAD/PERDIDA PERIODO`.

---

## 4. Reportes nuevos (deben coincidir con 1.1.18)

### 4.1 Balance de comprobación

- Parámetros: `CompanyId`, `FirstDate` / `EndDate` como `yyyyMM`.
- SP dump: `SP_AuxiliaryAccountsWithBalanceByDateRange` — path con `F_GetAccountPathForReport`, saldos desde `account_info`, **DebOrCred** 1=débito / 2=crédito según tipo (Activo/CostoVenta/Egreso débito; resto crédito).
- C# (idéntico en ambas fuentes): parte Prior / Mensual / Actual a columnas débito o crédito según `DebOCred`.
- Excel: mismas columnas, formato `₡#,##0.00`, path partido por `¡`.

**Nota de contrato SP vs C#:** el dump nombra la columna `DebOrCred`; la propiedad C# es `DebOCred`. Dapper no mapea esos nombres. Si en RDS el alias ya es `DebOCred`, no hay problema. Si el dump sigue tal cual, **1.1.18 y 1.2.0 fallarían igual** (no es una regresión de esta rama). Conviene confirmar con el query de la sección 7.

El SP **no filtra** `accounts.active`. 1.1.18 tampoco. Paridad entre versiones.

### 4.2 Estado de resultado integral

- SP dump: solo cuentas `account_type IN ('INGRESO','EGRESO','COSTO VENTA')`, mismos saldos de `account_info`.
- C# (idéntico): omite auxiliar editable con `CurrentBalance == 0`; títulos → `TOTAL {nombre}`; total = Ingreso − CostoVenta − Egreso (cuentas título).
- Excel: `ReportResultadoIntegralActions` (solo pasó de sync a `await`).

### 4.3 Asientos

- SP dump: `SP_JournalEntryReportByDateRange` sobre vista `accounting_entries_info`, `SET lc_time_names = 'es_MX'`.
- Filtro: `active <> 0` en líneas, asientos y meses (la vista).
- UI/Excel: mismas columnas y formatos de moneda.

`M012` recrea `accounting_entries_info` **sin** `ORDER BY` y sin `DEFINER`/schema `aries`. El SP ya ordena por `month_report, entry_id`. Los montos no deberían cambiar.

---

## 5. Reportes clásicos (aquí está la divergencia)

### 5.1 Saldos (auxiliares, situación, maestro)

Ambos lados suman `account_info` por `company_id` y `month_report` entre `yyyyMM` y `yyyyMM`, y luego suben débitos/créditos a padres.

| Pieza | 1.1.18 | Actual |
|---|---|---|
| Lista de cuentas | `SELECT … account_type+0` + `GenerarTipoCuenta` | `SP_GetAccountsByCompanyId` (CASE cruzado 4/5) |
| Saldos | `CuentaConSaldos` sobre `account_info` | `SP_GetAccountBalancesFromAccountInfo` (misma suma; no trae `cuadrado`) |
| Roll-up | `LLenarConSaldos` (auxiliares → padres) | `BuildAccountsBalance` (misma idea) |
| Quitar sin movimiento | títulos siempre; resto si prior/débito/crédito ≠ 0 | igual (`AccountRules.RemoveAccountsWithoutBalances`) |

`M011` / `M003` reescriben `account_info`: `GROUP BY` por `DATE_FORMAT(month_report,'%Y%m')` en lugar de `YEAR()`/`MONTH()` del dump. El grano sigue siendo **cuenta + YYYYMM**. Con `month_report` al día 1 del mes, los SUM deben coincidir. Eso no arregla el cruce Ingreso/Costo venta.

### 5.2 Meses del combo

| Form | 1.1.18 | Actual |
|---|---|---|
| Auxiliares | `FechaTransaccionCL.GetAll` → `accounting_months` **sin** `active=1` | `SP_GetAllPostingPeriod` → **solo** `active = true` |
| Situación / maestro | `GetAllActive` = `GetAll` ordenado (tampoco filtra `active`) | igual, solo meses activos |

Si nunca desactivan meses, no se nota. Si hay meses `active=0`, 1.1.18 los listaba y 1.2.0 no.

### 5.3 Movimientos de cuenta

SQL de líneas: mismo join `transactions_accounting` + `accounts` + `accounting_entries` + `accounting_months`, `active=1`, orden `month_report, entry_id`.

Saldo corrido: misma regla (Activo / Costo venta / Egreso = débito: `+débito − crédito`; resto al revés).

Diferencias visibles:

| Detalle | 1.1.18 | Actual |
|---|---|---|
| Idioma del mes | `SET lc_time_names = 'es_MX'` → “enero 2024” | sin SET → “January 2024” (o inglés del servidor) |
| Cuenta padre (no auxiliar) | `Movimiento a cuenta hija` si la hija es auxiliar | siempre `Movimiento a hija` |
| Header fecha | auxiliar: `Fecha Documento`; mayor: `Fecha documento` | siempre `Fecha Documento` |

Los importes y el orden de filas deben coincidir. El Excel ClosedXML es el mismo.

---

## 6. Vista `account_info` (alimenta casi todos los saldos)

Dump (1.1.18 / RDS):

```sql
-- grain: account_id + YEAR(month) + MONTH(month)
-- filtros: T0/T1/T2 active <> 0
-- débito/crédito CRC y USD igual que M011
GROUP BY T0.account_id, YEAR(T2.month_report), MONTH(T2.month_report)
```

Migración actual `M011` (y `M003`):

```sql
GROUP BY T0.account_id, T3.account_type, T3.company_id,
         DATE_FORMAT(T2.month_report, '%Y%m')
```

Misma fórmula de SUM. El `GROUP BY` extra es para MySQL 8 `ONLY_FULL_GROUP_BY`. No debería cambiar totales de reporte.

`SP_AuxiliaryAccountsWithBalanceByDateRange` y `SP_EstadoResultadoIntegralReport` **no están** en las migraciones C#; siguen siendo las del dump. `ExpectedSchema` solo exige que existan. Si alguien las reescribe en RDS, 1.1.18 y 1.2.0 se enteran igual (ambos las llaman por nombre).

---

## 7. Queries para revisar en la misma base

Sustituir `@CompanyId`, `@From`, `@To`. Fechas como `'202401'` / `'202412'`. Correr contra la **misma** compañía y meses que uses en el Excel de 1.1.18.

### 7.1 Contrato de rutinas (¿sigue el dump?)

```sql
SHOW CREATE PROCEDURE SP_AuxiliaryAccountsWithBalanceByDateRange;
SHOW CREATE PROCEDURE SP_EstadoResultadoIntegralReport;
SHOW CREATE PROCEDURE SP_JournalEntryReportByDateRange;
SHOW CREATE PROCEDURE SP_GetAccountsByCompanyId;
SHOW CREATE PROCEDURE SP_GetAccountBalancesFromAccountInfo;
SHOW CREATE PROCEDURE SP_GetAllPostingPeriod;
SHOW CREATE FUNCTION F_GetAccountPathForReport;
SHOW CREATE VIEW account_info;
SHOW CREATE VIEW accounting_entries_info;
```

Qué mirar:

- `SP_GetAccountsByCompanyId`: si 4 → `'CostoVenta'` y 5 → `'Ingreso'`, el clásico actual **no** replica `CuentaDao`.
- `SP_AuxiliaryAccountsWithBalanceByDateRange`: alias `DebOrCred` vs `DebOCred`.
- `account_info`: `GROUP BY` dump (`YEAR`/`MONTH`) vs migración (`DATE_FORMAT`).

### 7.2 Mapeo Ingreso vs Costo de venta (el check más importante)

```sql
SELECT
    T0.account_id,
    T1.name,
    T0.account_type AS enum_mysql,          -- INGRESO / COSTO VENTA
    T0.account_type + 0 AS n,             -- 4 o 5
    CASE T0.account_type + 0
        WHEN 4 THEN 'CostoVenta'          -- lo que hace SP_GetAccountsByCompanyId
        WHEN 5 THEN 'Ingreso'
        ELSE T0.account_type
    END AS tag_sp_get_accounts,
    CASE T0.account_type + 0
        WHEN 4 THEN 'Ingreso'             -- lo que hacía CuentaDao 1.1.18
        WHEN 5 THEN 'CostoVenta'
        ELSE T0.account_type
    END AS tag_cuenta_dao_1_1_18
FROM accounts T0
JOIN accounts_names T1 USING (account_name_id)
WHERE T0.company_id = @CompanyId
  AND T0.active = 1
  AND T0.account_type + 0 IN (4, 5)
ORDER BY T0.account_type, T1.name;
```

Si hay filas, el Excel de auxiliares / situación / maestro de **1.2.0 no es el de 1.1.18** para esas cuentas.

### 7.3 Comprobación (mismo SP que el exe 1.1.18)

```sql
CALL SP_AuxiliaryAccountsWithBalanceByDateRange(@CompanyId, @From, @To);
```

Revisar: `AccountTag` 4/5 correctos, columna de naturaleza (`DebOCred` / `DebOrCred`), `PriorBalance` / `DebitBalance` / `CreditBalance`, `PathDirection` con `¡`.

Totales de control (deben cuadrar débito vs crédito del rango si los asientos están cuadrados):

```sql
SELECT
    SUM(debito)  AS debito_crc,
    SUM(credito) AS credito_crc
FROM account_info
WHERE company_id = @CompanyId
  AND month_report BETWEEN @From AND @To;
```

### 7.4 Estado de resultado integral

```sql
CALL SP_EstadoResultadoIntegralReport(@CompanyId, @From, @To);
```

Control del total título (misma fórmula C#):

```sql
-- Después del CALL, o equivalentemente:
-- Ingreso título − Costo venta título − Egreso título
-- (CurrentBalance según DebOCred del SP)
```

### 7.5 Asientos

```sql
SET lc_time_names = 'es_MX';
CALL SP_JournalEntryReportByDateRange(@CompanyId, @From, @To);
```

Control vs vista:

```sql
SELECT
    DATE_FORMAT(month_report, '%M %y') AS mes,
    entry_id,
    account_name,
    debit,
    credit
FROM accounting_entries_info
WHERE company_id = @CompanyId
  AND DATE_FORMAT(month_report, '%Y%m') BETWEEN @From AND @To
ORDER BY month_report, entry_id;
```

### 7.6 Movimientos (cuenta auxiliar vs padre)

```sql
-- Auxiliar (1.1.18 y actual, salvo locale)
SET lc_time_names = 'es_MX';
SELECT
    (SELECT T1.name FROM accounts_names T1
     WHERE T1.account_name_id = T3.account_name_id LIMIT 1) AS Nombre,
    IF(T3.account_guide <> 'CUENTA AUXILIAR', 'Movimiento a hija', 'Movimiento a cuenta') AS TipoMoviento,
    T2.detail AS Detalle,
    T2.reference AS Referencia,
    T2.bill_date AS FechaDocumento,
    DATE_FORMAT(T5.month_report, '%M %Y') AS MesContable,
    T4.entry_id AS NumeroAsiento,
    IF(T2.balance_type+0 = 1, T2.balance, NULL) AS Debito,
    IF(T2.balance_type+0 = 2, T2.balance, NULL) AS Credito,
    T3.account_type+0 AS TipoParaSaldoCorrido
FROM transactions_accounting T2
LEFT JOIN accounts T3 ON T2.account_id = T3.account_id
LEFT JOIN accounting_entries T4 ON T2.accounting_entry_id = T4.accounting_entry_id
LEFT JOIN accounting_months T5 USING (accounting_months_id)
WHERE T3.account_id = @AccountId
  AND T2.active = 1 AND T3.active = 1 AND T4.active = 1
ORDER BY T5.month_report, T4.entry_id;

-- Padre (hijas): WHERE T3.father_account = @AccountId
-- 1.1.18 etiquetaba la hija auxiliar como 'Movimiento a cuenta hija'
```

### 7.7 Saldos clásicos (`account_info`, mismo grano que 1.1.18)

```sql
SELECT
    account_id,
    SUM(debito) AS debito,
    SUM(credito) AS credito,
    SUM(debito_USD) AS debito_usd,
    SUM(credito_USD) AS credito_usd
FROM account_info
WHERE company_id = @CompanyId
  AND month_report BETWEEN @From AND @To
GROUP BY account_id;
```

### 7.8 Meses que ve cada versión

```sql
-- 1.1.18 GetAll (auxiliares / situación / maestro)
SELECT accounting_months_id, month_report, closed, active
FROM accounting_months
WHERE company_id = @CompanyId
ORDER BY month_report;

-- Actual GetPostingPeriods
SELECT accounting_months_id, month_report, closed, active
FROM accounting_months
WHERE company_id = @CompanyId AND active = 1
ORDER BY month_report;
```

---

## 8. Cómo certificar en UI (sin tocar código)

Misma compañía, mismos meses, 1.1.18 y exe actual:

1. **Comprobación** — un rango cerrado conocido. Totales débito/crédito de las seis columnas.
2. **Estado de resultado** — misma fila `UTILIDAD/PERDIDA PERIODO`.
3. **Asientos** — conteo de líneas y suma débito/crédito.
4. **Auxiliares** — una hoja de un mes; sobre todo cuentas de ingreso y costo de venta.
5. **Balance de situación** — `TOTAL` de activo, pasivo, patrimonio y `UTILIDAD/PERDIDA PERIODO`.
6. **Movimientos** — una auxiliar y un título; mirar mes en español y etiqueta de tipo.
7. **Maestro de cuentas** — con saldos, desde el mes más viejo hasta uno cerrado.

El plan de mejoras ya dejaba esto como prueba manual (`PLAN-MEJORAS.md` §6 Reportes).

---

## 9. Conclusión

| Pregunta | Veredicto |
|---|---|
| ¿1.2.0 = 1.1.18 en **comprobación / ERI / asientos**? | **Sí, en diseño.** Mismo SP dump, mismo C# (solo async). Falta el Excel lado a lado y el `SHOW CREATE` de `DebOCred`. |
| ¿1.2.0 = 1.1.18 en **auxiliares / situación / maestro**? | **No, hasta que se corrija o se descarte el CASE 4/5** de `SP_GetAccountsByCompanyId`. Es el hallazgo que puede cambiar números. |
| ¿Movimientos? | **Casi.** Mismos importes y saldo corrido; mes en inglés; un texto de tipo distinto en cuentas padre. |
| ¿`account_info` M011 rompe saldos? | **No se espera.** Mismo SUM, mismo grano YYYYMM. |

**Qué no hacer todavía:** no conviene “arreglar” el CASE 4/5 a ciegas. Ese cruce está en el dump desde 1.1.18 para `SP_GetAccountsByCompanyId` / `SP_GetAccountById`. Los reportes **nuevos** usan otros SPs con el CASE correcto. El clásico 1.1.18 **no usaba** ese SP. Cualquier cambio hay que medirlo con los queries de §7 y un Excel de un mes cerrado.

Cuando se pase a código, el candidato es: mapear `AccountTag` de `GetAccounts` como `CuentaDao` (4=Ingreso, 5=Costo venta) **sin** alterar el CASE de los SPs de comprobación/ERI, o alinear el CASE del dump con el enum MySQL y revisar maestro de cuentas / `GetAccountById`.
