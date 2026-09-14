# Revisión diaria de mejora (web Blazor)

## Tú solo pegas esto

No elijas el área, no corras los `dotnet build`, no copies las tablas de lunes/martes. Eso lo hace el agente.

1. Abre un **Cloud Agent** (o Agents Window, modo Cloud).
2. Pega el bloque de abajo **tal cual**.
3. Espera el PR. Mañana lo revisas. **No mergees** desde el agente.

```text
Lee docs/TAREAS-REVISION-DIARIA.md (sección "Playbook del agente") y síguelo.

Haz la revisión diaria de la app web Blazor: un PR pequeño de mejora, que compile.
No implementes features nuevas. No toques WinForms ni src/desktop ni Capa*.
No hagas merge.
```

Si lo dejas en una **Automation** diaria: el mismo texto va en el campo de instrucciones. El resto de este archivo no se pega; el agente ya lo lee del repo.

---

## Por qué el documento habla de otras cosas

Hay dos audiencias en el mismo archivo:

| Quién | Qué hace con este doc |
|---|---|
| **Tú** | Pegas el recuadro de arriba. Nada más. |
| **El agente** | Abre este markdown y usa el playbook: qué módulo tocar hoy, qué tipo de arreglo buscar, cómo buildear, cómo titular el PR. |

Las tablas de lunes–domingo, la “lente” de la semana, los comandos `dotnet` y el formato del PR **no son un checklist tuyo**. Están escritas para que el agente no revise al azar ni abra un PR que no compila.

[`TAREAS-COMPRAS.md`](TAREAS-COMPRAS.md) es otro trabajo (features nuevas). Este archivo es solo higiene del código que ya existe.

---

## Playbook del agente

*(A partir de aquí: instrucciones para el agente. El humano no las ejecuta.)*

**Superficie:** `src/web/Aries.Contabilidad` + `src/hosts/Aries.WebAPI` + capas compartidas solo si el cambio es aditivo o un arreglo local.
**Prohibido:** `src/desktop/**`, `Capa*`, cambiar funciones del escritorio.
**Salida:** un PR que compila. Si no hay nada serio, no abrir PR.

### Orden (no aleatorio)

Recorrer la app como la usa un contador:

login → compañía → plan y periodos → asientos → reportes → POS → POS a contabilidad → cáscara (nav, dead code)

Dos ruedas:

1. **Área** = día de la semana en `America/Costa_Rica`.
2. **Lente** = semana del mes (qué tipo de defecto buscar en esa área).

Si el área del día está limpia, pasar al área del día siguiente. Máximo dos áreas. No un tercer módulo.

| Día | Área | Dónde mirar |
|---|---|---|
| Lunes | Auth, compañía, dashboard | `Pages/Auth`, `Pages/Companies`, `Pages/Dashboard`, `Layout/CompanySwitcher` |
| Martes | Plan de cuentas y periodos | `Pages/Account`, `Components/Accounts`, `Pages/Periods`, endpoints de account/period |
| Miércoles | Asientos web | `Pages/JournalEntry/Create.razor`, `Components/JournalEntry/*`, endpoints de asiento/línea |
| Jueves | Reportes y restore | `Pages/Reports/*`, `Pages/Restore` |
| Viernes | POS operativo | `Pages/Pos/*`, `PosService`, `PosEndpoints`, `PointOfSaleService`, `PosTax` |
| Sábado | Integración POS → asiento | `Pages/Integration/*`, `PosAccountingService` |
| Domingo | Cáscara y deuda | `Layout/*`, `wwwroot/css`, `ComingSoon`, páginas JE sin uso, ambos `Program.cs` |

Cuando exista `Pages` de compras, insertarlas entre viernes y sábado. Hasta entonces, ignorarlas aquí.

| Días del mes | Lente | Buscar |
|---|---|---|
| 1–7 | Corrección | `CompanyId`, dinero, IVA 13%, periodo cerrado, doble posteo, stock |
| 8–14 | Pruebas | Huecos en `AriesContador.Tests` / `Aries.WebAPI.Tests` |
| 15–21 | UI web | `page-title`, alerts, empty state, español, nav |
| 22–28 | Deuda | Código muerto, clientes HTTP duplicados, TODOs viejos |
| 29–31 | Corrección | Igual que 1–7 |

### Qué sí / qué no

Sí: un bug o invariante; un test que lo fije; alinear una página al vecino; borrar código muerto demostrable; extraer duplicado local. Un tema, ≤ ~12 archivos.

No: features nuevas; rewrite de una capa; cambiar firmas que usa WinForms; renames masivos; NuGet “porque sí”; varios temas en el mismo PR; PR solo de estilo.

### Pasos

1. Calcular área y lente.
2. Mirar PRs recientes `chore(web-review):` para no repetir.
3. Inspeccionar esa área. Elegir el hallazgo de más riesgo.
4. Implementar el mínimo. Tests si es regla de negocio.
5. Correr el build de abajo. Si falla, arreglar o no abrir PR.
6. Abrir PR. No mergear.
7. Sin hallazgo serio: no PR; reportar área, lente y “sin hallazgos”.

### Build (obligatorio antes del PR)

```bash
dotnet build src/domain/AriesContador.Core/AriesContador.Core.csproj --configuration Release --nologo
dotnet build src/application/AriesContador.Services/AriesContador.Services.csproj --configuration Release --nologo
dotnet build src/hosts/Aries.WebAPI/Aries.WebAPI.csproj --configuration Release --nologo
dotnet build src/web/Aries.Contabilidad/Aries.Contabilidad.csproj --configuration Release --nologo
dotnet test tests/AriesContador.Tests/AriesContador.Tests.csproj --configuration Release --nologo --no-build
dotnet test tests/Aries.WebAPI.Tests/Aries.WebAPI.Tests.csproj --configuration Release --nologo
```

Si el diff toca migraciones o `ExpectedSchema`:

```bash
dotnet test tests/Aries.Data.Tests/Aries.Data.Tests.csproj --configuration Release --nologo --filter FullyQualifiedName~SchemaMigrationsCatalogTests
```

Blazor y WebAPI tienen que compilar en Release. No tocar el exe; el CI de `Aries.sln` sigue verde por eso. No “arreglar” tests rotos de otro módulo.

### Forma del PR

- Branch: `chore/web-review-YYYYMMDD-area`
- Título: `chore(web-review): {área} — {qué y por qué}`
- Cuerpo: área/lente, qué, por qué, cómo probar en la web, comandos de build y que pasaron.
- No mergear, no aprobar, no tocar otros PRs.

No lanzar dos revisiones el mismo día: se pisan los archivos.
