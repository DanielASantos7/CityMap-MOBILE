using System.Net;
using System.Text.Json;

namespace Mobile.Service
{
    public static class ApiService
    {

        private static readonly HttpClient Http = new()
        {
            BaseAddress = new Uri("http://10.0.2.2:5080/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public static string Erro { get; private set; } = string.Empty;


        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            Erro = string.Empty;

            try
            {
                var response = await Http.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, _jsonOptions);
                }

                Erro = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Credenciais inválidas",
                    HttpStatusCode.BadRequest => "Dados preenchidos incorretamente",
                    HttpStatusCode.NotFound => "Endereço não encontrado na API",
                    _ => $"Erro no servidor (Código: {(int)response.StatusCode}"
                };

                return default;
            }
            catch (Exception ex)
            {
                Erro = $"Falha de conexão: {ex.Message}";
                return default;
            }
        }

    }
}
