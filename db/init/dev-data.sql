BEGIN;

-- 1. ROLES (Basado en los actores del CU 37)
-- El RI es el actor principal 
-- El Responsable de Reparaciones es notificado [cite: 207, 542]
INSERT INTO public."Roles" ("Nombre", "Descripcion")
VALUES
    ('Responsable de Inspecciones', 'Empleado que cierra órdenes de inspección (CU 37)'),
    ('Responsable de Reparaciones', 'Empleado que recibe notificación de sismógrafo "Fuera de Servicio"')
ON CONFLICT ("Nombre") DO NOTHING;

-- 2. EMPLEADOS (Los actores)
INSERT INTO public."Empleados" ("Mail", "Apellido", "Nombre", "Telefono", "RolNombre")
VALUES
    ('inspector.jefe@sismos.com', 'Gómez', 'Juan', '351-111111', 'Responsable de Inspecciones'),
    ('reparador.jefe@sismos.com', 'Díaz', 'Marcos', '351-222222', 'Responsable de Reparaciones')
ON CONFLICT ("Mail") DO NOTHING;

-- 3. USUARIO (Para que el RI pueda "loguearse" )
INSERT INTO public."Usuarios" ("NombreUsuario", "Contraseña", "EmpleadoMail")
VALUES
    ('jgomez', 'inspeccion123', 'inspector.jefe@sismos.com')
ON CONFLICT ("NombreUsuario") DO NOTHING;

-- 4. ESTADOS (Todos los estados mencionados en el flujo del CU 37)
INSERT INTO public."Estados" ("Nombre", "Descripcion", "Ambito")
VALUES
    ('Completamente Realizada', 'Pre-condición para CU 37 ', 'OrdenDeInspeccion'),
    ('Cerrada', 'Post-condición para CU 37 [cite: 205, 540]', 'OrdenDeInspeccion'),
    ('Inhabilitado por Inspección', 'Estado inicial del sismógrafo ', 'Sismografo'),
    ('Fuera de Servicio', 'Post-condición del sismógrafo ', 'Sismografo'),
    ('On-line', 'Estado para flujo alternativo A2 [cite: 211, 546]', 'Sismografo')
ON CONFLICT ("Nombre") DO NOTHING;

-- 5. TIPOS DE MOTIVO (Para el Paso 7 del CU 37 )
-- (Basados en la Regla de Negocio 11 [cite: 82, 417])
INSERT INTO public."TiposMotivo" ("Descripcion")
VALUES
    ('Avería por vibración'),
    ('Desgaste de componente'),
    ('Fallo en el sistema de registro'),
    ('Vandalismo');
    -- La PK ("Id") es autogenerada, no usamos ON CONFLICT aquí.

-- 6. SISMÓGRAFO Y ESTACIÓN (Las entidades a inspeccionar)
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
    ('2025-11-03T18:00:00Z', '2025-11-03T17:00:00Z', '2025-11-03T09:00:00Z', '(Observación de finalización, pendiente de cierre)', 'inspector.jefe@sismos.com', 'Completamente Realizada', 1)
RETURNING "NumeroOrden"; -- Esta será la Orden ID 1 (o la siguiente)

-- 8. ESTADO INICIAL DEL SISMÓGRAFO (Pre-condición clave)
-- El sismógrafo (ID 1) debe estar "Inhabilitado por Inspección" 
-- para que el CU 37 pueda pasarlo a "Fuera de Servicio".
INSERT INTO public."CambiosEstado" ("EstadoNombre", "FechaHoraInicio", "SismografoIdentificadorSismografo")
VALUES
    ('Inhabilitado por Inspección', '2025-11-03T09:00:01Z', 1);

END;