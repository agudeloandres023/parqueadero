namespace Parqueadero.Modelos
{
    /// <summary>
    /// Subclase concreta: Camión.
    /// Tarifa más alta por ocupar más espacio.
    /// Tiene un costo fijo de ingreso adicional.
    /// </summary>
    public class Camion : Vehiculo
    {
        private const decimal TARIFA_HORA = 6000m;
        private const decimal COSTO_INGRESO = 2000m; // cargo fijo al ingresar

        public override string TipoVehiculo => "Camión";
        public override decimal TarifaPorHora => TARIFA_HORA;

        public Camion(string placa, string propietario)
            : base(placa, propietario)
        {
        }

        /// <summary>
        /// Camión cobra tarifa por hora MÁS un costo fijo de ingreso.
        /// Demuestra override de CalcularCosto con lógica propia.
        /// </summary>
        public override decimal CalcularCosto(DateTime horaSalida)
        {
            double horas = (horaSalida - HoraEntrada).TotalHours;
            double horasCobradas = Math.Ceiling(horas < 1 ? 1 : horas);
            return (decimal)horasCobradas * TarifaPorHora + COSTO_INGRESO;
        }
    }
}
