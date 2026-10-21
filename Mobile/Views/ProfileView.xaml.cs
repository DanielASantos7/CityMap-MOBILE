using Mobile.Service;

namespace Mobile.Views;

public partial class ProfileView : ContentPage
{
    public ProfileView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarEstadoSessao();
    }

    private void AtualizarEstadoSessao()
    {
        bool logado = !string.IsNullOrWhiteSpace(ApiService.Token);

        layoutDeslogado.IsVisible = !logado;
        layoutLogado.IsVisible = logado;

        if (logado)
        {
            lblUserName.Text = string.IsNullOrWhiteSpace(ApiService.UserName) ? "Admin" : ApiService.UserName;
            lblEmail.Text = string.IsNullOrWhiteSpace(ApiService.UserEmail) ? "admin@example.com" : ApiService.UserEmail;
            imgAvatar.Source = string.IsNullOrWhiteSpace(ApiService.UserAvatar) ? "dotnet_bot.png" : ApiService.UserAvatar;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginView());
    }

    private async void OnMyReviewsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MyReviewsView());
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        ApiService.Token = string.Empty;
        DetailsView.UsuarioEstaLogado = false;
        AtualizarEstadoSessao();
    }
}