using Mobile.DTOs;
using Mobile.Service;
using System.Text.Json.Serialization;

namespace Mobile.Views;

public partial class ListRestaurantsView : ContentPage
{
    public ListRestaurantsView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarRestaurantes();
    }

    private async Task CarregarRestaurantes()
    {
        var response = await ApiService.GetAsync<ResponseApi<PageData<EstabelecimentoDto>>>("/system/api/restaurant/list");

        if (response != null && response.Code == 200 && response.Data != null)
        {
            cvLista.ItemsSource = response.Data.Rows;
        }
        else
        {
            await DisplayAlert("Erro de conexão", $"Falha ao carregar: {ApiService.Erro}", "OK");
        }
    }
}



public class EstabelecimentoDto
{
    [JsonPropertyName("restaurantId")]
    public int Id { get; set; }

    [JsonPropertyName("restaurantName")]
    public string NomeEstabelecimento { get; set; }

    [JsonPropertyName("phone")]
    public string telefone { get; set; }

    [JsonPropertyName("rating")]
    public string Rating { get; set; }

    [JsonPropertyName("priceRange")]
    public string PriceRange { get; set; }

    [JsonPropertyName("restaurantType")]
    public string TipoEstabelecimento { get; set; }

    [JsonPropertyName("imageUrl")]
    public string Imagem { get; set; }

    [JsonPropertyName("description")]
    public string Descricao { get; set; }

    [JsonPropertyName("distance")]
    public string Distancia { get; set; }


    public string DistanciaFormatada => $"Distância do destino: {Distancia}km";
}