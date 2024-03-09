using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Impl
{
    public class ProductoService : IProductoService
    {
        private readonly DatabaseContext _context;

        public ProductoService(
            DatabaseContext context)
        {
            _context = context;
     
        }

        public async Task<List<Producto>> GetAll()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<Producto> GetById(int id)
        {
            try
            {
                return await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public bool Create(Producto producto)
        {
            try
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task Delete(int id)
        {
            var eliminarProduct = await _context.Productos.FindAsync(id);

            if (eliminarProduct != null)
            {

                _context.Productos.Remove(eliminarProduct);
                await _context.SaveChangesAsync();

            }
        }

    }
}