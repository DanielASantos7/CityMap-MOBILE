using Mobile.DTOs;
using Mobile.Service;
using System.Text.Json.Serialization;

namespace Mobile.Views;

public partial class DetailsView : ContentPage
{
    private readonly int _id;
    private readonly string _businessType;

    // Gerenciador simples de sessão estática para a prova
    public static bool UsuarioEstaLogado { get; set; } = false;

    public DetailsView(int id, string businessType)
    {
        InitializeComponent();
        _id = id;
        _businessType = businessType;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Atualiza a visibilidade do bloco de login/avaliações baseado no estado do usuário
        layoutDeslogado.IsVisible = !UsuarioEstaLogado;
        layoutLogado.IsVisible = UsuarioEstaLogado;

        await CarregarDetalhesAsync();
    }

    private async Task CarregarDetalhesAsync()
    {
        try
        {
            if (_businessType == "restaurant")
            {
                var res = await ApiService.GetAsync<ResponseApi<RestaurantDetailDto>>($"/system/api/restaurant/{_id}");
                if (res?.Data != null) PreencherRestaurante(res.Data);
            }
            else
            {
                var res = await ApiService.GetAsync<ResponseApi<ScenicSpotDetailDto>>($"/system/api/scenicSpot/{_id}");
                if (res?.Data != null) PreencherAtracao(res.Data);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar detalhes: {ex.Message}", "OK");
        }
    }

    private void PreencherRestaurante(RestaurantDetailDto item)
    {
        imgCapa.Source = string.IsNullOrEmpty(item.ImageUrl) ? "" : $"http://10.106.130.113:5080{item.ImageUrl}";
        lblNome.Text = item.RestaurantName;
        lblTipo.Text = "Restaurante";
        lblRating.Text = item.Rating ?? "0.0";
        lblDistancia.Text = $"Distância: {item.Distance ?? "0.0"}km";

        lblCustoMedio.Text = $"$ Custo Médio {item.PriceRange}";
        lblHorarioRestaurante.Text = $"Horário: {item.BusinessHours}";
        lblEstacionamento.Text = $"Estacionamento: {item.RemainingParkingSpaces ?? "0"} disponíveis";

        gridRestaurante.IsVisible = true;
        gridAtracao.IsVisible = false;
    }

    private void PreencherAtracao(ScenicSpotDetailDto item)
    {
        imgCapa.Source = string.IsNullOrEmpty(item.ImageUrl) ? "" : $"http://10.106.130.113:5080{item.ImageUrl}";
        lblNome.Text = item.SpotName;
        lblTipo.Text = "Atração";
        lblRating.Text = item.Rating ?? "0.0";
        lblDistancia.Text = $"Distância: {item.Distance ?? "0.0"}km";

        lblHorarioAtracao.Text = $"Horário: {item.OpeningHours}";
        lblPrecoIngresso.Text = $"Preço Ingresso: ${item.TicketPrice}";
        lblMelhorEpoca.Text = $"Melhor Época: {item.TouristPeriod}";

        gridRestaurante.IsVisible = false;
        gridAtracao.IsVisible = true;
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        // Navega para a tela de Login (vamos criá-la na sequência)
        //await Navigation.PushAsync(new LoginView());
    }

    private async void OnVerMaisClicked(object sender, EventArgs e)
    {
        // TODO: Navegar para a lista completa de avaliações (Sessão 2.2)
        await DisplayAlert("Avaliações", "Navegar para Todos os Comentários", "OK");
    }
}

// ====================================================================
// DTOs ESPECÍFICOS DE DETALHES
// ====================================================================

public class RestaurantDetailDto
{
    [JsonPropertyName("restaurantId")] public int RestaurantId { get; set; }
    [JsonPropertyName("restaurantName")] public string RestaurantName { get; set; } = string.Empty;
    [JsonPropertyName("priceRange")] public string PriceRange { get; set; } = string.Empty;
    [JsonPropertyName("businessHours")] public string BusinessHours { get; set; } = string.Empty;
    [JsonPropertyName("remainingParkingSpaces")] public string? RemainingParkingSpaces { get; set; }
    [JsonPropertyName("rating")] public string? Rating { get; set; }
    [JsonPropertyName("distance")] public string? Distance { get; set; }
    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }
}

public class ScenicSpotDetailDto
{
    [JsonPropertyName("spotId")] public int SpotId { get; set; }
    [JsonPropertyName("spotName")] public string SpotName { get; set; } = string.Empty;
    [JsonPropertyName("openingHours")] public string OpeningHours { get; set; } = string.Empty;
    [JsonPropertyName("ticketPrice")] public string TicketPrice { get; set; } = string.Empty;
    [JsonPropertyName("touristPeriod")] public string TouristPeriod { get; set; } = string.Empty;
    [JsonPropertyName("rating")] public string? Rating { get; set; }
    [JsonPropertyName("distance")] public string? Distance { get; set; }
    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }
}