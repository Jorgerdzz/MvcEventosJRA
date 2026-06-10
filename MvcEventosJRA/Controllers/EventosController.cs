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

        public async Task<IActionResult> Index()
        {
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasAsync();

            List<Evento> eventos = await this.service.GetEventosAsync();
            return View(eventos);
        }

        public async Task<IActionResult> EventosPorCategoria(int idCategoria)
        {
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasAsync();

            List<Evento> eventos = await this.service.GetEventosByCategoriaAsync(idCategoria);
            return View("Index", eventos);
        }

    }
}
