using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Net.NetworkInformation; // Namespace para verificar a conexão

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    // Verifica se há conexão com a internet antes de fazer a requisição
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    {
                        await DisplayAlert("Sem Conexão", "Verifique sua conexão com a internet e tente novamente.", "OK");
                        return; // Interrompe a execução se não houver internet
                    }

                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";
                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Pôr do Sol: {t.sunset} \n" +
                                         $"Temp Máx.: {t.temp_max} \n" +
                                         $"Temp Min.: {t.temp_min} \n" +
                                         $"Descrição: {t.description} \n" +
                                         $"Velocidade: {t.speed} \n" +
                                         $"Visibilidade: {t.visibility} \n";

                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        lbl_res.Text = "Cidade não encontrada. Verifique o nome digitado.";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade";
                }
            }
            catch (Exception ex)
            {
                // Captura outras exceções que possam ocorrer (embora os erros de rede e cidade não encontrada sejam tratados acima)
                await DisplayAlert("Ops", $"Ocorreu um erro: {ex.Message}", "OK");
            }
        }
    }
}