using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;
            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";
            string url =
                $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={cidade}&units=metric&appid={chave}"; // Removi a chave duplicada

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new Tempo()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString("HH:mm"), // Formatação da hora
                            sunset = sunset.ToString("HH:mm"),   // Formatação da hora
                        };
                    }
                    else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Cidade não encontrada, retorna null para indicar o erro
                        return null;
                    }
                    else
                    {
                        // Outros erros de status HTTP (ex: erro no servidor)
                        // Você pode logar o erro aqui para depuração
                        Console.WriteLine($"Erro na requisição: {resp.StatusCode}");
                        return null; // Ou lançar uma exceção, dependendo do seu tratamento geral de erros
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Erro de conexão com a internet
                    Console.WriteLine($"Erro de conexão: {ex.Message}");
                    return null; // Indica falha na requisição devido à conexão
                }
                catch (Exception ex)
                {
                    // Outros erros inesperados (parsing JSON, etc.)
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    return null;
                }
            }
            return t;
        }
    }
}