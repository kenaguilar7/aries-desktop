# Archivo (no forma parte del producto)

## `AriesWebApi/` (git anidado)

El host HTTP canónico es [`src/hosts/Aries.WebAPI`](../src/hosts/Aries.WebAPI). La UI web canónica es [`src/web/Aries.Contabilidad`](../src/web/Aries.Contabilidad).

La cantera `AriesWebApi/` (API con Controllers, Blazor WASM y copias de Core/Data) se eliminó del working tree. Sigue ignorada en `.gitignore`. El historial vive en Azure DevOps:

`https://dev.azure.com/ariescontadorcr/AriesWebService/_git/AriesWebService`

Si hace falta consultar ese código, clonar el remote **fuera** de este árbol. No fusionar historiales.

## Squirrel / `CapaPresentacion.exe`

El proyecto de escritorio vive en `src/desktop/Aries.Desktop`, pero el binario de producción sigue llamándose **`CapaPresentacion.exe`** (`AssemblyName` congelado). Cambiar el nombre del exe rompe el feed S3 de actualizaciones. Eso es un corte de versión coordinado (feed nuevo o major), no parte de la reorganización de carpetas.
