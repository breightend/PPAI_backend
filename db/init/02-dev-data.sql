BEGIN;

-- 1. ROLES
INSERT INTO public."Roles" ("Nombre", "Descripcion")
VALUES
    ('Responsable de Inspecciones', 'Cierra órdenes'),
    ('Tecnico de Reparaciones', 'Recibe notificaciones'), -- Coincide con Rol.cs
    ('Administrador', 'Admin')
ON CONFLICT ("Nombre") DO NOTHING;

-- 2. EMPLEADOS
INSERT INTO public."Empleados" ("Mail", "Apellido", "Nombre", "Telefono", "RolNombre")
VALUES
    ('mikaelaherrero26@gmail.com', 'Herrero', 'Mikaela', '351-111111', 'Responsable de Inspección'),
    ('brendatapa6@gmail.com', 'Tapari', 'Brenda', '351-222222', 'Tecnico de Reparaciones'),
    ('ramondelligonzalo5@gmail.com', 'Ramondelli', 'Gonzalo', '351-333333', 'Tecnico de Reparaciones')
ON CONFLICT ("Mail") DO NOTHING;

-- 3. USUARIOS
INSERT INTO public."Usuarios" ("NombreUsuario", "Contraseña", "EmpleadoMail")
VALUES
    ('m_herrero', 'inspeccion123', 'mikaelaherrero26@gmail.com'),
    ('b_tapari', '1234', 'brendatapa6@gmail.com'),
    ('g_ramondelli', '1234', 'ramondelligonzalo5@gmail.com')
ON CONFLICT ("NombreUsuario") DO NOTHING;

-- 4. ESTADOS
INSERT INTO public."Estados" ("Nombre", "Descripcion", "Ambito")
VALUES
    ('Finalizada', 'Lista para cierre', 'OrdenDeInspeccion'), -- Coincide con Estado.cs
    ('En Ejecucion', 'En proceso', 'OrdenDeInspeccion'),
    ('Cerrada', 'Cerrada', 'OrdenDeInspeccion'),
    ('Disponible', 'Listo', 'Sismografo'),
    ('EnLinea', 'Operativo', 'Sismografo'),
    ('InhabilitadoPorInspeccion', 'Detenido', 'Sismografo'),
    ('Fuera de Servicio', 'Roto', 'Sismografo'), -- Coincide con Estado.cs
    ('EnReparacion', 'Taller', 'Sismografo'),
    ('DeBaja', 'Eliminado', 'Sismografo')
ON CONFLICT ("Nombre") DO NOTHING;

-- 5. TIPOS DE MOTIVO
INSERT INTO public."TiposMotivo" ("Id", "Descripcion")
VALUES 
    (1, 'Avería por vibración'), 
    (2, 'Desgaste de componente'), 
    (3, 'Fallo en el sistema de registro'), 
    (4, 'Vandalismo'), 
    (5, 'Falla en fuente de alimentación')
ON CONFLICT ("Id") DO NOTHING;

-- 5.1 MOTIVOS (Catálogo necesario para que no falle el ID al cerrar)
INSERT INTO public."MotivosFueraDeServicio" ("Comentario", "TipoMotivoId", "CambioEstadoId")
VALUES
    ('Detectado por sensores internos', 1, NULL),
    ('Requiere cambio de pieza', 2, NULL),
    ('Fallo de I/O', 3, NULL),
    ('Vandalismo externo', 4, NULL),
    ('Sin energía', 5, NULL);

-- 6. SISMOGRAFOS
INSERT INTO public."Sismografos" ("IdentificadorSismografo", "FechaAdquisicion", "NroSerie")
VALUES 
    (10, '2023-01-15 00:00:00', 998811),
    (11, '2023-02-20 00:00:00', 998822),
    (12, '2023-03-10 00:00:00', 998833),
    (13, '2023-05-05 00:00:00', 998844);

-- 7. ESTACIONES SISMOLOGICAS
INSERT INTO public."EstacionesSismologicas" ("CodigoEstacion", "Nombre", "Latitud", "EstadoNombre", "SismografoIdentificadorSismografo", "EmpleadoMail", "DocumentoCertificacionAdq", "FechaSolicitudCertificacion", "NroCertificacionAdquirida")
VALUES 
    (101, 'Estación Cerro Colorado', -30.1234, 'InhabilitadoPorInspeccion', 10, 'mikaelaherrero26@gmail.com', true, '2022-01-01 00:00:00', 1001),
    (102, 'Estación Altas Cumbres', -31.5678, 'EnLinea', 11, 'ramondelligonzalo5@gmail.com', true, '2022-02-01 00:00:00', 1002),
    (105, 'Estación La Falda', -31.1000, 'Fuera de Servicio', 12, 'brendatapa6@gmail.com', true, '2022-03-01 00:00:00', 1003),
    (108, 'Estación Villa María', -32.4000, 'EnLinea', 13, 'brendatapa6@gmail.com', true, '2022-04-01 00:00:00', 1004);

-- 8. ORDENES DE INSPECCION
INSERT INTO public."OrdenesDeInspeccion" ("NumeroOrden", "FechaHoraCierre", "FechaHoraFinalizacion", "FechaHoraInicio", "ObservacionCierre", "EmpleadoMail", "EstadoNombre", "EstacionSismologicaCodigoEstacion")
VALUES 
    -- CASO CLAVE: Estado 'Finalizada' para que el filtro C# funcione
    (3422, '0001-01-01 00:00:00', '2025-11-11 15:30:00', '2025-11-11 08:00:00', '', 'mikaelaherrero26@gmail.com', 'Finalizada', 101),
    (3423, '0001-01-01 00:00:00', '2025-11-12 12:00:00', '2025-11-12 09:00:00', '', 'ramondelligonzalo5@gmail.com', 'Finalizada', 102),
    (3420, '2025-10-20 10:00:00', '2025-10-19 18:00:00', '2025-10-19 07:00:00', 'Cambio de batería exitoso.', 'mikaelaherrero26@gmail.com', 'Cerrada', 101);

-- 9. CAMBIOS ESTADO INICIALES
INSERT INTO public."CambiosEstado" ("EstadoNombre", "FechaHoraInicio", "SismografoIdentificadorSismografo")
VALUES ('InhabilitadoPorInspeccion', '2025-11-11 08:00:00', 10);

-- 10. SESIÓN ACTIVA
-- Corrección clave: Usamos '-infinity' para que C# detecte que es la fecha default
INSERT INTO public."Sesiones" ("FechaHoraInicio", "FechaHoraFin", "UsuarioNombreUsuario")
VALUES ('2025-11-13 08:00:00', '-infinity', 'b_tapari');

COMMIT;