# Especificación técnica: Inventario robusto con DDD y Clean Architecture

**Proyecto:** Inventory (Domain, Application, Infrastructure, Desktop) **Destinatario:** agente de implementación **Autor del encargo:** Ezequiel Cano (legajo 102, Programador)

---

## 1. Propósito y rol del agente

Este documento es la especificación completa para ampliar el sistema de inventario existente. El agente que lo ejecute debe actuar como **arquitecto de software senior** que trabaja con **Domain-Driven Design (DDD)** y respeta de forma estricta los principios de **Clean Architecture** y el código escalable.

Objetivos del encargo:

1. Construir un **inventario grande y robusto** (datos de demostración) para poder probar el sistema.
2. **Stock:** buscar por PLU o SKU y consultar el stock real de la mercadería.
3. **Ventas:** ver un resumen de mercadería vendida por día, mes, año o rango.
4. **Consultas:** dado un PLU, ver información del producto, cantidad de unidades, última venta, última recepción y datos relacionados.
5. **Administración:** no disponible por ahora (botón visible pero deshabilitado).
6. **Usuarios:** conservar `admin` y crear a **Ezequiel Cano, legajo 102, puesto Programador**.
7. **Código anotado** para que el autor pueda leerlo y entenderlo después.

## 2. Estado actual del proyecto (punto de partida)

- **Arquitectura:** capas separadas `Inventory.Domain`, `Inventory.Application`, `Inventory.Infrastructure`, `Inventory.Desktop`.
- **Base de datos:** SQLite con `AppDbContext` (Entity Framework Core), timestamps automáticos.
- **UI:** Windows Forms con vistas navegables (`IView`), `MainShell` con `TableLayoutPanel`, `BottomNavigation` de 6 módulos (inicio, productos, stock, ventas, consultas, administración).
- **Autenticación:** BCrypt (`BCryptPasswordHasher` en Infrastructure, `IPasswordHasher` en Application), login por empresa.
- **Entidades existentes:** `Company`, `Product`, `User` (multi-empresa: cada producto y usuario tiene `CompanyId`).
- **Excepciones de dominio:** `DuplicateEntityException`, `EntityNotFoundException`, `InvalidCredentialsException`, `ProductNotFoundException`.
- **Servicios existentes:** `ProductService`, `CompanyService`, `AuthService`.
- **Repositorios existentes:** `ProductRepository`, `UserRepository`, `CompanyRepository`, `InMemoryInventoryRepository` (para pruebas).
- **Sesión:** `Session.User` y `Session.Company` estáticos.
- **Pendiente conocido:** `Di.cs` (contenedor de inyección de dependencias) no revisado; módulos Stock, Ventas y Consultas muestran "en desarrollo"; sin pruebas unitarias; sin validación de DTOs; sin manejo global de errores.

> **Instrucción previa obligatoria:** antes de escribir código, el agente debe **leer el código real** (`Product.cs`, `User.cs`, `AppDbContext.cs`, `Di.cs`, `Program.cs`, `Session`, las vistas y `BottomNavigation`) y adaptar esta especificación a los nombres y convenciones existentes. Si algo de este documento contradice el código real, debe priorizar no romper lo que funciona y dejar la discrepancia anotada en `docs/DECISIONES.md`.

## 3. Reglas de arquitectura (no negociables)

### 3.1 Regla de dependencias

`Desktop → Application → Domain` e `Infrastructure → Application → Domain`. Las dependencias solo apuntan hacia adentro.

| Capa | Puede conocer | No puede conocer |
| --- | --- | --- |
| Domain | Nada (solo la librería base de .NET) | EF Core, SQLite, DTOs, WinForms, BCrypt |
| Application | Domain | EF Core, SQLite, WinForms |
| Infrastructure | Application, Domain | WinForms |
| Desktop | Application; Infrastructure **solo** en `Di.cs`/`Program.cs` (composition root) | `AppDbContext`, entidades en las vistas |

### 3.2 Reglas derivadas

