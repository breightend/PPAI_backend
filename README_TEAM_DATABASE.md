# 🚀 Guía Completa: Compartir Base de Datos en Equipo

> **Para desarrolladores que trabajan con Docker + PostgreSQL + Entity Framework**

## 🎯 Objetivo

Que todos los miembros del equipo puedan:

- ✅ Tener exactamente los mismos datos de prueba
- ✅ Sincronizar cambios de base de datos fácilmente
- ✅ Setup rápido para nuevos desarrolladores
- ✅ Evitar conflictos y problemas de versiones

---

## 🌟 Solución Implementada

### **Estrategia Principal: Migraciones + Seeder Consistente**

1. **Migraciones EF**: Para cambios de estructura (tablas, columnas, etc.)
2. **Seeder con seed fijo**: Para datos de prueba idénticos en todos lados
3. **Scripts automatizados**: Para setup y maintenance
4. **Documentación clara**: Para que el equipo sepa qué hacer

---

## 🚀 Para Nuevos Desarrolladores

### Setup Automático (Recomendado)

```powershell
# 1. Clonar el repositorio
git clone [url-del-repo]
cd PPAI_backend

# 2. Ejecutar setup automático
.\scripts\setup-dev.ps1

# ¡Listo! Ya tienes todo funcionando
```

### Setup Manual (si tienes problemas)

```powershell
# 1. Iniciar Docker
docker-compose up -d

# 2. Aplicar migraciones
dotnet ef database update

# 3. Generar datos
dotnet run  # En una terminal
curl -X POST http://localhost:5199/seed-database  # En otra terminal
```

---

## 🔄 Flujo de Trabajo Diario

### 📤 Cuando HACES cambios (tu workflow)

```powershell
# 1. Modificas tus modelos/entidades
# (editar archivos en models/entities/)

# 2. Crear migración
dotnet ef migrations add MiCambio

# 3. Aplicar localmente
dotnet ef database update

# 4. Probar con datos frescos
curl -X POST http://localhost:5199/seed-database

# 5. Hacer commit
git add .
git commit -m "feat: agregar nueva funcionalidad"
git push origin main
```

### 📥 Cuando un compañero hace push (workflow del equipo)

```powershell
# 1. Pull del código
git pull origin main

# 2. Aplicar nuevas migraciones
dotnet ef database update

# 3. Regenerar datos consistentes
curl -X POST http://localhost:5199/seed-database

# ¡Listo! Tienes los mismos datos que tu compañero
```

---

## 🛠️ Características Clave

### ✅ Datos Consistentes

- **Seed fijo**: Todos generan exactamente los mismos registros
- **Configuración compartida**: Mismas cantidades, mismos nombres
- **Emails reales**: Para probar notificaciones (configurables)

### ✅ Scripts Automatizados

- **`setup-dev.ps1`**: Setup completo para nuevos developers
- **`export-database.ps1`**: Crear snapshots de tu BD
- **`import-database.ps1`**: Restaurar desde snapshots
- **`database-commands.md`**: Todos los comandos que necesitas

### ✅ Configuración Flexible

```csharp
// En DatabaseSeederConfig.cs
public static DatabaseSeederConfig TeamShared => new()
{
    NumeroEmpleados = 20,        // Ajustable para el equipo
    NumeroOrdenes = 30,
    NumeroEstaciones = 15,
    LimpiarDatosExistentes = true,  // Siempre datos limpios
    ContraseñaDefecto = "123456"    // Para testing
};
```

---

## 📊 ¿Qué Datos se Generan?

El seeder crea un dataset completo y realista:

- **👥 Empleados**: 20 empleados con roles reales
- **👤 Usuarios**: 10 cuentas de acceso (password: `123456`)
- **📡 Sismógrafos**: 25 equipos con números de serie
- **🏢 Estaciones**: 15 estaciones en ubicaciones argentinas
- **📋 Órdenes**: 30 órdenes con estados variados
- **🔄 Cambios**: Historial completo de transiciones
- **⚠️ Motivos**: Razones técnicas para cambios
- **🔐 Sesiones**: Historial de acceso

**🎯 Lo importante**: ¡Todos obtienen exactamente los mismos datos!

---

## 🆘 Troubleshooting Común

### ❌ "No tengo los mismos datos que mi compañero"

```powershell
# Regenerar datos:
curl -X POST http://localhost:5199/seed-database
```

### ❌ "Error de migración"

