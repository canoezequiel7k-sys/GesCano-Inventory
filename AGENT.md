# AGENT.md - InventoryApp

Este archivo brinda contexto a cualquier agente que trabaje en este proyecto. Contiene la arquitectura, estructura, tecnologías y cambios implementados.

---

## 1. Resumen del proyecto

**Nombre:** InventoryApp (GesCan)  
**Tecnología:** C# .NET 10  
**Tipo de app:** Escritorio (Windows Forms)  
**Base de datos:** SQLite local (sin servidor, sin internet)  
**Arquitectura:** Clean Architecture (4 capas)  
**Ubicación:** `C:\Users\SystemAncestral\source\repos\InventoryApp`

---

## 2. Arquitectura (Clean Architecture)

```
Inventory.slnx
└── src/
    ├── Inventory.Domain/             # Capa de dominio (sin dependencias externas)
    │   ├── Entities/
    │   │   ├── Company.cs
    │   │   ├── User.cs
    │   │   └── Product.cs
    │   ├── Exceptions/
    │   │   ├── EntityNotFoundException.cs
    │   │   ├── DuplicateEntityException.cs
    │   │   └── InvalidCredentialsException.cs
    │   └── Interfaces/
    │       ├── ICompanyRepository.cs
    │       ├── IUserRepository.cs
    │       └── IProductRepository.cs
    ├── Inventory.Application/        # Casos de uso y contratos de servicio
    │   ├── DTOs/
    │   │   ├── LoginRequest.cs
    │   │   ├── LoginResponse.cs
    │   │   ├── CompanyDto.cs
    │   │   ├── UserDto.cs
    │   │   ├── ProductDto.cs
    │   │   ├── CreateProductDto.cs
    │   │   └── UpdateProductDto.cs
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── ICompanyService.cs
    │   │   ├── IProductService.cs
    │   │   └── IPasswordHasher.cs
    │   └── Services/
    │       ├── AuthService.cs
    │       ├── CompanyService.cs
    │       └── ProductService.cs
    ├── Inventory.Infrastructure/     # Implementaciones técnicas
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs         # DbContext con SQLite (EF Core)
    │   │   ├── CompanyRepository.cs
    │   │   ├── UserRepository.cs
    │   │   └── ProductRepository.cs
    │   └── Services/
    │       └── BCryptPasswordHasher.cs
    └── Inventory.Desktop/            # Presentación (Windows Forms)
        ├── Program.cs                # Punto de entrada
        ├── Session.cs                # Sesión estática del usuario logueado
        ├── Di.cs                     # Inyección de dependencias manual
        ├── DatabaseInitializer.cs    # Seed de datos + EnsureCreated
        ├── MainShell.cs              # Ventana principal con layout persistente
        ├── Forms/
        │   └── Login.cs              # Pantalla de autenticación
        ├── Controls/
        │   ├── UserProfilePanel.cs   # Tarjeta de perfil (fila: foto + datos)
        │   ├── TerminalHeader.cs     # Encabezado del terminal de trabajo
        │   └── BottomNavigation.cs   # Barra inferior de navegación (6 módulos)
        └── Views/
            ├── IView.cs              # Contrato para vistas dinámicas
            ├── HomeView.cs           # Módulo: Inicio
            ├── ProductsView.cs       # Módulo: Productos (conectado a BD)
            ├── StockView.cs          # Módulo: Stock (placeholder)
            ├── SalesView.cs          # Módulo: Ventas (placeholder)
            ├── QueriesView.cs        # Módulo: Consultas (placeholder)
            └── AdministrationView.cs # Módulo: Administración (placeholder)
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
- **Desktop:** depende de Application e Infrastructure.

---

## 3. Pantalla principal (MainShell)

Distribución en 4 sectores:

| Sector | Ubicación | Descripción |
|--------|-----------|-------------|
| TopBar | Superior | Nombre del sistema "GesCan - Sistema de Gestión Empresarial" + buscador (TextBox, inactivo por ahora) |
| Panel Perfil | Izquierda | Tarjeta en fila: foto (izq) + nombre, legajo, sucursal y tipo (der). Siempre visible |
| Terminal | Derecho | Encabezado fijo + área de contenido dinámico (cambia por módulo) |
| BottomNavigation | Inferior | 6 botones: Inicio, Productos, Stock, Ventas, Consultas, Administración |

### Navegación

- Cada botón de la barra inferior invoca `NavigateTo(moduleKey)` en `MainShell`.
- `MainShell` instancia la `IView` correspondiente y la coloca en `_contentPanel` (el área dinámica del terminal).
- El panel de perfil, encabezado y barra inferior **nunca se reconstruyen**; solo cambia el contenido central.
- Los botones reflejan visualmente el módulo seleccionado (fondo azul, texto blanco).

### Módulos disponibles

| Clave | Vista | Estado |
|-------|-------|--------|
| `inicio` | `HomeView` | Placeholder |
| `productos` | `ProductsView` | Conectado a `ProductService` (datos reales de SQLite) |
| `stock` | `StockView` | Placeholder |
| `ventas` | `SalesView` | Placeholder |
| `consultas` | `QueriesView` | Placeholder |
| `administracion` | `AdministrationView` | Placeholder |

---

## 4. Autenticación y sesión

- Pantalla de login: `Forms/Login.cs`
- Usuario de prueba pre-cargado: `admin` / `123456`
- Empresa de prueba: `Demo Company S.A.`
- Contraseñas hasheadas con **BCrypt** (`BCryptPasswordHasher`)
- Sesión estática en `Session.cs` (`User`, `Company`, `IsLoggedIn`, `Clear()`)

---

## 5. Base de datos

- Motor: **SQLite** vía Entity Framework Core 10
- Archivo: `inventory.db` (se crea automáticamente en la carpeta de ejecución del `.exe`)
- Migraciones: NO se usan; se llama `db.Database.EnsureCreated()` en `DatabaseInitializer.cs`
- Seed inicial: 1 empresa, 1 usuario admin, 5 productos de ejemplo

---

## 6. Cómo ejecutar

```powershell
cd "C:\Users\SystemAncestral\source\repos\InventoryApp"
dotnet run --project src/Inventory.Desktop
```

O ejecutar directamente el compilado:

```
src\Inventory.Desktop\bin\Debug\net10.0-windows\Inventory.Desktop.exe
```

---

## 7. Cambios recientes (última implementación)

1. **Panel de perfil (`UserProfilePanel.cs`):** Rediseñado como tarjeta superior en modo fila. Foto a la izquierda (72x72), y a la derecha apilado verticalmente: "Usuario del Sistema" y debajo "Legajo: 80 / Sucursal: 80 / Tipo: [Role]".

2. **Barra superior (`MainShell.cs`):** 
   - Cambiado "GESCOM" por **"GesCan - Sistema de Gestión Empresarial"**.
   - Agregado **buscador** (`TextBox` con PlaceholderText "Buscar...") alineado a la derecha. Actualmente sin funcionalidad, listo para futuro.

3. **Arquitectura de navegación:** Implementada con `IView` contract, `MainShell` como layout persistente, y `BottomNavigation` con evento `ModuleSelected`.

4. **Eliminado:** Proyecto de consola (`Inventory.ConsoleApp`) que fue reemplazado por la app de escritorio.

---

## 8. Notas para futuros agentes

- El proyecto usa **Windows Forms puro** (no WPF, no XAML, no MVVM). Los controles se crean en código C#.
- No agregar dependencias NuGet innecesarias sin justificación.
- Para agregar un nuevo módulo: crear clase que implemente `IView` en `Views/`, registrarla en `_viewFactory` de `MainShell.cs`, y agregar un botón en `BottomNavigation.cs`.
- `Session.User` y `Session.Company` son estáticos; acceder con `Session.User` (no instanciar `Session`).
- Las vistas heredan de `UserControl` y se alojan dentro del panel `_contentPanel` de `MainShell`.
- `Di.cs` contiene las instancias singleton de repositorios y servicios.

---
