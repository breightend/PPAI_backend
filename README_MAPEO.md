# Sistema de Mapeo JSON a Entidades

Este proyecto incluye un sistema completo para mapear datos desde un archivo JSON a las entidades del modelo de dominio usando Newtonsoft.Json.

## Estructura del Sistema

### 1. DTOs (Data Transfer Objects)
Ubicados en `datos/dtos/`, estos objetos mapean directamente la estructura del JSON:

- `DatosJsonDTO.cs` - Contenedor principal de todos los datos
- `UsuarioDTO.cs` - Mapea usuarios del JSON
- `EmpleadoDTO.cs` - Mapea empleados del JSON
- `RolDTO.cs` - Mapea roles del JSON
- `EstadoDTO.cs` - Mapea estados del JSON
- `MotivoDTO.cs` - Mapea motivos del JSON
- `SismografoDTO.cs` - Mapea sismógrafos del JSON
- `EstacionSismologicaDTO.cs` - Mapea estaciones sismológicas del JSON
- `OrdenDeInspeccionJsonDTO.cs` - Mapea órdenes de inspección del JSON
- `SesionDTO.cs` - Mapea sesiones del JSON
- `CambioEstadoDto.cs` - Mapea cambios de estado del JSON

### 2. Servicios de Mapeo

#### JsonMappingService
Ubicado en `services/JsonMappingService.cs`, este servicio:
- Carga el archivo JSON usando Newtonsoft.Json
- Convierte los DTOs a entidades del modelo de dominio
- Maneja las relaciones entre entidades
- Mantiene un cache interno para optimizar el mapeo

#### DataLoaderService
Ubicado en `services/DataLoaderService.cs`, este servicio:
- Proporciona una interfaz de alto nivel para cargar datos
- Almacena las entidades cargadas
- Incluye métodos de consulta útiles

## Uso del Sistema

### 1. Configuración en Program.cs

```csharp
// Registrar los servicios
builder.Services.AddSingleton<JsonMappingService>();
builder.Services.AddSingleton<DataLoaderService>();
```

### 2. Cargar Datos desde JSON

```csharp
// Usando DataLoaderService (recomendado)
var dataLoader = new DataLoaderService();
await dataLoader.LoadAllDataAsync("datos/datos.json");

// Acceder a las entidades cargadas
var empleados = dataLoader.Empleados;
var usuarios = dataLoader.Usuarios;
var ordenes = dataLoader.OrdenesDeInspeccion;
```

### 3. Usando JsonMappingService directamente

```csharp
var mappingService = new JsonMappingService();
await mappingService.LoadJsonDataAsync("datos/datos.json");

// Obtener entidades específicas
var empleados = mappingService.GetEmpleados();
var usuarios = mappingService.GetUsuarios();
var sismografos = mappingService.GetSismografos();
```

### 4. Métodos de Consulta Disponibles

```csharp
// Buscar empleado por nombre de usuario
var empleado = dataLoader.BuscarEmpleadoPorUsuario("nombreUsuario");

// Obtener órdenes de un empleado específico
var ordenes = dataLoader.GetOrdenesPorEmpleado(empleado);

// Obtener órdenes finalizadas sin cerrar
var ordenesSinCerrar = dataLoader.GetOrdenesFinalizadasSinCerrar();

// Obtener sismógrafos por estado
var sismografos = dataLoader.GetSismografosPorEstado("Operativo");
```

## Endpoints de Ejemplo

El proyecto incluye endpoints de ejemplo que demuestran el uso:

- `GET /datos-json` - Carga todos los datos y muestra estadísticas
- `GET /empleados-json` - Obtiene la lista de empleados desde JSON

## Características Importantes

### 1. Manejo de Dependencias
El sistema maneja automáticamente las dependencias entre entidades:
- Los empleados requieren roles
- Los usuarios requieren empleados
- Los sismógrafos requieren estados y motivos
- Las estaciones requieren sismógrafos, empleados y estados
- Las órdenes requieren empleados, estados y estaciones

### 2. Cache Interno
El `JsonMappingService` mantiene un cache interno de entidades mapeadas para:
- Evitar duplicados
- Optimizar el rendimiento
- Facilitar la resolución de referencias

### 3. Manejo de Errores
- Validación de datos cargados
- Manejo de referencias faltantes
- Excepciones descriptivas

### 4. Flexibilidad
- No requiere modificar las clases de entidades existentes
- Permite agregar nuevos DTOs fácilmente
- Separación clara entre datos JSON y modelo de dominio

## Estructura del JSON Esperado

```json
{
  "usuarios": [...],
  "empleados": [...],
  "roles": [...],
  "estados": [...],
  "motivos": [...],
  "sismografos": [...],
  "estacionesSismologicas": [...],
  "ordenesDeInspeccion": [...],
  "sesiones": [...]
}
```

## Notas Técnicas

- Usa Newtonsoft.Json para la deserialización
- Maneja valores nulos y opcionales apropiadamente
- Convierte fechas automáticamente
- Preserva las relaciones entre entidades
- Optimizado para archivos JSON grandes 