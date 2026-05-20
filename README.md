# 🅿️ Sistema de Parqueadero — C# Windows Forms

> Aplicación de escritorio para gestión de entradas, salidas y tarifas de parqueadero, desarrollada en **C# .NET 8** con **Windows Forms**.

---

## 📋 Tabla de Contenidos

1. [Descripción](#descripción)
2. [Características](#características)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Arquitectura y POO](#arquitectura-y-poo)
5. [Tarifas](#tarifas)
6. [Requisitos y Ejecución](#requisitos-y-ejecución)
7. [Diagramas UML](#diagramas-uml)
8. [Validaciones](#validaciones)

---

## Descripción

Sistema de escritorio que permite a un operador de parqueadero:
- Registrar la **entrada** de vehículos (Carro, Moto, Camión).
- Registrar la **salida** y calcular automáticamente el cobro según el tiempo de estadía.
- Consultar los **vehículos activos** dentro del parqueadero.
- Ver el **historial** de vehículos que ya salieron con sus cobros.
- Consultar **estadísticas** de ocupación y recaudo.

---

## Características

| Funcionalidad | Detalle |
|---|---|
| Entradas y salidas | Registro con hora automática |
| Tarifas diferenciadas | Por tipo de vehículo y tiempo |
| Tiquete numerado | Con recibo detallado en pantalla |
| Listado en tiempo real | Grid actualizable de activos e historial |
| Estadísticas | Recaudo por tipo, ocupación, totales |
| Validaciones | Placa, campos vacíos, duplicados, capacidad |
| Eliminación de historial | Con confirmación |

---

## Estructura del Proyecto

```
Parqueadero/
├── Program.cs                          # Punto de entrada
├── Parqueadero.csproj                  # Configuración del proyecto
│
├── Modelos/                            # Capa de dominio
│   ├── Vehiculo.cs                     # Clase abstracta base
│   ├── Carro.cs                        # Subclase concreta
│   ├── Moto.cs                         # Subclase concreta
│   ├── Camion.cs                       # Subclase concreta
│   └── Tiquete.cs                      # Clase de comprobante
│
├── Repositorio/                        # Capa de datos en memoria
│   └── RepositorioTiquetes.cs          # CRUD sobre List<Tiquete>
│
├── Servicios/                          # Lógica de negocio
│   └── ParqueaderoServicio.cs          # Fábrica + validaciones
│
├── Formularios/                        # Interfaz gráfica
│   └── FormPrincipal.cs                # Ventana principal (5 pestañas)
│
└── Diagramas/
    └── DiagramaClases.puml             # UML en PlantUML
```

---

## Arquitectura y POO

### Herencia

```
Vehiculo (abstracta)
    ├── Carro    → tarifa $3.500/h, recargo 20% si > 8 horas
    ├── Moto     → tarifa $1.500/h
    └── Camion   → tarifa $6.000/h + $2.000 fijo de ingreso
```

### Polimorfismo

El método `CalcularCosto(DateTime horaSalida)` está declarado `virtual` en `Vehiculo` y se hace `override` en `Carro` y `Camion`. Cuando `Tiquete.Cerrar()` llama a `vehiculo.CalcularCosto(...)`, en **tiempo de ejecución** se ejecuta la implementación del tipo concreto:

```csharp
// En Tiquete.Cerrar() — polimorfismo real
TotalCobrado = Vehiculo.CalcularCosto(HoraSalida);
```

### Encapsulamiento

Todos los atributos relevantes son privados con propiedades que incluyen validación:

```csharp
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
```

### Estructura de datos

`RepositorioTiquetes` usa `List<Tiquete>` con LINQ para filtrado, búsqueda y agrupación:

```csharp
private readonly List<Tiquete> _tiquetes;

// Buscar activo por placa
_tiquetes.FirstOrDefault(t => t.EstaActivo && t.Vehiculo.Placa == placa);

// Recaudo por tipo
_tiquetes.Where(t => !t.EstaActivo)
         .GroupBy(t => t.Vehiculo.TipoVehiculo)
         .ToDictionary(g => g.Key, g => g.Sum(t => t.TotalCobrado));
```

---

## Tarifas

| Vehículo | Tarifa / hora | Regla especial |
|---|---|---|
| 🚗 Carro | $3.500 COP | +20% si estadía > 8 horas |
| 🏍 Moto | $1.500 COP | Ninguna |
| 🚛 Camión | $6.000 COP | +$2.000 cargo fijo de ingreso |

> Mínimo: **1 hora** de cobro. Las fracciones se redondean hacia arriba.

---

## Requisitos y Ejecución

### Requisitos

- .NET 8 SDK (Windows)
- Visual Studio 2022 o superior (con carga de trabajo Windows Forms)
- Sistema operativo: Windows 10/11

### Clonar y ejecutar

```bash
git clone https://github.com/tu-usuario/parqueadero.git
cd parqueadero
dotnet run
```

### Desde Visual Studio

1. Abrir `Parqueadero.csproj`
2. Pulsar `F5` o `Ctrl+F5`

---

## Diagramas UML

El diagrama completo de clases está en `Diagramas/DiagramaClases.puml`.

Para visualizarlo:
- Usar la extensión **PlantUML** en VS Code
- O pegar el contenido en [plantuml.com/plantuml](https://www.plantuml.com/plantuml)

### Resumen de relaciones

```
Vehiculo ◄─── Carro, Moto, Camion        (herencia)
Tiquete ──●── Vehiculo                    (composición)
RepositorioTiquetes ──◇── List<Tiquete>  (agregación)
ParqueaderoServicio ──── RepositorioTiquetes
FormPrincipal ──── ParqueaderoServicio
```

---

## Validaciones

| Campo | Regla |
|---|---|
| Placa | Obligatoria, 4-8 caracteres, solo letras/números/guión |
| Propietario | Obligatorio, mínimo 3 caracteres |
| Tipo vehículo | Selección de lista (Carro / Moto / Camión) |
| Capacidad | Máximo 20 vehículos simultáneos |
| Duplicados | No se permite la misma placa dos veces activa |
| Salida | Solo se puede sacar un vehículo que está activo |
| Eliminar historial | Solo tiquetes ya cerrados |

---

## Operaciones CRUD sobre Tiquete

| Operación | Método |
|---|---|
| **Crear** | `RepositorioTiquetes.RegistrarEntrada(vehiculo)` |
| **Listar** | `ListarActivos()`, `ListarHistorial()`, `ListarTodos()` |
| **Buscar** | `BuscarVehiculoActivo(placa)`, `BuscarPorNumero(num)` |
| **Eliminar** | `EliminarDelHistorial(numeroTiquete)` |

---

*Desarrollado como proyecto académico — POO con C# y Windows Forms.*
