namespace Parqueadero.Modelos
{
    /// <summary>
    /// Subclase concreta: Carro.
    /// Hereda de Vehiculo e implementa su tarifa específica.
    /// </summary>
    public class Carro : Vehiculo
    {
        // Tarifa fija para carros (pesos colombianos por hora)
        private const decimal TARIFA_HORA = 3500m;

        public override string TipoVehiculo => "Carro";
        public override decimal TarifaPorHora => TARIFA_HORA;

        public Carro(string placa, string propietario)
            : base(placa, propietario)
        {
        }

        /// <summary>
        /// Los carros tienen un recargo del 20% si superan las 8 horas.
        /// Demuestra override de polimorfismo.
        /// </summary>
        public override decimal CalcularCosto(DateTime horaSalida)
        {
            double horas = (horaSalida - HoraEntrada).TotalHours;
            double horasCobradas = Math.Ceiling(horas < 1 ? 1 : horas);

            decimal costoBase = (decimal)horasCobradas * TarifaPorHora;

            // Recargo por estadía prolongada (más de 8 horas)
            if (horasCobradas > 8)
                costoBase *= 1.20m; // 20% recargo

            return costoBase;
        }
    }
}
