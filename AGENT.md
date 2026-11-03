# AGENT.md - InventoryApp

Este archivo brinda contexto a cualquier agente que trabaje en este proyecto. Contiene la arquitectura, estructura, tecnologías y cambios implementados.

---

## 1. Resumen del proyecto

**Nombre:** InventoryApp (GesCan)  
**Tecnología:** C# .NET 10  
**Tipo de app:** Escritorio (Windows Forms)  
**Base de datos:** SQLite local (sin servidor, sin internet)  
**Arquitectura:** Clean Architecture (4 capas) + DDD  
**Ubicación:** `C:\Users\SystemAncestral\source\repos\InventoryApp`

---

## 2. Arquitectura (Clean Architecture)

```
Inventory.slnx
└── src/
    ├── Inventory.Domain/                 # Núcleo: sin dependencias externas
    │   ├── Common/                       # Entity, DomainException, DomainEvent
    │   ├── Catalog/                      # Plu, Sku, Money, Quantity
    │   ├── Entities/                     # Company, User, Product
    │   ├── Identity/                     # EmployeeNumber, JobTitle, UserRole
    │   ├── Inventory/                    # StockItem, StockMovement, MovementType
    │   ├── Exceptions/                   # Excepciones de negocio
    │   └── Interfaces/                   # IProductRepository, IUserRepository,
    │                                     # ICompanyRepository, IStockItemRepository
    ├── Inventory.Application/            # Casos de uso y contratos
    │   ├── DTOs/                         # ProductDto, CreateProductDto, etc.
    │   ├── Interfaces/                   # IAuthService, ICompanyService,
    │   │                                 # IProductService, IPasswordHasher
    │   └── Services/                     # AuthService, CompanyService, ProductService
    ├── Inventory.Infrastructure/         # Implementaciones técnicas
    │   ├── Persistence/                  # AppDbContext, repositorios EF Core
    │   └── Services/                     # BCryptPasswordHasher
    └── Inventory.Desktop/                # Presentación (Windows Forms)
        ├── Program.cs                    # Punto de entrada
        ├── Session.cs                    # Sesión estática del usuario logueado
        ├── Di.cs                         # Composition root manual
        ├── DatabaseInitializer.cs        # Creación de esquema + seed demo
        ├── MainShell.cs                  # Ventana principal y navegación
        ├── Forms/
        │   ├── Login.cs                  # Autenticación
        │   └── ProductEditor.cs          # Alta/edición de productos
        ├── Controls/
        │   ├── UserProfilePanel.cs       # Tarjeta de perfil + empresa
        │   ├── TerminalHeader.cs         # Encabezado del terminal
        │   └── BottomNavigation.cs       # Barra inferior de navegación
        └── Views/
            ├── IView.cs                  # Contrato para vistas dinámicas
            ├── HomeView.cs               # Dashboard con métricas
            ├── ProductsView.cs           # CRUD de productos
            ├── StockView.cs              # Consulta de stock
            ├── SalesView.cs              # Placeholder
            ├── QueriesView.cs            # Búsqueda por PLU/SKU
            └── AdministrationView.cs     # Placeholder (botón deshabilitado)
```

### Flujo de dependencias entre capas

```
Desktop  -->  Application  -->  Domain
   \           ^               ^
    \          |               |
     --> Infrastructure -------/
```

- **Domain:** no depende de nada.
- **Application:** depende solo de Domain.
- **Infrastructure:** depende de Domain y Application.
- **Desktop:** depende de Application e Infrastructure (solo en `Di.cs`/`Program.cs`).

### Modelo DDD relevante

- `Product` es un agregado de **Catalog**. Usa value objects `Plu`, `Sku`, `Money`.
- `StockItem` es un agregado de **Inventory**. El stock **no** se edita directo:
  cambia por `StockMovement` (recepción, venta o ajuste).
- `User` es un agregado de **Identity**. Usa `EmployeeNumber`, `JobTitle` y `UserRole`.
- EF Core mapea los value objects mediante `HasConversion` en `AppDbContext`.

---

## 3. Pantalla principal (MainShell)

Distribución en 4 sectores:

| Sector | Ubicación | Descripción |
|--------|-----------|-------------|
| TopBar | Superior | "GesCan - Sistema de Gestión Empresarial" + buscador de productos |
| Panel Perfil | Izquierda | Usuario, legajo, puesto, rol y empresa |
| Terminal | Derecho | Encabezado fijo + contenido dinámico por módulo |
| BottomNavigation | Inferior | Inicio, Productos, Stock, Ventas, Consultas, Administración |

### Navegación

