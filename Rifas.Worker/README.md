# Rifas.Worker

Worker para generación de resultados de rifas.

Resumen
- Lee rifas activas (RaffleEntity).
- Genera `ResultsEntity` con `WinningNumber` según `level` y `TopNUmber`.
- Evita duplicados y, si corresponde, genera números no vendidos.

Configuración (appsettings.json)
- WorkerSchedules: array con horas en formato "HH:mm" (UTC).
- RunOnStart: boolean.
- GenerationOptions: { MaxAttempts }.

Despliegue (resumen)
- Build: `dotnet publish -c Release`
- Docker: `docker build -t rifas-worker:latest .`
- Ejecutar con variables de entorno y el connection string de la DB.
- Asegurar que `Rifas.Cliente` registre el DbContext y repositorios en DI; en `Program.cs` del worker llama a ese registration.

Notas
- Ajusta el ProjectReference en el csproj si la ruta de `Rifas.Cliente` difiere.
- Registra dependencias del proyecto cliente antes de ejecutar el worker.