# Reglas de Desarrollo y Código Limpio — Proyecto SGAT

Este es un proyecto **avanzado** en .NET con **Clean Architecture** dividida en 4 proyectos (`SGAT.Domain`, `SGAT.Application`, `SGAT.Infrastructure`, `SGAT.Web`). Antes de escribir o modificar código, aplica estas reglas estrictas basadas en *Clean Code* (Robert C. Martin):

## 1. Protección de Módulos Existentes (Crítico)
- **Antes de crear o editar:** Inspecciona primero un módulo ya existente en `SGAT.Application`, `SGAT.Infrastructure` y `SGAT.Web` para imitar exactamente la estructura de carpetas, convenciones de nombres, manejo de errores y estilo del proyecto actual.
- **No rompas código en producción:** No renombres clases, interfaces, métodos públicos ni propiedades de entidades existentes a menos que se te pida explícitamente.
- **Migraciones de EF Core:** Nunca edites ni borres archivos dentro de `SGAT.Infrastructure/Migrations`. Si modificas una entidad o configuración, indica el comando `Add-Migration` correspondiente en lugar de alterar el historial.
- **Cambios mínimos:** Limítate estrictamente al alcance de la tarea solicitada; no refactorices archivos no relacionados.

## 2. Regla de Dependencia entre Capas
- **`SGAT.Domain`:** Sin dependencias externas (ni EF Core ni ASP.NET). Solo entidades, enums y excepciones de dominio.
- **`SGAT.Application`:** Solo depende de `SGAT.Domain`. Contiene servicios/casos de uso, DTOs (`record`) e interfaces de repositorios. Nunca referencia `SGAT.Infrastructure` ni `DbContext`.
- **`SGAT.Infrastructure`:** Depende de `SGAT.Application` y `SGAT.Domain`. Implementa `DbContext`, configuraciones Fluent API (`IEntityTypeConfiguration<T>`) y repositorios concretos.
- **`SGAT.Web`:** Depende de `SGAT.Application` (y `SGAT.Infrastructure` solo en `Program.cs` para inyección de dependencias). Nunca accede directo a `DbContext`.

## 3. Código Limpio en C#, ASP.NET Core y EF Core
- **Nombres con intención:** `PascalCase` para clases/métodos/propiedades, `camelCase` para variables locales/parámetros y `_camelCase` para campos privados `readonly`. Métodos asíncronos terminan en `Async` con `CancellationToken`.
- **Métodos cortos y una sola responsabilidad:** Máximo ~20 líneas por método y hasta 3 parámetros (agrupa en DTOs si hay más). Usa cláusulas de guarda (*early return*) para evitar anidamientos profundos.
- **EF Core limpio:** Usa Fluent API en clases `IEntityTypeConfiguration<T>`, `.AsNoTracking()` en lecturas y proyecciones a DTOs con `.Select(...)`. Nunca expongas entidades de `SGAT.Domain` directamente en `SGAT.Web`.
- **Errores explícitos:** Nunca dejes bloques `catch` vacíos. Sigue el patrón de respuesta/errores que ya usan los servicios existentes del proyecto.
