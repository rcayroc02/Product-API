# Products API

API REST para gestión de productos y categorías construida con ASP.NET Core 8 y Entity Framework Core.


## Tecnologías

    ASP.NET Core 8 Web API
    Entity Framework Core
    SQLite
    Swagger / OpenAPI


## Requisitos

    .NET 8 SDK
    https://dotnet.microsoft.com/download


## Instalación

    1. Clonar el repositorio

        git clone https://github.com/tuusuario/products-api.git
        cd products-api

    2. Aplicar migraciones

        dotnet ef database update

    3. Ejecutar el proyecto

        dotnet run

    4. Abrir Swagger en el navegador

        https://localhost:PUERTO/swagger


## Endpoints

    Productos

        GET     /api/products           Listar productos con paginación y filtros
        GET     /api/products/{id}      Obtener producto por ID
        POST    /api/products           Crear producto
        PUT     /api/products/{id}      Editar producto
        DELETE  /api/products/{id}      Eliminar producto

    Categorías

        GET     /api/categories         Listar todas las categorías
        GET     /api/categories/{id}    Obtener categoría por ID
        POST    /api/categories         Crear categoría
        PUT     /api/categories/{id}    Editar categoría
        DELETE  /api/categories/{id}    Eliminar categoría


## Parámetros de paginación y filtrado

    GET /api/products?page=1&pageSize=10&category=1

        page        Número de página (default: 1)
        pageSize    Cantidad de resultados por página (default: 10)
        category    ID de categoría para filtrar (opcional)


## Estructura del proyecto

    Controllers         Endpoints de la API
    DTOs                Objetos de transferencia de datos
    Data                DbContext y configuración de base de datos
    Middleware          Manejo global de errores
    Models              Modelos de la base de datos
    Migrations          Migraciones generadas por Entity Framework


## Ejemplo de respuesta exitosa

    {
        "success": true,
        "message": "Operación exitosa",
        "data": {
            "id": 1,
            "name": "Laptop HP",
            "price": 899.99,
            "stock": 10,
            "categoryName": "Electrónica"
        },
        "errors": null
    }


## Ejemplo de respuesta de error

    {
        "success": false,
        "message": "Error de validación",
        "data": null,
        "errors": [
            "El precio debe ser mayor a 0",
            "El nombre es requerido"
        ]
    }

## Colección de Postman

    Para una prueba mas eficiente. Importa la colección  ""Products API.postman_collection"" incluida en este repositorio.

