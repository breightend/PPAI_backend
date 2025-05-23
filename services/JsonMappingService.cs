using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PPAI_backend.datos.dtos;
using PPAI_backend.models.entities;

namespace PPAI_backend.services
{
    public class JsonMappingService
    {
        private DatosJsonDTO? _datosDto;
        private Dictionary<string, Empleado> _empleadosMap = new();
        private Dictionary<string, Estado> _estadosMap = new();
        private Dictionary<int, Rol> _rolesMap = new();
        private Dictionary<int, Motivo> _motivosMap = new();
        private Dictionary<string, Usuario> _usuariosMap = new();
        private Dictionary<string, Sismografo> _sismografosMap = new();
        private Dictionary<string, EstacionSismologica> _estacionesMap = new();

        public async Task<DatosJsonDTO?> LoadJsonDataAsync(string jsonFilePath)
        {
            try
            {
                string jsonData = await File.ReadAllTextAsync(jsonFilePath);
                _datosDto = JsonConvert.DeserializeObject<DatosJsonDTO>(jsonData);
                return _datosDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar los datos JSON: {ex.Message}", ex);
            }
        }

        public List<Empleado> GetEmpleados()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Primero mapear roles
            MapRoles();

            var empleados = new List<Empleado>();
            
            foreach (var empleadoDto in _datosDto.Empleados)
            {
                var rol = _rolesMap.GetValueOrDefault(empleadoDto.RolId);
                if (rol == null) continue;

                var empleado = new Empleado
                {
                    Apellido = empleadoDto.Apellido,
                    Nombre = empleadoDto.Nombre,
                    Mail = empleadoDto.Mail,
                    Telefono = empleadoDto.Telefono,
                    Rol = rol
                };

                empleados.Add(empleado);
                _empleadosMap[empleadoDto.Id] = empleado;
            }

            return empleados;
        }

        public List<Usuario> GetUsuarios()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Asegurar que los empleados estén mapeados
            if (_empleadosMap.Count == 0) GetEmpleados();

            var usuarios = new List<Usuario>();
            
            foreach (var usuarioDto in _datosDto.Usuarios)
            {
                var empleado = _empleadosMap.GetValueOrDefault(usuarioDto.EmpleadoId);
                if (empleado == null) continue;

                var usuario = new Usuario
                {
                    NombreUsuario = usuarioDto.NombreUsuario,
                    Contraseña = usuarioDto.Contraseña,
                    Empleado = empleado
                };

                usuarios.Add(usuario);
                _usuariosMap[usuarioDto.Id] = usuario;
            }

