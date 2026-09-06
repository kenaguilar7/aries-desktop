# Contexto: raíz del repo (Core / Data / Services + este host).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY AriesContador.Core/AriesContador.Core.csproj AriesContador.Core/
COPY AriesContador.Data/AriesContador.Data.csproj AriesContador.Data/
COPY AriesContador.Services/AriesContador.Services.csproj AriesContador.Services/
COPY Aries.WebAPI/Aries.WebAPI.csproj Aries.WebAPI/

RUN dotnet restore Aries.WebAPI/Aries.WebAPI.csproj --configfile nuget.config

COPY AriesContador.Core/ AriesContador.Core/
COPY AriesContador.Data/ AriesContador.Data/
COPY AriesContador.Services/ AriesContador.Services/
COPY Aries.WebAPI/ Aries.WebAPI/

RUN dotnet publish Aries.WebAPI/Aries.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Aries.WebAPI.dll"]
