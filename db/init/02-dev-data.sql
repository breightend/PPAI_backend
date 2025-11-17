BEGIN;

-- 1. ROLES
INSERT INTO public."Roles" ("Nombre", "Descripcion")
VALUES
    ('Responsable de Inspecciones', 'Cierra órdenes'),
    ('Tecnico de Reparaciones', 'Recibe notificaciones'),
    ('Administrador', 'Admin')
ON CONFLICT ("Nombre") DO NOTHING;

-- 2. EMPLEADOS
INSERT INTO public."Empleados" ("Mail", "Apellido", "Nombre", "Telefono", "RolNombre")
VALUES
    ('mikaelaherrero26@gmail.com', 'Herrero', 'Mikaela', '351-111111', 'Responsable de Inspecciones'),
    ('brendatapa6@gmail.com', 'Tapari', 'Brenda', '351-222222', 'Tecnico de Reparaciones'),
    ('ramondelligonzalo5@gmail.com', 'Ramondelli', 'Gonzalo', '351-333333', 'Tecnico de Reparaciones'),
    ('surlymeyer@gmail.com', 'Meyer', 'Surly', '351-444444', 'Tecnico de Reparaciones')
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
    ('Finalizada', 'Lista para cierre', 'OrdenDeInspeccion'),
    ('En Ejecucion', 'En proceso', 'OrdenDeInspeccion'),
    ('Cerrada', 'Cerrada', 'OrdenDeInspeccion'),
    ('Disponible', 'Listo', 'Sismografo'),
    ('EnLinea', 'Operativo', 'Sismografo'),
    ('InhabilitadoPorInspeccion', 'Detenido', 'Sismografo'),
    ('Fuera de Servicio', 'Roto', 'Sismografo'),
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

-- 6. SISMOGRAFOS
INSERT INTO public."Sismografos" ("IdentificadorSismografo", "FechaAdquisicion", "NroSerie")
VALUES 
    (10, '2023-01-15 00:00:00', 998811),
    (11, '2023-02-20 00:00:00', 998822),
    (12, '2023-03-10 00:00:00', 998833),
    (13, '2023-05-05 00:00:00', 998844),
    (14, '2023-06-10 00:00:00', 998855),
    (15, '2023-07-15 00:00:00', 998866)
ON CONFLICT ("IdentificadorSismografo") DO NOTHING;

-- 7. ESTACIONES SISMOLOGICAS
INSERT INTO public."EstacionesSismologicas" ("CodigoEstacion", "Nombre", "Latitud", "EstadoNombre", "SismografoIdentificadorSismografo", "EmpleadoMail", "DocumentoCertificacionAdq", "FechaSolicitudCertificacion", "NroCertificacionAdquirida")
VALUES 
    (101, 'Estación Cerro Colorado', -30.1234, 'InhabilitadoPorInspeccion', 10, 'mikaelaherrero26@gmail.com', true, '2022-01-01 00:00:00', 1001),
    (102, 'Estación Altas Cumbres', -31.5678, 'EnLinea', 11, 'ramondelligonzalo5@gmail.com', true, '2022-02-01 00:00:00', 1002),
    (105, 'Estación La Falda', -31.1000, 'Fuera de Servicio', 12, 'brendatapa6@gmail.com', true, '2022-03-01 00:00:00', 1003),
    (108, 'Estación Villa María', -32.4000, 'EnLinea', 13, 'brendatapa6@gmail.com', true, '2022-04-01 00:00:00', 1004),
    (109, 'Estación Río Cuarto', -33.1234, 'Fuera de Servicio', 14, 'mikaelaherrero26@gmail.com', true, '2022-05-01 00:00:00', 1005),
    (110, 'Estación Carlos Paz', -31.4200, 'Fuera de Servicio', 15, 'ramondelligonzalo5@gmail.com', true, '2022-06-01 00:00:00', 1006)
ON CONFLICT ("CodigoEstacion") DO NOTHING;

-- 8. ORDENES DE INSPECCION
INSERT INTO public."OrdenesDeInspeccion" ("NumeroOrden", "FechaHoraCierre", "FechaHoraFinalizacion", "FechaHoraInicio", "ObservacionCierre", "EmpleadoMail", "EstadoNombre", "EstacionSismologicaCodigoEstacion")
VALUES 
    -- Órdenes FINALIZADAS (listas para cerrar)
    (3422, '0001-01-01 00:00:00', '2025-11-11 15:30:00', '2025-11-11 08:00:00', '', 'mikaelaherrero26@gmail.com', 'Finalizada', 101),
    (3423, '0001-01-01 00:00:00', '2025-11-12 12:00:00', '2025-11-12 09:00:00', '', 'ramondelligonzalo5@gmail.com', 'Finalizada', 102),
    
    -- Órdenes ya CERRADAS (para que aparezcan en monitores)
    (3420, '2025-11-10 10:00:00', '2025-11-10 09:00:00', '2025-11-10 07:00:00', 'Cambio de batería exitoso. Sistema estabilizado.', 'mikaelaherrero26@gmail.com', 'Cerrada', 105),
    (3421, '2025-11-09 14:30:00', '2025-11-09 13:00:00', '2025-11-09 08:00:00', 'Reemplazo de sensor sísmico principal. Calibración completada.', 'brendatapa6@gmail.com', 'Cerrada', 109),
    (3424, '2025-11-08 16:00:00', '2025-11-08 15:00:00', '2025-11-08 09:00:00', 'Reparación de sistema de alimentación. Vandalism damage repaired.', 'ramondelligonzalo5@gmail.com', 'Cerrada', 110)