1. Las vistas **nunca** tocan `AppDbContext` ni entidades de dominio. Solo llaman a interfaces de servicio y reciben **DTOs**.
2. Las **consultas** (lectura) se definen como interfaces en Application (`IStockQueries`, `ISalesQueries`, `IProductInquiryQueries`) y se implementan en Infrastructure con proyecciones directas a DTO usando `AsNoTracking`.
3. Los **comandos** (escritura) pasan siempre por los agregados del dominio. Está prohibido modificar estado saltándose un agregado.
4. **Prohibido** `DateTime.Now`/`DateTime.UtcNow` en Domain y Application. Se usa `IClock`.
5. **Prohibido** usar `Session` estática dentro de Application o Domain. Se usa `ICurrentUser` (la implementación de Desktop puede apoyarse en `Session` mientras se migra).
6. Los errores **esperados** (código inexistente, stock insuficiente) se devuelven con un tipo `Result`; las **excepciones** quedan para violaciones de invariantes y errores inesperados.
7. Ninguna regla de negocio vive en la UI ni en Infrastructure.

## 4. Convención de anotaciones (el autor debe poder estudiar el código)

Todo tipo público lleva un comentario `///` con tres datos: **qué es, en qué capa vive y por qué vive ahí**.

```csharp
/// <summary>
/// CAPA: Domain. Registro inmutable de un cambio de stock.
/// POR QUÉ ACÁ: es una regla del negocio ("el stock solo cambia por movimientos"),
/// no depende de ninguna tecnología.
/// </summary>
public sealed class StockMovement { ... }
```

Marcadores en línea:

- `// [DDD]` para conceptos de dominio: agregado, invariante, value object, evento.
- `// [ARQ]` para decisiones de arquitectura (ej.: `// [ARQ] Esta interfaz está en Application y la implementa Infrastructure: inversión de dependencias.`).
- `// [NEGOCIO]` para reglas del negocio y su motivo.

Cada carpeta de bounded context incluye un `README.md` corto (5 a 10 líneas) que explique qué contiene y cómo se relaciona con los demás contextos.

Idioma: comentarios y documentación en **español**; identificadores de código en **inglés**, salvo términos del lenguaje ubicuo que se mantienen (PLU, SKU).

## 5. Lenguaje ubicuo (glosario)

El agente debe crear `docs/GLOSARIO.md` con estas definiciones y usar exactamente estos términos en el código.

| Término | Definición |
| --- | --- |
| **Empresa (Company)** | Organización dueña de productos, usuarios y movimientos. Todo dato pertenece a una empresa. |
| **Producto (Product)** | Artículo que la empresa compra y/o vende. |
| **PLU** | Código corto de venta del producto (tipo balanza/caja). Único por empresa. |
| **SKU** | Código interno de inventario. Único por empresa. |
| **Stock real** | Cantidad física actualmente disponible, derivada de los movimientos. |
| **Stock mínimo** | Umbral bajo el cual el producto se considera en nivel bajo. |
| **Movimiento de stock** | Registro inmutable de una entrada o salida de mercadería, con motivo, usuario y fecha. |
| **Recepción (Receipt)** | Ingreso de mercadería al inventario. |
| **Venta (Sale)** | Salida de mercadería por venta, con precio fijado al momento. |
| **Ajuste (Adjustment)** | Corrección de stock con motivo obligatorio (merma, conteo, rotura). |
| **Legajo (EmployeeNumber)** | Número de empleado, único por empresa. |
| **Puesto (JobTitle)** | Cargo del usuario (ej.: Programador). |

> Supuestos: PLU y SKU son campos distintos. Si el autor indica lo contrario, se ajusta el glosario y el modelo.

## 6. Bounded contexts

Un solo ensamblado por capa, pero con **carpetas por contexto** dentro de cada capa:

- **Catalog:** qué es un producto (PLU, SKU, precio, costo, categoría).
- **Inventory:** cuánto hay y cómo se movió (stock y movimientos).
- **Sales:** qué se vendió, cuándo y quién.
- **Identity:** empresas, usuarios, legajo, puesto, rol, autenticación.
- **Consultas:** **no es un contexto**. Es un *read model* que cruza datos de varios contextos solo para lectura.

### Estructura de carpetas objetivo

```
Inventory.Domain/
  Common/       Entity, AggregateRoot, IDomainEvent, Money, DomainException
  Catalog/      Product, Plu, Sku
  Inventory/    StockItem, StockMovement, MovementType, Quantity, eventos
  Sales/        Sale, SaleItem, SaleRegistered
  Identity/     Company, User, EmployeeNumber, UserRole
inventory.Application/
  Abstractions/ IUnitOfWork, IClock, ICurrentUser, Result, IPasswordHasher
  Catalog/      Commands, Queries, DTOs
  Inventory/    Commands (ReceiveStock, AdjustStock), Queries (GetStockByCode)
  Sales/        Commands (RegisterSale), Queries (GetSalesSummary)
  Inquiries/    Queries (GetProductInquiry)
  Identity/     Auth, DTOs
Inventory.Infrastructure/
  Persistence/  AppDbContext, Configurations/, Migrations/, repositorios
  Queries/      implementaciones de IStockQueries, ISalesQueries, etc.
  Seeding/      DemoDataSeeder
  Security/     BCryptPasswordHasher
  Time/         SystemClock
Inventory.Desktop/
  Views/  Navigation/  Composition/ (Di.cs)  Session/
Inventory.Tests/
  Domain/  Application/  Infrastructure/
```

