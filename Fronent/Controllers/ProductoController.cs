using System.Net.Http.Headers;
using AspNetCoreHero.ToastNotification.Abstractions;
using Fronent.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fronent.Controllers
{

    public class ProductoController : Controller
    {
        private readonly HttpClient httpClient;
         private readonly INotyfService _notifyService;

        public ProductoController(IHttpClientFactory httpClientFactory, INotyfService notifyService)
        {
            httpClient = httpClientFactory.CreateClient();
            _notifyService = notifyService;
            httpClient.BaseAddress = new Uri("http://localhost:5202");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> Index()
        {
            var respuesta = await httpClient.GetAsync("/api/Producto");
            if (respuesta.IsSuccessStatusCode)
            {
                var productos = await respuesta.Content.ReadAsAsync<List<Producto>>();
                return View(productos);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile documento, Producto producto)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    
                    content.Add(new StringContent(producto.Name), "Name");
                    content.Add(new StringContent(producto.Price.ToString()), "Price");
                    content.Add(new StringContent(producto.Description), "Description");

                    if (documento != null && documento.Length > 0)
                    {
                        var filestreamContent = new StreamContent(documento.OpenReadStream());
                        filestreamContent.Headers.ContentType = new MediaTypeHeaderValue(documento.ContentType);

                        // Usar el nombre del archivo para el nombre del campo
                        content.Add(filestreamContent, "File", documento.FileName);
                    }

                    var respuesta = await httpClient.PostAsync("/api/Producto", content);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        _notifyService.Success("Producto creado exitosamente");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }


         [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var respuesta = await httpClient.GetAsync($"/api/Producto/{id}");
                if (respuesta.IsSuccessStatusCode)
                {
                    var producto = await respuesta.Content.ReadAsAsync<Producto>();
                    return View(producto);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile documento, Producto producto)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                   
                    content.Add(new StringContent(producto.Name), "Name");
                    content.Add(new StringContent(producto.Price.ToString()), "Price");
                    content.Add(new StringContent(producto.Description), "Description");

                    if (documento != null && documento.Length > 0)
                    {
                        var filestreamContent = new StreamContent(documento.OpenReadStream());
                        filestreamContent.Headers.ContentType = new MediaTypeHeaderValue(documento.ContentType);

                        // Usar el nombre del archivo para el nombre del campo
                        content.Add(filestreamContent, "File", documento.FileName);
                    }

                    var respuesta = await httpClient.PutAsync($"/api/Producto/{id}", content);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        _notifyService.Information("Producto actualizado correctamente");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _notifyService.Warning("Por favor, completa todos los campos.");
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var respuesta = await httpClient.GetAsync($"/api/Producto/{id}");
            if (respuesta.IsSuccessStatusCode)
            {
                var producto = await respuesta.Content.ReadAsAsync<Producto>();
                return View(producto);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var respuesta = await httpClient.DeleteAsync($"/api/Producto/{id}");
            if (respuesta.IsSuccessStatusCode)
            {
                _notifyService.Success("Producto eliminado correctamente.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Error");
            }
        }
    }
}