ON CONFLICT ("NumeroOrden") DO NOTHING;

-- 9. CAMBIOS DE ESTADO PARA SISMOGRAFOS
INSERT INTO public."CambiosEstado" ("EstadoNombre", "FechaHoraInicio", "FechaHoraFin", "SismografoIdentificadorSismografo")
VALUES 
    -- Sismógrafo 10: InhabilitadoPorInspeccion (orden 3422 - para cerrar)
    ('InhabilitadoPorInspeccion', '2025-11-11 08:00:00', NULL, 10),
    
    -- Sismógrafo 12: Fuera de Servicio (orden 3420 - ya cerrada)
    ('Fuera de Servicio', '2025-11-10 09:00:00', NULL, 12),
    
    -- Sismógrafo 14: Fuera de Servicio (orden 3421 - ya cerrada)
    ('Fuera de Servicio', '2025-11-09 13:00:00', NULL, 14),
    
    -- Sismógrafo 15: Fuera de Servicio (orden 3424 - ya cerrada)
    ('Fuera de Servicio', '2025-11-08 15:00:00', NULL, 15)
ON CONFLICT DO NOTHING;

-- 10. MOTIVOS FUERA DE SERVICIO PARA LAS ÓRDENES CERRADAS
-- Primero obtenemos los IDs de los cambios de estado que acabamos de insertar
DO $$
DECLARE
    ce_id_12 INTEGER;
    ce_id_14 INTEGER;
    ce_id_15 INTEGER;
BEGIN
    -- Obtener ID del cambio de estado del sismógrafo 12
    SELECT "Id" INTO ce_id_12 
    FROM public."CambiosEstado" 
    WHERE "SismografoIdentificadorSismografo" = 12 
    AND "EstadoNombre" = 'Fuera de Servicio'
    ORDER BY "FechaHoraInicio" DESC LIMIT 1;
    
    -- Obtener ID del cambio de estado del sismógrafo 14
    SELECT "Id" INTO ce_id_14 
    FROM public."CambiosEstado" 
    WHERE "SismografoIdentificadorSismografo" = 14 
    AND "EstadoNombre" = 'Fuera de Servicio'
    ORDER BY "FechaHoraInicio" DESC LIMIT 1;
    
    -- Obtener ID del cambio de estado del sismógrafo 15
    SELECT "Id" INTO ce_id_15 
    FROM public."CambiosEstado" 
    WHERE "SismografoIdentificadorSismografo" = 15 
    AND "EstadoNombre" = 'Fuera de Servicio'
    ORDER BY "FechaHoraInicio" DESC LIMIT 1;
    
    -- Insertar motivos para orden 3420 (sismógrafo 12)
    IF ce_id_12 IS NOT NULL THEN
        INSERT INTO public."MotivosFueraDeServicio" ("Descripcion", "Comentario", "TipoMotivoId", "CambioEstadoId")
        VALUES
            ('Falla en fuente de alimentación', 'Batería agotada, requiere reemplazo inmediato', 5, ce_id_12)
        ON CONFLICT DO NOTHING;
    END IF;
    
    -- Insertar motivos para orden 3421 (sismógrafo 14)
    IF ce_id_14 IS NOT NULL THEN
        INSERT INTO public."MotivosFueraDeServicio" ("Descripcion", "Comentario", "TipoMotivoId", "CambioEstadoId")
        VALUES
            ('Desgaste de componente', 'Sensor sísmico presenta lecturas inconsistentes', 2, ce_id_14),
            ('Fallo en el sistema de registro', 'Pérdida intermitente de datos', 3, ce_id_14)
        ON CONFLICT DO NOTHING;
    END IF;
    
    -- Insertar motivos para orden 3424 (sismógrafo 15)
    IF ce_id_15 IS NOT NULL THEN
        INSERT INTO public."MotivosFueraDeServicio" ("Descripcion", "Comentario", "TipoMotivoId", "CambioEstadoId")
        VALUES
            ('Vandalismo', 'Daños en cableado externo y panel de control', 4, ce_id_15),
            ('Falla en fuente de alimentación', 'Sistema de respaldo comprometido', 5, ce_id_15)
        ON CONFLICT DO NOTHING;
    END IF;
END $$;

-- 11. SESIÓN ACTIVA
INSERT INTO public."Sesiones" ("FechaHoraInicio", "FechaHoraFin", "UsuarioNombreUsuario")
VALUES ('2025-11-17 08:00:00', '-infinity', 'm_herrero')
ON CONFLICT DO NOTHING;

COMMIT;