# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/contracts/ContractManager.Api/ContractManager.Api.csproj", "src/contracts/ContractManager.Api/"]
COPY ["src/shared/ContractManager.Shared.Api/ContractManager.Shared.Api.csproj", "src/shared/ContractManager.Shared.Api/"]
COPY ["src/shared/ContractManager.Shared.Application/ContractManager.Shared.Application.csproj", "src/shared/ContractManager.Shared.Application/"]
COPY ["src/shared/ContractManager.Shared.Core/ContractManager.Shared.Core.csproj", "src/shared/ContractManager.Shared.Core/"]
COPY ["src/shared/ContractManager.Shared.Data/ContractManager.Shared.Data.csproj", "src/shared/ContractManager.Shared.Data/"]
COPY ["src/shared/ContractManager.Shared.Sdk/ContractManager.Shared.Sdk.csproj", "src/shared/ContractManager.Shared.Sdk/"]
COPY ["src/contracts/ContractManager.Application/ContractManager.Application.csproj", "src/contracts/ContractManager.Application/"]
COPY ["src/contracts/ContractManager.Data/ContractManager.Data.csproj", "src/contracts/ContractManager.Data/"]
COPY ["src/contracts/ContractManager.Data.EF/ContractManager.Data.EF.csproj", "src/contracts/ContractManager.Data.EF/"]
RUN dotnet restore "./src/contracts/ContractManager.Api/ContractManager.Api.csproj"
COPY . .
WORKDIR "/src/src/contracts/ContractManager.Api"
RUN dotnet build "./ContractManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContractManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContractManager.Api.dll"]