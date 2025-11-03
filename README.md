# 🌍 PPAI Backend - Sistema de Gestión Sismológica

Backend API para el sistema de gestión de órdenes de inspección sismológica desarrollado en .NET 8 con PostgreSQL.

## 🚀 Quick Start para Nuevos Desarrolladores

```powershell
# 1. Clonar el repositorio
git clone [url-del-repo]
cd PPAI_backend

# 2. Setup automático completo
.\scripts\setup-dev.ps1

# ¡Listo! Ya tienes todo funcionando
```

**¿Primera vez en el proyecto?** → Lee la **[Guía Completa de Base de Datos en Equipo](README_TEAM_DATABASE.md)** 📚

---

## 📋 Características

- **🏗️ Arquitectura Clean**: Separación clara de responsabilidades
- **📊 Base de datos PostgreSQL**: Con migraciones automáticas
- **🌱 Datos de prueba**: Generación automática de datasets realistas
- **📧 Notificaciones**: Sistema de emails integrado
- **🐳 Docker**: Ambiente containerizado
- **🔄 Entity Framework**: ORM con migraciones automáticas
- **📚 API REST**: Endpoints documentados
- **👥 Trabajo en equipo**: Datos consistentes entre desarrolladores

---

## 🛠️ Tecnologías

- **.NET 8** - Framework principal
- **PostgreSQL 16** - Base de datos
- **Entity Framework Core** - ORM
- **Docker & Docker Compose** - Containerización
- **Bogus** - Generación de datos falsos
- **MailKit** - Servicio de emails
- **pgAdmin** - Administración de BD

---

## 🏗️ Estructura del Proyecto

```
PPAI_backend/
├── 📁 controllers/           # Controladores API
├── 📁 datos/                # DTOs y contexto de BD
├── 📁 models/               # Entidades y modelos
│   ├── entities/            # Entidades de base de datos
│   ├── gestor/              # Lógica de negocio
│   └── interfaces/          # Contratos
├── 📁 services/             # Servicios (Email, Seeder)
├── 📁 scripts/              # Scripts de automatización
├── 📁 Migrations/           # Migraciones EF Core
├── 📁 db/init/              # Scripts de inicialización
└── 📚 README_*.md           # Documentación específica
```

---

## 🚀 Comandos Frecuentes

### Para el día a día:

```powershell
# Regenerar datos de prueba
curl -X POST http://localhost:5199/seed-database

# Ver estadísticas de la BD
curl http://localhost:5199/database-stats

# Aplicar nuevas migraciones
dotnet ef database update
```

### Para cambios de modelos:

```powershell
# Crear nueva migración
dotnet ef migrations add MiCambio

# Aplicar migración
dotnet ef database update

# Regenerar datos
curl -X POST http://localhost:5199/seed-database
```

### Para problemas:

```powershell
# Reset completo
.\scripts\setup-dev.ps1 -Clean

# Solo migrar y generar datos
dotnet ef database update
curl -X POST http://localhost:5199/seed-database
```

---

## 🌐 Servicios Disponibles

### 🔗 URLs Principales

- **API Backend**: http://localhost:5199
- **PgAdmin**: http://localhost:5050 (admin@example.com / admin)
- **Health Check**: http://localhost:5199/health

### 📡 Endpoints Principales

- `POST /seed-database` - Generar datos de prueba
- `GET /database-stats` - Estadísticas de la BD
- `GET /api/gestorcerrarorden/*` - APIs de gestión de órdenes

---

## 📚 Documentación Específica

| Tema                      | Archivo                                                                  | Descripción                                      |
| ------------------------- | ------------------------------------------------------------------------ | ------------------------------------------------ |
| **🚀 Trabajo en Equipo**  | [README_TEAM_DATABASE.md](README_TEAM_DATABASE.md)                       | **Guía principal para sincronizar BD en equipo** |
| **🌱 Generador de Datos** | [README_DATABASE_SEEDER.md](README_DATABASE_SEEDER.md)                   | Cómo generar datos de prueba                     |
| **🐳 Docker Setup**       | [README_DOCKER_DB.md](README_DOCKER_DB.md)                               | Configuración de contenedores                    |
| **🔄 Migraciones EF**     | [README_MIGRATION_EF.md](README_MIGRATION_EF.md)                         | Entity Framework migrations                      |
| **📧 Servicio Email**     | [README_EMAIL_SERVICE.md](README_EMAIL_SERVICE.md)                       | Sistema de notificaciones                        |
| **👀 Observador CRSS**    | [README_OBSERVADOR_PANTALLA_CRSS.md](README_OBSERVADOR_PANTALLA_CRSS.md) | Patrón Observer                                  |
| **🗂️ Mapeo de Datos**     | [README_MAPEO.md](README_MAPEO.md)                                       | Conversión entre modelos                         |

