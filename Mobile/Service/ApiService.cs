using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mobile.Service
{
    public static class ApiService
    {

        private static readonly HttpClient Http = new()
        {
            BaseAddress = new Uri("http://10.106.130.106:5080/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public static string Erro { get; private set; } = string.Empty;

        private static string _token = string.Empty;

        public static string Token
        {
            get => _token;
            set
            {
                _token = value;
                Http.DefaultRequestHeaders.Remove("tokenHeader");
                if (!string.IsNullOrWhiteSpace(_token))
                    Http.DefaultRequestHeaders.Add("tokenHeader", _token);
            }
        }

        // Propriedades estáticas de sessão do usuário logado
        public static string UserName { get; set; } = "Admin";
        public static string UserEmail { get; set; } = "admin@example.com";
        public static string UserAvatar { get; set; } = "dotnet_bot.png";

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
                    _ => $"Erro no servidor (Código: {(int)response.StatusCode} / Mensagem: {response.Content})"
                };

                return default;
            }
            catch (Exception ex)
            {
                Erro = $"Falha de conexão: {ex.Message}";
                return default;
            }
        }

        public static async Task<TRes?> PostAsync<TRes>(string endpoint, object dados)
        {
            Erro = string.Empty;

            try
            {
                var response = await Http.PostAsJsonAsync(endpoint, dados);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<TRes>(_jsonOptions);

                Erro = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Credenciais inválidas",
                    HttpStatusCode.NotFound => "Endenreço não encontrado no banco",
                    HttpStatusCode.BadRequest => "Dados preenchidos incorretamente",
                    _ => $"Erro no servidor. Código {(int)response.StatusCode}"
                };
                return default;
            }
            catch (Exception ex)
            {
                Erro = $"Falha de conexão {ex.Message}";
                return default;
            }
        }

        public static async Task<TRes?> DeleteAsync<TRes>(string endpoint)
        {
            Erro = string.Empty;

            try
            {
                var response = await Http.DeleteAsync(endpoint);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<TRes>(_jsonOptions);

                Erro = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Credenciais inválidas",
                    HttpStatusCode.NotFound => "Item não encontrado no banco",
                    _ => $"Erro no servidor. Código {(int)response.StatusCode}"
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
