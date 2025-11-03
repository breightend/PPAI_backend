# Script para importar una base de datos desde un archivo SQL
# Uso: .\import-database.ps1 -FilePath "archivo.sql" [-Clean] [-Backup]

param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath,           # Archivo SQL a importar
    
    [switch]$Clean,              # Limpiar base de datos antes de importar
    [switch]$Backup,             # Crear backup antes de importar
    [switch]$Force               # Forzar importación sin confirmación
)

# Configuración
$CONTAINER_NAME = "sismos-postgres"
$DB_USER = "postgres"
$DB_NAME = "SismosDB"
$TIMESTAMP = Get-Date -Format "yyyyMMdd-HHmmss"

Write-Host "📥 Importando base de datos..." -ForegroundColor Green
Write-Host "📁 Archivo: $FilePath" -ForegroundColor Cyan

# Verificar que el archivo existe
if (-not (Test-Path $FilePath)) {
    Write-Host "❌ Error: El archivo $FilePath no existe" -ForegroundColor Red
    exit 1
}

# Verificar que Docker está disponible
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Error: Docker no está instalado o no está en el PATH" -ForegroundColor Red
    exit 1
}

# Verificar que el contenedor existe y está corriendo
$containerStatus = docker ps --filter "name=$CONTAINER_NAME" --format "{{.Names}}"
if (-not $containerStatus -or $containerStatus -ne $CONTAINER_NAME) {
    Write-Host "❌ Error: El contenedor $CONTAINER_NAME no está corriendo" -ForegroundColor Red
    Write-Host "💡 Ejecuta: docker-compose up -d" -ForegroundColor Yellow
    exit 1
}

# Obtener información del archivo
$fileInfo = Get-Item $FilePath
$sizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
$sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
$sizeDisplay = if ($sizeMB -gt 1) { "$sizeMB MB" } else { "$sizeKB KB" }

Write-Host "📊 Tamaño del archivo: $sizeDisplay" -ForegroundColor Cyan
Write-Host "📅 Modificado: $($fileInfo.LastWriteTime)" -ForegroundColor Cyan

# Manejar archivos comprimidos
$tempSqlFile = $null
if ($FilePath.EndsWith('.zip')) {
    Write-Host "📦 Detectado archivo comprimido, descomprimiendo..." -ForegroundColor Yellow
    $tempDir = "$env:TEMP\database-import-$TIMESTAMP"
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    
    try {
        Expand-Archive -Path $FilePath -DestinationPath $tempDir -Force
        $sqlFiles = Get-ChildItem -Path $tempDir -Filter "*.sql"
        
        if ($sqlFiles.Count -eq 0) {
            throw "No se encontraron archivos SQL en el archivo comprimido"
        } elseif ($sqlFiles.Count -gt 1) {
            Write-Host "⚠️ Se encontraron múltiples archivos SQL:" -ForegroundColor Yellow
            $sqlFiles | ForEach-Object { Write-Host "   - $($_.Name)" -ForegroundColor Gray }
            $sqlFile = $sqlFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
            Write-Host "📄 Usando el más reciente: $($sqlFile.Name)" -ForegroundColor Cyan
        } else {
            $sqlFile = $sqlFiles[0]
        }
        
        $tempSqlFile = $sqlFile.FullName
        $FilePath = $tempSqlFile
        Write-Host "✅ Archivo descomprimido: $($sqlFile.Name)" -ForegroundColor Green
    } catch {
        Write-Host "❌ Error descomprimiendo archivo: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Confirmación si no se usa -Force
if (-not $Force) {
    Write-Host ""
    Write-Host "⚠️ ADVERTENCIA: Esta operación modificará la base de datos actual" -ForegroundColor Yellow
    if ($Clean) {
        Write-Host "⚠️ Se eliminarán TODOS los datos existentes (-Clean activado)" -ForegroundColor Red
    }
    Write-Host ""
    $confirmation = Read-Host "¿Continuar con la importación? (y/N)"
    if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
        Write-Host "❌ Importación cancelada por el usuario" -ForegroundColor Yellow
        if ($tempSqlFile) { Remove-Item $tempDir -Recurse -Force }
        exit 0
    }
}

# Crear backup si se solicita
if ($Backup) {
    Write-Host "💾 Creando backup de seguridad..." -ForegroundColor Yellow
    $backupFile = "backup-before-import-$TIMESTAMP.sql"
    
    try {
        $output = docker exec $CONTAINER_NAME pg_dump -U $DB_USER -d $DB_NAME
        $output | Out-File -FilePath $backupFile -Encoding UTF8
        
        $backupSize = [math]::Round((Get-Item $backupFile).Length / 1KB, 2)
        Write-Host "✅ Backup creado: $backupFile ($backupSize KB)" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Error creando backup: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "💡 Continuando sin backup..." -ForegroundColor Gray
    }
}

# Limpiar base de datos si se solicita
if ($Clean) {
    Write-Host "🧹 Limpiando base de datos existente..." -ForegroundColor Yellow
    
    # Verificar si EF Core está disponible
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        try {
            Write-Host "🔄 Usando Entity Framework para limpiar..." -ForegroundColor Gray
            dotnet ef database drop --force 2>$null
            dotnet ef database update 2>$null
            Write-Host "✅ Base de datos limpiada con EF Core" -ForegroundColor Green
        } catch {
            Write-Host "⚠️ Error con EF Core, usando método directo..." -ForegroundColor Yellow
            # Fallback: limpiar tablas manualmente
            $cleanScript = @"
DO `$`$ DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
        EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END `$`$;
"@
            $cleanScript | docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME
        }
    } else {
        Write-Host "⚠️ .NET no disponible, limpieza manual..." -ForegroundColor Yellow
    }
}

