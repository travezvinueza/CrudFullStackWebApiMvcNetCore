using Backend.Data;
using Backend.Models;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _rutaServidor;
        private readonly DatabaseContext _dbContext;
        private readonly IProductoService _productoService;

        public ProductoController(
            IWebHostEnvironment environment,
            DatabaseContext dbContext,
            IProductoService productoService,
            IConfiguration config)

        {
            _environment = environment;
            _rutaServidor = config.GetSection("Configuracion:RutaServidor").Value!;
            _dbContext = dbContext;
            _productoService = productoService;
        }

        [HttpPost]
        public IActionResult Create([FromForm] Producto producto)
        {
            try
            {
                if (producto.File != null && producto.File.Length > 0)
                {
                    // Construir la ruta relativa dentro de la carpeta wwwroot/Uploads
                    var rutaRelativa = Path.Combine(_rutaServidor, Path.GetFileName(producto.File.FileName));

                    // Construir la ruta completa para guardar el archivo
                    var rutaCompleta = Path.Combine(_environment.WebRootPath, rutaRelativa);

                    // Guardar el archivo en la ruta completa
                    using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        producto.File.CopyTo(fileStream);
                    }

                    // Actualizar la propiedad Ruta en el modelo
                    producto.Ruta = rutaRelativa;
                    // Reemplazar espacios con guiones bajos en la ruta
                    producto.Ruta = producto.Ruta.Replace(" ", "_");
                }

                _dbContext.Productos.Add(producto);
                _dbContext.SaveChanges();

                return Ok(new { Message = "Producto creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var productos = _dbContext.Productos.ToList();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Producto producto = await _dbContext.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound("Producto no encontrado");
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] Producto producto)
        {
            try
            {
                var productoExistente = await _dbContext.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    return NotFound("Producto no encontrado");
                }

                productoExistente.Name = producto.Name;
                productoExistente.Price = producto.Price;
                productoExistente.Description = producto.Description;

                if (producto.File != null && producto.File.Length > 0)
                {
                    // Generar un nombre de archivo único para evitar conflictos
                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(producto.File.FileName);

                    // Construir la ruta relativa dentro de la carpeta wwwroot/Uploads
                    var rutaRelativa = Path.Combine(_rutaServidor, nombreArchivo);

                    // Construir la ruta completa para guardar el archivo
                    var rutaCompleta = Path.Combine(_environment.WebRootPath, rutaRelativa);

                    using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        producto.File.CopyTo(fileStream);
                    }

                    productoExistente.Ruta = rutaRelativa;
                    productoExistente.Ruta = productoExistente.Ruta.Replace(" ", "_");
                }

                await _dbContext.SaveChangesAsync();

                return Ok("Producto actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var producto = _dbContext.Productos.Find(id);

                if (producto == null)
                {
                    return NotFound("Producto no encontrado");
                }

                if (!string.IsNullOrEmpty(producto.Ruta))
                {
                    var rutaCompleta = Path.Combine(_environment.ContentRootPath, producto.Ruta);
                    if (System.IO.File.Exists(rutaCompleta))
                    {
                        System.IO.File.Delete(rutaCompleta);
                    }
                }

                _dbContext.Productos.Remove(producto);
                _dbContext.SaveChanges();

                return Ok("Producto eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}