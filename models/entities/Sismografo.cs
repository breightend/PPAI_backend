using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sismografo
    {
        // Atributos:
        public DateTime FechaAdquisicion { get; set; }
        public int IdentificadorSismografo { get; set; }
        public int NroSerie { get; set; }
        public required List<CambioEstado> CambioEstado { get; set; }
        

        // Metodos: 
        public int getIdentificadorSismografo()
        {
            return IdentificadorSismografo;
        }

        public void setEstadoActual(CambioEstado nuevoEstado)
        {
            CambioEstado.Add(nuevoEstado);
        }
    
    }
}