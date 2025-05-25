using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.services;

namespace PPAI_backend.models.entities
{
    public class Motivo
    {
        public int Id { get; set; }
        public required string Descripcion { get; set; }
        public string? Comentario { get; set; }
        
        public static List<Motivo> ObtenerMotivoFueraServicio(DataLoaderService dataLoader)
        {
           return dataLoader.Motivos;
        }



    }
}