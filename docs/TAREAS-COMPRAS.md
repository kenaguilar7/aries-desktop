# Tareas: compras, proveedores y cuentas por pagar

Documento de trabajo para PRs independientes. Pensado para lanzar agentes (local o cloud) y revisar mañana.

**Producto (esta oleada):** solo la app web Blazor (`src/web/Aries.Contabilidad`) + el API que la sirve (`src/hosts/Aries.WebAPI`).
**Dominio:** Costa Rica (cédula jurídica/nacional, DIMEX, NITE; IVA 13%).
**Fecha:** 2026-09-13.

---

## 0. Superficie: web sí, WinForms no

WinForms sigue siendo el **núcleo contable de escritorio** (asientos, plan, periodos, reportes clásicos). Eso no se reemplaza aquí y **no se le cambian funciones**.

Compras / proveedores / CxP son features **nuevas de la web**. El escritorio no las muestra, no las menúea y no las ejecuta.

| Se toca | No se toca |
|---|---|
| `src/web/Aries.Contabilidad` (páginas `.razor`, nav, clientes HTTP, CSS) | `src/desktop/**`, `CapaPresentacion`, `CapaLogica`, `CapaDatos`, `CapaEntidad` |
| `src/hosts/Aries.WebAPI` (endpoints nuevos, DI del API) | Formularios, menús, reportes o permisos de ventana de WinForms |
| Capas compartidas **en modo aditivo**: modelos nuevos, repos nuevos, `PurchasingService` nuevo, migraciones nuevas | Cambiar firmas o comportamiento de métodos que el escritorio ya llama (`FinancialService` asientos/periodos/cuentas, `AdministrationService` login/compañías) |
| Tests de Core / Data / WebAPI / Blazor | Tests o proyectos que solo existen para el exe |

Reglas para el agente:

1. **Cero diffs** bajo desktop / `Capa*`. Si un cambio “hace falta” ahí, no lo hagas: deja la feature solo en web.
2. No enganchar `PurchasingService` al exe. El host que lo registra es `Aries.WebAPI`.
3. T2 (cuentas default): se puede **agregar** nombres al catálogo `DefaultChartOfAccounts` (compañías nuevas, web o escritorio, reciben dos auxiliares más). No cambiar la pantalla de crear compañía ni el editor de cuentas de WinForms. `EnsurePurchaseAccounts` se dispara desde el API/web, no desde un form de escritorio.
4. Reusar `PosTax`, `IUnitOfWork` y el posteo de asientos **existente** (`FinancialService` crear asiento/líneas) sin alterar cómo el escritorio asienta a mano.
5. UI de referencia: páginas Blazor de POS e Integración, no pantallas WinForms.

---

## 1. Qué ya existe (no reinventar)

Hoy la app es un ciclo **vender → bajar stock → (opcional) asentar sesión POS**.

| Pieza | Qué hace | Copiar de |
|---|---|---|
| Inventario | CRUD de productos, barcode, precio, **costo**, stock, exento IVA | `Pages/Pos/Inventory.razor` |
| Ventas | Carrito, efectivo/tarjeta/transferencia, IVA 13% incluido en precio | `Pages/Pos/Sales.razor`, `PosTax` |
| Caja | Apertura / cierre; el asiento **no** se genera al vender | `Pages/Pos/Caja.razor` |
| Integración | Mapeo de cuentas + preview + posteo de sesión cerrada → asiento | `PosAccountingService`, `Pages/Integration/*` |
| Plan default | 62 cuentas. Hay `CUENTAS POR COBRAR`, `INVENTARIOS`, `IVA POR PAGAR`, `COSTO DE MERCADERÍA`. **No hay CxP ni IVA soportado** | `DefaultChartOfAccounts` |
| UI web | Bootstrap + MudBlazor, `page-title`, cards, español, `CompanyId` en la ruta | `NavMenu.razor`, páginas POS **Blazor** |

Patrón de una feature CRUD (obligatorio en cada PR):

1. Modelo en `src/domain/AriesContador.Core/Models/...` (hereda `BaseModel`: `Id`, `CreatedAt`, `UpdateAt`, `CreatedBy`, `UpdatedBy`, `Active`)
2. Interfaz de repositorio + propiedad en `IUnitOfWork`
3. Repo Dapper + SQL en `src/infrastructure/AriesContador.Data/Query/`
4. Migración `M0xx_*.cs` con `-- BATCH`, tablas `utf8mb4_unicode_ci`, `company_id VARCHAR(5)`
5. Actualizar `ExpectedSchema.Tables` / columnas
6. Servicio de aplicación + tests en `tests/AriesContador.Tests`
7. Minimal API `MapGroup("/...").RequireAuthorization()` + `Program.cs`
8. Cliente HTTP Blazor + registro en `Program.cs` del WASM
9. Página `.razor` + grupo en `NavMenu.razor`