```powershell
# Aplicar migraciones:
dotnet ef database update
```

### ❌ "Error de conexión a BD"

```powershell
# Reiniciar Docker:
docker-compose restart
```

### ❌ "Todo está roto"

```powershell
# Reset completo:
.\scripts\setup-dev.ps1 -Clean
```

---

## 📋 Comandos Más Usados

```powershell
# Regenerar datos
curl -X POST http://localhost:5199/seed-database

# Ver estadísticas
curl http://localhost:5199/database-stats

# Nueva migración
dotnet ef migrations add NombreMigracion
dotnet ef database update

# Reset completo
dotnet ef database drop --force
dotnet ef database update
curl -X POST http://localhost:5199/seed-database

# Setup nuevo developer
.\scripts\setup-dev.ps1
```

---

## 🎯 Ventajas de Esta Solución

### ✅ **Versionado en Git**

- Migraciones están en código fuente
- Cambios trackeables y reversibles
- Historial completo de evolución

### ✅ **Datos Consistentes**

- Mismo seed = mismos datos siempre
- No hay "funciona en mi máquina"
- Testing predecible

### ✅ **Automatización**

- Scripts para todo el flujo
- Setup de nuevos developers en minutos
- Cero configuración manual

### ✅ **Escalable**

- Funciona para equipos pequeños y grandes
- Fácil agregar nuevos datos de prueba
- Configuraciones por ambiente (dev/prod)

---

## 📚 Documentación Adicional

- **📋 Comandos de BD**: `scripts/database-commands.md`
- **🌱 Detalles del Seeder**: `README_DATABASE_SEEDER.md`
- **🐳 Docker Setup**: `README_DOCKER_DB.md`
- **📧 Servicio Email**: `README_EMAIL_SERVICE.md`
- **🔄 Migraciones EF**: `README_MIGRATION_EF.md`

---

## 🔧 Configuración Avanzada

### Cambiar Cantidades de Datos

Edita `services/DatabaseSeederConfig.cs`:

```csharp
public static DatabaseSeederConfig TeamShared => new()
{
    NumeroEmpleados = 50,    // Más empleados
    NumeroOrdenes = 100,     // Más órdenes
    // ...
};
```

### Agregar Emails Reales

Para probar notificaciones:

```csharp
public List<(string Email, string Nombre, string Apellido, string RolNombre)> EmpleadosReales { get; set; } = new()
{
    ("tu-email@gmail.com", "Tu Nombre", "Tu Apellido", "Responsable de Reparación"),
    // Agregar más...
};
```

### Configuraciones por Ambiente

```csharp
// Para testing (pocos datos)
_config = DatabaseSeederConfig.Testing;

// Para desarrollo (datos normales)
_config = DatabaseSeederConfig.TeamShared;

// Para demos (muchos datos)
_config = DatabaseSeederConfig.Production;
```

---

## 💡 Tips para el Equipo

### 🎯 **Mejores Prácticas**

- Siempre haz `git pull` antes de crear migraciones
- Ejecuta el seeder después de cada cambio importante
- Usa nombres descriptivos para migraciones
- Documenta cambios complejos en commit messages

### ⚠️ **Evitar Problemas**

- No modifiques migraciones ya pusheadas
- No hagas cambios manuales en la BD de desarrollo
- Siempre usa el seeder para datos de prueba
- Haz backup antes de cambios grandes

### 🚀 **Optimizaciones**

- Usa `DatabaseSeederConfig.Testing` para desarrollo rápido
- Ejecuta setup completo solo cuando sea necesario
- Configura tu IDE para ejecutar comandos frecuentes

---

## 🎉 ¡Conclusión!

Con esta configuración, tu equipo puede:

✅ **Trabajar sin conflictos** de base de datos  
✅ **Onboarding rápido** para nuevos developers  
✅ **Datos consistentes** para testing  
✅ **Versionado completo** de estructura y datos  
✅ **Automatización total** del flujo de trabajo

**¡Todos tendrán exactamente la misma experiencia de desarrollo!**

---

### 🆘 ¿Necesitas Ayuda?

1. **📖 Lee la documentación**: Archivos README\_\*.md
2. **🔍 Revisa comandos**: `scripts/database-commands.md`
3. **🛠️ Usa scripts**: `scripts/setup-dev.ps1`
4. **📧 Contacta al equipo**: [tu-contacto]

**¡Happy coding! 🚀**
