namespace Parqueadero.Modelos
{
    /// <summary>
    /// Clase Tiquete: representa el comprobante de estadía.
    /// Relación de composición con Vehiculo.
    /// </summary>
    public class Tiquete
    {
        private static int _consecutivo = 1;

        // Propiedades encapsuladas
        public int Numero { get; private set; }
        public Vehiculo Vehiculo { get; private set; }
        public DateTime HoraSalida { get; private set; }
        public decimal TotalCobrado { get; private set; }
        public bool EstaActivo { get; private set; }

        public Tiquete(Vehiculo vehiculo)
        {
            Numero = _consecutivo++;
            Vehiculo = vehiculo ?? throw new ArgumentNullException(nameof(vehiculo));
            EstaActivo = true;
        }

        /// <summary>
        /// Cierra el tiquete registrando la salida y calculando el costo total.
        /// Usa polimorfismo: llama al CalcularCosto de la subclase concreta.
        /// </summary>
        public void Cerrar()
        {
            if (!EstaActivo)
                throw new InvalidOperationException("Este tiquete ya fue cerrado.");

            HoraSalida = DateTime.Now;
            TotalCobrado = Vehiculo.CalcularCosto(HoraSalida); // polimorfismo
            EstaActivo = false;
        }

        /// <summary>
        /// Cierra con una hora de salida personalizada (para pruebas).
        /// </summary>
        public void Cerrar(DateTime horaSalidaManual)
        {
            if (!EstaActivo)
                throw new InvalidOperationException("Este tiquete ya fue cerrado.");

            HoraSalida = horaSalidaManual;
            TotalCobrado = Vehiculo.CalcularCosto(HoraSalida);
            EstaActivo = false;
        }

        public TimeSpan TiempoEstadia()
        {
            DateTime fin = EstaActivo ? DateTime.Now : HoraSalida;
            return fin - Vehiculo.HoraEntrada;
        }

        public override string ToString()
        {
            string estado = EstaActivo ? "ACTIVO" : $"CERRADO - Total: ${TotalCobrado:N0}";
            return $"Tiquete #{Numero:D4} | {Vehiculo.TipoVehiculo} {Vehiculo.Placa} | {estado}";
        }

        /// <summary>Genera un recibo completo en texto.</summary>
        public string GenerarRecibo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════╗");
            sb.AppendLine("║       PARQUEADERO SISTEMA             ║");
            sb.AppendLine("╠══════════════════════════════════════╣");
            sb.AppendLine($"║  Tiquete N°: {Numero:D4,-24}║");
            sb.AppendLine($"║  Tipo:       {Vehiculo.TipoVehiculo,-24}║");
            sb.AppendLine($"║  Placa:      {Vehiculo.Placa,-24}║");
            sb.AppendLine($"║  Propietario:{Vehiculo.Propietario.PadRight(24).Substring(0, 24)}║");
            sb.AppendLine($"║  Entrada:    {Vehiculo.HoraEntrada:dd/MM/yy HH:mm,-18}      ║");
            if (!EstaActivo)
            {
                sb.AppendLine($"║  Salida:     {HoraSalida:dd/MM/yy HH:mm,-18}      ║");
                sb.AppendLine($"║  Tiempo:     {TiempoEstadia():hh\\:mm,-24}║");
                sb.AppendLine($"║  Tarifa/h:   ${Vehiculo.TarifaPorHora:N0,-22}║");
                sb.AppendLine("╠══════════════════════════════════════╣");
                sb.AppendLine($"║  TOTAL:  ${TotalCobrado,28:N0} ║");
            }
            sb.AppendLine("╚══════════════════════════════════════╝");
            return sb.ToString();
        }

        // Reinicia el consecutivo (útil para pruebas)
        public static void ReiniciarConsecutivo() => _consecutivo = 1;
    }
}
