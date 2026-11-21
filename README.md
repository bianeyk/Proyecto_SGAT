# Proyecto SGAT

Sistema de Gestión Administrativa / de Actividades Turísticas (SGAT) — aplicación web
desarrollada en **C#** sobre **.NET**, organizada siguiendo una arquitectura
limpia por capas (Clean Architecture).

> [!NOTE]
> Este README es una base generada automáticamente a partir de la estructura del
> repositorio. Completa las secciones marcadas con `TODO` con la información
> funcional real del proyecto.

---

## Tabla de contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Requisitos previos](#requisitos-previos)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Módulos](#módulos)
- [Flujo de trabajo y ramas](#flujo-de-trabajo-y-ramas)
- [Autores](#autores)

---

## Descripción

SGAT es el proyecto de desarrollo de la aplicación de gestión que implementa los
diferentes módulos del sistema (clientes, y futuros módulos de reservas, servicios,
reportes, etc.) sobre una base común separada por responsabilidades.

**TODO:** describe aquí el objetivo de negocio concreto del sistema y sus usuarios.

---

## Arquitectura

La solución (`SGAT.slnx`) está dividida en cuatro proyectos:

| Proyecto | Capa | Responsabilidad |
| --- | --- | --- |
| `SGAT.Domain` | Dominio | Entidades, enums, reglas de negocio y contratos. Sin dependencias externas. |
| `SGAT.Application` | Aplicación | Casos de uso, DTOs, servicios de aplicación y validaciones. |
| `SGAT.Infrastructure` | Infraestructura | Acceso a datos, persistencia, configuraciones externas e integraciones. |
| `SGAT` | Presentación | Aplicación web (MVC/Razor), controladores, vistas, JS/CSS y arranque. |

La dirección de las dependencias siempre apunta hacia el dominio:

```text
SGAT (Presentación)
   └──> SGAT.Application
            └──> SGAT.Domain
   └──> SGAT.Infrastructure ──> SGAT.Application / SGAT.Domain
