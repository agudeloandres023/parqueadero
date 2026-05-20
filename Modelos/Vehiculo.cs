namespace Parqueadero.Modelos
{
    /// <summary>
    /// Clase abstracta base que representa un vehículo en el parqueadero.
    /// Implementa encapsulamiento y define el contrato para todas las subclases.
    /// </summary>
    public abstract class Vehiculo
    {
        // Atributos encapsulados
        private string _placa;
        private string _propietario;
        private DateTime _horaEntrada;

        // Propiedades con validación
        public string Placa
        {
            get => _placa;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La placa no puede estar vacía.");
                _placa = value.ToUpper().Trim();
            }
        }

        public string Propietario
        {
            get => _propietario;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre del propietario no puede estar vacío.");
                _propietario = value.Trim();
            }
        }

        public DateTime HoraEntrada
        {
            get => _horaEntrada;
            set => _horaEntrada = value;
        }

        // Propiedad abstracta: cada subclase define su tipo
        public abstract string TipoVehiculo { get; }

        // Método abstracto: cada subclase calcula su tarifa por hora
        public abstract decimal TarifaPorHora { get; }

        // Constructor
        protected Vehiculo(string placa, string propietario)
        {
            Placa = placa;
            Propietario = propietario;
            HoraEntrada = DateTime.Now;
        }

        /// <summary>
        /// Calcula el costo total según el tiempo transcurrido.
        /// Polimorfismo: usa la TarifaPorHora de cada subclase.
        /// Mínimo 1 hora de cobro.
        /// </summary>
        public virtual decimal CalcularCosto(DateTime horaSalida)
        {
            if (horaSalida < HoraEntrada)
                throw new ArgumentException("La hora de salida no puede ser anterior a la de entrada.");

            double horas = (horaSalida - HoraEntrada).TotalHours;
            double horasCobradas = Math.Ceiling(horas < 1 ? 1 : horas); // mínimo 1 hora
            return (decimal)horasCobradas * TarifaPorHora;
        }

        /// <summary>
        /// Descripción del vehículo para mostrar en la UI.
        /// </summary>
        public override string ToString()
        {
            return $"[{TipoVehiculo}] Placa: {Placa} | Propietario: {Propietario} | Entrada: {HoraEntrada:dd/MM/yyyy HH:mm}";
        }
    }
}
