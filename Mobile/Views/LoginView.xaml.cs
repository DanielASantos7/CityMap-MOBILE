using Mobile.DTOs;
using Mobile.Service;
using System.Text.Json.Serialization;

namespace Mobile.Views;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string username = txtUsername.Text?.ToString() ?? "";
        string password = txtPassword.Text?.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Erro", "Digite corretamente suas credenciais", "OK");
            return;
        }

        var loginDto = new { Username = username, Password = password };

        var res = await ApiService.PostAsync<ResponseApi<LoginResponseDto>>("system/api/auth/login", loginDto);

        if (res != null && res.Code == 200 && res.Data != null)
        {
            ApiService.Token = res.Data.Token;
            DetailsView.UsuarioEstaLogado = true;
            await DisplayAlert("Sucesso", "Login realizado!", "OK");


            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Erro", "Nome de usuário ou senha inválidos, tente novamente.", "OK");
            return;
        }
    }
}

public class LoginResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}