using Parqueadero.Modelos;
using Parqueadero.Repositorio;

namespace Parqueadero.Servicios
{
    /// <summary>
    /// Servicio de negocio del parqueadero.
    /// Coordina la lógica entre la UI y el repositorio.
    /// Incluye validaciones adicionales y creación de vehículos mediante fábrica.
    /// </summary>
    public class ParqueaderoServicio
    {
        private readonly RepositorioTiquetes _repositorio;

        public ParqueaderoServicio(RepositorioTiquetes repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        // Exponer propiedades del repositorio
        public int VehiculosActivos => _repositorio.VehiculosActivos;
        public int EspaciosDisponibles => _repositorio.EspaciosDisponibles;
        public int CapacidadMaxima => _repositorio.CapacidadMaxima;

        /// <summary>
        /// Fábrica: crea el tipo de vehículo correcto según la selección del usuario.
        /// Valida la placa según formato colombiano básico.
        /// </summary>
        public Tiquete RegistrarEntrada(string tipoVehiculo, string placa, string propietario)
        {
            // Validar placa (formato básico: letras y números, 5-7 caracteres)
            placa = placa?.Trim().ToUpper() ?? "";
            if (string.IsNullOrWhiteSpace(placa))
                throw new ArgumentException("La placa es obligatoria.");
            if (placa.Length < 4 || placa.Length > 8)
                throw new ArgumentException("La placa debe tener entre 4 y 8 caracteres.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(placa, @"^[A-Z0-9\-]+$"))
                throw new ArgumentException("La placa solo puede contener letras, números y guiones.");

            // Validar propietario
            if (string.IsNullOrWhiteSpace(propietario))
                throw new ArgumentException("El nombre del propietario es obligatorio.");
            if (propietario.Trim().Length < 3)
                throw new ArgumentException("El nombre del propietario debe tener al menos 3 caracteres.");

            // Fábrica de vehículos (polimorfismo de creación)
            Vehiculo vehiculo = tipoVehiculo switch
            {
                "Carro"  => new Carro(placa, propietario),
                "Moto"   => new Moto(placa, propietario),
                "Camión" => new Camion(placa, propietario),
                _ => throw new ArgumentException($"Tipo de vehículo '{tipoVehiculo}' no reconocido.")
            };

            return _repositorio.RegistrarEntrada(vehiculo);
        }

        /// <summary>
        /// Registra la salida de un vehículo y retorna el tiquete cerrado.
        /// </summary>
        public Tiquete RegistrarSalida(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new ArgumentException("Debe ingresar una placa para buscar el vehículo.");

            return _repositorio.RegistrarSalida(placa);
        }

        /// <summary>
        /// Busca un vehículo activo por placa.
        /// </summary>
        public Tiquete? BuscarActivo(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa)) return null;
            return _repositorio.BuscarVehiculoActivo(placa);
        }

        public List<Tiquete> ListarActivos() => _repositorio.ListarActivos();
        public List<Tiquete> ListarHistorial() => _repositorio.ListarHistorial();
        public List<Tiquete> ListarTodos() => _repositorio.ListarTodos();

        public bool EliminarDelHistorial(int numeroTiquete) =>
            _repositorio.EliminarDelHistorial(numeroTiquete);

        public decimal TotalRecaudado() => _repositorio.TotalRecaudado();
        public Dictionary<string, decimal> RecaudoPorTipo() => _repositorio.RecaudoPorTipo();
        public Dictionary<string, int> ConteoActivosPorTipo() => _repositorio.ConteoActivosPorTipo();

        // Tarifas informativas
        public static Dictionary<string, decimal> ObtenerTarifas() => new()
        {
            { "Carro",  3500m },
            { "Moto",   1500m },
            { "Camión", 6000m }
        };
    }
}
