using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaConversaoDeMoedas.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SistemaConversaoDeMoedas.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        private const string API_URL = "http://economia.awesomeapi.com.br/json/last/";
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(ConversaoMoedaModel conversao)
        {
            if (ModelState.IsValid)
            {
                string url = $"{API_URL}{conversao.De}-{conversao.Para}";
                var resposta = await httpClient.GetAsync(url);

                if (conversao.De == conversao.Para)
                {
                    conversao.ValorConvertido = conversao.Valor;
                    ViewBag.Mensagem = $"Conversão de {conversao.Valor} {conversao.De} para {conversao.Para} = {conversao.ValorConvertido.ToString("f2")}";

                }

                else if (resposta.IsSuccessStatusCode)
                     {
                          var resultadoJson = await resposta.Content.ReadAsStringAsync();
                          var resultado = JsonConvert.DeserializeObject<Dictionary<string, CotacaoMoedaModel>>(resultadoJson); 
                    
                          if (resultado != null && resultado.Count > 0)
                          {
                              var cotacao = resultado.First().Value;                        
                              conversao.ValorConvertido = conversao.Valor * cotacao.Bid;
                              ViewBag.Mensagem = $"Conversão de {conversao.Valor} {conversao.De} para {conversao.Para} = {conversao.ValorConvertido.ToString("f2")}";
                          }
                     }
            }
            return View(conversao);
        }
    }
}