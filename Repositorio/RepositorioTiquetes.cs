using Parqueadero.Modelos;

namespace Parqueadero.Repositorio
{
    /// <summary>
    /// Repositorio en memoria que gestiona los tiquetes del parqueadero.
    /// Implementa las operaciones: crear, listar, buscar, eliminar.
    /// Usa List<T> como estructura de datos principal.
    /// </summary>
    public class RepositorioTiquetes
    {
        // Estructura de datos principal: List<Tiquete>
        private readonly List<Tiquete> _tiquetes;
        private readonly int _capacidadMaxima;

        public RepositorioTiquetes(int capacidadMaxima = 50)
        {
            if (capacidadMaxima <= 0)
                throw new ArgumentException("La capacidad debe ser mayor a 0.");

            _tiquetes = new List<Tiquete>();
            _capacidadMaxima = capacidadMaxima;
        }

        /// <summary>Cantidad de vehículos actualmente dentro del parqueadero.</summary>
        public int VehiculosActivos => _tiquetes.Count(t => t.EstaActivo);

        /// <summary>Espacios disponibles.</summary>
        public int EspaciosDisponibles => _capacidadMaxima - VehiculosActivos;

        public int CapacidadMaxima => _capacidadMaxima;

        // ─── CREAR ────────────────────────────────────────────────
        /// <summary>
        /// Registra la entrada de un vehículo creando un tiquete.
        /// Valida capacidad y placa duplicada.
        /// </summary>
        public Tiquete RegistrarEntrada(Vehiculo vehiculo)
        {
            if (vehiculo == null)
                throw new ArgumentNullException(nameof(vehiculo));

            if (EspaciosDisponibles <= 0)
                throw new InvalidOperationException($"Parqueadero lleno. Capacidad máxima: {_capacidadMaxima}.");

            if (BuscarVehiculoActivo(vehiculo.Placa) != null)
                throw new InvalidOperationException($"El vehículo con placa {vehiculo.Placa} ya se encuentra en el parqueadero.");

            var tiquete = new Tiquete(vehiculo);
            _tiquetes.Add(tiquete);
            return tiquete;
        }

        // ─── BUSCAR ───────────────────────────────────────────────
        /// <summary>Busca un tiquete activo por placa del vehículo.</summary>
        public Tiquete? BuscarVehiculoActivo(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa)) return null;
            return _tiquetes.FirstOrDefault(t =>
                t.EstaActivo &&
                t.Vehiculo.Placa.Equals(placa.ToUpper().Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Busca un tiquete por número (activo o histórico).</summary>
        public Tiquete? BuscarPorNumero(int numero)
        {
            return _tiquetes.FirstOrDefault(t => t.Numero == numero);
        }

        // ─── LISTAR ───────────────────────────────────────────────
        /// <summary>Retorna todos los vehículos actualmente dentro.</summary>
        public List<Tiquete> ListarActivos()
        {
            return _tiquetes.Where(t => t.EstaActivo).ToList();
        }

        /// <summary>Retorna el historial de vehículos que ya salieron.</summary>
        public List<Tiquete> ListarHistorial()
        {
            return _tiquetes.Where(t => !t.EstaActivo).OrderByDescending(t => t.HoraSalida).ToList();
        }

        /// <summary>Todos los tiquetes (activos e histórico).</summary>
        public List<Tiquete> ListarTodos()
        {
            return _tiquetes.ToList();
        }

        // ─── SALIDA ───────────────────────────────────────────────
        /// <summary>
        /// Registra la salida de un vehículo cerrando su tiquete.
        /// </summary>
        public Tiquete RegistrarSalida(string placa)
        {
            var tiquete = BuscarVehiculoActivo(placa);
            if (tiquete == null)
                throw new InvalidOperationException($"No se encontró ningún vehículo activo con placa {placa.ToUpper()}.");

            tiquete.Cerrar();
            return tiquete;
        }

        // ─── ELIMINAR ─────────────────────────────────────────────
        /// <summary>
        /// Elimina un tiquete del historial (solo los ya cerrados).
        /// Los activos no pueden eliminarse directamente.
        /// </summary>
        public bool EliminarDelHistorial(int numeroTiquete)
        {
            var tiquete = _tiquetes.FirstOrDefault(t => t.Numero == numeroTiquete && !t.EstaActivo);
            if (tiquete == null) return false;
            _tiquetes.Remove(tiquete);
            return true;
        }

        // ─── ESTADÍSTICAS ─────────────────────────────────────────
        /// <summary>Total recaudado en el día (tiquetes cerrados).</summary>
        public decimal TotalRecaudado()
        {
            return _tiquetes.Where(t => !t.EstaActivo).Sum(t => t.TotalCobrado);
        }

        /// <summary>Recaudo agrupado por tipo de vehículo.</summary>
        public Dictionary<string, decimal> RecaudoPorTipo()
        {
            return _tiquetes
                .Where(t => !t.EstaActivo)
                .GroupBy(t => t.Vehiculo.TipoVehiculo)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.TotalCobrado));
        }

        /// <summary>Conteo de vehículos activos por tipo.</summary>
        public Dictionary<string, int> ConteoActivosPorTipo()
        {
            return _tiquetes
                .Where(t => t.EstaActivo)
                .GroupBy(t => t.Vehiculo.TipoVehiculo)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
