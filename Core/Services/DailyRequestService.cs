using Core.DataAccessLayer;
using Core.Dto;
using Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public class DailyRequestService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DailyRequestService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyRequestService(HttpClient httpClient, ILogger<DailyRequestService> logger, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _httpClient = httpClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(async o =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    //string json = @"[{""table"":""A"",""no"":""034/A/NBP/2022"",""effectiveDate"":""2022-02-19"",""rates"":[{""currency"":""bat (Tajlandia)"",""code"":""THB"",""mid"":0.1237},{""currency"":""dolar amerykański"",""code"":""USD"",""mid"":3.9798},{""currency"":""dolar australijski"",""code"":""AUD"",""mid"":2.8716},{""currency"":""dolar Hongkongu"",""code"":""HKD"",""mid"":0.5103},{""currency"":""dolar kanadyjski"",""code"":""CAD"",""mid"":3.1386},{""currency"":""dolar nowozelandzki"",""code"":""NZD"",""mid"":2.6753},{""currency"":""dolar singapurski"",""code"":""SGD"",""mid"":2.9625},{""currency"":""euro"",""code"":""EUR"",""mid"":4.5256},{""currency"":""forint (Węgry)"",""code"":""HUF"",""mid"":0.012702},{""currency"":""frank szwajcarski"",""code"":""CHF"",""mid"":4.3233},{""currency"":""funt szterling"",""code"":""GBP"",""mid"":5.4222},{""currency"":""hrywna (Ukraina)"",""code"":""UAH"",""mid"":0.1404},{""currency"":""jen (Japonia)"",""code"":""JPY"",""mid"":0.034561},{""currency"":""korona czeska"",""code"":""CZK"",""mid"":0.1862},{""currency"":""korona duńska"",""code"":""DKK"",""mid"":0.6084},{""currency"":""korona islandzka"",""code"":""ISK"",""mid"":0.032096},{""currency"":""korona norweska"",""code"":""NOK"",""mid"":0.4463},{""currency"":""korona szwedzka"",""code"":""SEK"",""mid"":0.4277},{""currency"":""kuna (Chorwacja)"",""code"":""HRK"",""mid"":0.6005},{""currency"":""lej rumuński"",""code"":""RON"",""mid"":0.9153},{""currency"":""lew (Bułgaria)"",""code"":""BGN"",""mid"":2.3139},{""currency"":""lira turecka"",""code"":""TRY"",""mid"":0.2920},{""currency"":""nowy izraelski szekel"",""code"":""ILS"",""mid"":1.2460},{""currency"":""peso chilijskie"",""code"":""CLP"",""mid"":0.005004},{""currency"":""peso filipińskie"",""code"":""PHP"",""mid"":0.0774},{""currency"":""peso meksykańskie"",""code"":""MXN"",""mid"":0.1964},{""currency"":""rand (Republika Południowej Afryki)"",""code"":""ZAR"",""mid"":0.2654},{""currency"":""real (Brazylia)"",""code"":""BRL"",""mid"":0.7694},{""currency"":""ringgit (Malezja)"",""code"":""MYR"",""mid"":0.9507},{""currency"":""rubel rosyjski"",""code"":""RUB"",""mid"":0.0526},{""currency"":""rupia indonezyjska"",""code"":""IDR"",""mid"":0.00027776},{""currency"":""rupia indyjska"",""code"":""INR"",""mid"":0.053308},{""currency"":""won południowokoreański"",""code"":""KRW"",""mid"":0.003333},{""currency"":""yuan renminbi (Chiny)"",""code"":""CNY"",""mid"":0.6291},{""currency"":""SDR (MFW)"",""code"":""XDR"",""mid"":5.5849}]}]";
                    string json = await GetResponse();
                    var context = scope.ServiceProvider.GetRequiredService<CurrencyContext>();

                    if (json is not null)
                    {
                        await SaveData(context, json);
                        _logger.LogInformation("Udało się pobrać nowe kursy");
                    }
                    else
                    {
                        await UpdateData(context);
                        _logger.LogInformation("Udało się zaktualizować kursy");
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogInformation($"Nie udało sie: {exception.Message}");
                }
            },
                null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task stopped");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private async Task<string> GetResponse()
        {
            string path = "http://api.nbp.pl/api/exchangerates/tables/A/today/?format=json";
            HttpResponseMessage result = await _httpClient.GetAsync(path);

            string responseBody = await result.Content.ReadAsStringAsync();

            return responseBody;
        }


        private static async Task SaveData(CurrencyContext context, string json)
        {
            CompleteResponse response = JsonConvert.DeserializeObject<List<CompleteResponse>>(json).First();
            DateTime date = response.EffectiveDate;

            IList<Rate> rates = response.Rates.ToList();

            foreach (var rate in rates)
            {
                CurrencyRate newRate = new CurrencyRate()
                {
                    Value = rate.Mid,
                    Code = rate.Code,
                    Date = date
                };
                await context.CurrencyRates.AddAsync(newRate);
            }
            await context.SaveChangesAsync();
        }

        private static async Task UpdateData(CurrencyContext context)
        {
            var oldRates = context.CurrencyRates.Where(x => x.Date == DateTime.Today.AddDays(-1).Date).ToList();
            List<CurrencyRate> newRates = new List<CurrencyRate>();
            foreach (var oldRate in oldRates)
            {
                var rate = new CurrencyRate()
                {
                    Code = oldRate.Code,
                    Value = oldRate.Value,
                    Date = DateTime.Today.Date
                };
                await context.CurrencyRates.AddAsync(rate);
            }
            await context.SaveChangesAsync();
        }
    }
}
