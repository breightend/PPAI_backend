using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.services;

namespace PPAI_backend.models.entities
{
    public class MotivoFueraDeServicio
    {
        [Key]
        public required int Id { get; set; }

        
        public required TipoMotivo TipoMotivo { get; set; }
        public string? Comentario { get; set; }
        
        //TODO: cambiar esto luego por el motivo fuera de servicio.
        public MotivoFueraDeServicio ObtenerMotivoFueraServicio()
        {
            var descripcion = TipoMotivo.Descripcion;
            return this;
        }


        public string Descripcion => TipoMotivo.Descripcion;

    }
}