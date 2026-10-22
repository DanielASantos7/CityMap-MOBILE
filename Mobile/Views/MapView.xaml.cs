namespace Mobile.Views;

public partial class MapView : ContentPage
{
    private double _currentScale = 1.0;
    private readonly RouteMapDrawble _drawble = new();
    public MapView()
    {
        InitializeComponent();

        gvRotas.Drawable = _drawble;

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnMapTapped;
        gvRotas.GestureRecognizers.Add(tap);
    }

    // Manipulador acionado a cada toque na superfície do mapa
    private void OnMapTapped(object? sender, TappedEventArgs e)
    {
        // Obtém a coordenada do toque RELATIVA ao elemento gvRotas
        // Usar gvRotas em vez de this garante que o ponto (0,0) seja sempre o superior esquerdo do mapa
        var touchPos = e.GetPosition(gvRotas);
        if (!touchPos.HasValue) return;

        // Converte o ponto de toque (double) para estrutura PointF (float) usado pelo motor de desenho.
        PointF p = new PointF((float)touchPos.Value.X, (float)touchPos.Value.Y);

        if (EhProximo(p, _drawble.Home))
        {
            _drawble.AtualizarSelecao("Home");
        }

        else if (EhProximo(p, _drawble.GasStation))
        {
            _drawble.AtualizarSelecao("GasStation");
        }

        else if (EhProximo(p, _drawble.Restaurant))
        {
            _drawble.AtualizarSelecao("Restaurant");
        }

        else if (EhProximo(p, _drawble.Company))
        {
            _drawble.AtualizarSelecao("Company");
        }

        // Limpa o canvas e executa o método Draw() novamente
        gvRotas.Invalidate();
    }


    // Coalisão Euclidiana para área de toque circular.
    // Em vez de calcular a distância real usando a raiz quadrade Math.Sqrt(dx² + dy²),
    // comparamos a soma dos quadrados com o raio ao quadrado (dx² + dy² <= r²).
    private bool EhProximo(PointF toque, PointF no, float raio = 35f)
    {
        float dx = toque.X - no.X;
        float dy = toque.Y - no.Y;

        return (dx * dx + dy * dy) <= (raio * raio);
    }

    private async void OnZoomInClicked(object sender, EventArgs e)
    {
        if (_currentScale < 3.0)
        {
            _currentScale += 0.25;
            await mapContainer.ScaleTo(_currentScale, 150, Easing.CubicOut);
        }

    }

    private async void OnZoomOutClicked(object sender, EventArgs e)
    {
        if (_currentScale > 0.75)
        {
            _currentScale -= 0.25;
            await mapContainer.ScaleTo(_currentScale, 150, Easing.CubicOut);
        }

    }
}

// CLASSE DE DESENHO: Desenha APENAS as 3 rotas coloridas sobre o mapa da imagem
public class RouteMapDrawble : IDrawable
{
    // Coordenadas aproximadas do centro de cada ícone da imagem da prova
    public PointF Home { get; set; } = new PointF(510, 682);
    public PointF GasStation { get; set; } = new PointF(530, 510);
    public PointF Restaurant { get; set; } = new PointF(486, 338);
    public PointF Company { get; set; } = new PointF(253, 388);

    // Controla o professo da exibição do traçado de acordo com os cliques do usuário:
    // 0 = Nenhuma rota ativa
    // 1 = Exibir segmento home -> gas station (vermelho)
    // 2 = Exibir segmentos até restaurant (Vermelho + amarelo)
    // 3 = Exibir segmentos até company (Vermleho + amarelo + verde)
    public int NivelRotaAtiva { get; private set; } = 0;

    // Transita o estado da rota com base no nó do mapa identificado pelo clique do usuário
    public void AtualizarSelecao(string localClicado)
    {
        switch (localClicado)
        {
            case "Home":
                NivelRotaAtiva = 0;
                break;
            case "GasStation":
                NivelRotaAtiva = 1;
                break;
            case "Restaurant":
                NivelRotaAtiva = 2;
                break;
            case "Company":
                NivelRotaAtiva = 3;
                break;
            default:
                NivelRotaAtiva = 0;
                break;
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true; // Ativa suavizado nas bordas das linhas (evita efeito serrilhado)
        canvas.StrokeSize = 8; // Espessura da linha
        canvas.StrokeLineCap = LineCap.Round; // Extremidades arredondadas

        if (NivelRotaAtiva >= 1)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.DrawLine(Home.X, Home.Y, GasStation.X, GasStation.Y);
        }

        if (NivelRotaAtiva >= 2)
        {
            canvas.StrokeColor = Colors.Yellow;
            canvas.DrawLine(GasStation.X, GasStation.Y, Restaurant.X, Restaurant.Y);
        }

        if (NivelRotaAtiva >= 3)
        {
            canvas.StrokeColor = Colors.Green;
            canvas.DrawLine(Restaurant.X, Restaurant.Y, Company.X, Company.Y);
        }

        DesenharAnelDeSelecao(canvas);
    }

    // Desenha um anel indicador ao redor do destino ativo
    private void DesenharAnelDeSelecao(ICanvas canvas)
    {
        PointF noAtivo = NivelRotaAtiva switch
        {
            0 => Home,
            1 => GasStation,
            2 => Restaurant,
            3 => Company,
            _ => Home
        };

        canvas.StrokeColor = Colors.DarkBlue;
        canvas.StrokeSize = 4;
        canvas.DrawCircle(noAtivo.X, noAtivo.Y, 35);
    }
}