La venta **no** asienta. El asiento sale de un posteo explícito. Las compras deben seguir esa separación: registrar operativo primero, postear a contabilidad después.

---

## 2. Qué falta (gap real)

No hay ni una mención de compra, proveedor, vendor, accounts payable o factura de compra en dominio, API ni web. El escritorio tampoco las tiene; **no es trabajo de esta oleada agregarlas ahí**.

### Hueco operativo (el que se siente en la app)

- No se puede dar de alta un **proveedor**.
- No se puede registrar una **factura de compra**.
- El stock **solo baja** al vender o se edita a mano en Inventario. Nadie lo **sube** con una recepción.
- El **costo** del producto no se actualiza con compras.
- No hay **pagos** a proveedores (solo métodos de cobro en el POS).
- No hay módulo de **gastos** (alquiler, servicios) distinto de un asiento manual.

### Hueco contable

El plan default tiene el lado de ventas y no el de compras:

| Existe | Falta el espejo |
|---|---|
| `CUENTAS POR COBRAR` | `CUENTAS POR PAGAR` |
| `IVA POR PAGAR` (débito fiscal) | `IVA SOPORTADO` / acreditable |
| `VENTAS` | compras al inventario (el debe va a `INVENTARIOS`) |
| Posteo POS → asiento | Posteo compra → asiento |
| Conciliación sesión ↔ asiento | Conciliación factura ↔ asiento |

Sin CxP e IVA soportado, un asiento de compra o se tira a `OTROS GASTOS` o queda descuadrado respecto a Hacienda.

### Huecos de producto que no son compras (fuera de estas PRs, para no mezclar)

- Clientes / cuentas por cobrar operativas (hoy CxC es solo cuenta).
- Factura electrónica CR (XML, clave numérica, Hacienda).
- Reportes web: balance de comprobación, ERI, balance de situación (API ya existe; UI Blazor “en construcción”). No portar esas pantallas desde WinForms en estas PRs.
- Usuarios y permisos en la web.
- Devoluciones POS, órdenes de compra, multi-bodega.
- Cualquier pantalla o menú nuevo en WinForms.

---

## 3. Alcance v1 (estas PRs)

Un ciclo mínimo, simétrico al POS:

1. Maestro de proveedores.
2. Cuentas nuevas en el plan (CxP + IVA soportado).
3. Factura de compra con líneas de producto → sube stock y refresca costo.
4. Mapeo + posteo a asiento (Dr Inventario, Dr IVA soportado, Cr CxP o Caja).
5. Pago a proveedor (Dr CxP, Cr Caja/Bancos).

**Fuera de v1:** factura electrónica, órdenes de compra, retenciones, múltiples impuestos, landing cost, tres vías (OC vs factura vs recepción), proveedores del extranjero con tipo de cambio.

IVA: reutilizar `PosTax.DefaultRate = 0.13m` y `SplitGross`. En compras el default también es **precio con IVA incluido**, igual que el POS (`PricesIncludeTax = true`).

Costo: al confirmar la compra, `product.Cost` pasa a ser el **último costo neto** de esa línea. No calcular promedio ponderado en v1.

---

## 4. Cómo partir los PRs

```text
T1 Proveedores          ──┐
                          ├── T3 Facturas de compra + stock
T2 Cuentas del plan     ──┤
                          └── T4 Posteo compras → asientos
                                    └── T5 Pagos a proveedores
```

**Esta noche en paralelo:** T1 y T2 (no se pisan).
**Después, en serie:** T3 → T4 → T5.

Si un solo agente va a trabajar toda la noche, que haga T1 → T3 en **un** branch (T2 puede ir en el mismo o aparte). No lances T3 en paralelo a T1: va a inventar el modelo de proveedor.

Cada PR:

- Un slice vertical **web**: BD → servicio nuevo o extensión aditiva → API → Blazor.
- Tests del servicio con `FakeUnitOfWork` (ver `tests/AriesContador.Tests/PosTests`).
- Mensajes de error en español, como el POS web (`"Ya hay una caja abierta"`).
- Soft delete (`active = 0`). No `DELETE` físico.
- **No tocar WinForms** (ver sección 0). El PR se rechaza si incluye forms, menús o cambios de función del exe.
- No mergear. Abrir PR contra el default branch y parar.