# Importar el archivo
Write-Host "📥 Importando datos..." -ForegroundColor Yellow
$startTime = Get-Date

try {
    # Leer y enviar el archivo al contenedor
    Get-Content $FilePath -Raw | docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME

    if ($LASTEXITCODE -eq 0) {
        $endTime = Get-Date
        $duration = ($endTime - $startTime).TotalSeconds
        
        Write-Host "✅ Importación completada exitosamente" -ForegroundColor Green
        Write-Host "⏱️ Tiempo transcurrido: $([math]::Round($duration, 2)) segundos" -ForegroundColor Cyan
        
        # Verificar datos importados
        Write-Host "📊 Verificando datos importados..." -ForegroundColor Yellow
        
        $tableCount = docker exec $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -t -c "SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public';"
        Write-Host "📋 Tablas importadas: $($tableCount.Trim())" -ForegroundColor Cyan
        
        # Mostrar algunas estadísticas básicas si es posible
        try {
            $employeeCount = docker exec $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -t -c 'SELECT count(*) FROM "Empleados";' 2>$null
            if ($employeeCount) {
                Write-Host "👥 Empleados: $($employeeCount.Trim())" -ForegroundColor Cyan
            }
            
            $orderCount = docker exec $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -t -c 'SELECT count(*) FROM "OrdenesDeInspeccion";' 2>$null
            if ($orderCount) {
                Write-Host "📋 Órdenes: $($orderCount.Trim())" -ForegroundColor Cyan
            }
        } catch {
            # Ignorar errores de estadísticas
        }
        
        Write-Host ""
        Write-Host "🎉 Base de datos restaurada exitosamente!" -ForegroundColor Green
        
        if ($Backup -and (Test-Path $backupFile)) {
            Write-Host "💾 Backup disponible en: $backupFile" -ForegroundColor Cyan
        }
        
    } else {
        throw "Error en la importación (código de salida: $LASTEXITCODE)"
    }
    
} catch {
    Write-Host "❌ Error durante la importación: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($Backup -and (Test-Path $backupFile)) {
        Write-Host "🔄 ¿Restaurar desde backup? (y/N)" -ForegroundColor Yellow
        $restore = Read-Host
        if ($restore -eq 'y' -or $restore -eq 'Y') {
            Write-Host "⏮️ Restaurando backup..." -ForegroundColor Yellow
            Get-Content $backupFile | docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME
            Write-Host "✅ Backup restaurado" -ForegroundColor Green
        }
    }
    
    exit 1
} finally {
    # Limpiar archivos temporales
    if ($tempSqlFile -and (Test-Path $tempDir)) {
        Remove-Item $tempDir -Recurse -Force
        Write-Host "🗑️ Archivos temporales limpiados" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "📋 Próximos pasos recomendados:" -ForegroundColor White
Write-Host "   🔄 Aplicar migraciones: dotnet ef database update" -ForegroundColor Gray
Write-Host "   🌱 Regenerar datos de prueba: curl -X POST http://localhost:5199/seed-database" -ForegroundColor Gray
Write-Host "   🏃 Iniciar aplicación: dotnet run" -ForegroundColor Gray