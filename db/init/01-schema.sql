-- Crear las tablas en orden de dependencia
BEGIN;

CREATE TABLE IF NOT EXISTS public."Roles" (
    "Nombre" VARCHAR(255) PRIMARY KEY,
    "Descripcion" TEXT
);

CREATE TABLE IF NOT EXISTS public."Empleados" (
    "Mail" VARCHAR(255) PRIMARY KEY,
    "Apellido" VARCHAR(255),
    "Nombre" VARCHAR(255),
    "Telefono" VARCHAR(255),
    "RolNombre" VARCHAR(255) REFERENCES public."Roles"("Nombre")
);

CREATE TABLE IF NOT EXISTS public."Usuarios" (
    "NombreUsuario" VARCHAR(255) PRIMARY KEY,
    "Contraseña" VARCHAR(255),
    "EmpleadoMail" VARCHAR(255) REFERENCES public."Empleados"("Mail")
);

CREATE TABLE IF NOT EXISTS public."Estados" (
    "Nombre" VARCHAR(255) PRIMARY KEY,
    "Descripcion" TEXT,
    "Ambito" VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS public."TiposMotivo" (
    "Id" SERIAL PRIMARY KEY,
    "Descripcion" TEXT
);

CREATE TABLE IF NOT EXISTS public."Sismografos" (
    "IdentificadorSismografo" SERIAL PRIMARY KEY,
    "FechaAdquisicion" TIMESTAMP,
    "NroSerie" INTEGER
);

CREATE TABLE IF NOT EXISTS public."EstacionesSismologicas" (
    "CodigoEstacion" SERIAL PRIMARY KEY,
    "DocumentoCertificacionAdq" BOOLEAN,
    "FechaSolicitudCertificacion" TIMESTAMP,
    "Latitud" DECIMAL,
    "Nombre" VARCHAR(255),
    "NroCertificacionAdquirida" INTEGER,
    "SismografoIdentificadorSismografo" INTEGER REFERENCES public."Sismografos"("IdentificadorSismografo"),
    "EstadoNombre" VARCHAR(255) REFERENCES public."Estados"("Nombre")
);

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

CREATE TABLE IF NOT EXISTS public."CambiosEstado" (
    "Id" SERIAL PRIMARY KEY,
    "EstadoNombre" VARCHAR(255) REFERENCES public."Estados"("Nombre"),
    "FechaHoraInicio" TIMESTAMP,
    "FechaHoraFin" TIMESTAMP,
    "SismografoIdentificadorSismografo" INTEGER REFERENCES public."Sismografos"("IdentificadorSismografo")
);

CREATE TABLE IF NOT EXISTS public."MotivosFueraDeServicio" (
    "Id" SERIAL PRIMARY KEY,
    "Descripcion" TEXT,
    "Comentario" TEXT,
    "TipoMotivoId" INTEGER REFERENCES public."TiposMotivo"("Id")
);

COMMIT;