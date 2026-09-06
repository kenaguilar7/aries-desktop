# Aries.WebAPI (fase 5)

Host HTTP **Minimal APIs** sobre `AriesContador.Services` canónico. Sin `Controllers/`, sin `Aries.WebServices` paralelo, sin filtro `Id <= 57` al copiar el plan.

## Ambientes

Ver [`scripts/local/README.md`](../scripts/local/README.md). Hay dos perfiles en `Properties/launchSettings.json`. El default es Local.

| Perfil | `ASPNETCORE_ENVIRONMENT` | MySQL |
|---|---|---|
| **Aries.WebAPI (Local)** | Development | `127.0.0.1:3307` (`appsettings.Development.json`) |
| **Aries.WebAPI (Production)** | Production | RDS (`appsettings.Production.json`) |

```powershell
.\scripts\local\start-local.ps1
dotnet run --project Aries.WebAPI
```

Swagger: `http://localhost:5088/`. F5 del escritorio **no** arranca este host: login y maestros van in-process a MySQL. Para los dos a la vez, en Visual Studio usa **Escritorio + API (Local)**.

Debe loguear `Ambiente Development: MySQL 127.0.0.1:3307 / aries` y `Aries.WebAPI escuchando en http://localhost:5088`.

Contra RDS (cuidado: es producción):

```powershell
dotnet run --project Aries.WebAPI --launch-profile "Aries.WebAPI (Production)"
```

Override local: copia `appsettings.Local.json.example` → `appsettings.Local.json` (gitignored). No se carga si el ambiente es Production.

El Debug del escritorio usa el mismo MySQL local y `HttpBaseUrl=http://localhost:5088/`. Release usa `App.Production.config` (RDS + Elastic Beanstalk).

## Contrato que el exe ya conoce

| Método | Ruta |
|---|---|
| POST | `/auth/login` (anónimo) |
| GET | `/company/getAll` |
| GET | `/company/BuildCode` |
| POST | `/company/Create` |
| DELETE | `/company/delete/{code}` |

El resto (cuentas, periodos, asientos) replica las rutas de `HttpFinancialService`. JSON en PascalCase (Newtonsoft del WinForms). `UserId` sale del claim JWT, no de un `1` hardcodeado.

Login in-process del exe **no cambia**: sigue `IAdministrationService`. El API emite JWT real en `WebToken.Token`.

## Decisiones de fase 5 que no se hacen aquí

- **Git anidado `AriesWebApi/`:** se deja parqueado. El host nuevo vive en la raíz (`Aries.WebAPI/`).
- **Mover carpetas a `src/`:** no.
- **net8.0-windows:** no. El exe de producción sigue en .NET Framework 4.8.
- **Blazor:** parqueado; si se retoma, debe pegarle a estas mismas rutas.
- **Hash de passwords:** posterior. Login busca un usuario por nombre y compara texto plano.
