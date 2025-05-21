using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sismografo
    {
        public DateTime FechaAdquisicion {  get; set; }
        public int IdentificadorSismografo { get; set; }
        public int NroSerie {  get; set; }
        public required CambioEstado CambioEstado { get; set; }
    }
}