# Archivo (no forma parte del producto)

## `AriesWebApi/` (git anidado)

El host HTTP canónico es [`src/hosts/Aries.WebAPI`](../src/hosts/Aries.WebAPI). La carpeta `AriesWebApi/` en el working tree (si existe) es un **clone aparte** de Azure DevOps:

`https://dev.azure.com/ariescontadorcr/AriesWebService/_git/AriesWebService`

No se fusiona con este repo. Contiene el API con Controllers, Blazor WASM y copias viejas de Core/Data. Sirve como cantera de código, no como solución de desarrollo.

No commitear `AriesWebApi/` aquí (está en `.gitignore`). Si hace falta el historial, clonar ese remote **fuera** de este árbol.

## Squirrel / `CapaPresentacion.exe`

El proyecto de escritorio vive en `src/desktop/Aries.Desktop`, pero el binario de producción sigue llamándose **`CapaPresentacion.exe`** (`AssemblyName` congelado). Cambiar el nombre del exe rompe el feed S3 de actualizaciones. Eso es un corte de versión coordinado (feed nuevo o major), no parte de la reorganización de carpetas.
