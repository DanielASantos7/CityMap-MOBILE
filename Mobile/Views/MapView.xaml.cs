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
        tap.Tapped += (s, e) =>
        {
            var pos = e.GetPosition(gvRotas);
            if (pos.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"--> PONTO CLICADO: new PointF({(int)pos.Value.X}, {(int)pos.Value.Y});");
            }
        };
        gvRotas.GestureRecognizers.Add(tap);
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

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;
        canvas.StrokeSize = 8; // Espessura da linha
        canvas.StrokeLineCap = LineCap.Round;

        // Segmento: Home -> Gas Station (Vermelho - congestionado)
        canvas.StrokeColor = Colors.Red;
        canvas.DrawLine(Home.X, Home.Y, GasStation.X, GasStation.Y);

        // Segmento: Gas Station-> Restaurant (amarelo - Tráfego Lento)
        canvas.StrokeColor = Colors.Yellow;
        canvas.DrawLine(GasStation.X, GasStation.Y, Restaurant.X, Restaurant.Y);

        // Segmento: Restaurant -> Company (Verde - Tráfego normal)
        canvas.StrokeColor = Colors.Green;
        canvas.DrawLine(Restaurant.X, Restaurant.Y, Company.X, Company.Y);
    }
}