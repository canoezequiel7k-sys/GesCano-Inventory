# Administración — Reconteo de stock (próxima versión)

Módulo futuro para hacer un **reconteo físico mensual** y eliminar el stock fantasma.

## Objetivo

Comparar el stock del sistema contra la mercadería real contada a mano, separando
**depósito** y **salón (a la venta)**. El total físico de ambos lugares debe coincidir
con el stock registrado.

## Flujo previsto

1. Elegir una **sección/categoría** (ej.: Panadería, Limpieza) o un producto puntual.
2. Iniciar un **reconteo** (queda registrado con fecha, usuario y sección).
3. Escanear el **código EAN** del producto (el EAN se vincula al PLU/SKU interno)
   o buscarlo manualmente por PLU/SKU.
4. Ingresar la **cantidad contada en depósito**.
5. Ingresar la **cantidad contada en salón**.
6. El sistema calcula:
   - total físico = depósito + salón
   - diferencia = total físico − stock del sistema
7. Confirmar el reconteo: se genera un `StockMovement` de **ajuste** por la diferencia.
8. Guardar el historial del reconteo para auditoría.

## Reglas

- El stock **no** se edita directo: siempre se ajusta con `StockMovement`.
- Si hay diferencia negativa, queda registrada como faltante; si es positiva, como sobrante.
- Un producto escaneado por EAN debe resolver a su PLU/SKU y a un único producto de la empresa.
- Se debe poder reanudar un reconteo incompleto y ver el detalle de los ya cerrados.
- Reportes: diferencias por sección, por producto, unidad y valorizado al costo.

## Cambios técnicos necesarios

- Agregar `Ean` (código de barras) a `Product`, DTOs y mapeo EF Core.
- Crear entidades `StockCount` y `StockCountLine` (depósito, salón, diferencia).
- Servicio `StockCountService` en Application.
- Pantalla `AdministrationView` con selección de sección, escaneo EAN y carga de cantidades.
- Habilitar el botón de Administración en `BottomNavigation.cs`.
