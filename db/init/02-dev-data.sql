BEGIN;

-- 1. ROLES 
INSERT INTO public."Roles" ("Nombre", "Descripcion")
VALUES
    ('Responsable de Inspecciones', 'Empleado que cierra órdenes de inspección (CU 37)'),
    ('Responsable de Reparaciones', 'Empleado que recibe notificación de sismógrafo "Fuera de Servicio"')
ON CONFLICT ("Nombre") DO NOTHING;

-- 2. EMPLEADOS
-- IMPORTANTE: Estos son correos de prueba para desarrollo local
-- Para producción, modificar estos valores o usar variables de entorno
INSERT INTO public."Empleados" ("Mail", "Apellido", "Nombre", "Telefono", "RolNombre")
VALUES
    ('besume03@gmai.com', 'Gómez', 'Juan', '351-111111', 'Responsable de Inspecciones'),
    ('brendatapa6@gmail.com', 'Díaz', 'Marcos', '351-222222', 'Responsable de Reparaciones'),
    ('lucianonavarro44@gmail.com', 'Navarro', 'Luciano', '351-333333', 'Responsable de Reparaciones')
ON CONFLICT ("Mail") DO UPDATE 
SET "Apellido" = EXCLUDED."Apellido",
    "Nombre" = EXCLUDED."Nombre",
    "Telefono" = EXCLUDED."Telefono",
    "RolNombre" = EXCLUDED."RolNombre";

-- 3. USUARIO
-- IMPORTANTE: Esta es una contraseña de prueba para desarrollo local
-- Para producción, usar un hash seguro y variables de entorno
INSERT INTO public."Usuarios" ("NombreUsuario", "Contraseña", "EmpleadoMail")
VALUES
    ('jgomez', 'inspeccion123', 'besume03@gmai.com')
ON CONFLICT ("NombreUsuario") DO UPDATE 
SET "Contraseña" = EXCLUDED."Contraseña",
    "EmpleadoMail" = EXCLUDED."EmpleadoMail";

-- 4. ESTADOS
INSERT INTO public."Estados" ("Nombre", "Descripcion", "Ambito")
VALUES
    ('Completamente Realizada', 'Pre-condición para CU 37 ', 'OrdenDeInspeccion'),
    ('Cerrada', 'Post-condición para CU 37 [cite: 205, 540]', 'OrdenDeInspeccion'),
    ('Inhabilitado por Inspección', 'Estado inicial del sismógrafo ', 'Sismografo'),
    ('Fuera de Servicio', 'Post-condición del sismógrafo ', 'Sismografo'),
    ('On-line', 'Estado para flujo alternativo A2 [cite: 211, 546]', 'Sismografo')
ON CONFLICT ("Nombre") DO NOTHING;

-- 5. TIPOS DE MOTIVO
INSERT INTO public."TiposMotivo" ("Descripcion")
VALUES
    ('Avería por vibración'),
    ('Desgaste de componente'),
    ('Fallo en el sistema de registro'),
    ('Vandalismo');

INSERT INTO public."Sismografos" ("FechaAdquisicion", "NroSerie")
VALUES ('2023-01-01T12:00:00Z', 1001)
RETURNING "IdentificadorSismografo"; -- Este será el Sismógrafo ID 1 (o el siguiente)

INSERT INTO public."EstacionesSismologicas" ("DocumentoCertificacionAdq", "FechaSolicitudCertificacion", "Latitud", "Nombre", "NroCertificacionAdquirida", "SismografoIdentificadorSismografo", "EstadoNombre")
VALUES (true, '2023-01-01T00:00:00Z', -31.4201, 'Estación Observatorio', 500, 1, 'On-line')
RETURNING "CodigoEstacion"; -- Esta será la Estación ID 1 (o la siguiente)

-- 7. ORDEN DE INSPECCIÓN (La entidad principal del CU 37)
-- La creamos en estado "Completamente Realizada" y asignada al RI 'jgomez' 
INSERT INTO public."OrdenesDeInspeccion" ("FechaHoraCierre", "FechaHoraFinalizacion", "FechaHoraInicio", "ObservacionCierre", "EmpleadoMail", "EstadoNombre", "EstacionSismologicaCodigoEstacion")
VALUES
    ('2025-11-03T18:00:00Z', '2025-11-03T17:00:00Z', '2025-11-03T09:00:00Z', '(Observación de finalización, pendiente de cierre)', 'besume03@gmai.com', 'Completamente Realizada', 1)
RETURNING "NumeroOrden"; -- Esta será la Orden ID 1 (o la siguiente)

-- 8. ESTADO INICIAL DEL SISMÓGRAFO (Pre-condición clave)
-- El sismógrafo (ID 1) debe estar "Inhabilitado por Inspección" 
-- para que el CU 37 pueda pasarlo a "Fuera de Servicio".
INSERT INTO public."CambiosEstado" ("EstadoNombre", "FechaHoraInicio", "SismografoIdentificadorSismografo")
VALUES
    ('Inhabilitado por Inspección', '2025-11-03T09:00:01Z', 1);

END;