using Mobile.DTOs;
using Mobile.Service;

namespace Mobile.Views;

public partial class MyReviewsView : ContentPage
{
    private List<ReviewDto> _minhasAvaliacoes = new();

    public MyReviewsView()
    {
        InitializeComponent();
        pckOrdenacao.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarMinhasAvaliacoesAsync();
    }

    private async Task CarregarMinhasAvaliacoesAsync()
    {
        try
        {
            string endpoint = "system/api/Review/mine?pageNum=1&pageSize=50";
            var resPage = await ApiService.GetAsync<ResponseApi<PageData<ReviewDto>>>(endpoint);

            if (resPage?.Data?.Rows != null)
            {
                _minhasAvaliacoes = resPage.Data.Rows.ToList();

                foreach (var review in _minhasAvaliacoes)
                {
                    if (review.ImageUrls != null && review.ImageUrls.Count > 0)
                    {
                        review.ImageUrls = review.ImageUrls
                            .Select(FormatarImagem)
                            .ToList();
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_minhasAvaliacoes.Count == 0)
                    {
                        layoutVazio.IsVisible = true;
                        cvMinhasAvaliacoes.IsVisible = false;
                    }
                    else
                    {
                        layoutVazio.IsVisible = false;
                        cvMinhasAvaliacoes.IsVisible = true;
                        AplicarOrdenacao();
                    }
                });
            }
            else if (!string.IsNullOrWhiteSpace(ApiService.Erro))
            {
                await DisplayAlert("Erro API", ApiService.Erro, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro Crítico", $"Falha ao carregar suas avaliações: {ex.Message}", "OK");
        }
    }

    private string FormatarImagem(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/data/") || url.StartsWith("/storage/") || url.StartsWith("file://"))
            return url;

        return $"http://10.106.130.106:5080/{url.TrimStart('/')}";
    }

    private void AplicarOrdenacao()
    {
        var lista = _minhasAvaliacoes.AsEnumerable();

        if (pckOrdenacao.SelectedIndex == 1) // Time (oldest first)
        {
            lista = lista.OrderBy(r => r.ReviewTime);
        }
        else // Time (newest first)
        {
            lista = lista.OrderByDescending(r => r.ReviewTime);
        }

        cvMinhasAvaliacoes.ItemsSource = lista.ToList();
    }

    private void OnOrdenacaoChanged(object sender, EventArgs e) => AplicarOrdenacao();

    private async void OnExcluirClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int reviewId)
        {
            bool confirmar = await DisplayAlert("Excluir", "Deseja realmente excluir esta avaliação?", "Sim", "Cancelar");
            if (!confirmar) return;

            var res = await ApiService.DeleteAsync<ResponseApi<object>>($"system/api/Review/{reviewId}");

            if (res != null && res.Code == 200)
            {
                await DisplayAlert("Sucesso", "Avaliação removida com sucesso!", "OK");
                await CarregarMinhasAvaliacoesAsync();
            }
            else
            {
                await DisplayAlert("Erro", $"Falha ao excluir: {ApiService.Erro}", "OK");
            }
        }
    }
}