Siguiente migración: **M021**. Hoy el tope es M020.

---

## T1 — Proveedores (CRUD)

**Branch sugerido:** `feat/purchases-suppliers`
**Depende de:** nada
**Archivos guía:** `Product.cs`, `Inventory.razor`, `PosEndpoints.MapProduct`, `M019_PosTables`

### Modelo `Supplier`

`src/domain/AriesContador.Core/Models/Purchases/Supplier.cs`

| Campo | Tipo | Notas |
|---|---|---|
| CompanyId | string | `VARCHAR(5)` |
| Name | string | Razón social / nombre |
| IdType | `IdType` | Mismo enum que compañías |
| NumberId | string | Cédula / DIMEX / NITE |
| Email | string | |
| Phone | string | |
| Address | string | |
| Notes | string | |
| + BaseModel | | |

Tabla `suppliers`: PK `supplier_id`, unique `(company_id, number_id)` donde `number_id` no sea vacío, index `company_id`, columnas de auditoría iguales a `products`.

### Capas

- `ISupplierRepository` + `SupplierRepository` + `PurchasesQuery`
- `IPurchasingService` / `PurchasingService` (nuevo servicio; no inflar `PointOfSaleService`)
- API `/supplier/GetAll/{companyId}`, `/Find/{id}`, `/Create`, `/Update`, `/Delete/{id}`
- Cliente `IPurchasingClient` en Blazor
- Página `/purchases/{CompanyId}/suppliers` (lista + formulario inline como Inventario)
- Nav: grupo **Compras** con “Proveedores”
- Ampliar `NavMenu.TryCompanyIdFromPath` con prefijo `purchases`

### Tests mínimos

- Crear proveedor en una compañía.
- Rechazar duplicado de `NumberId` en la misma compañía.
- Soft delete: deja de salir en GetAll.
- Compañía A no ve proveedores de B.

### Prompt para el agente

```text
Repo: este codebase Aries. Lee docs/TAREAS-COMPRAS.md, sección T1.

Implementa el CRUD de proveedores (Supplier) siguiendo el patrón de productos del POS Blazor
(Inventory.razor), no pantallas WinForms.
Capa completa: modelo, migración M021_Suppliers, ExpectedSchema, repo, IPurchasingService,
API, cliente Blazor, página, grupo Compras en NavMenu, tests xUnit con FakeUnitOfWork.

Solo web: cero cambios en src/desktop, CapaPresentacion, CapaLogica u otros forms.
No implementes facturas de compra ni asientos. Soft delete. Español en UI y errores.
Abre un PR con título: "feat(purchases): maestro de proveedores".
No hagas merge. Corre los tests del proyecto AriesContador.Tests y Aries.Data.Tests
relacionados al catálogo de migraciones.
```

---

## T2 — Cuentas del plan para compras

**Branch sugerido:** `feat/purchases-chart-accounts`
**Depende de:** nada (puede ir en paralelo a T1)
**Archivos guía:** `DefaultChartOfAccounts.cs`, `PosAccountingService` (ensure-accounts), `DefaultChartOfAccountsTests.cs`

### Qué agregar al catálogo default

Hoy hay 62 cuentas (índices 1–62). Agregar auxiliares:

| Nombre | Padre conceptual | Tag | Tipo |
|---|---|---|---|
| `CUENTAS POR PAGAR` | `PASIVO CORTO PLAZO` | Pasivo | Auxiliar |
| `IVA SOPORTADO` | `ACTIVO CORRIENTE` | Activo | Auxiliar |

Reglas:

- Misma mecánica que cuentas 58–62: nombre en `accounts_names`, `Editable = false`.
- Subir `AccountCount`.
- Compañías **nuevas** lo reciben al crear con plan “POR DEFECTO”.
- Compañías **viejas:** un `EnsurePurchaseAccounts(companyId)` que cree las dos si faltan por nombre (copiar el ensure de POS). No reescribir planes custom.

No hace falta UI nueva: el plan de cuentas ya lista lo que hay. Si agregas un botón “Crear cuentas de compras” en Integración, que sea opcional y pequeño.

### Tests

- `Create()` devuelve count nuevo y contiene ambos nombres.
- Padres correctos (CxP bajo pasivo corto plazo; IVA soportado bajo activo corriente).
- Ensure es idempotente.

