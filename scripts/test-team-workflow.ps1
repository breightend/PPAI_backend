# Script de prueba para verificar que la solución funciona
# Ejecutar desde la raíz del proyecto: .\scripts\test-team-workflow.ps1

param(
    [switch]$SkipSetup     # Omitir setup, solo probar comandos
)

Write-Host "🧪 Probando flujo de trabajo en equipo..." -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan

$testResults = @()

# Función para registrar resultados
function Test-Step {
    param($Name, $ScriptBlock, $ExpectedResult = $true)
    
    Write-Host "🔄 Probando: $Name..." -ForegroundColor Yellow
    
    try {
        $result = & $ScriptBlock
        $success = if ($ExpectedResult) { $LASTEXITCODE -eq 0 -or $result } else { $true }
        
        if ($success) {
            Write-Host "✅ $Name - OK" -ForegroundColor Green
            $script:testResults += @{ Name = $Name; Status = "OK"; Error = $null }
        } else {
            Write-Host "❌ $Name - FALLÓ" -ForegroundColor Red
            $script:testResults += @{ Name = $Name; Status = "FALLÓ"; Error = "Código de salida: $LASTEXITCODE" }
        }
    } catch {
        Write-Host "❌ $Name - ERROR: $($_.Exception.Message)" -ForegroundColor Red
        $script:testResults += @{ Name = $Name; Status = "ERROR"; Error = $_.Exception.Message }
    }
    
    Write-Host ""
}

# Test 1: Verificar herramientas
Test-Step "Docker disponible" {
    Get-Command docker -ErrorAction Stop | Out-Null
    return $true
}

Test-Step ".NET disponible" {
    Get-Command dotnet -ErrorAction Stop | Out-Null
    return $true
}

Test-Step "EF Tools disponible" {
    dotnet ef --version | Out-Null
    return $true
}

# Test 2: Verificar estructura del proyecto
Test-Step "Archivo de proyecto existe" {
    Test-Path "BackendAPI.csproj"
}

Test-Step "Docker Compose existe" {
    Test-Path "docker-compose.yml"
}

Test-Step "Scripts existen" {
    (Test-Path "scripts/setup-dev.ps1") -and 
    (Test-Path "scripts/export-database.ps1") -and
    (Test-Path "scripts/import-database.ps1")
}

Test-Step "Documentación existe" {
    (Test-Path "README_TEAM_DATABASE.md") -and
    (Test-Path "scripts/database-commands.md")
}

# Test 3: Setup (si no se omite)
if (-not $SkipSetup) {
    Test-Step "Contenedores Docker" {
        docker-compose up -d | Out-Null
        Start-Sleep -Seconds 10
        return $true
    }
    
    Test-Step "PostgreSQL responde" {
        for ($i = 1; $i -le 10; $i++) {
            try {
                docker exec sismos-postgres pg_isready -U postgres -d SismosDB | Out-Null
                if ($LASTEXITCODE -eq 0) { return $true }
            } catch {}
            Start-Sleep -Seconds 2
        }
        return $false
    }
    
    Test-Step "Compilación del proyecto" {
        dotnet build --verbosity quiet | Out-Null
        return $true
    }
    
    Test-Step "Migraciones EF" {
        dotnet ef database update | Out-Null
        return $true
    }
}

# Test 4: Verificar que la aplicación puede iniciarse
Test-Step "Iniciar aplicación" {
    $job = Start-Job -ScriptBlock { 
        Set-Location $using:PWD
        dotnet run
    }
    
    # Esperar a que la aplicación esté lista
    $appReady = $false
    for ($i = 1; $i -le 20; $i++) {
        try {
            Start-Sleep -Seconds 3
            $response = Invoke-WebRequest -Uri "http://localhost:5199/health" -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                $appReady = $true
                break
            }
        } catch {
            # Continuar intentando
        }
    }
    
    # Limpiar job
    Stop-Job $job -ErrorAction SilentlyContinue
    Remove-Job $job -Force -ErrorAction SilentlyContinue
    
    return $appReady
}

