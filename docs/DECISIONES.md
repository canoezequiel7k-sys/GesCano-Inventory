# Decisiones Arquitectónicas - Inventario Robusto

## 1. Distinción PLU vs SKU
**Decisión:** PLU y SKU son campos distintos y ambos únicos por empresa.
**Motivación:** La especificación lo exige para permitir búsquedas por código de balanza (PLU) y código interno de inventario (SKU).
**Impacto:** Se agregan como value objects separados en Domain con validaciones distintas.
**Documentación:** `docs/GLOSARIO.md` contiene ambas definiciones.

## 2. Stock solo por movimientos
**Decisión:** El stock real se deriva exclusivamente de la suma de movimientos de stock (recepciones, ventas, ajustes).
**Motivación:** Garantiza trazabilidad y auditoría completa de cada cambio de inventario.
**Impacto:** La entidad `Product` ya no almacena `Stock` como propiedad directamente modificable; usa `StockItem` agregado que gestiona el saldo por medio de eventos.

## 3. Patrones Application/Infrastructure
**Decisión:** 
- Application define puertos (interfaces): `IUnitOfWork`, `IClock`, `ICurrentUser`, `Result`, consultas y comandos.
- Infrastructure implementa los puertos con EF Core, repositorios configuraciones.
**Motivación:** Permite testear lógica de negocio sin base de datos y cambiar de proveedor de BD con mínima afectación.
**Impacto:** Se crean carpetas `Abstractions` en Application y `Persistence/Configurations` en Infrastructure.

## 4. Usuarios semilla
**Decisión:** 
- `admin` se conserva tal cual (legado del sistema anterior).
- **Ezequiel Cano** ingresa con legajo 102, puesto Programador y usuario `ecano`, rol Employee.
**Motivación:** Cumplir requisito del encargo y permitir pruebas con cuentas reales.
**Notas:** Contraseña inicial debe definirse en configuración local, no hardcodeada.

## 5. Inyección de dependencias via Di.cs
**Decisión:** Usar `Microsoft.Extensions.DependencyInjection` para compor la raíz en `Program.cs`.
**Motivación:** Desacoplar la creación de instancias de las vistas del código UI principal.
**Impacto:** `MainShell` y vistas se resolvían desde el `ServiceProvider` en lugar de `new` directo.

## 6. Manejo global de excepciones
**Decisión:** Suscribirse a `Application.ThreadException` y `AppDomain.UnhandledException` con logging a archivo via Serilog.
**Motivación:** Evitar cierre inesperado de la app y tener registro de errores en producción.
**Impacto:** Se añade código en `Program.cs` y `App.config`/`appsettings.json`.

## 7. Patrón Result en lugar de excepciones esperadas
**Decisión:** Errores esperados (stock insuficiente, código inexistente, duplicados) se devuelven con `Result`/`Result<T>`.
**Motivación:** Flujos de control explícitos y evitables, a diferencia de excepciones para situaciones inesperadas.
**Impacto:** Servicios de Application devuelven `Result` en lugar de lanzar excepciones por casos esperados.

## 8. AdministrationView deshabilitada de forma toggleable
**Decisión:** Botón visible en BottomNavigation con tooltip "No disponible por ahora", controlado por bandera `ModuleAvailability`.
**Motivación:** No eliminar la UI por si se implementa después, pero marcar claramente que no está lista.
**Impacto:** Se agrega propiedad `Available` al botón y lógica de visibilidad condicional.

## 9. .NET Core / .NET Framework compatibility
**Decisión:** El proyecto usa .NET Framework (Visual Studio 2022 con .NET Framework). Las bibliotecas nuevas (FluentValidation, BCrypt, etc.) deben ser compatibles o instalarse versiones que soporten FW.
**Motivación:** Mantener la base existente sin forzar migración inmediata a .NET 8.
**Impacto:** Se verifican versiones NuGet compatibles con .NET Framework.

## 10. Migrations EF Core preservar datos existentes
**Decisión:** Generar migración incremental a partir de las entidades actuales, preservando datos de `inventory.db` si existen.
**Motivación:** No perder la base de datos de demostración al agregar nuevas entidades.
**Impacto:** `dotnet ef migrations add InitialCreate --context AppDbContext` con scaffold que incluye nuevas tablas pero respeta datos existentes.