using PPAI_backend.models.interfaces;

public class ObservadorPantallaCRSS : IObservadorNotificacion
{
    public void Actualizar(OrdenDeInspeccion orden)
    {
        Console.WriteLine($"[PANTALLA CRSS] La orden de inspección número {orden.NumeroOrden} ha sido cerrada.");
    }
}