using Microsoft.AspNetCore.Mvc;
using MvcEventosJRA.Models;
using MvcEventosJRA.Services;

namespace MvcEventosJRA.Controllers
{
    public class EventosController : Controller
    {
        private ServiceEventos service;

        public EventosController(ServiceEventos service)
        {
            this.service = service;
        }

        // GET: Se ejecuta cuando entras a la página normalmente
        public async Task<IActionResult> Index()
        {
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasAsync();

            List<Evento> eventos = await this.service.GetEventosAsync();
            return View(eventos);
        }

        // POST: NUEVO MÉTODO - Se ejecuta cuando pulsas el botón de "Preguntar"
        [HttpPost]
        public async Task<IActionResult> Index(string pregunta)
        {
            // 1. Necesitamos recargar las categorías y eventos para que la vista no se quede en blanco
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasAsync();
            List<Evento> eventos = await this.service.GetEventosAsync();

            // 2. Comprobamos que el usuario realmente ha escrito algo
            if (!string.IsNullOrEmpty(pregunta))
            {
                // Llamamos a nuestro servicio de IA
                string respuesta = await this.service.GetRespuestaIAAsync(pregunta);

                // Pasamos los datos a la vista mediante ViewData
                ViewData["PREGUNTA"] = pregunta; // Para mantener el texto que escribió en la caja
                ViewData["RESPUESTA_IA"] = respuesta; // Para mostrar lo que contestó GPT
            }

            // Devolvemos la vista con los eventos
            return View(eventos);
        }

        // GET: Se ejecuta cuando pulsas en una categoría específica
        public async Task<IActionResult> EventosPorCategoria(int idCategoria)
        {
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasAsync();

            List<Evento> eventos = await this.service.GetEventosByCategoriaAsync(idCategoria);
            return View("Index", eventos);
        }
    }
}