- Cada botón invoca `NavigateTo(moduleKey)` en `MainShell`.
- `MainShell` instancia la `IView` correspondiente y la coloca en `_contentPanel`.
- El panel de perfil, encabezado y barra inferior no se reconstruyen.
- El buscador superior filtra Productos y Stock al escribir; con Enter navega a Productos.
- Administración está visible pero **deshabilitado** (según especificación).

### Módulos disponibles

| Clave | Vista | Estado |
|-------|-------|--------|
| `inicio` | `HomeView` | Dashboard con productos, unidades, stock bajo y valor |
| `productos` | `ProductsView` | CRUD completo contra SQLite |
| `stock` | `StockView` | Listado real, búsqueda y filtro de stock bajo |
| `ventas` | `SalesView` | Placeholder visual |
| `consultas` | `QueriesView` | Búsqueda por PLU o SKU con ficha del producto |
| `administracion` | `AdministrationView` | Placeholder; botón deshabilitado |

---

## 4. Autenticación y sesión

- Pantalla de login: `Forms/Login.cs`
- Usuarios demo:
  - `admin` / `123456` — Administrador de Sistemas, legajo 1, rol Admin
  - `ecano` / `123456` — Programador, legajo 102, rol Employee
- Empresa demo: `Demo Company S.A.`
- Contraseñas hasheadas con **BCrypt** (`BCryptPasswordHasher`)
- Sesión estática en `Session.cs` (`User`, `Company`, `IsLoggedIn`, `Clear()`)

---

## 5. Base de datos

- Motor: **SQLite** vía Entity Framework Core 10
- Archivo: `inventory.db` en la carpeta de ejecución del `.exe`
- Migraciones: NO se usan; se llama `db.Database.EnsureCreated()` en `DatabaseInitializer.cs`
- Si se detecta el esquema viejo (sin columna `Plu`), la base demo se recrea automáticamente.
- Seed inicial:
  - 1 empresa
  - 2 usuarios (`admin` y `ecano`)
  - 12 productos con PLU, SKU, costo, precio, stock mínimo
  - 12 `StockItem` con movimientos de recepción inicial

### Tablas principales

`Companies`, `Users`, `Products`, `StockItems`, `StockMovements`.

---

## 6. Cómo ejecutar

```powershell
cd "C:\Users\SystemAncestral\source\repos\InventoryApp"
dotnet run --project src/Inventory.Desktop
```

O ejecutar el compilado:

```
src\Inventory.Desktop\bin\Debug\net10.0-windows\Inventory.Desktop.exe
```

Compilar:

```powershell
dotnet build Inventory.slnx
```

---

## 7. Cambios recientes

1. **Migración DDD completada:** se corrigieron namespaces de `Catalog`, se conectaron
   los value objects con EF Core y se actualizaron Application/Infrastructure/Desktop.
2. **Productos funcional:** alta, edición y eliminación desde `ProductsView`, con buscador,
   validaciones y resaltado de stock bajo.
3. **Stock funcional:** `StockView` muestra stock real, permite buscar por PLU/SKU y
   filtrar productos con stock bajo.
4. **Consultas funcional:** `QueriesView` busca por PLU o SKU y muestra ficha completa.
5. **Inicio con métricas:** `HomeView` muestra productos activos, unidades, stock bajo y
   valor del inventario al costo.
6. **UI renovada:** login con branding, tarjetas de perfil/empresa, grillas con estilo
   consistente, toolbar de acciones y placeholders visuales.
7. **Limpieza:** se eliminaron formularios y servicios legacy duplicados
   (`Main`, `ProductList`, `IInventoryService`, `InMemoryInventoryRepository`, `Class1`).

---

## 8. Notas para futuros agentes

- El proyecto usa **Windows Forms puro** (no WPF, no XAML, no MVVM). Los controles se crean en código C#.
- No agregar dependencias NuGet innecesarias sin justificación.
- Para agregar un nuevo módulo: crear clase que implemente `IView` en `Views/`, registrarla en
  `_viewFactory` de `MainShell.cs` y agregar un botón en `BottomNavigation.cs`.
- `Session.User` y `Session.Company` son estáticos; acceder con `Session.User` (no instanciar `Session`).
- Las vistas heredan de `UserControl` y se alojan dentro de `_contentPanel` de `MainShell`.
- `Di.cs` contiene las instancias singleton de repositorios y servicios.
- El stock se modifica únicamente mediante `StockMovement`; no agregar una propiedad `Stock`
  editable a `Product`.
- PLU y SKU son inmutables una vez creado el producto (identificadores de negocio).
- Las vistas **no** tocan `AppDbContext` ni entidades de dominio: solo DTOs y servicios.
