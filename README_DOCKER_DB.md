# Base de datos con Docker (PostgreSQL)

Esta guía te ayuda a levantar PostgreSQL y pgAdmin con Docker y a ejecutar las migraciones de EF Core.

## Requisitos

- Windows con Docker Desktop instalado y en ejecución
- PowerShell (se asume `pwsh`)
- .NET SDK 8 o superior

## 1) Configurar credenciales

Opcionalmente crea un archivo `.env` en la raíz del repo (misma carpeta que `docker-compose.yml`) basado en `.env.example`:

```
POSTGRES_PASSWORD=postgres
```

Si no creas `.env`, el compose usa `postgres` por defecto.

## 2) Levantar servicios Docker

En PowerShell, desde la carpeta del proyecto:

```
docker compose up -d
```

Esto crea:

- Postgres en localhost:5432 (DB: `SismosDB`, user: `postgres`, pass: `postgres`)
- pgAdmin en http://localhost:5050 (usuario: `admin@local`, pass: `admin`)

La data se persiste en `./postgres-data` y `./pgadmin-data`.

## 3) Aplicar migraciones EF Core

Una vez que la base de datos esté healthy, ejecuta:

```
dotnet ef database update
```

El proyecto ya apunta a la cadena de conexión:

```
Host=localhost;Database=SismosDB;Username=postgres;Password=postgres
```

Si prefieres variables de entorno, puedes exportarlas antes de correr la app o migrations:

```
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=SismosDB;Username=postgres;Password=$env:POSTGRES_PASSWORD"
dotnet ef database update
Remove-Item Env:\ConnectionStrings__DefaultConnection
```

## 4) Abrir pgAdmin (opcional)

1. Ir a http://localhost:5050
2. Login: `admin@example.com` / `admin`
3. El servidor **SismosDB Local** ya está preconfigurado automáticamente; aparecerá en el panel izquierdo
4. Si necesitas agregar otro servidor manualmente:
   - Add New Server -> Connection:
     - Host: `host.docker.internal` (o `localhost`)
     - Port: `5432`
     - Username: `postgres`
     - Password: `postgres`

**Nota**: La configuración del servidor se monta desde `pgadmin-servers.json` y las credenciales desde `pgadmin-pgpass` (ignorado por git).

## 5) Troubleshooting

- Error de conexión 10061: asegúrate de que Docker Desktop está corriendo y `docker compose ps` muestra `db` healthy.
- Cambiaste la contraseña en `.env`: también actualiza `appsettings.json` o usa la variable `ConnectionStrings__DefaultConnection` para override sin tocar archivos.
- Puerto 5432 ocupado: cambia el mapeo en `docker-compose.yml` (por ejemplo `5433:5432`) y ajusta la cadena de conexión a `Host=localhost;Port=5433;...`.
- Timeouts al migrar: espera a que el healthcheck pase o corre `docker compose logs -f db` para ver el estado.
