namespace Parqueadero.Modelos
{
    /// <summary>
    /// Subclase concreta: Moto.
    /// Tarifa más económica que el carro.
    /// </summary>
    public class Moto : Vehiculo
    {
        private const decimal TARIFA_HORA = 1500m;

        public override string TipoVehiculo => "Moto";
        public override decimal TarifaPorHora => TARIFA_HORA;

        public Moto(string placa, string propietario)
            : base(placa, propietario)
        {
        }

        // Hereda CalcularCosto base (cobro estándar por horas, mínimo 1)
    }
}
