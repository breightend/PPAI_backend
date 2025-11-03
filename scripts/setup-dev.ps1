# Script de configuración para nuevos desarrolladores
# Ejecutar desde la raíz del proyecto: .\scripts\setup-dev.ps1

param(
    [switch]$SkipDocker,     # Omitir verificación/inicio de Docker
    [switch]$SkipSeed,       # Omitir generación de datos
    [switch]$Clean           # Limpiar todo y empezar desde cero
)

Write-Host "🚀 Configurando entorno de desarrollo para PPAI Backend..." -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan

# Función para verificar comandos
function Test-Command {
    param($Command)
    $null = Get-Command $Command -ErrorAction SilentlyContinue
    return $?
}

# Función para esperar a que un servicio esté listo
function Wait-ForService {
    param($Url, $MaxAttempts = 30, $SleepSeconds = 2)
    
    for ($i = 1; $i -le $MaxAttempts; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -TimeoutSec 5 -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
                return $true
            }
        }
        catch {
            Write-Host "  Intento $i/$MaxAttempts..." -ForegroundColor Yellow
            Start-Sleep -Seconds $SleepSeconds
        }
    }
    return $false
}

# 1. Verificar prerequisitos
Write-Host "🔍 Verificando prerequisitos..." -ForegroundColor Yellow

if (-not (Test-Command "dotnet")) {
    Write-Host "❌ .NET no está instalado. Instala .NET 8 SDK." -ForegroundColor Red
    exit 1
}

if (-not $SkipDocker -and -not (Test-Command "docker")) {
    Write-Host "❌ Docker no está instalado" -ForegroundColor Red
    exit 1
}

if (-not $SkipDocker -and -not (Test-Command "docker-compose")) {
    Write-Host "❌ Docker Compose no está instalado" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Prerequisitos verificados" -ForegroundColor Green

# 2. Verificar estructura del proyecto
if (-not (Test-Path "BackendAPI.csproj")) {
    Write-Host "❌ No se encontró BackendAPI.csproj. Ejecuta desde la raíz del proyecto." -ForegroundColor Red
    exit 1
}

# 3. Limpiar si se solicita
if ($Clean) {
    Write-Host "🧹 Limpiando entorno..." -ForegroundColor Yellow
    
    # Parar contenedores
    if (-not $SkipDocker) {
        docker-compose down -v
        Write-Host "  🐳 Contenedores detenidos" -ForegroundColor Gray
    }
    
    # Limpiar builds
    dotnet clean
    Write-Host "  🗑️ Build limpio" -ForegroundColor Gray
    
    # Borrar base de datos (si existe)
    try {
        dotnet ef database drop --force
        Write-Host "  🗃️ Base de datos eliminada" -ForegroundColor Gray
    }
    catch {
        Write-Host "  ⚠️ Base de datos no existía" -ForegroundColor Gray
    }
}

# 4. Iniciar contenedores de Docker
if (-not $SkipDocker) {
    Write-Host "🐳 Iniciando contenedores de Docker..." -ForegroundColor Yellow
    
    # Verificar si docker-compose.yml existe
    if (-not (Test-Path "docker-compose.yml")) {
        Write-Host "❌ No se encontró docker-compose.yml" -ForegroundColor Red
        exit 1
    }
    
    # Iniciar contenedores
    docker-compose up -d
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error iniciando contenedores Docker" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "⏳ Esperando a que PostgreSQL esté listo..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
    
    # Verificar que PostgreSQL responda
    $dbReady = $false
    for ($i = 1; $i -le 10; $i++) {
        try {
            docker exec sismos-postgres pg_isready -U postgres -d SismosDB
            if ($LASTEXITCODE -eq 0) {
                $dbReady = $true
                break
            }
        }
        catch {}
        Write-Host "  Intento $i/10 - PostgreSQL no listo aún..." -ForegroundColor Yellow
        Start-Sleep -Seconds 3
    }
    
    if (-not $dbReady) {
        Write-Host "❌ PostgreSQL no responde después de 30 segundos" -ForegroundColor Red
        Write-Host "💡 Verifica: docker-compose logs db" -ForegroundColor Cyan
        exit 1
    }
    
    Write-Host "✅ PostgreSQL está listo" -ForegroundColor Green
}

# 5. Restaurar paquetes NuGet
Write-Host "📦 Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error restaurando paquetes" -ForegroundColor Red
    exit 1
}

# 6. Compilar proyecto
Write-Host "🔨 Compilando proyecto..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error compilando proyecto" -ForegroundColor Red
    exit 1
}

# 7. Aplicar migraciones
Write-Host "🔄 Aplicando migraciones de base de datos..." -ForegroundColor Yellow

