# MyCleanApp

Aplicación con arquitectura Clean Onion que incluye Backend en .NET 9 y Frontend con Blazor WebAssembly, usando Node.js y Tailwind CSS para estilos.

---

## Comandos para ejecución del proyecto

### Construir imagen Docker para la base de datos
```
docker run -d --name sqlserver-container -p 1433:1433 mi-sqlserver-custom .
```

### Ejecutar imagen Docker (base de datos)
```
docker run -d --name sqlserver-container -p 1433:1433 mi-sqlserver-custom
```
### Instalar JSON Server
```
npm install -g json-server
```
### Ejecutar el servidor de la FAKE API
```
json-server --watch db.json
```

### Ejecutar API Backend
```
dotnet run --project MyCleanApp.API
```


### Ejecutar Aplicación Frontend (Blazor WebAssembly)
```
dotnet run --project MyCleanApp.Client
```


---

## Dependencias necesarias en la infraestructura

Para Entity Framework Core y Microsoft Server:
```
dotnet add package Microsoft.EntityFrameworkCore
```

---

## Uso de Tailwind CSS y Node.js

El frontend utiliza Tailwind CSS para estilos y Node.js para el proceso de build. 

### Requisitos para Tailwind

- Node.js (v14 o superior recomendado)
- npm o yarn

### Comandos para instalar dependencias y construir CSS (ejemplo)
```
npm install
npm run build:css
```


*(Recuerda completar con los comandos y detalles específicos de tu configuración de Tailwind y build)*

---
## Estructura del proyecto

- BaseDatos - Carpeta con recursos para la base de datos (Docker, scripts, etc.)
- MyCleanApp.API - Proyecto Backend con .NET 9 y arquitectura Clean Onion
- MyCleanApp.Application - Capa de lógica de negocio y casos de uso
- MyCleanApp.Client - Proyecto Frontend con Blazor WebAssembly y Tailwind CSS
- MyCleanApp.Domain - Entidades, objetos de valor y lógica de dominio
- MyCleanApp.Infrastructure - Implementaciones de acceso a datos, persistencia, etc.
- .gitignore - Archivo para ignorar archivos en Git
- MyCleanApp.sln - Solución .NET que agrupa todos los proyectos
- readme.md - Documento de descripción y guía del proyecto
---

## Notas

- Asegúrate de tener Docker instalado y funcionando para correr la base de datos.
- Configura las cadenas de conexión en los archivos de configuración para apuntar a la base de datos en Docker.
- Actualiza este README con detalles adicionales según avances en la implementación.

---
