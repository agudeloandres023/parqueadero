using Parqueadero.Modelos;
using Parqueadero.Repositorio;
using Parqueadero.Servicios;

namespace Parqueadero.Formularios
{
    /// <summary>
    /// Formulario principal del sistema de parqueadero.
    /// Interfaz gráfica con pestañas para: Entrada, Salida, Vehículos Activos, Historial, Estadísticas.
    /// </summary>
    public partial class FormPrincipal : Form
    {
        private readonly ParqueaderoServicio _servicio;

        // Controles de la UI
        private TabControl tabControl;
        private TabPage tabEntrada, tabSalida, tabActivos, tabHistorial, tabEstadisticas;

        // Tab Entrada
        private ComboBox cmbTipo;
        private TextBox txtPlaca, txtPropietario;
        private Button btnEntrada;
        private Button btnIrASalida; 
        private Label lblEntradaInfo;

        // Tab Salida
        private TextBox txtPlacaSalida;
        private Button btnBuscarSalida, btnConfirmarSalida;
        private RichTextBox rtbTiquete;
        private Panel pnlInfoSalida;

        // Tab Activos
        private DataGridView dgvActivos;
        private Button btnRefrescarActivos;
        private Label lblTotalActivos;

        // Tab Historial
        private DataGridView dgvHistorial;
        private Button btnRefrescarHistorial, btnEliminarHistorial;
        private Label lblTotalHistorial;

        // Tab Estadísticas
        private RichTextBox rtbEstadisticas;
        private Button btnRefrescarEstadisticas;

        // Panel superior de estado
        private Panel pnlEstado;
        private Label lblEstadoCapacidad, lblEstadoDisponibles, lblEstadoActivos;

        private Tiquete? _tiqueteEnSalida; 

        public FormPrincipal()
        {
            var repositorio = new RepositorioTiquetes(capacidadMaxima: 20);
            _servicio = new ParqueaderoServicio(repositorio);

            InitializeComponent();
            CargarDatosIniciales();
        }

        private void InitializeComponent()
        {
            this.Text = "🅿️ Sistema de Parqueadero";
            this.Size = new Size(820, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(820, 600);
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9f);

            // ── 1. Panel superior de estado ─────────────────────────
            // Se crea e inicializa PRIMERO para asegurar su espacio en el tope
            pnlEstado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(30, 30, 50),
                Padding = new Padding(10, 5, 10, 5)
            };

            var lblTitulo = new Label
            {
                Text = "🅿️  PARQUEADERO SISTEMA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 14)
            };

            lblEstadoCapacidad = CrearLabelEstado("Capacidad: 20", 240);
            lblEstadoActivos   = CrearLabelEstado("Activos: 0", 380);
            lblEstadoDisponibles = CrearLabelEstado("Libres: 20", 500);

            pnlEstado.Controls.AddRange(new Control[] { lblTitulo, lblEstadoCapacidad, lblEstadoActivos, lblEstadoDisponibles });
            
            // IMPORTANTE: Agregar el panel de estado antes del TabControl
            this.Controls.Add(pnlEstado);

            // ── 2. TabControl ───────────────────────────────────────
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill, // Ahora llenará el espacio restante ABAJO del panel
                Font = new Font("Segoe UI", 9.5f),
                Padding = new Point(12, 6)
            };

            tabEntrada      = new TabPage("  ⬇ Entrada  ");
            tabSalida       = new TabPage("  ⬆ Salida  ");
            tabActivos      = new TabPage("  🚗 Activos  ");
            tabHistorial    = new TabPage("  📋 Historial  ");
            tabEstadisticas = new TabPage("  📊 Estadísticas  ");

            tabControl.TabPages.AddRange(new[] { tabEntrada, tabSalida, tabActivos, tabHistorial, tabEstadisticas });
            
            // Agregamos el TabControl al formulario
            this.Controls.Add(tabControl);

