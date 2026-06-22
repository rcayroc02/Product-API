using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuProyecto.Data;
using TuProyecto.DTOs;
using TuProyecto.Models;

namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ProductResponseDto>>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? category = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (category.HasValue)
                query = query.Where(p => p.CategoryId == category.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    CategoryName = p.Category!.Name
                })
                .ToListAsync();

            var paginated = new PaginatedResponseDto<ProductResponseDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<PaginatedResponseDto<ProductResponseDto>>.Ok(paginated));
        }

        // GET /api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(ApiResponse<ProductResponseDto>.Fail($"Producto con ID {id} no encontrado"));

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryName = product.Category!.Name
            };

            return Ok(ApiResponse<ProductResponseDto>.Ok(response));
        }

        // GET /api/products/chat?query=laptops menores a 1500
        [HttpGet("chat")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> Chatbot([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponse<IEnumerable<ProductResponseDto>>.Fail("El mensaje no puede estar vacío"));

            // 1. Obtener las categorías REALES de la base de datos
            var categoriasDb = await _context.Categories.Select(c => c.Name).ToListAsync();
            string listaCategorias = string.Join(", ", categoriasDb);

            // 2. Configurar la API de Gemini
            string? apiKey = _configuration["Gemini:ApiKey"];
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            // 3. El Prompt Dinámico (Le inyectamos la lista de la BD directamente)
            string promptIA = $@"
                Eres el asistente virtual de una tienda. El usuario dice: '{query}'.
                Tu tarea es extraer los filtros de búsqueda.
                
                REGLA ESTRICTA PARA LA CATEGORÍA:
                Solo puedes clasificar la categoría en UNA de estas opciones exactas: {listaCategorias}
                Si el usuario menciona un sinónimo, mapealo a la categoría correcta de la lista.
                Si no menciona ninguna categoría, o no coincide con la lista, devuelve null.

                Responde ÚNICAMENTE con un JSON válido con esta estructura exacta:
                {{
                    ""categoria"": ""nombre exacto de la lista o null"",
                    ""precioMaximo"": numero o null
                }}";

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = promptIA } } } }
            };
            
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorReal = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, 
                    ApiResponse<IEnumerable<ProductResponseDto>>.Fail($"Error de Gemini: {response.StatusCode} - {errorReal}"));
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonResponse);
            
            var iaText = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            iaText = iaText!.Replace("```json", "").Replace("```", "").Trim();

            var opcionesJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var filtrosIA = JsonSerializer.Deserialize<FiltrosIADto>(iaText, opcionesJson);

            var dbQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (filtrosIA != null)
            {
                // 4. EL CAMBIO CLAVE: Búsqueda exacta (==) en lugar de ToLower()
                if (!string.IsNullOrEmpty(filtrosIA.Categoria))
                {
                    dbQuery = dbQuery.Where(p => p.Category!.Name == filtrosIA.Categoria);
                }

                if (filtrosIA.PrecioMaximo.HasValue)
                {
                    dbQuery = dbQuery.Where(p => p.Price <= filtrosIA.PrecioMaximo.Value);
                }
            }

            var resultados = await dbQuery
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    CategoryName = p.Category!.Name
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<ProductResponseDto>>.Ok(resultados, "Consulta procesada por IA exitosamente"));
        }


        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct(ProductCreateDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest(ApiResponse<ProductResponseDto>.Fail("La categoría no existe"));

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryName = product.Category!.Name
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id },
                ApiResponse<ProductResponseDto>.Ok(response, "Producto creado exitosamente"));
        }

        // PUT /api/products/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProduct(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(ApiResponse<object>.Fail($"Producto con ID {id} no encontrado"));

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest(ApiResponse<object>.Fail("La categoría no existe"));

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new {}, "Producto actualizado exitosamente"));
        }

        // DELETE /api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(ApiResponse<object>.Fail($"Producto con ID {id} no encontrado"));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new {}, "Producto eliminado exitosamente"));
        }
    }
    public class FiltrosIADto
    {
        public string? Categoria { get; set; }
        public decimal? PrecioMaximo { get; set; }
    }
}