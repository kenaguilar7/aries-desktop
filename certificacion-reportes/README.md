# Certificación de reportes 1.1.15 vs 1.2.0

Orden correcto: **primero el estable, después M014, después los mismos Excel en Debug 1.2.0**. No al revés. Si generas 1.2.0 antes de M014, auxiliares / situación / maestro salen con Ingreso y Costo de venta cruzados.

Misma base MySQL, misma compañía, mismos meses. Si las bases no coinciden, la comparación no vale.

```text
1. Completar SESION.md
2. Generar Excel en el exe 1.1.15  →  01-estable-1.1.15\
3. Aplicar M014 en 1.2.0 (Sistema → Actualizaciones)
4. Generar los mismos Excel en Debug 1.2.0  →  02-debug-1.2.0\
5. Avisar en el chat para validar números
```

---

## 0. Sesión

Abre [`SESION.md`](SESION.md) y rellena compañía, meses y cuentas de movimientos **antes** de generar nada.

**Misma base (crítico en esta laptop):**

| Exe | Cómo saber a qué MySQL está pegado |
|---|---|
| 1.2.0 Debug (F5) | Título del menú: `v.1.2.0 [PROD] aries` o `[Local] aries`. `local-db.json` `"use"` debe ser el mismo mundo que el estable. |
| 1.1.15 Squirrel de esta PC | El `exe.config` instalado puede apuntar a `aries-test34`, **no** a prod. No uses ese Squirrel si Debug está en `aries`. Usa una PC cliente real, o el mismo connection string a ojo (server + Database=). |

Si Debug está en Docker `:3307` y el estable en RDS, los Excel no son comparables.

No crear ni editar cuentas ni asientos entre el paso 2 y el 4.

---

## 1. Excel en el estable (1.1.15) — todavía **sin** M014

Abre el exe FileVersion **1.1.15**. Login, elige la compañía de `SESION.md`.

Guarda cada archivo **con el nombre exacto** en:

`C:\Aries\certificacion-reportes\01-estable-1.1.15\`

| Archivo | Menú | Qué seleccionar |
|---|---|---|
| `01-comprobacion.xlsx` | Reportes → Balance de Comprobación | Mes inicio y fin de SESION |
| `02-estado-resultado.xlsx` | Reportes → Estado de resultado integral | Mismo rango |
| `03-asientos.xlsx` | Contable → Asientos Contables → botón de reporte | Mismo rango |
| `04-auxiliares.xlsx` | Reportes → Balance de Auxiliares | Mismo rango |
| `05-balance-situacion.xlsx` | Reportes → Balance de Situación | Mismo rango |
| `06-movimientos-auxiliar.xlsx` | Reportes → Movimientos de cuenta **o** Maestro de Cuentas → Movimientos | La cuenta auxiliar de SESION |
| `06b-movimientos-titulo.xlsx` | Igual | La cuenta título / mayor de SESION |
| `07-maestro-cuentas.xlsx` | Maestro de Cuentas → Ver Lista (Excel) | Marcar imprimir saldos; desde el mes más viejo hasta el mes fin de SESION |

Cierra el 1.1.15. No apliques M014 todavía.

---

## 2. Migración M014 (solo administrador)

1. F5 de `C:\Aries` (1.2.0). Mira el título: ambiente + base.
2. Login con usuario **administrador**.
3. Sistema → Actualizaciones.
4. En Base de datos debe aparecer pendiente `014_StandardizeReportContract`.
5. **Actualizar base de datos**. Confirma.
6. Si dice que el esquema ya está al día, anótalo en SESION (M014 ya se había aplicado).

El arranque **no** migra solo. Si no entras a Actualizaciones, 1.2.0 sigue con el CASE cruzado.

---

## 3. Excel en Debug 1.2.0 — **después** de M014

Misma compañía, mismos meses, mismas cuentas.

Guarda con **los mismos nombres** en:

`C:\Aries\certificacion-reportes\02-debug-1.2.0\`

---

## 4. Validación

Cuando las dos carpetas tengan los 8 `.xlsx`, escribe en el chat que ya están. Se comparan montos (no el diseño):

- Comprobación: totales de las seis columnas débito/crédito
- Estado de resultado: fila UTILIDAD/PERDIDA PERIODO
- Asientos: conteo de líneas y suma débito = crédito
- Auxiliares: Ingreso, Costo venta, Egreso
- Situación: TOTAL activo / pasivo / patrimonio / utilidad
- Movimientos: importes y mes en español
- Maestro: saldo actual

Cualquier diferencia en auxiliares, situación o maestro **bloquea** publicar 1.2.0 a S3.
