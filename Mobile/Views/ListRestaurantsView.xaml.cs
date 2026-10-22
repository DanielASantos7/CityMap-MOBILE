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

        if (pkFiltro.SelectedIndex == -1)
        {
            pkFiltro.SelectedIndex = 0; // Seleciona "Todos" por padrão
        }
        else
        {
            await CarregarDadosAsync();
        }
    }

    private async void OnFiltroChanged(object sender, EventArgs e)
    {
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        string opcao = pkFiltro.SelectedItem?.ToString() ?? "Todos";
        var listaUnificada = new List<ServicoProximoDto>();

        try
        {
            if (opcao == "Restaurante")
            {
                listaUnificada = await BuscarRestaurantesAsync();
            }
            else if (opcao == "Atração")
            {
                listaUnificada = await BuscarAtracoesAsync();
            }
            else
            {
                var taskRestaurantes = BuscarRestaurantesAsync();
                var taskAtracoes = BuscarAtracoesAsync();

                await Task.WhenAll(taskRestaurantes, taskAtracoes);

                listaUnificada.AddRange(taskRestaurantes.Result);
                listaUnificada.AddRange(taskAtracoes.Result);
            }

            cvLista.ItemsSource = listaUnificada;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar dados: {ex.Message}", "OK");
        }
    }

    private async Task<List<ServicoProximoDto>> BuscarAtracoesAsync()
    {
        var res = await ApiService.GetAsync<ResponseApi<PageData<ScenicSpotApiDto>>>("system/api/scenicSpot/list");
        if (res?.Data?.Rows == null) return new List<ServicoProximoDto>();

        return res.Data.Rows.Select(s => new ServicoProximoDto
        {
            Id = s.SpotId,
            Nome = s.SpotName,
            TipoTag = "Atração",
            Rating = s.Rating ?? "0.0",
            Distancia = s.Distance ?? "0.0",
            Imagem = s.ImageUrl ?? "",
            BusinessType = "scenic"
        }).ToList();
    }

    private async Task<List<ServicoProximoDto>> BuscarRestaurantesAsync()
    {
        var res = await ApiService.GetAsync<ResponseApi<PageData<RestaurantApiDto>>>("system/api/restaurant/list");
        if (res?.Data?.Rows == null) return new List<ServicoProximoDto>();

        return res.Data.Rows.Select(r => new ServicoProximoDto
        {
            Id = r.RestaurantId,
            Nome = r.RestaurantName,
            TipoTag = "Restaurante",
            Rating = r.Rating ?? "0.0",
            Distancia = r.Distance ?? "0.0",
            Imagem = r.ImageUrl ?? "",
            BusinessType = "restaurant"
        }).ToList();
    }
    private async void OnItemSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ServicoProximoDto item)
        {
            cvLista.SelectedItem = null; // Limpa a seleção do card

            // Preparado para a próxima etapa: Navegar para a tela de Detalhes
            await Navigation.PushAsync(new DetailsView(item.Id, item.BusinessType));
        }
    }

}

public class ServicoProximoDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string TipoTag { get; set; }
    public string Rating { get; set; }
    public string Distancia { get; set; }
    public string Imagem { get; set; }
    public string BusinessType { get; set; }

    public string DistanciaFormatada => $"Distância do destino: {Distancia}km";
}

public class RestaurantApiDto
{
    [JsonPropertyName("restaurantId")]
    public int RestaurantId { get; set; }

    [JsonPropertyName("restaurantName")]
    public string RestaurantName { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("distance")]
    public string? Distance { get; set; }
}

public class ScenicSpotApiDto
{
    [JsonPropertyName("spotId")]
    public int SpotId { get; set; }

    [JsonPropertyName("spotName")]
    public string SpotName { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("distance")]
    public string? Distance { get; set; }
}