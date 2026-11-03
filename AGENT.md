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
        ├── DatabaseInitializer.cs        # Creación de esquema + seed demo (314 productos)
        ├── MainShell.cs                  # Ventana principal y navegación
        ├── Seeding/
        │   └── DemoCatalog.cs            # Catálogo demo de supermercado (314 productos)
        ├── Imaging/
        │   └── ProductImages.cs          # Guardado, carga y placeholder de imágenes
        ├── Forms/
        │   ├── Login.cs                  # Autenticación
        │   ├── ProductEditor.cs          # Alta de productos nuevos (con imagen)
        │   └── ProductPreview.cs         # Detalle de producto de solo lectura
        ├── Controls/
        │   ├── UserProfilePanel.cs       # Tarjeta de perfil + empresa
        │   ├── TerminalHeader.cs         # Encabezado del terminal
        │   └── BottomNavigation.cs       # Barra inferior de navegación
        └── Views/
            ├── IView.cs                  # Contrato para vistas dinámicas
            ├── HomeView.cs               # Dashboard con métricas
            ├── ProductsView.cs           # Catálogo de solo lectura + alta/eliminación
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
| `productos` | `ProductsView` | Catálogo de solo lectura: ver detalle, alta y eliminación |
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
- Si se detecta el esquema viejo (sin columna `Plu` o `ImagePath`), la base demo se recrea automáticamente.
- Seed inicial:
  - 1 empresa
  - 2 usuarios (`admin` y `ecano`)
  - 314 productos reales de supermercado argentino en 11 categorías
  - 314 `StockItem` con movimientos de recepción inicial
  - 25 productos quedan a propósito con stock bajo para probar la alerta
- Catálogo demo (`Seeding/DemoCatalog.cs`): Bebidas, Cereales y desayuno, Golosinas,
  Panadería, Almacén, Lácteos, Limpieza, Perfumería, Congelados, Carnes y Frutas y Verduras.
- PLU generados de `1001` a `1314`; SKU por categoría (`BEB-001`, `ALM-015`, etc.).
- Costo demo calculado al 72% del precio de venta; precios de referencia entre `$500` y `$22.000` ARS.

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

## 7. Cómo agregar productos

1. Iniciar sesión y entrar a **Productos**.
2. Presionar **Nuevo** (arriba a la derecha de la grilla).
3. Completar PLU, SKU, nombre, categoría, costo, precio, stock mínimo y stock inicial.
4. Opcionalmente, presionar **Elegir imagen...** para seleccionar una foto del producto.
   La imagen se copia a la carpeta `images/` junto al `.exe` y se guarda su ruta relativa.
5. Presionar **Guardar**.

- **Ver detalle** (o doble clic sobre una fila) abre `ProductPreview`, una ficha de
  solo lectura con imagen, nombre, PLU, SKU, categoría, costo, precio, stock actual,
  stock mínimo, estado y valor del stock.
- Los productos existentes **no se pueden editar**: solo consultar o eliminar.
- Si un producto no tiene imagen, se muestra un placeholder con sus iniciales y un
  color asociado a la categoría.

---

## 8. Cambios recientes

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
8. **Catálogo demo ampliado:** 314 productos reales de supermercado en 11 categorías,
   con marcas conocidas y precios de referencia argentinos (`Seeding/DemoCatalog.cs`).
9. **Imagen de producto:** nuevo campo `ImagePath` en dominio, DTOs, EF Core y servicios.
   `Imaging/ProductImages.cs` guarda, carga y genera placeholders por categoría.
10. **Edición reemplazada por vista previa:** `ProductEditor` es solo para alta;
    `ProductPreview` muestra la ficha completa en modo solo lectura.
11. **Productos de solo lectura:** `ProductsView` usa **Ver detalle**, doble clic para
    el preview y mantiene **Nuevo**, **Eliminar** y **Actualizar**.

---

## 9. Notas para futuros agentes

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
- Los productos existentes no se editan: `ProductPreview` es solo lectura. Para cambiarlos
  hay que eliminarlos y volver a crearlos con **Nuevo**.
- Las imágenes se guardan en `images/` bajo `AppContext.BaseDirectory`; usar siempre
  `ProductImages.Save/Load/Resolve` y no rutas absolutas en la base.
- Las vistas **no** tocan `AppDbContext` ni entidades de dominio: solo DTOs y servicios.