## 7. Modelo de dominio

### 7.1 Value objects (inmutables, se validan al crearse)

| Value object | Regla |
| --- | --- |
| `Plu` | No vacío, solo dígitos, longitud entre 1 y 8 (confirmar con el autor). Se normaliza (trim). |
| `Sku` | No vacío, alfanumérico con guiones, máximo 32 caracteres, se normaliza a mayúsculas. |
| `Quantity` | Decimal o entero no negativo. Define si admite fracciones (productos por peso). |
| `Money` | Monto ≥ 0 con moneda única de la empresa. |
| `EmployeeNumber` | Entero positivo. |
| `DateRange` | `From <= To`; construcción desde día, mes, año o rango libre. |

`// [DDD]` Un value object inválido **no puede existir**: la validación ocurre en el constructor o en un método de fábrica.

### 7.2 Agregados e invariantes

| Agregado (raíz) | Contiene | Invariantes |
| --- | --- | --- |
| `Product` | `Plu`, `Sku`, `Description`, `Category`, `UnitCost`, `UnitPrice`, `MinimumStock` | PLU y SKU únicos por empresa; precio y costo ≥ 0; descripción obligatoria. |
| `StockItem` | `ProductId`, `CompanyId`, saldo actual, lista de `StockMovement` nuevos, versión de concurrencia | El saldo nunca es negativo; todo cambio de saldo genera un movimiento con tipo, motivo, usuario y fecha; saldo = suma de movimientos. |
| `Sale` | `SaleItem`s (producto, cantidad, precio unitario al momento de la venta) | Al menos un ítem; cantidad > 0; precio fijado al registrar; total calculado por el agregado; no se modifica una venta registrada. |
| `User` | `EmployeeNumber`, `JobTitle`, `Role`, hash de contraseña | Legajo único por empresa; hash obligatorio; rol dentro de `UserRole` (`Admin`, `Employee`). |

`// [DDD]` `StockItem` guarda el **saldo** y registra cada **movimiento**. No se recalcula sumando todo el historial en cada consulta (se vuelve lento con los años). Un test garantiza que `saldo == suma de movimientos`. La concurrencia se controla con un token optimista (`RowVersion` o contador) para que dos ventas simultáneas no rompan el saldo.

### 7.3 Movimientos

`StockMovement`: `Id`, `CompanyId`, `ProductId`, `Type` (`Receipt`, `Sale`, `Adjustment`), `Quantity` con signo (entradas positivas, salidas negativas), `OccurredAt`, `UserId`, `Reference` (por ejemplo, id de venta o remito), `Reason` (obligatorio en ajustes).

Inmutable: no hay métodos que modifiquen un movimiento existente. Una corrección se hace con un nuevo movimiento de ajuste.

### 7.4 Eventos de dominio

- `SaleRegistered` → el contexto Inventory genera las salidas de stock.
- `StockReceived`.
- `StockBelowMinimumReached`.

Se despachan **en memoria dentro de la misma transacción** (un `IDomainEventDispatcher` implementado en Infrastructure, invocado desde el `IUnitOfWork`). `// [ARQ]` Más adelante puede migrarse a un patrón outbox sin tocar el dominio.

### 7.5 Excepciones

Se conservan las existentes y se agregan: `InsufficientStockException`, `InvalidQuantityException`, `InvalidCodeException` (para PLU/SKU). Todas heredan de una base `DomainException`.

## 8. Capa Application

### 8.1 Puertos (abstracciones)

- `IUnitOfWork`: confirma una transacción y despacha eventos.
- `IClock`: hora actual (`SystemClock` en producción, `FakeClock` en tests).
- `ICurrentUser`: `UserId`, `CompanyId`, `Role`, `DisplayName`.
- `Result` y `Result<T>`: éxito o error con código y mensaje legible. Errores tipados (ej.: `ProductNotFound`, `InsufficientStock`).
- `IPasswordHasher` (ya existe).

### 8.2 Comandos (escritura)

