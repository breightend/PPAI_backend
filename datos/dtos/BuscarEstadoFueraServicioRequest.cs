using System;
using System.Collections.Generic;

namespace PPAI_backend.datos.dtos
{
    public class BuscarEstadoFueraServicioRequest
    {
        public int OrdenId { get; set; }
        public SismografoDTO Sismografo { get; set; } = new SismografoDTO();
    }
}
