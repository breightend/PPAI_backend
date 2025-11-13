BEGIN;

-- 1. ROLES
CREATE TABLE IF NOT EXISTS public."Roles" (
    "Nombre" VARCHAR(255) PRIMARY KEY,
    "Descripcion" TEXT
);

-- 2. EMPLEADOS
CREATE TABLE IF NOT EXISTS public."Empleados" (
    "Mail" VARCHAR(255) PRIMARY KEY,
    "Apellido" VARCHAR(255),
    "Nombre" VARCHAR(255),
    "Telefono" VARCHAR(255),
    "RolNombre" VARCHAR(255) REFERENCES public."Roles"("Nombre")
);

-- 3. USUARIOS
CREATE TABLE IF NOT EXISTS public."Usuarios" (
    "NombreUsuario" VARCHAR(255) PRIMARY KEY,
    "Contraseña" VARCHAR(255),
    "EmpleadoMail" VARCHAR(255) REFERENCES public."Empleados"("Mail")
);

-- 4. ESTADOS
CREATE TABLE IF NOT EXISTS public."Estados" (
    "Nombre" VARCHAR(255) PRIMARY KEY,
    "Descripcion" TEXT,
    "Ambito" VARCHAR(255)
);

-- 5. TIPOS DE MOTIVO
CREATE TABLE IF NOT EXISTS public."TiposMotivo" (
    "Id" SERIAL PRIMARY KEY,
    "Descripcion" TEXT
);

-- 6. SISMOGRAFOS
CREATE TABLE IF NOT EXISTS public."Sismografos" (
    "IdentificadorSismografo" SERIAL PRIMARY KEY,
    "FechaAdquisicion" TIMESTAMP,
    "NroSerie" INTEGER
);

-- 7. ESTACIONES SISMOLOGICAS
CREATE TABLE IF NOT EXISTS public."EstacionesSismologicas" (
    "CodigoEstacion" SERIAL PRIMARY KEY,
    "DocumentoCertificacionAdq" BOOLEAN,
    "FechaSolicitudCertificacion" TIMESTAMP,
    "Latitud" DECIMAL,
    "Nombre" VARCHAR(255),
    "NroCertificacionAdquirida" INTEGER,
    "SismografoIdentificadorSismografo" INTEGER REFERENCES public."Sismografos"("IdentificadorSismografo"),
    "EstadoNombre" VARCHAR(255) REFERENCES public."Estados"("Nombre"),
    "EmpleadoMail" VARCHAR(255) REFERENCES public."Empleados"("Mail")
);

-- 8. ORDENES DE INSPECCION
CREATE TABLE IF NOT EXISTS public."OrdenesDeInspeccion" (
    "NumeroOrden" SERIAL PRIMARY KEY,
    "FechaHoraCierre" TIMESTAMP,
    "FechaHoraFinalizacion" TIMESTAMP,
    "FechaHoraInicio" TIMESTAMP,
    "ObservacionCierre" TEXT,
    "EmpleadoMail" VARCHAR(255) REFERENCES public."Empleados"("Mail"),
    "EstadoNombre" VARCHAR(255) REFERENCES public."Estados"("Nombre"),
    "EstacionSismologicaCodigoEstacion" INTEGER REFERENCES public."EstacionesSismologicas"("CodigoEstacion")
);

-- 9. CAMBIOS ESTADO
CREATE TABLE IF NOT EXISTS public."CambiosEstado" (
    "Id" SERIAL PRIMARY KEY,
    "EstadoNombre" VARCHAR(255) REFERENCES public."Estados"("Nombre"),
    "FechaHoraInicio" TIMESTAMP,
    "FechaHoraFin" TIMESTAMP,
    "SismografoIdentificadorSismografo" INTEGER REFERENCES public."Sismografos"("IdentificadorSismografo"),
    -- ESTA ES LA COLUMNA QUE FALTABA Y CAUSA EL ERROR EN EL BACKEND:
    "OrdenDeInspeccionNumeroOrden" INTEGER REFERENCES public."OrdenesDeInspeccion"("NumeroOrden")
);

-- 10. MOTIVOS FUERA DE SERVICIO
CREATE TABLE IF NOT EXISTS public."MotivosFueraDeServicio" (
    "Id" SERIAL PRIMARY KEY,
    "Descripcion" TEXT,
    "Comentario" TEXT,
    "TipoMotivoId" INTEGER REFERENCES public."TiposMotivo"("Id"),
    "CambioEstadoId" INTEGER REFERENCES public."CambiosEstado"("Id")
);

-- 11. SESIONES (¡ESTA ES LA QUE TE FALTABA!)
CREATE TABLE IF NOT EXISTS public."Sesiones" (
    "Id" SERIAL PRIMARY KEY,
    "FechaHoraInicio" TIMESTAMP,
    "FechaHoraFin" TIMESTAMP,
    "UsuarioNombreUsuario" VARCHAR(255) REFERENCES public."Usuarios"("NombreUsuario")
);

COMMIT;