| Comando | Comportamiento |
| --- | --- |
| `RegisterSale` | Crea una `Sale` con sus ítems usando el precio vigente del producto; dispara `SaleRegistered`; descuenta stock. Falla con `InsufficientStock` si no alcanza. |
| `ReceiveStock` | Registra una recepción (entrada) para un producto, con referencia (remito) opcional. |
| `AdjustStock` | Ajusta con motivo obligatorio; puede ser positivo o negativo sin dejar el saldo negativo. |

Todos validan con **FluentValidation** (PLU/SKU no vacío, cantidad > 0, motivo en ajustes) y devuelven `Result`.

### 8.3 Consultas (lectura, CQRS liviano)

| Consulta | Entrada | Salida |
| --- | --- | --- |
| `GetStockByCode` | Texto PLU o SKU | `StockItemDto`: PLU, SKU, descripción, categoría, stock real, mínimo, estado (`Ok`, `Low`, `OutOfStock`), última actualización. |
| `GetSalesSummary` | `SalesPeriod` (`Day`, `Month`, `Year`, `Custom`) + fecha/rango | `SalesSummaryDto`: total de unidades, total en dinero, cantidad de ventas; filas por producto (`SalesSummaryRowDto`) y serie por día/mes; top de productos. |
| `GetProductInquiry` | PLU | `ProductInquiryDto`: datos del producto, unidades actuales, **última venta** (fecha, cantidad, usuario), **última recepción** (fecha, cantidad, referencia), movimientos recientes (últimos N). Las fechas son nulas si no existen. |

`GetStockByCode`: decide si el texto es PLU o SKU de forma determinista (primero PLU exacto, luego SKU exacto, siempre dentro de la empresa del `ICurrentUser`). Si hay ambigüedad se prioriza PLU y se documenta con `// [NEGOCIO]`.

### 8.4 Aislamiento multi-empresa

Toda consulta y comando se filtra por la `CompanyId` del `ICurrentUser`. En Infrastructure se aplica además un **filtro global** (`HasQueryFilter`) basado en la empresa actual. Un usuario de la empresa A **nunca** debe ver datos de la empresa B (hay un test específico).

## 9. Capa Infrastructure

1. **Configuraciones EF** (`IEntityTypeConfiguration<T>`) por entidad; value objects mapeados con *value converters*.
2. **Índices únicos:** `(CompanyId, Plu)`, `(CompanyId, Sku)`, `(CompanyId, EmployeeNumber)`, `(CompanyId, Username)`. Se captura la excepción de unicidad de EF y se traduce a `DuplicateEntityException`.
3. Índices de consulta: `StockMovement (CompanyId, ProductId, OccurredAt)` y `Sale (CompanyId, OccurredAt)`.
4. **Repositorios** por agregado (`IProductRepository`, `IStockItemRepository`, `ISaleRepository`, `IUserRepository`, `ICompanyRepository`).
5. **Implementaciones de consultas** (`IStockQueries`, `ISalesQueries`, `IProductInquiryQueries`) con proyecciones a DTO y `AsNoTracking`.
6. **Migración** de EF Core que parte de la base actual sin perder datos existentes. Si hay incompatibilidad, documentar el plan de recreación de la base de demo.
7. `SystemClock`, `IDomainEventDispatcher`, `UnitOfWork`.
8. `InMemoryInventoryRepository` e `IInventoryRepository`: evaluar solapamiento con `IProductRepository`; consolidar en una única abstracción y dejar el criterio documentado.

### 9.1 DemoDataSeeder (inventario grande para pruebas)

Principio: el seeder usa **los mismos comandos de Application** para cargar datos, de modo que los datos de prueba también cumplen las reglas del dominio.

Contenido:

- **\~500 productos** en **\~10 categorías** (almacén, bebidas, lácteos, limpieza, perfumería, panadería, fiambrería, congelados, verdulería por peso, librería), con PLU y SKU realistas y únicos, y precios/costos coherentes (precio > costo).
- **\~6 meses de historia:** recepciones periódicas y ventas distribuidas con estacionalidad simple (más ventas viernes y sábados).
- **Semilla fija** del generador aleatorio (ej.: `new Random(20260101)`), para resultados reproducibles.
- **Casos borde a propósito** (marcados en una lista pública del seeder para que los tests los usen):
  - productos con stock bajo (por debajo del mínimo),
  - productos con stock en cero,
  - productos **sin ventas**,
  - productos **sin recepciones**,
  - productos con muchas ventas el mismo día,
  - productos inactivos o con precio cero si el modelo lo permite.
