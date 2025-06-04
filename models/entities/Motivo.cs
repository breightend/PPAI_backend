using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.services;

namespace PPAI_backend.models.entities
{
    public class Motivo
    {
        public required int Id { get; set; }
        public required TipoMotivo TipoMotivo { get; set; }
        public string? Comentario { get; set; }

        //Controlar
        public string ObtenerMotivoFueraServicio()
        {
            return TipoMotivo.Descripcion;
        }

        // Método estático para obtener motivos de fuera de servicio
        public static List<Motivo> ObtenerMotivoFueraServicio(DataLoaderService dataLoader)
        {
            // Retornar todos los motivos disponibles desde el DataLoader
            return dataLoader.Motivos;
        }

        // Propiedad para obtener la descripción del motivo
        public string Descripcion => TipoMotivo.Descripcion;

    }
}