using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class TipoMotivo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}