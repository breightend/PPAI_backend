using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.services;

namespace PPAI_backend.models.entities
{
    public class MotivoFueraDeServicio
    {
        public required int Id { get; set; }
        public required TipoMotivo TipoMotivo { get; set; }
        public string? Comentario { get; set; }


        public MotivoFueraDeServicio ObtenerMotivoFueraServicio()
        {
            var descripcion = TipoMotivo.Descripcion;
            return this;
        }


        public string Descripcion => TipoMotivo.Descripcion;

    }
}