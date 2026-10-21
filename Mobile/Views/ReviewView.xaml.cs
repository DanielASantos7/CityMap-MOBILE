using Mobile.DTOs;
using Mobile.Service;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Mobile.Views;

public partial class ReviewView : ContentPage
{
    private readonly int _businessId;
    private readonly string _businessType;

    private int _notaSelecionada = 5;
    private List<ReviewDto> _todasAvaliacoes = new();
    private readonly List<string> _fotosAnexadas = new();

    public ReviewView(int businessId, string businessType)
    {
        InitializeComponent();
        _businessId = businessId;
        _businessType = businessType?.ToLower() == "restaurant" || businessType?.ToLower() == "restaurante" ? "restaurant" : "scenic";

        pckOrdenacao.SelectedIndex = 0;
        pckFiltroEstrelas.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAvaliacoesAsync();
    }

    private async Task CarregarAvaliacoesAsync()
    {
        try
        {
            string endpoint = $"system/api/Review/list?pageNum=1&pageSize=50&businessType={_businessType}&businessId={_businessId}";
            var resPage = await ApiService.GetAsync<ResponseApi<PageData<ReviewDto>>>(endpoint);

            if (resPage?.Data?.Rows != null)
            {
                _todasAvaliacoes = resPage.Data.Rows.ToList();

                foreach (var item in _todasAvaliacoes)
                {
                    if (item.ImageUrls != null)
                    {
                        item.ImageUrls = item.ImageUrls.Select(FormatarUrl).ToList();
                    }
                }

                AplicarFiltroEOrdenacao();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao listar comentários: {ex.Message}", "OK");
        }
    }

    private void AplicarFiltroEOrdenacao()
    {
        var listaExibicao = _todasAvaliacoes.AsEnumerable();

        if (pckFiltroEstrelas.SelectedIndex > 0)
        {
            int estrelasFiltro = 6 - pckFiltroEstrelas.SelectedIndex;
            listaExibicao = listaExibicao.Where(r => r.Rating == estrelasFiltro.ToString());
        }

        if (pckOrdenacao.SelectedIndex == 1)
        {
            listaExibicao = listaExibicao.OrderBy(r => r.ReviewTime);
        }
        else
        {
            listaExibicao = listaExibicao.OrderByDescending(r => r.ReviewTime);
        }

        cvListaComentarios.ItemsSource = listaExibicao.ToList();
    }

    private void OnFiltroChanged(object sender, EventArgs e) => AplicarFiltroEOrdenacao();

    private void OnStarTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string starStr && int.TryParse(starStr, out int star))
        {
            _notaSelecionada = star;
            for (int i = 0; i < layoutEstrelas.Children.Count; i++)
            {
                if (layoutEstrelas.Children[i] is Label lbl)
                    lbl.TextColor = i < star ? Color.FromArgb("#FF8800") : Color.FromArgb("#CCCCCC");
            }
        }
    }

    private void TxtComentario_TextChanged(object sender, TextChangedEventArgs e)
    {
        int len = e.NewTextValue?.Length ?? 0;
        lblContador.Text = $"{len}/15 caracteres";

        if (len < 5)
        {
            lblContador.TextColor = Color.FromArgb("#AA0000");
            lblContador.Text += " (mínimo 5)";
            btnEnviar.IsEnabled = false;
        }
        else
        {
            lblContador.TextColor = Color.FromArgb("#00AA00");
            btnEnviar.IsEnabled = true;
        }
    }

    private async void OnAnexarFotoClicked(object sender, EventArgs e)
    {
        if (_fotosAnexadas.Count >= 3)
        {
            await DisplayAlert("Limite", "Você pode anexar no máximo 3 fotos.", "OK");
            return;
        }

        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                _fotosAnexadas.Add(photo.FullPath);
                AtualizarPreviewFotos();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Aviso", $"Falha ao selecionar foto: {ex.Message}", "OK");
        }
    }

    private void AtualizarPreviewFotos()
    {
        layoutPreviewFotos.Children.Clear();
        layoutPreviewFotos.IsVisible = _fotosAnexadas.Count > 0;

        foreach (var imgPath in _fotosAnexadas)
        {
            var img = new Image { Source = imgPath, WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFill };
            layoutPreviewFotos.Children.Add(img);
        }
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        btnEnviar.IsEnabled = false;

        try
        {
            List<string> urlsServidor = await FazerUploadImagensAsync(_fotosAnexadas);

            var dto = new AddReviewDto
            {
                BusinessType = _businessType,
                BusinessId = _businessId,
                Rating = _notaSelecionada.ToString(),
                ReviewContent = txtComentario.Text?.Trim() ?? "",
                ImageUrls = urlsServidor
            };

            var res = await ApiService.PostAsync<ResponseApi<object>>("system/api/Review/add", dto);

            if (res != null && res.Code == 200)
            {
                await DisplayAlert("Sucesso", "Avaliação enviada com sucesso!", "OK");

                txtComentario.Text = string.Empty;
                _fotosAnexadas.Clear();
                AtualizarPreviewFotos();

                await CarregarAvaliacoesAsync();
            }
            else
            {
                await DisplayAlert("Erro", $"Falha ao enviar comentário: {ApiService.Erro}", "OK");
                btnEnviar.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro Crítico", $"Falha no processo: {ex.Message}", "OK");
            btnEnviar.IsEnabled = true;
        }
    }

    private async Task<List<string>> FazerUploadImagensAsync(List<string> arquivosLocais)
    {
        var resultado = new List<string>();
        if (arquivosLocais == null || arquivosLocais.Count == 0) return resultado;

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://10.106.130.106:5080/") };
            if (!string.IsNullOrEmpty(ApiService.Token))
            {
                client.DefaultRequestHeaders.Add("tokenHeader", ApiService.Token);
            }

            using var content = new MultipartFormDataContent();

            foreach (var caminho in arquivosLocais)
            {
                if (File.Exists(caminho))
                {
                    var fileBytes = await File.ReadAllBytesAsync(caminho);
                    var byteContent = new ByteArrayContent(fileBytes);
                    byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(byteContent, "files", Path.GetFileName(caminho));
                }
            }

            var response = await client.PostAsync("api/upload/imagens", content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in dataElement.EnumerateArray())
                    {
                        if (el.ValueKind == JsonValueKind.String)
                            resultado.Add(el.GetString()!);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UPLOAD ERROR] {ex.Message}");
        }

        return resultado;
    }

    private string FormatarUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/data/") || url.StartsWith("file://")) return url;
        return $"http://10.106.130.106:5080/{url.TrimStart('/')}";
    }
}