using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using Microsoft.Maui.Controls;
using System.Net.NetworkInformation; // Namespace para verificar a conexão

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
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

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                        wv_mapa.Source = mapa;
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

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                   GeolocationAccuracy.Medium,
                   TimeSpan.FromSeconds(10)
                );

                Location? local = await Geolocation.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_disp;

                    //pega nome das cidades que estão as coordenadas
                    GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Nenhuma localização";
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }


        private async void GetCidade(double lat, double lon)
        {
            try
            {

                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Otenção do nome da Cidade", ex.Message, "Ok");
            }
        }
    }
}