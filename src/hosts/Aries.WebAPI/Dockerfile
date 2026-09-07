# Contexto: raíz del repo.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY src/domain/AriesContador.Core/AriesContador.Core.csproj src/domain/AriesContador.Core/
COPY src/infrastructure/AriesContador.Data/AriesContador.Data.csproj src/infrastructure/AriesContador.Data/
COPY src/application/AriesContador.Services/AriesContador.Services.csproj src/application/AriesContador.Services/
COPY src/hosts/Aries.WebAPI/Aries.WebAPI.csproj src/hosts/Aries.WebAPI/

RUN dotnet restore src/hosts/Aries.WebAPI/Aries.WebAPI.csproj --configfile nuget.config

COPY src/domain/AriesContador.Core/ src/domain/AriesContador.Core/
COPY src/infrastructure/AriesContador.Data/ src/infrastructure/AriesContador.Data/
COPY src/application/AriesContador.Services/ src/application/AriesContador.Services/
COPY src/hosts/Aries.WebAPI/ src/hosts/Aries.WebAPI/

RUN dotnet publish src/hosts/Aries.WebAPI/Aries.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Aries.WebAPI.dll"]
