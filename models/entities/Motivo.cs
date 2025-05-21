using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.models.entities;

namespace PPAI_backend.models.entities
{
    public class Motivo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        //Relaciones
        public required MotivoTipo MotivoTipo { get; set; }
    }
}