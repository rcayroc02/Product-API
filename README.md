# Product API

Aplicacion web completa para administrar categorias y productos, con un asistente conversacional que interpreta consultas en lenguaje natural y muestra coincidencias almacenadas en la base de datos.

## Funcionalidades principales

- **Gestion de productos:** crear, consultar, editar y eliminar productos.
- **Gestion de categorias:** crear, consultar, editar y eliminar categorias.
- **Listado paginado:** navegar por los productos con un numero configurable de registros por pagina.
- **Filtrado por categoria:** consultar los productos asociados a una categoria.
- **Chatbot con Gemini:** enviar consultas como "laptops menores a 1500" y recibir productos que coincidan.
- **Busqueda asistida por IA:** Gemini transforma la consulta en filtros de categoria y precio maximo.
- **Datos reales:** la API aplica los filtros a los productos almacenados en SQL Server.
- **Interfaz Angular:** navegacion entre productos, categorias y chatbot.
- **Validaciones:** reglas de datos en el backend y mensajes de error en la interfaz.
- **Documentacion Swagger:** prueba interactiva de los endpoints durante el desarrollo.
- **Manejo global de errores:** respuestas HTTP consistentes mediante middleware.

## Tecnologias

### Frontend

- Angular 21
- TypeScript
- RxJS
- Angular Router
- SCSS

### Backend

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Google Gemini API

## Arquitectura

```text
Frontend Angular (localhost:4200)
        |
        | HTTP REST
        v
ASP.NET Core API (localhost:5041)
        |
        +-- Entity Framework Core --> SQL Server
        |
        +-- Google Gemini API --> Interpretacion de consultas
```

## Requisitos

- .NET 10 SDK
- Node.js y npm
- Angular CLI
- SQL Server
- Instancia local de SQL Express con la base `ProductosDb`
- Una clave API de Google Gemini

## Configuracion

### 1. Base de datos

La conexion predeterminada usa SQL Server Express:

```json
Server=localhost\SQLEXPRESS;Database=ProductosDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Puedes modificarla en `Backend/appsettings.json` mediante `ConnectionStrings:DefaultConnection`.

Desde la carpeta del backend, aplica las migraciones:

```powershell
cd Backend
dotnet ef database update
```

### 2. Clave de Gemini

Configura la clave con secretos de usuario para evitar incluirla en el codigo:

```powershell
cd Backend
dotnet user-secrets set "Gemini:ApiKey" "TU_API_KEY"
```

No publiques la clave en `appsettings.json`, Git o capturas del proyecto.

## Ejecucion

Abre dos terminales.

### Backend

```powershell
cd Backend
dotnet run --launch-profile http
```

La API quedara disponible en:

- HTTP: `http://localhost:5041`
- Swagger: `http://localhost:5041/swagger`

Tambien puede iniciarse con el perfil HTTPS:

```powershell
dotnet run --launch-profile https
```

### Frontend

```powershell
cd Frontend/products-frontend
npm install
npm start
```

La interfaz quedara disponible en `http://localhost:4200`.

## Endpoints principales

### Productos

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/products?page=1&pageSize=10&category=1` | Lista productos paginados y opcionalmente filtrados por categoria |
| GET | `/api/products/{id}` | Obtiene un producto por ID |
| POST | `/api/products` | Crea un producto |
| PUT | `/api/products/{id}` | Actualiza un producto |
| DELETE | `/api/products/{id}` | Elimina un producto |
| GET | `/api/products/chat?query=laptops menores a 1500` | Busca productos mediante lenguaje natural |

### Categorias

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/categories` | Lista todas las categorias |
| GET | `/api/categories/{id}` | Obtiene una categoria por ID |
| POST | `/api/categories` | Crea una categoria |
| PUT | `/api/categories/{id}` | Actualiza una categoria |
| DELETE | `/api/categories/{id}` | Elimina una categoria |

## Como funciona el chatbot

1. El usuario escribe una consulta en la interfaz Angular.
2. El frontend la envia al endpoint `/api/products/chat`.
3. El backend obtiene las categorias existentes en SQL Server.
4. Gemini interpreta la consulta y responde con una categoria valida y un precio maximo.
5. Entity Framework aplica esos filtros sobre la tabla de productos.
6. Angular muestra los resultados y sus datos principales.

Ejemplos de consultas admitidas:

- `laptops menores a 1500`
- `ropa con precio menor a 100`
- `alimentos menores a 20`

El asistente esta especializado en categorias y precios. Para una busqueda sin coincidencias devuelve una lista vacia.

## Pruebas

Desde el frontend:

```powershell
cd Frontend/products-frontend
npm test
```

Desde el backend:

```powershell
cd Backend
dotnet build
```

La carpeta `Backend/DOTNET.http` y la coleccion `Backend/Products API.postman_collection` pueden utilizarse para probar manualmente la API.

## Estructura del proyecto

```text
Product-API/
|-- Backend/
|   |-- Controllers/       Endpoints REST
|   |-- DTOs/              Datos de entrada y salida
|   |-- Data/              DbContext de Entity Framework
|   |-- Middleware/        Manejo centralizado de errores
|   |-- Migrations/        Migraciones de base de datos
|   |-- Models/            Entidades del dominio
|   `-- Program.cs         Configuracion de servicios y CORS
`-- Frontend/products-frontend/
    `-- src/app/
        |-- core/          Servicios, modelos e interceptores
        |-- features/      Productos, categorias y chatbot
        `-- shared/        Componentes reutilizables
```
