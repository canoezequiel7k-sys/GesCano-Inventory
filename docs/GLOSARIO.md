# Glosario - Inventario Robusto con DDD y Clean Architecture

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
| **Agregado (Root)** | Entidad raíz de un bounded context que garantiza la consistencia de invariantes. |
| **Value Object** | Objeto inmutable que describe características del dominio, sin identidad propia. Se valida al crearse. |
| **Invariante** | Regla de negocio que debe cumplirse en todo momento para la consistencia del agregado. |
| **Dominio** | Lógica de negocio pura, independiente de tecnologías (BD, UI, etc.). |
| **Application** | Capa de casos de uso (puertos/usecases) que orquestan el dominio. |
| **Infrastructure** | Implementación técnica: EF Core, repositorios, configuraciones, BCrypt. |
| **Desktop** | Interfaz de usuario Windows Forms. |
| **Result** | Patrón de retorno que encapsula éxito o error con código y mensaje legibles. |
| **IUnitOfWork** | Interfaz que coordina la persistencia y despacha eventos de dominio. |
| **IClock** | Abstracción de la hora actual, permitiendo usar reloj real o falso en tests. |
| **ICurrentUser** | Abstracción del usuario autenticado actual (CompanyId, UserId, Role). |
| **Bounded Context** | Contexto acotado: un límite dentro del cual un modelo ubíquo tiene sentido. |
| **Aggregate Root** | Raíz de un agregado que protege su invariante y es el único punto de entrada. |
| **Command** | Objeto que representa una intención de escritura (cambiar estado). |
| **Query** | Objeto que representa una intención de lectura (obtener datos). |
| **CQRS** | Patrón que separa operaciones de lectura y escritura. En este proyecto es "ligero". |
| **Seeder** | Generador de datos de demostración para pruebas. |
| **Idempotente** | Operación que puede aplicarse múltiples veces sin cambiar el resultado beyond the first application. |