### 📋 Scripts y Comandos

- **[database-commands.md](scripts/database-commands.md)** - Todos los comandos de BD que necesitas
- **setup-dev.ps1** - Setup automático completo
- **export-database.ps1** - Crear snapshots de BD
- **import-database.ps1** - Restaurar desde snapshots

---

## 🔧 Configuración

### Variables de Entorno

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SismosDB;Username=postgres;Password=postgres"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true
  }
}
```

### Docker

```yaml
# docker-compose.yml
services:
  db:
    image: postgres:16-alpine
    ports: ["5432:5432"]
    environment:
      POSTGRES_DB: SismosDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
```

---

## 👥 Para Equipos de Desarrollo

### ✅ **Flujo de Trabajo Estándar**

1. **Pull**: `git pull origin main`
2. **Migrar**: `dotnet ef database update`
3. **Datos**: `curl -X POST http://localhost:5199/seed-database`
4. **Desarrollar**: Hacer tus cambios
5. **Push**: `git add . && git commit && git push`

### ✅ **Onboarding Nuevos Developers**

```powershell
# Un solo comando para setup completo:
.\scripts\setup-dev.ps1
```

### ✅ **Datos Consistentes**

- Mismo seed = mismos datos siempre
- 20 empleados, 30 órdenes, 15 estaciones
- Emails reales configurables para testing
- Passwords: `123456` (desarrollo)

---

## 🆘 Troubleshooting

### ❌ Problemas Comunes

| Problema             | Solución                                           |
| -------------------- | -------------------------------------------------- |
| BD no actualizada    | `dotnet ef database update`                        |
| Datos inconsistentes | `curl -X POST http://localhost:5199/seed-database` |
| Error de conexión    | `docker-compose restart`                           |
| Todo está roto       | `.\scripts\setup-dev.ps1 -Clean`                   |

### 🔍 Verificación Rápida

```powershell
# ¿Está todo funcionando?
curl http://localhost:5199/health
curl http://localhost:5199/database-stats
```

### 📞 Obtener Ayuda

1. Lee la [documentación específica](#-documentación-específica)
2. Revisa [comandos de BD](scripts/database-commands.md)
3. Usa los scripts automáticos
4. Contacta al equipo

---

## 🤝 Contribuir

### Antes de hacer cambios:

1. **Lee**: [README_TEAM_DATABASE.md](README_TEAM_DATABASE.md)
2. **Pull**: Siempre actualiza antes de empezar
3. **Migra**: Crea migraciones para cambios de modelo
4. **Documenta**: Actualiza READMEs si es necesario

### Estándares de commits:

```
feat: agregar nueva funcionalidad
fix: corregir bug
docs: actualizar documentación
refactor: refactorizar código
test: agregar tests
```

---

## 📊 Datos del Proyecto

- **Versión**: 1.0.0
- **Framework**: .NET 8
- **Base de datos**: PostgreSQL 16
- **Arquitectura**: API REST
- **Entorno**: Docker

---

## 🎉 ¡Listo para Empezar!

**Para nuevos desarrolladores**:

1. Ejecuta `.\scripts\setup-dev.ps1`
2. Lee [README_TEAM_DATABASE.md](README_TEAM_DATABASE.md)
3. ¡Empieza a desarrollar!

**Para el equipo actual**:

- Usa `curl -X POST http://localhost:5199/seed-database` para datos frescos
- Siempre `git pull` antes de hacer migraciones
- Consulta [database-commands.md](scripts/database-commands.md) para comandos

---

**¡Happy Coding! 🚀**
