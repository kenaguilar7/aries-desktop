# Layout del repo Aries

Fuente de verdad de carpetas y soluciones **después** de la reorganización. Los planes históricos (`PLAN-MIGRACION.md`, `PLAN-MEJORAS.md`, `ARQUITECTURA.md`) describen el estado anterior; este archivo describe el disco actual.

## Árbol

```text
Aries/
├── Aries.sln              # única solución (VS 2026 / 18.x y CI)
├── Aries.slnLaunch        # perfiles F5: escritorio, API, ambos
├── Directory.Build.props
├── global.json            # SDK 8.0.x
├── nuget.config
├── docker-compose.yml
├── docs/                  # arquitectura, planes, modelos BD
├── scripts/               # local Docker + SQL por fase
├── src/
│   ├── domain/AriesContador.Core
│   ├── application/AriesContador.Services
│   ├── infrastructure/AriesContador.Data
│   ├── desktop/
│   │   ├── Aries.Desktop      # WinForms 4.8 → CapaPresentacion.exe
│   │   └── Aries.Reporting    # Excel / VerificaString (ex CapaEntidad)
│   └── hosts/Aries.WebAPI
├── tests/
├── packages/              # packages.config del exe 4.8
└── archive/               # puntero al git anidado, no el producto
```

Una sola solución (`Aries.sln`) para **Visual Studio 2026** (18.x). Las carpetas virtuales usan el GUID de carpeta de VS 18 (`2150E333-8FDC-42A3-9474-1A3956D46DE8`); el GUID de VS 2022 no está registrado y VS las reporta como *No admitidos*.

En Solution Explorer no se llaman `src` ni `tests` (chocan con directorios físicos):

- `Source` → `domain` / `application` / `infrastructure` / `desktop` / `hosts`
- `Test projects`
- `Documentation` — los `.md` de `docs/`
- `Solution Items` — `Directory.Build.props`, `docker-compose.yml`, `global.json`, `nuget.config`, `README.md`

El `.sln` va en **CRLF**. [`.gitattributes`](../.gitattributes) lo fija para que Git no lo vuelva a LF.

Plataformas de solución: **Debug|Any CPU** y **Release|Any CPU** (igual que CI). Las configs x86 del csproj de escritorio se dejan en el proyecto; no se exponen en la solución.

Perfiles F5 (`Aries.slnLaunch`): Escritorio + API (Local), Solo escritorio (Local), Solo API (Local).

El escritorio y el API llaman los mismos `AriesContador.Services` **async** (`await` + `CancellationToken`). Data usa Dapper `QueryAsync`/`ExecuteAsync`; las transacciones (asiento + líneas, clone de plan de cuentas, cierre de periodo) abren con `OpenAsync`/`BeginTransactionAsync`. El exe de producción sigue siendo `CapaPresentacion.exe` (Squirrel). Detalle en [`archive/README.md`](../archive/README.md). Cómo abrir en Visual Studio: [`README.md`](../README.md).
