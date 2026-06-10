using MvcEventosJRA.Models;
using System.Net.Http.Headers;

namespace MvcEventosJRA.Services
{
    public class ServiceEventos
    {
        private MediaTypeWithQualityHeaderValue header;
        private string UrlApi;

        public ServiceEventos(IConfiguration configuration)
        {
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEventos");
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadFromJsonAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Evento>> GetEventosAsync()
        {
            string request = "api/Eventos";
            return await this.CallApiAsync<List<Evento>>(request);
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            string request = "api/Eventos/Categorias";
            return await this.CallApiAsync<List<Categoria>>(request);
        }
        public async Task<List<Evento>> GetEventosByCategoriaAsync(int idCategoria)
        {
            string request = $"api/Eventos/EventosCategoria/{idCategoria}";
            return await this.CallApiAsync<List<Evento>>(request);
        }
    }
}
