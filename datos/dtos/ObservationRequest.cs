using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.datos.dtos
{
    public class ObservationRequest
    {
        public int OrderId { get; set; }
        public string Observation { get; set; }
    }
}