# Verificar si hay migraciones
$migrations = dotnet ef migrations list 2>$null
if ($LASTEXITCODE -eq 0 -and $migrations) {
    dotnet ef database update
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error aplicando migraciones" -ForegroundColor Red
        Write-Host "💡 Verifica la conexión a la base de datos" -ForegroundColor Cyan
        exit 1
    }
    Write-Host "✅ Migraciones aplicadas" -ForegroundColor Green
} else {
    Write-Host "⚠️ No se encontraron migraciones" -ForegroundColor Yellow
}

# 8. Iniciar aplicación en background
Write-Host "🚀 Iniciando aplicación..." -ForegroundColor Yellow

# Matar procesos existentes de dotnet run en este proyecto
$existingProcesses = Get-Process | Where-Object { $_.ProcessName -eq "dotnet" -and $_.Path -like "*BackendAPI*" }
if ($existingProcesses) {
    Write-Host "🔄 Deteniendo instancias existentes..." -ForegroundColor Yellow
    $existingProcesses | Stop-Process -Force
}

# Iniciar nueva instancia
$job = Start-Job -ScriptBlock { 
    Set-Location $using:PWD
    dotnet run 
}

# Esperar a que la aplicación esté lista
Write-Host "⏳ Esperando a que la aplicación esté lista..." -ForegroundColor Yellow
$appReady = Wait-ForService -Url "http://localhost:5199/health" -MaxAttempts 20 -SleepSeconds 3

if (-not $appReady) {
    # Intentar endpoint alternativo
    $appReady = Wait-ForService -Url "http://localhost:5199" -MaxAttempts 10 -SleepSeconds 2
}

if (-not $appReady) {
    Write-Host "❌ La aplicación no responde después de 60 segundos" -ForegroundColor Red
    Write-Host "💡 Verifica manualmente: dotnet run" -ForegroundColor Cyan
    
    # Mostrar logs del job
    Write-Host "📋 Logs de la aplicación:" -ForegroundColor Yellow
    Receive-Job $job
    Remove-Job $job -Force
    exit 1
}

Write-Host "✅ Aplicación iniciada y respondiendo" -ForegroundColor Green

# 9. Generar datos de prueba
if (-not $SkipSeed) {
    Write-Host "🌱 Generando datos de prueba..." -ForegroundColor Yellow
    
    Start-Sleep -Seconds 2  # Esperar un poco más
    
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5199/seed-database" -Method POST -TimeoutSec 30
        Write-Host "✅ Datos de prueba generados correctamente" -ForegroundColor Green
        
        # Mostrar estadísticas
        try {
            $stats = Invoke-RestMethod -Uri "http://localhost:5199/database-stats" -Method GET -TimeoutSec 10
            Write-Host "📊 Estadísticas de la base de datos:" -ForegroundColor Cyan
            Write-Host $stats -ForegroundColor Gray
        }
        catch {
            Write-Host "⚠️ No se pudieron obtener estadísticas" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "⚠️ Error generando datos: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "💡 Puedes generar datos manualmente: curl -X POST http://localhost:5199/seed-database" -ForegroundColor Cyan
    }
}

# 10. Resumen final
Write-Host "`n🎉 ¡Setup completado exitosamente!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "🌐 Servicios disponibles:" -ForegroundColor White
Write-Host "  📡 API Backend:   http://localhost:5199" -ForegroundColor Cyan
Write-Host "  🗃️ PgAdmin:       http://localhost:5050" -ForegroundColor Cyan
Write-Host "     Usuario:      admin@example.com" -ForegroundColor Gray
Write-Host "     Contraseña:   admin" -ForegroundColor Gray
Write-Host "  📊 Health Check:  http://localhost:5199/health" -ForegroundColor Cyan
Write-Host "  🌱 Seed Data:     curl -X POST http://localhost:5199/seed-database" -ForegroundColor Cyan

Write-Host "`n📋 Comandos útiles:" -ForegroundColor White
Write-Host "  Parar todo:       docker-compose down" -ForegroundColor Gray
Write-Host "  Ver logs API:     Receive-Job $($job.Id)" -ForegroundColor Gray
Write-Host "  Regenerar datos:  curl -X POST http://localhost:5199/seed-database" -ForegroundColor Gray
Write-Host "  Migración nueva:  dotnet ef migrations add NombreMigracion" -ForegroundColor Gray

Write-Host "`n🔗 Documentación:" -ForegroundColor White
Write-Host "  📚 Readme DB:     ./README_DATABASE_SEEDER.md" -ForegroundColor Gray
Write-Host "  🐳 Readme Docker: ./README_DOCKER_DB.md" -ForegroundColor Gray
Write-Host "  📧 Readme Email:  ./README_EMAIL_SERVICE.md" -ForegroundColor Gray

# Limpiar job al final (solo si queremos que termine el script)
# Remove-Job $job -Force

Write-Host "`n🚀 ¡Tu entorno de desarrollo está listo para usar!" -ForegroundColor Green