# Contexto: raíz del repo. WASM estático servido por nginx.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY src/domain/AriesContador.Core/AriesContador.Core.csproj src/domain/AriesContador.Core/
COPY src/web/Aries.Contabilidad/Aries.Contabilidad.csproj src/web/Aries.Contabilidad/
RUN dotnet restore src/web/Aries.Contabilidad/Aries.Contabilidad.csproj --configfile nuget.config

COPY src/domain/AriesContador.Core/ src/domain/AriesContador.Core/
COPY src/web/Aries.Contabilidad/ src/web/Aries.Contabilidad/

ARG ARIES_API_BASE_URL=http://localhost:5088/
ENV ARIES_API_BASE_URL=${ARIES_API_BASE_URL}

RUN dotnet publish src/web/Aries.Contabilidad/Aries.Contabilidad.csproj -c Release -o /app/publish --no-restore

RUN set -eux; \
    url="${ARIES_API_BASE_URL%/}/"; \
    printf '{\n  "ApiSettings": {\n    "BaseUrl": "%s",\n    "Timeout": 30\n  }\n}\n' "$url" \
      > /app/publish/wwwroot/appsettings.json; \
    cp /app/publish/wwwroot/appsettings.json /app/publish/wwwroot/appsettings.Production.json; \
    cat /app/publish/wwwroot/appsettings.json

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY src/web/Aries.Contabilidad/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
