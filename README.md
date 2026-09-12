# Aries Contador

Contabilidad de escritorio (.NET Framework 4.8) y API Minimal (`net8.0`) sobre el mismo dominio MySQL.

- Layout: [`docs/LAYOUT.md`](docs/LAYOUT.md)
- Arranque local / laptop QA (Docker MySQL `:3307` + API + feed `/updates/`): [`scripts/local/README.md`](scripts/local/README.md)
- Arquitectura histórica: [`docs/ARQUITECTURA.md`](docs/ARQUITECTURA.md)

Solución canónica: **`Aries.sln`**. Proyecto de escritorio: `src/desktop/Aries.Desktop` (output `CapaPresentacion.exe`).

## Abrir en Visual Studio 2026

Este repo se abre con **Visual Studio Community 2026** (18.x), no con VS 2022. Las carpetas de solución usan el GUID de VS 18; el de VS 2022 las marca *No admitidos*.

1. Workloads: **.NET desktop development** (targeting pack 4.8) y **ASP.NET and web development** (.NET 8 SDK).
2. Cerrar la solución, borrar `C:\Aries\.vs` y abrir **`Aries.sln`**. El SDK lo fija [`global.json`](global.json) (`8.0.x`).
3. Restore dual (el exe 4.8 usa `packages.config`; Core/Data/Services/API usan PackageReference):
   - `nuget restore Aries.sln`
   - luego Build, o `msbuild Aries.sln /restore /p:Configuration=Debug /p:Platform="Any CPU"`
   Sin `packages/` restaurado el diseñador WinForms falla (HintPaths y Squirrel).
4. En el combo de inicio elegir un perfil (`Aries.slnLaunch`):
   - **Solo escritorio (Local)** — F5 arranca `CapaPresentacion.exe`
   - **Solo API (Local)**
   - **Escritorio + API (Local)**
5. MySQL local en `:3307`: [`scripts/local/start-local.ps1`](scripts/local/README.md).

### Cambiar de base en F5 (sin variables de entorno)

GitHub Actions usa secrets. En tu PC: `src/desktop/Aries.Desktop/local-db.json` (gitignored; el primer Debug lo crea desde [`scripts/local/local-db.json.example`](scripts/local/local-db.json.example)).

1. Abre `local-db.json` (junto a `app.config` en disco, no en el proyecto).
2. Rellena `Server` y `Password` del bloque `aries-test`.
3. Pon `"use": "aries-test"` o `"use": "docker"` y F5.
4. El título del menú muestra `[AriesTest] aries-test` o `[Local] aries`.

## CI / CD

PRs y pushes a `master` / `main` / `dev` corren [`.github/workflows/ci.yml`](.github/workflows/ci.yml):

- **Windows:** `nuget restore` + MSBuild Release, tests de Core / Desktop / WebAPI, `Verify-NoSecrets` y `Verify-DesktopPublish`.
- **Ubuntu + MySQL 8:** `Aries.Data.Tests` (migraciones e insert de compañía) y otra pasada de `Aries.WebAPI.Tests`.

El pack de escritorio de **prueba** se dispara a mano en [`.github/workflows/cd-test.yml`](.github/workflows/cd-test.yml) (canal `updates-test`). El de **producción** es [`.github/workflows/cd-prod.yml`](.github/workflows/cd-prod.yml): tag `vX.Y.Z` igual a [`version.props`](version.props), hay que escribir `production` y marcar `publish_to_s3`. Runbook local: [`scripts/ops/Publish-Production.ps1`](scripts/ops/Publish-Production.ps1) (`-ConfirmProduction`). Detalle de ambientes: [`scripts/local/README.md`](scripts/local/README.md). Ciclo de vida: [`docs/ANALISIS-CICLO-VIDA.md`](docs/ANALISIS-CICLO-VIDA.md).
