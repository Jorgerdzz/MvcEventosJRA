using MvcEventosJRA.Models;
using System.Net.Http.Headers;
using System.Text; // Añadido para poder codificar el texto de la pregunta

namespace MvcEventosJRA.Services
{
    public class ServiceEventos
    {
        private MediaTypeWithQualityHeaderValue header;
        private string UrlApi;
        private string UrlApiLambda; // Nueva variable para la URL de la IA

        public ServiceEventos(IConfiguration configuration)
        {
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEventos");

            // Leemos la URL de tu API Gateway que dispara la Lambda
            this.UrlApiLambda = configuration.GetValue<string>("ApiUrls:ApiLambdaIA");
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

        // --- NUEVA FUNCIONALIDAD: LLAMADA A LA IA (LAMBDA) ---
        public async Task<string> GetRespuestaIAAsync(string pregunta)
        {
            using (HttpClient client = new HttpClient())
            {
                // Preparamos la pregunta para enviarla como texto plano
                StringContent content = new StringContent(pregunta, Encoding.UTF8, "text/plain");

                // Hacemos un POST a la URL de tu API Gateway de la Lambda
                HttpResponseMessage response = await client.PostAsync(this.UrlApiLambda, content);

                if (response.IsSuccessStatusCode)
                {
                    // Leemos la respuesta (el string que devuelve tu función Lambda)
                    string data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    return "Error al conectar con la IA de AWS. Revisa la URL o tu API Gateway.";
                }
            }
        }
    }
}