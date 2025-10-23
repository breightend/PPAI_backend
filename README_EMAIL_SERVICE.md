# Servicio de Notificaciones por Email - PPAI Backend

## 📧 Funcionalidad Implementada

Se ha implementado un sistema completo de notificaciones por email para el cierre de órdenes de inspección sismológica utilizando **SendGrid**. 

## 🚀 Características

- **Interfaz `IObservadorNotificacion`**: Define los contratos para el envío de notificaciones
- **Servicio `EmailService`**: Implementación concreta usando SendGrid
- **Integración automática**: Se envían emails automáticamente al cerrar una orden de inspección
- **Plantillas HTML**: Los emails incluyen formato HTML profesional
- **Validación de emails**: Verifica que las direcciones de email sean válidas
- **Logging**: Registra el estado de los envíos para seguimiento

## 📁 Archivos Creados/Modificados

### Nuevos Archivos:
- `services/EmailService.cs` - Servicio principal de emails
- `models/interfaces/IObservadorNotificacion.cs` - Interfaz actualizada

### Archivos Modificados:
- `Program.cs` - Configuración de dependencias
- `models/gestor/GestorCerrarOrdenDeInspeccion.cs` - Integración del servicio
- `datos/dtos/InterfazMail.cs` - Adaptación para usar el nuevo servicio
- `BackendAPI.csproj` - Agregado DotNetEnv
- `appsettings.json` - Configuración de SendGrid

## ⚙️ Configuración

### 1. Variables de Entorno (.env)

Crea o actualiza tu archivo `.env` en la raíz del proyecto con:

```env
# Configuración de SendGrid
SENDGRID_API_KEY=tu_api_key_de_sendgrid_aquí
SENDGRID_FROM_EMAIL=noreply@tudominio.com
```

### 2. Obtener API Key de SendGrid

1. Regístrate en [SendGrid](https://sendgrid.com/)
2. Ve a Settings > API Keys
3. Crea una nueva API Key con permisos de "Mail Send"
4. Copia la API Key al archivo `.env`

### 3. Configurar Email Remitente

1. En SendGrid, verifica tu dominio o email remitente
2. Actualiza `SENDGRID_FROM_EMAIL` en el archivo `.env`

## 📋 Uso

### Envío Automático

El sistema envía automáticamente emails cuando:
- Se cierra una orden de inspección
- Se confirma el cierre en el endpoint `/confirmar-cierre`

### Contenido del Email

Los emails incluyen:
- **Asunto**: "Notificación de Cierre de Orden de Inspección Sismológica"
- **Información del sismógrafo**: Número identificador
- **Estado**: Estado actual del equipo
- **Fecha y hora de cierre**
- **Observaciones del cierre**
- **Motivos y comentarios detallados**
- **Formato HTML profesional**

### Destinatarios

Los emails se envían automáticamente a todos los empleados que tengan el rol de "Responsable de Reparación".

## 🔧 API Endpoints Afectados

### POST `/confirmar-cierre`

**Antes:**
```json
{
  "confirmado": true
}
```

**Ahora:** Mismo request, pero adicionalmente:
- Envía emails automáticamente
- Retorna confirmación de envío: `"Cierre confirmado correctamente y notificaciones enviadas."`

## 🛠️ Arquitectura

```
IObservadorNotificacion (Interfaz)
    ↓
EmailService (Implementación con SendGrid)
    ↓
InterfazMail (Adaptador)
    ↓
GestorCerrarOrdenDeInspeccion (Uso)
```

### Inyección de Dependencias

El servicio está registrado en `Program.cs`:

```csharp
builder.Services.AddScoped<IObservadorNotificacion, EmailService>();
```

## 🧪 Testing

### Modo Simulación

Si no tienes configurada la API Key de SendGrid, el sistema funcionará en modo simulación:
- Los emails se "envían" pero solo se muestran en la consola
- No se producen errores
- Ideal para desarrollo y testing

### Logs

Revisa la consola del servidor para ver:
- Confirmación de envíos exitosos
- Errores de configuración
- Modo simulación activado

## ⚠️ Consideraciones de Seguridad

1. **Nunca** commits tu API Key de SendGrid al repositorio
2. Usa variables de entorno para todas las credenciales
3. El archivo `.env` debe estar en `.gitignore`
4. Valida siempre las direcciones de email antes del envío

## 🔍 Troubleshooting

### Error: "EmailService no configurado"
- Verifica que el archivo `.env` exista
- Confirma que `SENDGRID_API_KEY` esté definida
- Reinicia la aplicación después de cambiar el `.env`

### Error: "Invalid API Key"
- Verifica que la API Key sea correcta
- Confirma que la API Key tenga permisos de "Mail Send"
- Asegúrate de no tener espacios extra en el `.env`

### Error: "Sender email not verified"
- Ve a SendGrid > Settings > Sender Authentication
- Verifica tu dominio o email individual
- Actualiza `SENDGRID_FROM_EMAIL` con el email verificado

## 📞 Soporte

Si tienes problemas con la implementación, verifica:
1. Configuración del archivo `.env`
2. Logs en la consola del servidor
3. Estado de tu cuenta de SendGrid
4. Conectividad a internet

---

**✅ ¡La implementación está completa y lista para usar!**