            return usuarios;
        }

        public List<Estado> GetEstados()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            var estados = new List<Estado>();
            
            foreach (var estadoDto in _datosDto.Estados)
            {
                var estado = new Estado
                {
                    Nombre = estadoDto.Nombre,
                    Descripcion = estadoDto.Descripcion,
                    Ambito = estadoDto.Ambito
                };

                estados.Add(estado);
                _estadosMap[estadoDto.Id] = estado;
            }

            return estados;
        }

        public List<Motivo> GetMotivos()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            var motivos = new List<Motivo>();
            
            foreach (var motivoDto in _datosDto.Motivos)
            {
                var motivo = new Motivo
                {
                    Id = motivoDto.Id,
                    Descripcion = motivoDto.Descripcion,
                    Comentario = motivoDto.Comentario
                };

                motivos.Add(motivo);
                _motivosMap[motivoDto.Id] = motivo;
            }

            return motivos;
        }

        public List<Sismografo> GetSismografos()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Asegurar que estados y motivos estén mapeados
            if (_estadosMap.Count == 0) GetEstados();
            if (_motivosMap.Count == 0) GetMotivos();

            var sismografos = new List<Sismografo>();
            
            foreach (var sismografoDto in _datosDto.Sismografos)
            {
                var cambiosEstado = MapCambiosEstado(sismografoDto.CambioEstado);

                var sismografo = new Sismografo
                {
                    FechaAdquisicion = sismografoDto.FechaAdquisicion,
                    IdentificadorSismografo = sismografoDto.IdentificadorSismografo,
                    NroSerie = sismografoDto.NroSerie,
                    CambioEstado = cambiosEstado
                };

                sismografos.Add(sismografo);
                _sismografosMap[sismografoDto.Id] = sismografo;
            }

            return sismografos;
        }

        public List<EstacionSismologica> GetEstacionesSismologicas()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Asegurar que las dependencias estén mapeadas
            if (_sismografosMap.Count == 0) GetSismografos();
            if (_empleadosMap.Count == 0) GetEmpleados();
            if (_estadosMap.Count == 0) GetEstados();

            var estaciones = new List<EstacionSismologica>();
            
            foreach (var estacionDto in _datosDto.EstacionesSismologicas)
            {
                var sismografo = _sismografosMap.GetValueOrDefault(estacionDto.SismografoId);
                var empleado = _empleadosMap.GetValueOrDefault(estacionDto.EmpleadoId);
                var estado = _estadosMap.GetValueOrDefault(estacionDto.EstadoId);

                if (sismografo == null || empleado == null || estado == null) continue;

                var estacion = new EstacionSismologica
                {
                    CodigoEstacion = estacionDto.CodigoEstacion,
                    DocumentoCertificacionAdq = estacionDto.DocumentoCertificacionAdq,
                    FechaSolicitudCertificacion = estacionDto.FechaSolicitudCertificacion,
                    Latitud = estacionDto.Latitud,
                    Nombre = estacionDto.Nombre,
                    NroCertificacionAdquirida = estacionDto.NroCertificacionAdquirida,
                    Sismografo = sismografo,
                    Empleado = empleado,
                    Estado = estado
                };

                estaciones.Add(estacion);
                _estacionesMap[estacionDto.Id] = estacion;
            }

            return estaciones;
        }

        public List<OrdenDeInspeccion> GetOrdenesDeInspeccion()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Asegurar que las dependencias estén mapeadas
            if (_estacionesMap.Count == 0) GetEstacionesSismologicas();
            if (_empleadosMap.Count == 0) GetEmpleados();
            if (_estadosMap.Count == 0) GetEstados();

            var ordenes = new List<OrdenDeInspeccion>();
            
            foreach (var ordenDto in _datosDto.OrdenesDeInspeccion)
            {
                var empleado = _empleadosMap.GetValueOrDefault(ordenDto.EmpleadoId);
                var estado = _estadosMap.GetValueOrDefault(ordenDto.EstadoId);
                var estacion = _estacionesMap.GetValueOrDefault(ordenDto.EstacionSismologicaId);

                if (empleado == null || estado == null || estacion == null) continue;

                var cambiosEstado = MapCambiosEstado(ordenDto.CambioEstado);

                var orden = new OrdenDeInspeccion
                {
                    NumeroOrden = ordenDto.NumeroOrden,
                    FechaHoraInicio = ordenDto.FechaHoraInicio,
                    FechaHoraFinalizacion = ordenDto.FechaHoraFinalizacion ?? DateTime.MinValue,
                    FechaHoraCierre = ordenDto.FechaHoraCierre ?? DateTime.MinValue,
                    ObservacionCierre = ordenDto.ObservacionCierre ?? string.Empty,
                    Empleado = empleado,
                    Estado = estado,
                    EstacionSismologica = estacion,
                    CambioEstado = cambiosEstado
                };

                ordenes.Add(orden);
            }

            return ordenes;
        }

        public List<Sesion> GetSesiones()
        {
            if (_datosDto == null) throw new InvalidOperationException("Datos no cargados");

            // Asegurar que usuarios estén mapeados
            if (_usuariosMap.Count == 0) GetUsuarios();

            var sesiones = new List<Sesion>();
            
            foreach (var sesionDto in _datosDto.Sesiones)
            {
                var usuario = _usuariosMap.GetValueOrDefault(sesionDto.UsuarioId);
                if (usuario == null) continue;

                var sesion = new Sesion
                {
                    FechaHoraInicio = sesionDto.FechaHoraInicio,
                    FechaHoraFin = sesionDto.FechaHoraFin ?? DateTime.MinValue,
                    Usuario = usuario
                };

                sesiones.Add(sesion);
            }

            return sesiones;
        }

        private void MapRoles()
        {
            if (_datosDto == null || _rolesMap.Count > 0) return;

            foreach (var rolDto in _datosDto.Roles)
            {
                var rol = new Rol
                {
                    Nombre = rolDto.Descripcion, // Nota: En el JSON es "Descripcion", en la entidad parece ser "Nombre"
                    Descripcion = rolDto.Descripcion
                };

                _rolesMap[rolDto.Id] = rol;
            }
        }

        private List<CambioEstado> MapCambiosEstado(List<CambioEstadoDto> cambiosDto)
        {
            var cambios = new List<CambioEstado>();
            
            foreach (var cambioDto in cambiosDto)
            {
                var estado = _estadosMap.GetValueOrDefault(cambioDto.EstadoId);
                if (estado == null) continue;

                var motivos = cambioDto.Motivos
                    .Select(motivoId => _motivosMap.GetValueOrDefault(motivoId))
                    .Where(motivo => motivo != null)
                    .ToList()!;

                var cambio = new CambioEstado
                {
                    FechaHoraInicio = cambioDto.FechaHoraInicio,
                    FechaHoraFin = cambioDto.FechaHoraFin ?? DateTime.MinValue,
                    Estado = estado,
                    Motivos = motivos
                };

                cambios.Add(cambio);
            }

            return cambios;
        }

        public void ClearCache()
        {
            _empleadosMap.Clear();
            _estadosMap.Clear();
            _rolesMap.Clear();
            _motivosMap.Clear();
            _usuariosMap.Clear();
            _sismografosMap.Clear();
            _estacionesMap.Clear();
        }
    }
} 