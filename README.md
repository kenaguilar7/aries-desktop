# Aries Contador

Contabilidad de escritorio (.NET Framework 4.8) y API Minimal (`net8.0`) sobre el mismo dominio MySQL.

- Layout: [`docs/LAYOUT.md`](docs/LAYOUT.md)
- Arranque local (Docker MySQL `:3307` + API): [`scripts/local/README.md`](scripts/local/README.md)
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
