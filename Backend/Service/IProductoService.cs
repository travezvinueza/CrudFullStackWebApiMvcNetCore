using Backend.Models;

namespace Backend.Service
{
    public interface IProductoService
    {
        Task<List<Producto>> GetAll();
        Task<Producto> GetById(int id);
        bool Create(Producto producto);
        Task Delete(int id);
    }
}