# Arquitectura General - Inventario Robusto

## Estructura de Carpetas por Capa y Contexto

```
Inventory.Domain/
  Common/           Entity, AggregateRoot, IDomainEvent, DomainException
  Catalog/          Plu, Sku (value objects)
  Inventory/        StockItem, StockMovement, MovementType, Quantity, eventos
  Sales/            Sale, SaleItem, SaleRegistered
  Identity/         Company, User, EmployeeNumber, UserRole

Inventory.Application/
  Abstractions/     IUnitOfWork, IClock, ICurrentUser, Result, IPasswordHasher
  Catalog/          Commands, Queries, DTOs
  Inventory/        Commands (ReceiveStock, AdjustStock), Queries (GetStockByCode)
  Sales/            Commands (RegisterSale), Queries (GetSalesSummary)
  Inquiries/        Queries (GetProductInquiry)
  Identity/         Auth, DTOs

Inventory.Infrastructure/
  Persistence/      AppDbContext, Configurations/, Migrations/, repositorios
  Queries/          implementaciones de IStockQueries, ISalesQueries, etc.
  Seeding/          DemoDataSeeder
  Security/         BCryptPasswordHasher
  Time/             SystemClock (implementa IClock)

Inventory.Desktop/
  Views/            HomeView, ProductsView, StockView, SalesView, QueriesView, AdministrationView
  Controls/         BottomNavigation, TerminalHeader, UserProfilePanel
  Composition/      Di.cs (ServiceProvider)
  Session/          Sesión de usuario actual

Inventory.Tests/
  Domain/           Tests de value objects y agregados (sin EF)
  Application/      Tests de casos de uso (SQLite en memoria)
  Infrastructure/   Tests de repositorios y migraciones (SQLite en memoria)
```

## Dependencias Entre Capas

```
Desktop → Application → Domain (solo librería base .NET)
Infrastructure → Application → Domain
Desktop solo referencia Infrastructure desde Composition Root (Di.cs / Program.cs)
NINGUNA capa puede referir a WinForms, EF Core ni BCrypt en Domain.
```

## Patrones Utilizados

1. **Clean Architecture**: Dependencias hacia adentro. El núcleo (Domain) no sabe nada de tecnología.
2. **DDD (Domain-Driven Design)**: Bounded contexts, agregados, value objects, invariantes, eventos de dominio.
3. **CQRS Ligero**: Separación de comandos (escritura) y consultas (lectura), pero compartiendo el mismo modelo y BD.
4. **Repository Pattern**: Interfaces en Application, implementaciones en Infrastructure con EF Core.
5. **Unit of Work**: Coordinación de transacciones y despacho de eventos en una sola unidad.
6. **Result Pattern**: Éxito/Error explícito en lugar de excepciones para casos esperados.
7. **FluentValidation**: Validación de comandos y DTOs en capa Application.

## Flujo de Datos Principales

### Flujo de Escritura (Ej.: RegisterSale)
1. Vista Desktop → Service Application (`IProductService` / `IStockService`)
2. Application → Dominio (crear/validar `Sale` agregado)
3. Agregado valida invariantes (stock suficiente, ítems válidos, etc.)
4. Dominio lanza eventos (`SaleRegistered`)
5. Unit of Work confirma transacción y despacha eventos
6. Repositorios persisten cambios en BD (EF Core)

### Flujo de Lectura (Ej.: GetStockByCode)
1. Vista Desktop → Query Handler Application
2. Application → Infrastructure (repositorio con `AsNoTracking`)
3. EF Core ejecuta query con filtro multi-empresa (`CompanyId`)
4. Proyección directa a `StockItemDto` (sin rastreo)
5. Retorno a vista para display

## Bounded Contexts Definidos

1. **Catalog**: Productos, PLU, SKU, precios, categorías. Es el catálogo base.
2. **Inventory**: Stock, movimientos, recepciones, ajustes. Corazón del inventario.
3. **Sales**: Ventas registradas, ítems, resúmenes, series temporales.
4. **Identity**: Empresas, usuarios, legajos, roles, autenticación.
5. **Consultas**: *Read model* que cruza datos de otros contextos solo para lectura (no es un contexto de escritura).