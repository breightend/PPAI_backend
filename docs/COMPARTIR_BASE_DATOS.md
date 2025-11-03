# 🚀 Cómo Compartir Cambios de Base de Datos con el Equipo

## 📋 Estrategia Recomendada: Migraciones + Seeder

### 🎯 Flujo de Trabajo Completo

#### 1. **Cuando hagas cambios en el modelo de datos:**

```bash
# 1. Crear una nueva migración
dotnet ef migrations add NombreDeTuCambio

# 2. Aplicar la migración a tu base de datos local
dotnet ef database update
```

#### 2. **Cuando generes/modifiques datos:**

```bash
# Ejecuta el seeder para poblar con datos consistentes
curl -X POST http://localhost:5199/seed-database
```

#### 3. **Antes de hacer commit:**

```bash
# Hacer commit de las migraciones Y del código del seeder
git add .
git commit -m "feat: agregar nueva entidad + datos de prueba"
git push origin main
```

#### 4. **Cuando un compañero haga pull:**

```bash
# 1. Pull del código
git pull origin main

# 2. Aplicar nuevas migraciones
dotnet ef database update

# 3. Regenerar datos de prueba
curl -X POST http://localhost:5199/seed-database
```

---

## 🛠️ Configuraciones Importantes

### 1. **Configurar el Seeder para Consistencia**

Edita `services/DatabaseSeederConfig.cs`:

```csharp
public class DatabaseSeederConfig
{
    // ✅ IMPORTANTE: Usar seed fijo para datos consistentes
    public DatabaseSeederConfig()
    {
        // Seed fijo para que todos generen los mismos datos
        Randomizer.Seed = new Random(12345); // Número fijo para consistencia
    }

    // Configuración estándar para el equipo
    public static DatabaseSeederConfig TeamShared => new()
    {
        NumeroEmpleados = 20,
        NumeroOrdenes = 30,
        NumeroEstaciones = 15,
        LimpiarDatosExistentes = true, // Siempre empezar limpio
        ContraseñaDefecto = "123456", // Password común para testing
        IdiomaFaker = "es"
    };
}
```

### 2. **Script de Setup para Nuevos Desarrolladores**

Crea `scripts/setup-dev.ps1`:

```powershell
# Script de configuración para nuevos desarrolladores
Write-Host "🚀 Configurando entorno de desarrollo..." -ForegroundColor Green

# 1. Verificar Docker
Write-Host "📦 Verificando Docker..." -ForegroundColor Yellow
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker no está instalado" -ForegroundColor Red
    exit 1
}

# 2. Levantar contenedores
Write-Host "🐳 Iniciando contenedores..." -ForegroundColor Yellow
docker-compose up -d

# 3. Esperar a que la base de datos esté lista
Write-Host "⏳ Esperando a que PostgreSQL esté listo..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# 4. Aplicar migraciones
Write-Host "🔄 Aplicando migraciones..." -ForegroundColor Yellow
dotnet ef database update

# 5. Iniciar aplicación en background
Write-Host "🚀 Iniciando aplicación..." -ForegroundColor Yellow
Start-Process -NoNewWindow dotnet -ArgumentList "run"
Start-Sleep -Seconds 5

# 6. Generar datos de prueba
Write-Host "🌱 Generando datos de prueba..." -ForegroundColor Yellow
try {
    Invoke-RestMethod -Uri "http://localhost:5199/seed-database" -Method POST
    Write-Host "✅ Datos generados correctamente" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Error generando datos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "🎉 Setup completado! Tu entorno está listo." -ForegroundColor Green
Write-Host "🌐 PgAdmin: http://localhost:5050 (admin@example.com / admin)" -ForegroundColor Cyan
Write-Host "🔗 API: http://localhost:5199" -ForegroundColor Cyan
```

### 3. **Archivo de Comandos Útiles**

Crea `scripts/database-commands.md`:

````markdown
# 🗃️ Comandos Útiles de Base de Datos

## 🔄 Comandos Frecuentes

### Resetear base de datos completa:

```bash
# 1. Borrar migración (si es necesario)
dotnet ef migrations remove

# 2. Borrar base de datos
dotnet ef database drop

# 3. Crear desde cero
dotnet ef database update

# 4. Generar datos
curl -X POST http://localhost:5199/seed-database
```
````

### Crear nueva migración:

```bash
dotnet ef migrations add NombreMigración
dotnet ef database update
```

### Ver estado de migraciones:

```bash
dotnet ef migrations list
```

### Generar script SQL:

```bash
dotnet ef migrations script > migration.sql
```

## 🔧 Troubleshooting

### La base de datos no está actualizada:

```bash
dotnet ef database update
```

### Los datos no coinciden:

```bash
curl -X POST http://localhost:5199/seed-database
```

### Error de conexión:

```bash
docker-compose restart db
```

````

---

## 📦 Alternativa: Dump de Base de Datos

Si prefieres compartir un snapshot exacto:

### 1. **Crear dump:**

```bash
# Desde tu máquina (con datos que quieres compartir)
docker exec sismos-postgres pg_dump -U postgres -d SismosDB > database-snapshot.sql
````

### 2. **Restaurar dump:**

```bash
# En la máquina del compañero
docker exec -i sismos-postgres psql -U postgres -d SismosDB < database-snapshot.sql
```

### 3. **Automatizar con script:**

Crea `scripts/export-database.ps1`:

```powershell
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$filename = "database-snapshot-$timestamp.sql"

Write-Host "📦 Exportando base de datos..." -ForegroundColor Yellow
docker exec sismos-postgres pg_dump -U postgres -d SismosDB > $filename

Write-Host "✅ Base de datos exportada a: $filename" -ForegroundColor Green
Write-Host "📤 Para compartir: git add $filename && git commit -m 'feat: snapshot de BD'" -ForegroundColor Cyan
```

---

## 🎯 Recomendación Final

**Usa la estrategia de Migraciones + Seeder** porque:

✅ **Versionado**: Cada cambio está en git  
✅ **Reproducible**: Cualquiera puede recrear el estado  
✅ **Escalable**: Funciona para equipos grandes  
✅ **Flexible**: Fácil de modificar y actualizar  
✅ **Automático**: Scripts para automatizar todo

### Pasos para implementar:

1. **Modifica el seeder** para usar seed fijo (datos consistentes)
2. **Crea los scripts** de setup para nuevos desarrolladores
3. **Documenta el proceso** para el equipo
4. **Entrena al equipo** en el flujo de trabajo

¿Te ayudo a implementar alguna de estas mejoras específicas?
