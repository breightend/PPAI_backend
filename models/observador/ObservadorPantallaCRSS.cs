using PPAI_backend.models.entities;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.monitores;

public class ObservadorPantallaCRSS : IObservadorNotificacion
{
    public static void PantallaCCRS()
    {
        PantallaCCRS pantallaCRSS = new PantallaCCRS();
        pantallaCRSS.SetFecha(DateTime.Now);
        pantallaCRSS.SetMensaje("Orden de inspección cerrada.");
    }


    public void Actualizar(OrdenDeInspeccion orden)
    {
        Console.WriteLine($"[PANTALLA CRSS] La orden de inspección número {orden.NumeroOrden} ha sido cerrada.");
    }
}