### Prompt para el agente

```text
Repo: este codebase Aries. Lee docs/TAREAS-COMPRAS.md, sección T2.

Agrega CUENTAS POR PAGAR e IVA SOPORTADO al plan de cuentas por defecto
(DefaultChartOfAccounts) y un EnsurePurchaseAccounts idempotente, igual que el
ensure de cuentas POS, disparado desde el API web. Actualiza tests de
DefaultChartOfAccounts y FinancialService si aplica, sin cambiar el comportamiento
de crear/editar cuentas que ya usa el escritorio (solo catálogo aditivo).
No toques WinForms. No crees tablas de compras.
PR: "feat(accounts): cuentas CxP e IVA soportado".
No mergees.
```

---

## T3 — Facturas de compra + stock

**Branch sugerido:** `feat/purchases-invoices`
**Depende de:** T1 mergeado (o mismo branch que T1)
**Archivos guía:** `Sale` / `SaleLine` / `PointOfSaleService.CreateSaleAsync` / `Sales.razor` / `PosTax`

### Modelos

`Purchase`

| Campo | Notas |
|---|---|
| CompanyId | |
| SupplierId | FK lógica a suppliers |
| DocumentNumber | Número de factura del proveedor (unique por compañía + proveedor) |
| PurchasedAt | Fecha del documento |
| PaymentMethod | Reusar enum POS: Cash / Card / Transfer. Más `OnAccount` (crédito → CxP) |
| PaymentReference | |
| Total, NetAmount, TaxAmount | Igual que Sale |
| Notes | |
| Status | `Draft` / `Confirmed` (solo Confirmed mueve stock) |

`PurchaseLine`

| Campo | Notas |
|---|---|
| PurchaseId | |
| ProductId | Obligatorio en v1 (solo mercadería, no gasto puro) |
| ProductName | Snapshot |
| Quantity | |
| UnitPrice | Precio bruto si PricesIncludeTax |
| LineTotal, NetAmount, TaxAmount | |
| TaxExempt | Del producto al momento de comprar |
| CostAmount | Neto de la línea (eso se copia a `product.Cost`) |

Migración **M022** (si T1 ya usó M021): `purchases`, `purchase_lines`. Columnas de auditoría estándar.

### Reglas de negocio

- Confirmar compra: transacción. Inserta header+lines, incrementa `product.Stock`, asigna `product.Cost = neto unitario`.
- IVA: `PosTax.SplitGross` por línea. Exento → tax 0.
- No confirmar si el periodo contable no está abierto **solo si** también se asienta en este PR. En T3 **no asentar**. El periodo no bloquea el registro operativo (igual que vender con caja abierta sin postear).
- No editar líneas de una Confirmada. Anular = soft delete + revertir stock y no tocar costo (documentar; si es complejo, v1 sin anulación, solo Active).
- `OnAccount` en el enum: agregar valor al `PaymentMethod` existente **o** crear `PurchasePaymentMethod` para no romper POS. Preferible enum nuevo `PurchaseSettlement { Cash, Card, Transfer, OnAccount }` para no mezclar venta y compra.

### UI

- `/purchases/{CompanyId}/new` — elegir proveedor, número de factura, fecha, modo de pago; agregar productos por barcode/búsqueda (reusar el buscador de Sales).
- `/purchases/{CompanyId}` — listado con total, proveedor, fecha, estado.
- Nav: “Compras” y “Nueva compra”.

### Tests

- Confirmar compra de 2 unidades sube stock en 2 y deja Cost = neto unitario.
- Línea exenta: TaxAmount 0, Net = Total.
- Línea gravada 13% con precio bruto 11300 → net 10000, tax 1300 (usar `PosTax.SplitGross`).
- Duplicate (company, supplier, document number) se rechaza.
- GetAll filtra por CompanyId.

### Prompt para el agente

```text
Repo: este codebase Aries. Lee docs/TAREAS-COMPRAS.md, sección T3.
Asume que T1 (proveedores) ya está en el branch o en main.

Implementa facturas de compra (Purchase + PurchaseLine): migración, repo,
PurchasingService.ConfirmPurchase, API, páginas Blazor (lista + alta), nav.
Copia el flujo de Sales.razor (web), no del escritorio.
Al confirmar: sube stock y actualiza product.Cost al último costo neto.
IVA 13% con PosTax.SplitGross. No genere asientos contables (eso es T4).
No toques WinForms ni cambies cómo el POS web vende.
Tests xUnit con FakeUnitOfWork cubriendo stock, IVA y duplicados.
PR: "feat(purchases): facturas de compra y recepción de inventario".
No mergees.
```