- **Idempotente:** si ya existen datos, no duplica nada.
- Solo corre en modo demostración (bandera de configuración), nunca por defecto en producción.

### 9.2 Usuarios semilla

| Usuario | Datos | Rol |
| --- | --- | --- |
| `admin` | El que ya existe; se conserva sin cambios | Admin |
| **Ezequiel Cano** | Legajo **102**, puesto **Programador**, usuario `ecano` (confirmar) | Employee (confirmar si debe ser Admin) |

- Contraseñas hasheadas con `BCryptPasswordHasher`.
- Para demostración se usa una contraseña temporal definida en configuración local (**no** hardcodeada en el repositorio de producción) y debe quedar documentado que hay que cambiarla.
- El login mantiene mensaje de error **genérico** (no distingue usuario inexistente de contraseña incorrecta).

## 10. Capa Desktop (vistas)

Principios: las vistas no contienen lógica de negocio; llaman a servicios de Application con `async/await`; muestran errores del `Result` en un label o `MessageBox` (sin excepciones sueltas); todo se resuelve desde `Di.cs`.

| Vista | Funcionalidad |
| --- | --- |
| **StockView** | Cuadro de búsqueda (PLU o SKU, Enter para buscar). Muestra una tarjeta con descripción, categoría, **stock real**, mínimo y estado con color (verde Ok, amarillo Low, rojo OutOfStock). Mensaje claro si no se encuentra. |
| **SalesView** | Selector de período (día, mes, año, rango). Grilla con resumen por producto, totales de unidades e importe y cantidad de ventas. Ordenable por unidades o importe. |
| **QueriesView** | Búsqueda por PLU. Ficha con información del producto, unidades actuales, **última venta**, **última recepción** y movimientos recientes. Muestra "Sin registros" cuando no hay datos. |
| **AdministrationView** | **Deshabilitada.** El botón de `BottomNavigation` queda visible pero sin navegar, con tooltip "No disponible por ahora". Se implementa con una bandera de disponibilidad por módulo (`ModuleAvailability`) para activarlo después sin reescribir. |

Mejoras obligatorias en Desktop asociadas al encargo:

- Completar `Di.cs` con `Microsoft.Extensions.DependencyInjection` y construir el `ServiceProvider` en `Program.cs`; las vistas se resuelven desde el contenedor.
- Manejo global de excepciones (`Application.ThreadException` y `AppDomain.UnhandledException`) con logging a archivo (Serilog).
- Implementar `ICurrentUser` en Desktop sobre la sesión actual.

## 11. Pruebas

Proyecto `Inventory.Tests` (xUnit). Los tests de Domain no usan base de datos; los de Application/Infrastructure usan **SQLite en memoria** con el seeder.

**Domain**

- Value objects: valores válidos e inválidos de `Plu`, `Sku`, `Quantity`, `Money`, `DateRange`.
- `StockItem`: el saldo nunca queda negativo; saldo = suma de movimientos; ajustes sin motivo fallan.
- `Sale`: sin ítems falla; total calculado correctamente; precio queda fijado.

**Application / Infrastructure**

- Buscar por PLU y por SKU: existe, no existe, de otra empresa.
- Stock = recepciones − ventas ± ajustes con datos conocidos.
- `RegisterSale` descuenta stock y falla con stock insuficiente.
- Resumen de ventas por día y por mes usando `FakeClock` (incluye bordes: medianoche, fin de mes).
- Última venta y última recepción en producto sin movimientos (devuelve nulos, no error).
- **Aislamiento multi-empresa:** usuario de la empresa A nunca ve datos de la B.
- Duplicados: la segunda inserción de PLU/SKU/legajo falla por índice único y se traduce a `DuplicateEntityException`.
- Seeder: es idempotente y crea los usuarios `admin` y Ezequiel Cano.

## 12. Plan de ejecución por fases

Cada fase termina con la solución **compilando** y los tests en verde, y con un commit propio.

