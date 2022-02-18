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
using System.Text;
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
            timer = new Timer(o =>
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        string json = GetResponse().Result;

                        CompleteResponse response = JsonConvert.DeserializeObject<List<CompleteResponse>>(json).First();
                        DateTime date = response.EffectiveDate;

                        IList<Rate> rates = response.Rates.ToList();
                        var context = scope.ServiceProvider.GetRequiredService<CurrencyContext>();
                        foreach (var rate in rates)
                        {
                            CurrencyRate newRate = new CurrencyRate()
                            {
                                Value = rate.Mid,
                                Code = rate.Code,
                                Date = date
                            };
                            context.CurrencyRates.AddAsync(newRate);
                        }
                        context.SaveChangesAsync();
                    }
                }
                catch(Exception exception)
                {
                    _logger.LogInformation($"Nie udało sie: {exception.Message}");
                }
                    
                _logger.LogInformation("Udało się");

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
    }
}
