# 🌱 Generador de Datos Aleatorios para Base de Datos

Este sistema te permite generar datos aleatorios realistas para poblar tu base de datos PostgreSQL con información de prueba.

## 🚀 ¿Cómo Usar?

### Opción 1: Usando la API (Recomendado)

1. **Inicia tu aplicación:**

   ```bash
   dotnet run
   ```

2. **Genera datos usando el endpoint:**

   ```bash
   # Método POST para generar datos
   curl -X POST http://localhost:5199/seed-database

   # O usando un cliente como Postman/Insomnia
   POST http://localhost:5199/seed-database
   ```

3. **Ver estadísticas:**

   ```bash
   # Método GET para ver estadísticas
   curl http://localhost:5199/database-stats

   # O en el navegador
   http://localhost:5199/database-stats
   ```

### Opción 2: Modificando el Código

Si necesitas personalizar los datos antes de generar:

## 🛠️ Personalización

### Cambiar Cantidades de Datos

Edita el archivo `services/DatabaseSeederConfig.cs`:

```csharp
public int NumeroEmpleados { get; set; } = 20;        // Cambia a 50
public int NumeroOrdenes { get; set; } = 30;          // Cambia a 100
public int NumeroEstaciones { get; set; } = 15;       // Cambia a 25
```

### Usar Configuraciones Predefinidas

En `services/DatabaseSeeder.cs`, línea 15, cambia:

```csharp
// Para pocos datos (testing)
_config = DatabaseSeederConfig.Testing;

// Para muchos datos (producción)
_config = DatabaseSeederConfig.Production;

// Para configuración personalizada
_config = DatabaseSeederConfig.Default;
```

### Personalizar Roles y Estados

En `DatabaseSeederConfig.cs`, puedes modificar:

```csharp
public List<(string Nombre, string Descripcion)> RolesPredefinidos { get; set; } = new()
{
    ("Tu Rol Personalizado", "Descripción de tu rol"),
    // ... más roles
};
```

### Cambiar Idioma de Datos Falsos

```csharp
public string IdiomaFaker { get; set; } = "es";  // Cambiar a "en" para inglés
```

### Configurar Fechas

```csharp
public int AñosAtrasAdquisicion { get; set; } = 10;   // Sismógrafos de hace X años
public int MesesAtrasOrdenes { get; set; } = 6;      // Órdenes de hace X meses
```

## 📊 Datos Generados

El generador crea:

- **👥 Roles**: Responsable de Inspección, Técnico de Reparaciones, etc.
- **👨‍💼 Empleados**: Con nombres, emails, teléfonos reales
- **👤 Usuarios**: Cuentas de acceso para algunos empleados
- **📡 Sismógrafos**: Con números de serie y fechas de adquisición
- **🏢 Estaciones**: Con ubicaciones geográficas argentinas
- **📋 Órdenes de Inspección**: Con estados y cambios realistas
- **🔄 Cambios de Estado**: Historial completo de transiciones
- **⚠️ Motivos**: Razones técnicas para cambios de estado
- **🔐 Sesiones**: Historial de acceso al sistema

## 🎯 Configuraciones Útiles

### Para Development (Pocos datos)

```csharp
_config = DatabaseSeederConfig.Testing;
```

- 5 empleados
- 3 estaciones
- 8 órdenes
- Rápido de generar

### Para Testing (Datos medianos)

```csharp
_config = DatabaseSeederConfig.Default;
```

- 20 empleados
- 15 estaciones
- 30 órdenes
- Datos representativos

### Para Demo/Producción (Muchos datos)

```csharp
_config = DatabaseSeederConfig.Production;
```

- 100 empleados
- 80 estaciones
- 500 órdenes
- Base de datos robusta

## ⚠️ Importante

### Limpieza de Datos

Por defecto, el generador **BORRA todos los datos existentes** antes de crear nuevos.

Para cambiar esto:

```csharp
public bool LimpiarDatosExistentes { get; set; } = false; // No borrar datos existentes
```

### Backup

**¡SIEMPRE haz backup de tu base de datos antes de ejecutar!**

```sql
-- PostgreSQL backup
pg_dump -h localhost -U postgres -d SismosDB > backup.sql
```

## 📋 Checklist Antes de Ejecutar

- [ ] ✅ Base de datos conectada y funcionando
- [ ] ✅ Backup realizado (si tienes datos importantes)
- [ ] ✅ Configuración revisada en `DatabaseSeederConfig.cs`
- [ ] ✅ Aplicación compilando sin errores
- [ ] ✅ Conexión a PostgreSQL verificada

## 🐛 Troubleshooting

### Error: "No se puede conectar a la base de datos"

- Verifica que PostgreSQL esté corriendo
- Revisa la cadena de conexión en `appsettings.json`

### Error: "Tabla no existe"

- Ejecuta las migraciones: `dotnet ef database update`

### Datos no aparecen

- Revisa la consola para errores
- Verifica que no haya restricciones de clave foránea

### Muy lento

- Reduce las cantidades en `DatabaseSeederConfig`
- Usa la configuración `Testing`

## 📈 Monitoreo

Después de ejecutar, verás en la consola:

```
🌱 Iniciando generación de datos aleatorios...
📋 Configuración: 20 empleados, 30 órdenes, 15 estaciones
👥 Generando roles...
👨‍💼 Generando empleados...
...
✅ Generación de datos completada exitosamente!

📊 ESTADÍSTICAS DE LA BASE DE DATOS:
👥 Roles: 5
👨‍💼 Empleados: 20
👤 Usuarios: 10
📡 Sismógrafos: 25
🏢 Estaciones Sismológicas: 15
📋 Órdenes de Inspección: 30
```

---

**💡 Tip**: Ejecuta primero con configuración `Testing` para probar que todo funcione, luego cambia a la configuración que necesites.