            // Inicializar cada pestaña
            InicializarTabEntrada();
            InicializarTabSalida();
            InicializarTabActivos();
            InicializarTabHistorial();
            InicializarTabEstadisticas();

            // Eventos de cambio de pestaña
            tabControl.SelectedIndexChanged += (s, e) => RefrescarPestañaActual();
        }

        // ─── Helpers de UI ────────────────────────────────────────
        private Label CrearLabelEstado(string texto, int x) => new Label
        {
            Text = texto,
            ForeColor = Color.FromArgb(180, 200, 255),
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            Location = new Point(x, 18)
        };

        private Panel CrearPanelFormulario()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Color.FromArgb(240, 242, 245)
            };
        }

        private Label CrearLabel(string texto, int x, int y) => new Label
        {
            Text = texto,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 70)
        };

        private TextBox CrearTextBox(int x, int y, int ancho = 280) => new TextBox
        {
            Location = new Point(x, y),
            Width = ancho,
            Height = 28,
            Font = new Font("Segoe UI", 10f),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };

        private Button CrearBoton(string texto, int x, int y, Color color) => new Button
        {
            Text = texto,
            Location = new Point(x, y),
            Width = 180,
            Height = 38,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        // ─── TAB ENTRADA ──────────────────────────────────────────
        private void InicializarTabEntrada()
        {
            var pnl = CrearPanelFormulario();

            var lblTitulo = new Label
            {
                Text = "Registrar Entrada de Vehículo",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblTipo = CrearLabel("Tipo de Vehículo:", 0, 45);
            cmbTipo = new ComboBox
            {
                Location = new Point(0, 68),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White
            };
            cmbTipo.Items.AddRange(new object[] { "Carro", "Moto", "Camión" });
            cmbTipo.SelectedIndex = 0;

            var pnlTarifas = new Panel
            {
                Location = new Point(320, 45),
                Size = new Size(280, 100),
                BackColor = Color.FromArgb(225, 235, 255),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblTarifasTitulo = new Label { Text = "Tarifas por hora:", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(8, 8), AutoSize = true, ForeColor = Color.FromArgb(30, 30, 100) };
            var lblTarifas = new Label
            {
                Text = "🚗 Carro:   $3.500  (+20% si >8h)\nItem Moto:    $1.500\n🚛 Camión:  $6.000  (+$2.000 fijo)",
                Location = new Point(8, 28),
                AutoSize = true,
                Font = new Font("Consolas", 8.5f),
                ForeColor = Color.FromArgb(40, 40, 80)
            };
            pnlTarifas.Controls.AddRange(new Control[] { lblTarifasTitulo, lblTarifas });

            var lblPlaca = CrearLabel("Placa:", 0, 115);
            txtPlaca = CrearTextBox(0, 138);
            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtPlaca.MaxLength = 8;
            var lblPlacaHint = new Label { Text = "Ej: ABC123 o ABC12D", Location = new Point(0, 168), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8f) };

            var lblProp = CrearLabel("Nombre del Propietario:", 0, 190);
            txtPropietario = CrearTextBox(0, 213);
            txtPropietario.MaxLength = 60;

            btnEntrada = CrearBoton("✅  Registrar Entrada", 0, 265, Color.FromArgb(34, 139, 34));
            btnEntrada.Click += BtnEntrada_Click;

            lblEntradaInfo = new Label
            {
                Location = new Point(0, 315),
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.Green
            };

            pnl.Controls.AddRange(new Control[] {
                lblTitulo, lblTipo, cmbTipo, pnlTarifas,
                lblPlaca, txtPlaca, lblPlacaHint,
                lblProp, txtPropietario,
                btnEntrada, lblEntradaInfo
            });

            tabEntrada.Controls.Add(pnl);
        }

        // ─── TAB SALIDA ───────────────────────────────────────────
        private void InicializarTabSalida()
        {
            var pnl = CrearPanelFormulario();

            var lblTitulo = new Label
            {
                Text = "Registrar Salida de Vehículo",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblPlaca = CrearLabel("Placa del Vehículo:", 0, 45);
            txtPlacaSalida = CrearTextBox(0, 68);
            txtPlacaSalida.CharacterCasing = CharacterCasing.Upper;
            txtPlacaSalida.MaxLength = 8;

            btnBuscarSalida = CrearBoton("🔍  Buscar Vehículo", 0, 110, Color.FromArgb(30, 100, 170));
            btnBuscarSalida.Click += BtnBuscarSalida_Click;

            pnlInfoSalida = new Panel
            {
                Location = new Point(0, 165),
                Size = new Size(680, 130),
                BackColor = Color.FromArgb(220, 240, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            rtbTiquete = new RichTextBox
            {
                Location = new Point(0, 310),
                Size = new Size(680, 170),
                Font = new Font("Consolas", 9f),
                ReadOnly = true,
                BackColor = Color.FromArgb(20, 20, 35),
                ForeColor = Color.FromArgb(180, 255, 180),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            btnConfirmarSalida = CrearBoton("⬆  Confirmar Salida", 0, 165, Color.FromArgb(200, 60, 40));
            btnConfirmarSalida.Visible = false;
            btnConfirmarSalida.Click += BtnConfirmarSalida_Click;

            pnl.Controls.AddRange(new Control[] {
                lblTitulo, lblPlaca, txtPlacaSalida,
                btnBuscarSalida, pnlInfoSalida, btnConfirmarSalida, rtbTiquete
            });

            tabSalida.Controls.Add(pnl);
        }

        // ─── TAB ACTIVOS ──────────────────────────────────────────
        private void InicializarTabActivos()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(240, 242, 245) };
            var lblTitulo = new Label { Text = "Vehículos Actualmente en el Parqueadero", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 80), AutoSize = true, Location = new Point(0, 12) };
            btnRefrescarActivos = new Button { Text = "🔄 Refrescar", Location = new Point(430, 10), Width = 120, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(50, 100, 200), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnRefrescarActivos.Click += (s, e) => RefrescarActivos();
            lblTotalActivos = new Label { Location = new Point(570, 15), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.DarkGreen };
            pnlTop.Controls.AddRange(new Control[] { lblTitulo, btnRefrescarActivos, lblTotalActivos });

            dgvActivos = CrearDataGridView();
            dgvActivos.Dock = DockStyle.Fill;
            ConfigurarColumnasActivos();

            pnl.Controls.Add(dgvActivos);
            pnl.Controls.Add(pnlTop);

            tabActivos.Controls.Add(pnl);
        }

        // ─── TAB HISTORIAL ────────────────────────────────────────
        private void InicializarTabHistorial()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            var lblTitulo = new Label { Text = "Historial de Salidas", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 80), AutoSize = true, Location = new Point(0, 12) };
            btnRefrescarHistorial = new Button { Text = "🔄 Refrescar", Location = new Point(320, 10), Width = 120, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(50, 100, 200), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnRefrescarHistorial.Click += (s, e) => RefrescarHistorial();
            btnEliminarHistorial = new Button { Text = "🗑 Eliminar", Location = new Point(455, 10), Width = 120, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(180, 40, 40), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnEliminarHistorial.Click += BtnEliminarHistorial_Click;
            lblTotalHistorial = new Label { Location = new Point(590, 15), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.DarkBlue };
            pnlTop.Controls.AddRange(new Control[] { lblTitulo, btnRefrescarHistorial, btnEliminarHistorial, lblTotalHistorial });

            dgvHistorial = CrearDataGridView();
            dgvHistorial.Dock = DockStyle.Fill;
            ConfigurarColumnasHistorial();

            pnl.Controls.Add(dgvHistorial);
            pnl.Controls.Add(pnlTop);
            tabHistorial.Controls.Add(pnl);
        }

        // ─── TAB ESTADÍSTICAS ─────────────────────────────────────
        private void InicializarTabEstadisticas()
        {
            var pnl = CrearPanelFormulario();

            var lblTitulo = new Label { Text = "Estadísticas del Parqueadero", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 80), AutoSize = true, Location = new Point(0, 0) };

            btnRefrescarEstadisticas = CrearBoton("🔄  Actualizar", 0, 40, Color.FromArgb(80, 60, 160));
            btnRefrescarEstadisticas.Width = 160;
            btnRefrescarEstadisticas.Click += (s, e) => RefrescarEstadisticas();

            rtbEstadisticas = new RichTextBox
            {
                Location = new Point(0, 95),
                Size = new Size(720, 380),
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BackColor = Color.FromArgb(20, 20, 35),
                ForeColor = Color.FromArgb(220, 220, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            pnl.Controls.AddRange(new Control[] { lblTitulo, btnRefrescarEstadisticas, rtbEstadisticas });
            tabEstadisticas.Controls.Add(pnl);
        }

        // ─── DataGridView helper ──────────────────────────────────
        private DataGridView CrearDataGridView()
        {
            var dgv = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(210, 215, 230),
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 },
                Font = new Font("Segoe UI", 9f)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 252);
            return dgv;
        }

        private void ConfigurarColumnasActivos()
        {
            dgvActivos.Columns.Add("Tiquete", "N° Tiquete");
            dgvActivos.Columns.Add("Tipo", "Tipo");
            dgvActivos.Columns.Add("Placa", "Placa");
            dgvActivos.Columns.Add("Propietario", "Propietario");
            dgvActivos.Columns.Add("Entrada", "Hora Entrada");
            dgvActivos.Columns.Add("Tiempo", "Tiempo Estadia");
            dgvActivos.Columns.Add("TarifaH", "Tarifa/Hora");
        }

        private void ConfigurarColumnasHistorial()
        {
            dgvHistorial.Columns.Add("Tiquete", "N° Tiquete");
            dgvHistorial.Columns.Add("Tipo", "Tipo");
            dgvHistorial.Columns.Add("Placa", "Placa");
            dgvHistorial.Columns.Add("Propietario", "Propietario");
            dgvHistorial.Columns.Add("Entrada", "Entrada");
            dgvHistorial.Columns.Add("Salida", "Salida");
            dgvHistorial.Columns.Add("Tiempo", "Duración");
            dgvHistorial.Columns.Add("Total", "Total Cobrado");
        }

        // ─── EVENTOS ──────────────────────────────────────────────
        private void BtnEntrada_Click(object? sender, EventArgs e)
        {
            lblEntradaInfo.Text = "";
            try
            {
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "";
                string placa = txtPlaca.Text.Trim();
                string propietario = txtPropietario.Text.Trim();

                var tiquete = _servicio.RegistrarEntrada(tipo, placa, propietario);

                lblEntradaInfo.ForeColor = Color.DarkGreen;
                lblEntradaInfo.Text = $"✅ Entrada registrada. Tiquete #{tiquete.Numero:D4}  |  {tipo}: {placa}";

                txtPlaca.Text = "";
                txtPropietario.Text = "";
                cmbTipo.SelectedIndex = 0;
                txtPlaca.Focus();

                ActualizarEstado();
            }
            catch (Exception ex)
            {
                lblEntradaInfo.ForeColor = Color.DarkRed;
                lblEntradaInfo.Text = $"❌ {ex.Message}";
            }
        }

        private void BtnBuscarSalida_Click(object? sender, EventArgs e)
        {
            pnlInfoSalida.Visible = false;
            rtbTiquete.Visible = false;
            btnConfirmarSalida.Visible = false;
            _tiqueteEnSalida = null;

            try
            {
                string placa = txtPlacaSalida.Text.Trim();
                if (string.IsNullOrWhiteSpace(placa))
                    throw new ArgumentException("Ingrese una placa para buscar.");

                var tiquete = _servicio.BuscarActivo(placa);
                if (tiquete == null)
                {
                    MostrarMensaje($"No se encontró ningún vehículo activo con placa {placa.ToUpper()}.", "No encontrado", MessageBoxIcon.Warning);
                    return;
                }

                _tiqueteEnSalida = tiquete;

                pnlInfoSalida.Controls.Clear();
                pnlInfoSalida.Visible = true;

                var info = new Label
                {
                    Text = $"🚗  {tiquete.Vehiculo.TipoVehiculo}  |  Placa: {tiquete.Vehiculo.Placa}  |  " +
                           $"Propietario: {tiquete.Vehiculo.Propietario}\n" +
                           $"⏱  Entrada: {tiquete.Vehiculo.HoraEntrada:dd/MM/yyyy HH:mm}  |  " +
                           $"Tiempo: {tiquete.TiempoEstadia():hh\\:mm} h  |  " +
                           $"Costo aprox: ${tiquete.Vehiculo.CalcularCosto(DateTime.Now):N0}",
                    Location = new Point(10, 10),
                    Size = new Size(655, 55),
                    Font = new Font("Segoe UI", 9.5f),
                    ForeColor = Color.FromArgb(20, 50, 120)
                };
                pnlInfoSalida.Controls.Add(info);

                btnConfirmarSalida.Location = new Point(0, 310);
                btnConfirmarSalida.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnConfirmarSalida_Click(object? sender, EventArgs e)
        {
            if (_tiqueteEnSalida == null) return;

            try
            {
                var tiquete = _servicio.RegistrarSalida(_tiqueteEnSalida.Vehiculo.Placa);

                rtbTiquete.Text = tiquete.GenerarRecibo();
                rtbTiquete.Visible = true;

                pnlInfoSalida.Visible = false;
                btnConfirmarSalida.Visible = false;
                txtPlacaSalida.Text = "";
                _tiqueteEnSalida = null;

                ActualizarEstado();
                MostrarMensaje($"✅ Salida registrada. Total cobrado: ${tiquete.TotalCobrado:N0}", "Salida exitosa", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarHistorial_Click(object? sender, EventArgs e)
        {
            if (dgvHistorial.SelectedRows.Count == 0)
            {
                MostrarMensaje("Seleccione un registro del historial para eliminar.", "Aviso", MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvHistorial.SelectedRows[0];
            string numStr = fila.Cells["Tiquete"].Value?.ToString()?.Replace("#", "") ?? "0";
            if (!int.TryParse(numStr, out int num))
            {
                MostrarMensaje("No se pudo identificar el tiquete.", "Error", MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show($"¿Eliminar el tiquete #{num:D4} del historial?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            bool ok = _servicio.EliminarDelHistorial(num);
            if (ok)
            {
                RefrescarHistorial();
                MostrarMensaje("Registro eliminado del historial.", "Eliminado", MessageBoxIcon.Information);
            }
            else
            {
                MostrarMensaje("No se pudo eliminar. Solo se pueden eliminar tiquetes cerrados.", "Error", MessageBoxIcon.Error);
            }
        }

        // ─── REFRESCO DE DATOS ────────────────────────────────────
        private void RefrescarActivos()
        {
            dgvActivos.Rows.Clear();
            var activos = _servicio.ListarActivos();
            foreach (var t in activos)
            {
                dgvActivos.Rows.Add(
                    $"#{t.Numero:D4}",
                    t.Vehiculo.TipoVehiculo,
                    t.Vehiculo.Placa,
                    t.Vehiculo.Propietario,
                    t.Vehiculo.HoraEntrada.ToString("dd/MM/yy HH:mm"),
                    $"{t.TiempoEstadia():hh\\:mm} h",
                    $"${t.Vehiculo.TarifaPorHora:N0}"
                );
            }
            lblTotalActivos.Text = $"Total: {activos.Count} vehículo(s)";
        }

        private void RefrescarHistorial()
        {
            dgvHistorial.Rows.Clear();
            var historial = _servicio.ListarHistorial();
            foreach (var t in historial)
            {
                dgvHistorial.Rows.Add(
                    $"#{t.Numero:D4}",
                    t.Vehiculo.TipoVehiculo,
                    t.Vehiculo.Placa,
                    t.Vehiculo.Propietario,
                    t.Vehiculo.HoraEntrada.ToString("dd/MM/yy HH:mm"),
                    t.HoraSalida.ToString("dd/MM/yy HH:mm"),
                    $"{t.TiempoEstadia():hh\\:mm} h",
                    $"${t.TotalCobrado:N0}"
                );
            }
            lblTotalHistorial.Text = $"Total recaudado: ${_servicio.TotalRecaudado():N0}";
        }

        private void RefrescarEstadisticas()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════╗");
            sb.AppendLine("║          ESTADÍSTICAS DEL PARQUEADERO        ║");
            sb.AppendLine($"║         {DateTime.Now:dd/MM/yyyy HH:mm:ss}                 ║");
            sb.AppendLine("╠══════════════════════════════════════════════╣");
            sb.AppendLine($"║  Capacidad total:    {_servicio.CapacidadMaxima,-26}║");
            sb.AppendLine($"║  Vehículos activos:  {_servicio.VehiculosActivos,-26}║");
            sb.AppendLine($"║  Espacios libres:    {_servicio.EspaciosDisponibles,-26}║");
            sb.AppendLine("╠══════════════════════════════════════════════╣");
            sb.AppendLine("║  VEHÍCULOS ACTIVOS POR TIPO                  ║");

            var porTipo = _servicio.ConteoActivosPorTipo();
            if (porTipo.Count == 0)
                sb.AppendLine("║    (No hay vehículos activos)                ║");
            else
                foreach (var kv in porTipo)
                    sb.AppendLine($"║    {kv.Key,-18} : {kv.Value,-23}║");

            sb.AppendLine("╠══════════════════════════════════════════════╣");
            sb.AppendLine("║  RECAUDO POR TIPO (sesión actual)            ║");

            var recaudo = _servicio.RecaudoPorTipo();
            if (recaudo.Count == 0)
                sb.AppendLine("║    (Sin ventas aún)                          ║");
            else
                foreach (var kv in recaudo)
                    sb.AppendLine($"║    {kv.Key,-18} : ${kv.Value,20:N0} ║");

            sb.AppendLine("╠══════════════════════════════════════════════╣");
            sb.AppendLine($"║  TOTAL RECAUDADO:    ${_servicio.TotalRecaudado(),23:N0} ║");
            sb.AppendLine("╚══════════════════════════════════════════════╝");

            rtbEstadisticas.Text = sb.ToString();
        }

        private void ActualizarEstado()
        {
            lblEstadoCapacidad.Text   = $"Capacidad: {_servicio.CapacidadMaxima}";
            lblEstadoActivos.Text     = $"Activos: {_servicio.VehiculosActivos}";
            lblEstadoDisponibles.Text = $"Libres: {_servicio.EspaciosDisponibles}";
        }

        private void RefrescarPestañaActual()
        {
            switch (tabControl.SelectedIndex)
            {
                case 2: RefrescarActivos(); break;
                case 3: RefrescarHistorial(); break;
                case 4: RefrescarEstadisticas(); break;
            }
        }

        private void CargarDatosIniciales()
        {
            ActualizarEstado();
        }

        private void MostrarMensaje(string texto, string titulo, MessageBoxIcon icono)
        {
            MessageBox.Show(texto, titulo, MessageBoxButtons.OK, icono);
        }
    }
}