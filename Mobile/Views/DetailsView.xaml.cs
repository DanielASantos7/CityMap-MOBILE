using Mobile.DTOs;
using Mobile.Service;

namespace Mobile.Views;

public partial class DetailsView : ContentPage
{
    private readonly int _id;
    private readonly string _businessType;

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

        layoutDeslogado.IsVisible = !UsuarioEstaLogado;
        layoutLogado.IsVisible = UsuarioEstaLogado;

        await CarregarDetalhesAsync();

        if (UsuarioEstaLogado)
            await CarregarAvaliacaosAsync();
    }

    private async Task CarregarAvaliacaosAsync()
    {
        try
        {
            string tipo = _businessType?.ToLower() == "restaurant" || _businessType?.ToLower() == "restaurante"
                ? "restaurant"
                : "scenic";

            string endpoint = $"system/api/Review/list?pageNum=1&pageSize=1&businessType={tipo}&businessId={_id}";

            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTICO] Chamando endpoint: {endpoint}");

            var resPage = await ApiService.GetAsync<ResponseApi<PageData<ReviewDto>>>(endpoint);
            List<ReviewDto>? lista = resPage?.Data?.Rows?.ToList();

            if (lista != null && lista.Count > 0)
            {
                var itemPreview = lista.FirstOrDefault();
                if (itemPreview != null)
                {
                    cvAvaliacoes.ItemsSource = new List<ReviewDto> { itemPreview };
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Falha ao carregar preview de avaliações: {ex.Message}");
        }
    }

    private async Task CarregarDetalhesAsync()
    {
        try
        {
            string tipo = _businessType?.ToLower() ?? "";

            if (tipo == "restaurant" || tipo == "restaurante")
            {
                var res = await ApiService.GetAsync<ResponseApi<RestaurantDetailDto>>($"system/api/restaurant/{_id}");

                if (res?.Data != null)
                {
                    PreencherRestaurante(res.Data);
                }
                else
                {
                    await DisplayAlert("Erro", $"{ApiService.Erro}", "OK");
                }
            }
            else
            {
                var res = await ApiService.GetAsync<ResponseApi<ScenicSpotDetailDto>>($"system/api/scenicSpot/{_id}");

                if (res?.Data != null)
                {
                    PreencherAtracao(res.Data);
                }
                else
                {
                    await DisplayAlert("Erro", $"{ApiService.Erro}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro Crítico", $"Falha ao carregar detalhes: {ex.Message}", "OK");
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
        lblEstacionamento.Text = $"Estacionamento: {item.RemainingParkingSpaces?.ToString() ?? "0"} disponíveis";

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
        await Navigation.PushAsync(new LoginView());
    }

    private async void OnVerMaisClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReviewView(_id, _businessType));
    }
}