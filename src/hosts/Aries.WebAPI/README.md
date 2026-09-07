# Aries.WebAPI (fase 5)

Host HTTP **Minimal APIs** sobre `AriesContador.Services` canónico. Sin `Controllers/`, sin `Aries.WebServices` paralelo, sin filtro `Id <= 57` al copiar el plan.

## Ambientes

Ver [`scripts/local/README.md`](../../../scripts/local/README.md). El camino local es **Docker** (API + MySQL), no `dotnet run`.

```powershell
.\scripts\local\start-local.ps1
```

- Swagger: `http://localhost:5088/`
- Salud: `http://localhost:5088/health`
- Login de prueba: `POST /auth/login` con `{"UserId":"kenneth","Password":"96321"}`

El contenedor `aries_api_local` habla con MySQL en `host.docker.internal:3307` (`aries_mysql_local`). El escritorio Debug usa el mismo MySQL y `HttpBaseUrl=http://localhost:5088/`. Feed Squirrel del mismo host: `http://localhost:5088/updates/` (ver [`scripts/local/README.md`](../../../scripts/local/README.md)).

Para depurar el API **en el host** (no Docker): `docker compose stop api` y F5 en el perfil `Aries.WebAPI (Local)`. Contra RDS: perfil Production — no es el contenedor local.

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

- **Git anidado `AriesWebApi/`:** se deja parqueado. Ver [`archive/README.md`](../../../archive/README.md). El host canónico es este proyecto (`src/hosts/Aries.WebAPI`).
- **Layout `src/`:** ver [`docs/LAYOUT.md`](../../../docs/LAYOUT.md).
- **net8.0-windows:** no. El exe de producción sigue en .NET Framework 4.8.
- **Blazor:** parqueado; si se retoma, debe pegarle a estas mismas rutas.
- **Hash de passwords:** posterior. Login busca un usuario por nombre y compara texto plano.