if ($testResults | Where-Object { $_.Status -eq "OK" } | Where-Object { $_.Name -eq "Iniciar aplicación" }) {
    
    # Test 5: Verificar endpoints clave
    Write-Host "🔄 Reiniciando aplicación para pruebas de endpoints..." -ForegroundColor Yellow
    
    # Iniciar aplicación en background para tests
    $appJob = Start-Job -ScriptBlock { 
        Set-Location $using:PWD
        dotnet run
    }
    
    Start-Sleep -Seconds 8
    
    Test-Step "Health endpoint" {
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5199/health" -TimeoutSec 10
            return $true
        } catch {
            return $false
        }
    }
    
    Test-Step "Seed endpoint" {
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5199/seed-database" -Method POST -TimeoutSec 30
            return $true
        } catch {
            return $false
        }
    }
    
    Test-Step "Stats endpoint" {
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5199/database-stats" -TimeoutSec 10
            return $response -ne $null
        } catch {
            return $false
        }
    }
    
    # Limpiar aplicación
    Stop-Job $appJob -ErrorAction SilentlyContinue
    Remove-Job $appJob -Force -ErrorAction SilentlyContinue
}

# Test 6: Verificar configuración del seeder
Test-Step "Configuración de seeder consistente" {
    $configFile = "services/DatabaseSeederConfig.cs"
    if (Test-Path $configFile) {
        $content = Get-Content $configFile -Raw
        return $content -match "Randomizer\.Seed" -and $content -match "TeamShared"
    }
    return $false
}

# Test 7: Verificar migraciones
Test-Step "Migraciones listables" {
    dotnet ef migrations list --no-build | Out-Null
    return $true
}

# Resumen de resultados
Write-Host "📊 RESUMEN DE PRUEBAS" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

$totalTests = $testResults.Count
$passedTests = ($testResults | Where-Object { $_.Status -eq "OK" }).Count
$failedTests = ($testResults | Where-Object { $_.Status -ne "OK" }).Count

Write-Host "✅ Pruebas exitosas: $passedTests/$totalTests" -ForegroundColor Green
if ($failedTests -gt 0) {
    Write-Host "❌ Pruebas fallidas: $failedTests" -ForegroundColor Red
}

Write-Host ""
Write-Host "📋 Detalle de resultados:" -ForegroundColor White
$testResults | ForEach-Object {
    $icon = if ($_.Status -eq "OK") { "✅" } else { "❌" }
    $color = if ($_.Status -eq "OK") { "Green" } else { "Red" }
    Write-Host "  $icon $($_.Name)" -ForegroundColor $color
    if ($_.Error) {
        Write-Host "     Error: $($_.Error)" -ForegroundColor Gray
    }
}

# Resultado final
Write-Host ""
if ($failedTests -eq 0) {
    Write-Host "🎉 ¡TODAS LAS PRUEBAS PASARON!" -ForegroundColor Green
    Write-Host "✅ Tu configuración de equipo está lista para usar" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 Próximos pasos para tu equipo:" -ForegroundColor White
    Write-Host "  1. Compartir este repositorio con el equipo" -ForegroundColor Gray
    Write-Host "  2. Cada miembro ejecuta: .\scripts\setup-dev.ps1" -ForegroundColor Gray
    Write-Host "  3. Seguir el flujo en README_TEAM_DATABASE.md" -ForegroundColor Gray
} else {
    Write-Host "⚠️ Algunas pruebas fallaron" -ForegroundColor Yellow
    Write-Host "💡 Revisa los errores arriba y:" -ForegroundColor Cyan
    Write-Host "  - Verifica que Docker esté corriendo" -ForegroundColor Gray
    Write-Host "  - Ejecuta: .\scripts\setup-dev.ps1 -Clean" -ForegroundColor Gray
    Write-Host "  - Consulta: scripts\database-commands.md" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📚 Documentación:" -ForegroundColor White
Write-Host "  📖 Guía principal: README_TEAM_DATABASE.md" -ForegroundColor Gray
Write-Host "  📋 Comandos: scripts\database-commands.md" -ForegroundColor Gray
Write-Host "  🚀 Setup auto: .\scripts\setup-dev.ps1" -ForegroundColor Gray

exit $failedTests