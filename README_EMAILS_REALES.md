# 📧 Configuración de Emails Reales para Testing

## 🎯 ¿Para qué sirve?

Cuando generes datos aleatorios, algunos empleados tendrán **emails reales** (los tuyos) para que puedas:

- ✅ **Probar el envío de emails** al cerrar órdenes de inspección
- ✅ **Ver las notificaciones** en tu bandeja de entrada real
- ✅ **Verificar el formato** y contenido de los emails
- ✅ **Testear la funcionalidad** completa de SendGrid

## 🔧 Cómo Configurar Tus Emails

### Paso 1: Edita la Configuración

Abre el archivo `services/DatabaseSeederConfig.cs` y busca esta sección (línea ~60):

```csharp
[Description("Empleados con emails reales para testing de notificaciones")]
public List<(string Email, string Nombre, string Apellido, string RolNombre)> EmpleadosReales { get; set; } = new()
{
    ("tu.email.real@gmail.com", "Brenda", "Desarrolladora", "Responsable de Inspección"),
    ("segundo.email@gmail.com", "Carlos", "Técnico Senior", "Tecnico de Reparaciones"),
    ("tercer.email@outlook.com", "Ana", "Supervisora", "Tecnico de Reparaciones"),
    // 📧 IMPORTANTE: Cambia estos emails por emails reales donde quieras recibir las notificaciones
};
```

### Paso 2: Reemplaza con Tus Emails

```csharp
EmpleadosReales { get; set; } = new()
{
    ("brenda@tudominio.com", "Brenda", "Desarrolladora", "Responsable de Inspección"),
    ("tu.personal@gmail.com", "Tu Nombre", "Técnico", "Tecnico de Reparaciones"),
    ("colega@empresa.com", "Colega", "Supervisor", "Tecnico de Reparaciones"),
    // Agrega más si quieres probar con varios destinatarios
};
```

### Paso 3: Tipos de Roles Importantes

Para recibir notificaciones, asegúrate de que al menos uno tenga el rol **"Tecnico de Reparaciones"**:

- **"Responsable de Inspección"** → Puede cerrar órdenes (envía emails)
- **"Tecnico de Reparaciones"** → Recibe notificaciones (recibe emails)

## 🚀 Proceso Completo de Testing

### 1. Configurar SendGrid

Asegúrate de tener en tu `.env`:

```env
SENDGRID_API_KEY=tu_api_key_real_de_sendgrid
SENDGRID_FROM_EMAIL=noreply@tudominio.com
```

### 2. Generar Datos con Emails Reales

```bash
# Ejecutar la aplicación
dotnet run

# Generar datos (incluirá tus emails reales)
curl -X POST http://localhost:5199/seed-database
```

### 3. Verificar en la Consola

Deberías ver algo como:

```
📧 Creando empleados con emails reales para testing...
  ✅ Brenda Desarrolladora (brenda@tudominio.com) - Rol: Responsable de Inspección
  ✅ Tu Nombre (tu.personal@gmail.com) - Rol: Tecnico de Reparaciones
📝 Generando 17 empleados adicionales con emails aleatorios...
✅ Total empleados creados: 20
📧 Empleados con emails reales: 3
```

### 4. Probar el Envío de Emails

1. **Usar tu aplicación frontend** para cerrar una orden de inspección
2. **O usar el endpoint directamente:**

```bash
# 1. Primero obtener órdenes disponibles
curl http://localhost:5199/ordenes-inspeccion

# 2. Cerrar una orden específica
curl -X POST http://localhost:5199/cerrar-orden \
  -H "Content-Type: application/json" \
  -d '{
    "ordenId": 1,
    "observation": "Equipo calibrado correctamente",
    "motivos": [
      {"idMotivo": 1, "comentario": "Mantenimiento completado"}
    ]
  }'

# 3. Confirmar cierre (aquí se envían los emails)
curl -X POST http://localhost:5199/confirmar-cierre \
  -H "Content-Type: application/json" \
  -d '{"confirmado": true}'
```

### 5. Verificar en Tu Email

Deberías recibir un email como:

```
Asunto: Notificación de Cierre de Orden de Inspección Sismológica

Estimado responsable de reparación,

Se ha cerrado una orden de inspección con los siguientes detalles:

• Sismógrafo N°: 123
• Estado: Cerrada
• Fecha y hora de cierre: 23/10/2025 14:30
• Observación de cierre: Equipo calibrado correctamente

Motivos y comentarios:
- Mantenimiento preventivo programado: Mantenimiento completado

Por favor, tome las acciones correspondientes según sea necesario.

Saludos cordiales,
Sistema de Gestión Sismológica
```

## 🎯 Configuraciones Útiles

### Para Testing Individual (Solo tu email)

```csharp
EmpleadosReales { get; set; } = new()
{
    ("tu.email@gmail.com", "Tu Nombre", "Tester", "Tecnico de Reparaciones")
};
```

### Para Testing en Equipo (Varios emails)

```csharp
EmpleadosReales { get; set; } = new()
{
    ("dev1@empresa.com", "Dev 1", "Desarrollador", "Responsable de Inspección"),
    ("dev2@empresa.com", "Dev 2", "Tester", "Tecnico de Reparaciones"),
    ("manager@empresa.com", "Manager", "Jefe", "Tecnico de Reparaciones"),
    ("qa@empresa.com", "QA", "Quality", "Tecnico de Reparaciones")
};
```

### Para Producción (Sin emails reales)

```csharp
EmpleadosReales { get; set; } = new(); // Lista vacía
```

## 🐛 Troubleshooting

### No recibo emails

- ✅ Verifica tu API Key de SendGrid
- ✅ Confirma que el email remitente esté verificado en SendGrid
- ✅ Revisa la carpeta de spam
- ✅ Verifica que el empleado tenga rol "Tecnico de Reparaciones"

### Emails con formato extraño

- ✅ Revisa la configuración de `SENDGRID_FROM_EMAIL`
- ✅ Verifica que tengas permisos de envío en SendGrid

### Error al generar datos

- ✅ Verifica que los emails tengan formato válido
- ✅ Confirma que los roles existan en `RolesPredefinidos`

## 💡 Tips

1. **Usa emails reales** solo para development/testing
2. **Cambia a lista vacía** para producción
3. **Prueba primero** con un solo email tuyo
4. **Verifica SendGrid** antes de generar muchos datos
5. **Haz backup** antes de regenerar datos

---

**🎉 ¡Listo! Ahora puedes probar el sistema completo de notificaciones con tus emails reales.**
