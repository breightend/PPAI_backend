# Script para exportar la base de datos actual (Windows PowerShell)
# Uso: .\export-database.ps1 [nombre-opcional]

param(
    [string]$Name = "",          # Nombre opcional para el archivo
    [switch]$SchemaOnly,         # Solo estructura, sin datos
    [switch]$DataOnly,           # Solo datos, sin estructura
    [switch]$Compress            # Comprimir el archivo resultante
)

# Configuración
$CONTAINER_NAME = "sismos-postgres"
$DB_USER = "postgres"
$DB_NAME = "SismosDB"
$TIMESTAMP = Get-Date -Format "yyyyMMdd-HHmmss"

# Determinar nombre del archivo
if ($SchemaOnly) {
    $suffix = "schema"
} elseif ($DataOnly) {
    $suffix = "data"
} else {
    $suffix = "full"
}

if ($Name) {
    $FILENAME = "database-$Name-$suffix-$TIMESTAMP.sql"
} else {
    $FILENAME = "database-$suffix-$TIMESTAMP.sql"
}

Write-Host "📦 Exportando base de datos..." -ForegroundColor Green
Write-Host "🕒 Timestamp: $TIMESTAMP" -ForegroundColor Cyan
Write-Host "📁 Archivo: $FILENAME" -ForegroundColor Cyan

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

# Construir comando pg_dump
$pgDumpArgs = "-U $DB_USER -d $DB_NAME"

if ($SchemaOnly) {
    $pgDumpArgs += " --schema-only"
    Write-Host "🏗️ Exportando solo estructura..." -ForegroundColor Yellow
} elseif ($DataOnly) {
    $pgDumpArgs += " --data-only"
    Write-Host "📊 Exportando solo datos..." -ForegroundColor Yellow
} else {
    Write-Host "🔄 Exportando estructura y datos..." -ForegroundColor Yellow
}

# Crear el dump
try {
    $output = docker exec $CONTAINER_NAME pg_dump $pgDumpArgs.Split(' ')
    $output | Out-File -FilePath $FILENAME -Encoding UTF8
    
    if (-not (Test-Path $FILENAME) -or (Get-Item $FILENAME).Length -eq 0) {
        throw "El archivo de dump está vacío o no se creó"
    }
    
    # Obtener información del archivo
    $fileInfo = Get-Item $FILENAME
    $sizeKB = [math]::Round($fileInfo.Length / 1KB, 2)
    $sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
    
    $sizeDisplay = if ($sizeMB -gt 1) { "$sizeMB MB" } else { "$sizeKB KB" }
    
    Write-Host "✅ Base de datos exportada exitosamente" -ForegroundColor Green
    Write-Host "📊 Tamaño: $sizeDisplay" -ForegroundColor Cyan
    Write-Host "📍 Ubicación: $($fileInfo.FullName)" -ForegroundColor Cyan
    
    # Comprimir si se solicita
    if ($Compress) {
        Write-Host "🗜️ Comprimiendo archivo..." -ForegroundColor Yellow
        $zipFile = "$($FILENAME).zip"
        Compress-Archive -Path $FILENAME -DestinationPath $zipFile -Force
        
        $zipInfo = Get-Item $zipFile
        $zipSizeMB = [math]::Round($zipInfo.Length / 1MB, 2)
        $compressionRatio = [math]::Round((1 - ($zipInfo.Length / $fileInfo.Length)) * 100, 1)
        
        Write-Host "📦 Archivo comprimido: $zipFile ($zipSizeMB MB, $compressionRatio% reducción)" -ForegroundColor Green
        
        # Preguntar si eliminar el original
        $response = Read-Host "¿Eliminar archivo original no comprimido? (y/N)"
        if ($response -eq 'y' -or $response -eq 'Y') {
            Remove-Item $FILENAME
            Write-Host "🗑️ Archivo original eliminado" -ForegroundColor Gray
            $FILENAME = $zipFile
        }
    }
    
    Write-Host ""
    Write-Host "📤 Para compartir con el equipo:" -ForegroundColor White
    Write-Host "   git add $FILENAME" -ForegroundColor Gray
    Write-Host "   git commit -m `"feat: snapshot de BD - $TIMESTAMP`"" -ForegroundColor Gray
    Write-Host "   git push origin main" -ForegroundColor Gray
    Write-Host ""
    Write-Host "🔄 Para restaurar:" -ForegroundColor White
    if ($Compress -and $FILENAME.EndsWith('.zip')) {
        Write-Host "   # Primero descomprimir:" -ForegroundColor Gray
        Write-Host "   Expand-Archive -Path $FILENAME -DestinationPath ." -ForegroundColor Gray
        $sqlFile = $FILENAME.Replace('.zip', '')
        Write-Host "   # Luego restaurar:" -ForegroundColor Gray
        Write-Host "   Get-Content $sqlFile | docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME" -ForegroundColor Gray
    } else {
        Write-Host "   Get-Content $FILENAME | docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "📋 Información adicional:" -ForegroundColor White
    Write-Host "   📅 Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
    Write-Host "   🗃️ Base de datos: $DB_NAME" -ForegroundColor Gray
    Write-Host "   🐳 Contenedor: $CONTAINER_NAME" -ForegroundColor Gray
    Write-Host "   📏 Tipo: $(if ($SchemaOnly) { 'Solo estructura' } elseif ($DataOnly) { 'Solo datos' } else { 'Completo' })" -ForegroundColor Gray

} catch {
    Write-Host "❌ Error creando el dump: $($_.Exception.Message)" -ForegroundColor Red
    if (Test-Path $FILENAME) {
        Remove-Item $FILENAME  # Limpiar archivo vacío o corrupto
    }
    exit 1
}

Write-Host ""
Write-Host "🎉 Exportación completada exitosamente!" -ForegroundColor Green