---

## T4 — Posteo compras → asientos

**Branch sugerido:** `feat/purchases-posting`
**Depende de:** T2 + T3
**Archivos guía:** `PosAccountMap`, `PosAccountingService`, `IntegrationEndpoints`, `Pages/Integration/AccountMap.razor`, `PosAccounting.razor`

Simétrico a POS:

`PurchaseAccountMap` por compañía:

- InventoryAccountId (default `INVENTARIOS`)
- TaxAccountId (default `IVA SOPORTADO`)
- PayableAccountId (default `CUENTAS POR PAGAR`)
- CashAccountId / CardAccountId / TransferAccountId (reusar nombres de POS)
- TaxRate, PricesIncludeTax

`PurchasePosting`: una compra Confirmada → un asiento. Unique `purchase_id`. Hash de totales para conciliar.

Asiento:

- **Debe** Inventario = neto
- **Debe** IVA soportado = tax (omitir si 0)
- **Haber** CxP si `OnAccount`; si no, la cuenta del medio de pago = total

UI: `/integration/{CompanyId}/purchase-accounting` (lista de compras sin postear, preview, post) y extender mapeo (página nueva o sección en Account Map).

No postear dos veces. Periodo abierto obligatorio al postear (copiar la validación de POS).

### Prompt para el agente

```text
Repo: este codebase Aries. Lee docs/TAREAS-COMPRAS.md, sección T4.

Implementa mapeo de cuentas de compras y posteo de facturas confirmadas a asientos,
clonando PosAccountingService y las páginas Blazor de Integración (no WinForms).
Dr Inventario + Dr IVA SOPORTADO, Cr CxP o caja. Una compra = un asiento.
Reusa FinancialService solo para crear el asiento; no cambies el editor de asientos
del escritorio ni el de la web. Tests de preview/post/idempotencia.
No toques WinForms. PR: "feat(purchases): posteo de compras a asientos".
No mergees.
```

---

## T5 — Pagos a proveedores

**Branch sugerido:** `feat/purchases-payments`
**Depende de:** T3 + T4
**Archivos guía:** posteo T4, enum de medios de pago

`SupplierPayment`: compañía, proveedor, purchaseId opcional (pago a una factura), monto, fecha, medio (Cash/Card/Transfer), referencia.

- Solo facturas `OnAccount` pendientes.
- Posteo: Dr CxP / Cr Caja|Bancos|Tarjeta.
- UI: desde el detalle de la compra, “Registrar pago”.
- v1: pago total, no parciales (si quieres parciales, un campo `PaidAmount` en Purchase).

### Prompt para el agente

```text
Repo: este codebase Aries. Lee docs/TAREAS-COMPRAS.md, sección T5.

Implementa pagos a proveedores sobre facturas a crédito ya posteadas.
Pago total v1. Asiento Dr CxP Cr medio de pago. UI Blazor en el detalle de la compra.
Tests de saldo y de rechazo si ya está pagada.
No toques WinForms. PR: "feat(purchases): pagos a proveedores".
No mergees.
```

---

## 5. Orden recomendado para esta noche

| Agente | Tarea | Puede empezar ya |
|---|---|---|
| A | T1 Proveedores | Sí |
| B | T2 Cuentas CxP + IVA soportado | Sí |
| — | T3–T5 | Mañana, cuando T1/T2 estén en el branch base |

Si usas **Cloud Agents**, commitea y pushea este archivo (y el WIP local) antes de lanzarlos: clonan el remoto, no tu working tree.

Si usas agentes **locales** en este workspace, pueden leer el doc sin push.

---

## 6. Criterio de “listo para revisar mañana”

Cada PR debe tener:

- [ ] Migración (si aplica) y `ExpectedSchema` al día
- [ ] Tests del servicio en verde
- [ ] Página **Blazor** usable con una compañía seleccionada
- [ ] Nav web visible (grupo Compras / Integración)
- [ ] Ningún asiento generado en T1/T2/T3
- [ ] Diff sin archivos de WinForms / `Capa*` / `src/desktop`
- [ ] Descripción del PR con cómo probar a mano en la **web** (login → compañía → ruta)

Cómo probar T1 a mano: login, elegir compañía, Compras → Proveedores, alta con cédula jurídica, recargar, editar, eliminar, confirmar que desaparece de la lista.
)