| Fase | Contenido | Criterio de aceptación |
| --- | --- | --- |
| **0. Reconocimiento** | Leer el código real; crear `docs/GLOSARIO.md` y `docs/DECISIONES.md`; reportar discrepancias con esta especificación. | Documentos creados; sin cambios de código funcional. |
| **1. Domain + tests** | Common, value objects, agregados, eventos, excepciones, carpetas por contexto con README. | Tests de Domain en verde, sin referencias a EF ni WinForms. |
| **2. Application** | Puertos, `Result`, comandos, consultas (interfaces), DTOs, validadores. | Compila; ninguna referencia a Infrastructure ni a EF. |
| **3. Infrastructure** | Configuraciones EF, índices, repositorios, consultas, migración, `UnitOfWork`, `SystemClock`. | Migración aplicada; tests de integración en verde. |
| **4. Seeder y usuarios** | `DemoDataSeeder`, casos borde, `admin` + Ezequiel Cano. | Seeder idempotente; \~500 productos y \~6 meses de historia. |
| **5. DI y soporte en Desktop** | `Di.cs`, `Program.cs`, `ICurrentUser`, manejo global de errores, logging. | La app arranca y hace login. |
| **6. Vistas** | StockView, QueriesView, SalesView, Administración deshabilitada. | Flujos de la sección 13 funcionando. |
| **7. Cierre** | Revisión de anotaciones, READMEs, matriz de dependencias, `docs/ARQUITECTURA.md`. | Checklist de la sección 14 completo. |

## 13. Criterios de aceptación funcionales

1. Iniciar sesión con `admin` y con Ezequiel Cano (legajo 102, Programador) funciona; credenciales erróneas muestran un mensaje genérico.
2. En **Stock**, buscar un PLU y un SKU existentes devuelve el mismo producto con su stock real; un código inexistente muestra un mensaje claro.
3. El stock mostrado coincide con la suma de movimientos del producto (verificado por test).
4. En **Ventas**, el resumen de hoy, de un mes y de un rango coincide con los datos del seeder (valores conocidos verificados por tests).
5. En **Consultas**, un PLU con historial muestra última venta y última recepción; un PLU sin movimientos muestra "Sin registros" sin errores.
6. **Administración** aparece deshabilitada con su tooltip.
7. Ninguna vista accede a `AppDbContext` ni a entidades de dominio.

## 14. Checklist de Definition of Done

- [ ] La solución compila sin advertencias nuevas.
- [ ] Todos los tests pasan.
- [ ] Domain no referencia ningún paquete externo de infraestructura.
- [ ] Application no referencia EF Core ni WinForms.
- [ ] Desktop solo referencia Infrastructure desde la composición (`Di.cs`/`Program.cs`).
- [ ] No hay `DateTime.Now` ni `Session` estática en Domain/Application.
- [ ] Todo tipo público tiene el comentario `///` (qué, capa, por qué).
- [ ] Los marcadores `[DDD]`, `[ARQ]` y `[NEGOCIO]` explican las decisiones clave.
- [ ] Cada carpeta de contexto tiene su `README.md`.
- [ ] `docs/GLOSARIO.md`, `docs/DECISIONES.md` y `docs/ARQUITECTURA.md` existen y están al día.
- [ ] No hay contraseñas ni secretos hardcodeados en código de producción.

## 15. Fuera de alcance (por ahora)

- Módulo de **Administración** (solo botón deshabilitado).
- Permisos por rol aplicados a cada acción (el rol existe y se guarda; la aplicación de permisos es una fase posterior).
- Reportes exportables (PDF/Excel).
- Sincronización con sistemas externos, outbox y mensajería.
- Multi-moneda.

## 16. Supuestos y preguntas abiertas

El agente debe asumir lo siguiente salvo que se indique lo contrario, y dejarlo anotado en `docs/DECISIONES.md`:

1. PLU y SKU son campos distintos, ambos únicos por empresa.
2. El stock se modifica **solo** por movimientos.
3. Ezequiel Cano entra con rol **Employee** y usuario `ecano`.
4. Longitud y formato de PLU (1 a 8 dígitos) y SKU (alfanumérico, hasta 32) son provisorios.
5. Los productos por peso admiten cantidades decimales; el resto, enteras.
6. Una sola moneda por empresa.

**Preguntas para confirmar con el autor antes de la Fase 1:**

- ¿Ezequiel Cano debe ser Employee o Admin?
- ¿El PLU debe tener un formato específico (longitud, solo números)?
- ¿Se debe conservar la base SQLite actual o se puede recrear la base de demostración?

## 17. Reglas de trabajo para el agente

1. No avanzar a la fase siguiente si la actual no compila o tiene tests en rojo.
2. No cambiar nombres públicos existentes sin dejar nota en `docs/DECISIONES.md`.
3. Preferir cambios pequeños y commits por fase.
4. Ante una duda de diseño, elegir la opción que **mantenga el dominio puro** y documentar el motivo.
5. Al finalizar cada fase, entregar un resumen breve: qué se hizo, qué decisiones se tomaron y qué queda pendiente.
