# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copiar csproj y restaurar si quieres aprovechar caches (opcional)
# COPY Rifas.Worker/*.csproj ./Rifas.Worker/
# RUN dotnet restore "./Rifas.Worker/Rifas.Worker.csproj"

COPY . .
RUN dotnet publish "Rifas.Worker.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Opcional: exponer puerto si necesitas métricas/health (no obligatorio)
# EXPOSE 80

ENTRYPOINT ["dotnet", "Rifas.Worker.dll"]