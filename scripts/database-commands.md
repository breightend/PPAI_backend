# 🗃️ Comandos Útiles de Base de Datos - PPAI Backend

## 🔄 Comandos Frecuentes

### Resetear base de datos completa:

```powershell
# 1. Parar aplicación (Ctrl+C si está corriendo)

# 2. Borrar base de datos
dotnet ef database drop --force

# 3. Aplicar migraciones desde cero
dotnet ef database update

# 4. Iniciar aplicación
dotnet run

# 5. Generar datos (en otra terminal)
curl -X POST http://localhost:5199/seed-database
```

### Crear nueva migración:

```powershell
# 1. Hacer cambios en tus modelos (entidades)

# 2. Crear migración
dotnet ef migrations add NombreDeTuMigracion

# 3. Aplicar migración
dotnet ef database update

# 4. Regenerar datos de prueba
curl -X POST http://localhost:5199/seed-database
```

### Ver estado de migraciones:

```powershell
# Listar todas las migraciones
dotnet ef migrations list

# Ver migración pendiente
dotnet ef migrations list --no-build

# Ver script SQL de migración
dotnet ef migrations script
```

### Generar script SQL:

```powershell
# Script completo de base de datos
dotnet ef migrations script > database-complete.sql

# Script desde migración específica
dotnet ef migrations script MigracionInicial > migration-from-initial.sql

# Script entre dos migraciones
dotnet ef migrations script MigracionA MigracionB > migration-between.sql
```

## 🔧 Troubleshooting Común

### ❌ "La base de datos no está actualizada"

```powershell
# Solución:
dotnet ef database update
```

### ❌ "Los datos no coinciden con mis compañeros"

```powershell
# Solución:
curl -X POST http://localhost:5199/seed-database

# Si no funciona, resetear completo:
dotnet ef database drop --force
dotnet ef database update
curl -X POST http://localhost:5199/seed-database
```

### ❌ "Error de conexión a la base de datos"

```powershell
# Verificar que Docker esté corriendo:
docker ps

# Si no hay contenedores, iniciar:
docker-compose up -d

# Ver logs de la base de datos:
docker-compose logs db

# Reiniciar contenedores:
docker-compose restart
```

### ❌ "Cannot connect to PostgreSQL server"

```powershell
# 1. Verificar estado del contenedor
docker-compose ps

# 2. Ver logs de PostgreSQL
docker-compose logs db

# 3. Verificar puertos
netstat -an | findstr 5432

# 4. Reiniciar servicio
docker-compose restart db
```

### ❌ "Migration already exists"

```powershell
# Ver migraciones existentes:
dotnet ef migrations list

# Remover última migración:
dotnet ef migrations remove

# Crear nueva migración:
dotnet ef migrations add NuevoNombre
```

### ❌ "Endpoint '/seed-database' not found"

```powershell
# Verificar que la aplicación esté corriendo:
curl http://localhost:5199/health

# Si no responde, iniciar aplicación:
dotnet run

# Verificar endpoint correcto:
curl -X POST http://localhost:5199/seed-database
```

## 📊 Comandos de Monitoreo

### Ver estadísticas de la base de datos:

```powershell
curl http://localhost:5199/database-stats
```

### Verificar salud de la aplicación:

```powershell
curl http://localhost:5199/health
```

### Ver logs de la aplicación:

```powershell
# Si usaste el script setup-dev.ps1
Get-Job | Receive-Job

# Para ver logs en tiempo real durante desarrollo:
dotnet run --verbosity normal
```

### Conectar a PostgreSQL directamente:

```powershell
# Usando Docker:
docker exec -it sismos-postgres psql -U postgres -d SismosDB

# Comandos SQL útiles dentro de psql:
# \dt          - Ver todas las tablas
# \d+ tabla    - Ver estructura de una tabla
# SELECT count(*) FROM "Empleados";
# \q           - Salir
```

## 🚀 Comandos de Automatización

### Setup completo para nuevo desarrollador:

```powershell
.\scripts\setup-dev.ps1
```

### Setup rápido (sin Docker):

```powershell
.\scripts\setup-dev.ps1 -SkipDocker
```

### Setup sin datos (solo estructura):

```powershell
.\scripts\setup-dev.ps1 -SkipSeed
```

### Limpiar todo y empezar desde cero:

```powershell
.\scripts\setup-dev.ps1 -Clean
```

## 📦 Comandos de Exportación/Importación

### Exportar datos actuales:

```powershell
# Crear dump de la base de datos
docker exec sismos-postgres pg_dump -U postgres -d SismosDB > backup-$(Get-Date -Format "yyyyMMdd-HHmmss").sql

# Solo estructura (sin datos)
docker exec sismos-postgres pg_dump -U postgres -d SismosDB --schema-only > schema.sql

# Solo datos (sin estructura)
docker exec sismos-postgres pg_dump -U postgres -d SismosDB --data-only > data.sql
```

### Importar datos:

```powershell
# Importar desde dump
docker exec -i sismos-postgres psql -U postgres -d SismosDB < backup.sql

# Limpiar e importar
dotnet ef database drop --force
dotnet ef database update
docker exec -i sismos-postgres psql -U postgres -d SismosDB < backup.sql
```

## 🔄 Flujo de Trabajo en Equipo

### Cuando haces cambios (flujo personal):

```powershell
# 1. Hacer cambios en modelos
# 2. Crear migración
dotnet ef migrations add MiCambio

# 3. Aplicar localmente
dotnet ef database update

# 4. Probar con datos frescos
curl -X POST http://localhost:5199/seed-database

# 5. Commit y push
git add .
git commit -m "feat: agregar nueva funcionalidad"
git push origin main
```

### Cuando un compañero hace push (flujo del equipo):

```powershell
# 1. Pull del código
git pull origin main

# 2. Aplicar nuevas migraciones
dotnet ef database update

# 3. Regenerar datos consistentes
curl -X POST http://localhost:5199/seed-database

# ¡Listo! Todos tienen los mismos datos
```

### Setup para nuevo miembro del equipo:

```powershell
# 1. Clonar repositorio
git clone [url-del-repo]
cd PPAI_backend

# 2. Ejecutar setup automático
.\scripts\setup-dev.ps1

# ¡Listo! Entorno completamente configurado
```

## 💡 Tips y Mejores Prácticas

### Para desarrollo:

- Usa `DatabaseSeederConfig.Testing` para generar pocos datos
- Ejecuta `curl -X POST http://localhost:5199/seed-database` después de cada cambio importante
- Siempre haz `git pull` antes de crear nuevas migraciones

### Para testing:

- Los datos generados son consistentes (mismo seed = mismos datos)
- Usa emails reales en `DatabaseSeederConfig.cs` para probar notificaciones
- Los usuarios tienen contraseña `123456` por defecto

### Para producción:

- Cambia `LimpiarDatosExistentes = false` en producción
- Usa `DatabaseSeederConfig.Production` para más datos
- Siempre haz backup antes de aplicar migraciones

## 🆘 Ayuda Rápida

### Si nada funciona:

```powershell
# Setup completamente limpio:
docker-compose down -v
dotnet clean
.\scripts\setup-dev.ps1 -Clean
```

### Contactos:

- 📧 Problemas con base de datos: [tu-email]
- 🐛 Bugs del seeder: [tu-email]
- 📚 Documentación: Ver archivos README\_\*.md

---

**💡 Tip**: Guarda este archivo en tus marcadores. Tendrás todos los comandos que necesitas para el día a día.
